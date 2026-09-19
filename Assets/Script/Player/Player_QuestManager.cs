using System;
using System.Collections.Generic;
using UnityEngine;

public class Player_QuestManager : MonoBehaviour, ISaveable
{
    public const int maxActiveQuest = 10;

    public List<QuestData> activeQuest;
    public List<QuestData> completedQuest; // chỉ chứa MainQuest/SideQuest đã hoàn thành vĩnh viễn
    private Entity_DropManager dropManager;
    private InventoryPlayer inventory;
    [Header("Quest Database")]
    [SerializeField] private QuestDataBaseSo questDatabase;
    [Header("Daily Quest")]
    [SerializeField] private float dailyQuestCooldownHours = 24f;// questSaveID - thời điểm (UTC) có thể nhận lại DailyQuest
    [Header("Quest Mặc Định (Tự Động Nhận Khi Bắt Đầu Game Mới, Chưa Có Save Data)")]
    [SerializeField] private QuestDataSO startingMainQuest; // vd: Quest - Phàm Nhân MainQuest01

    private Dictionary<string, DateTime> dailyQuestNextAvailableTime = new Dictionary<string, DateTime>();
    public event Action onQuestListChanged;

    private void Awake()
    {
        dropManager = GetComponent<Entity_DropManager>();
        inventory = GetComponent<InventoryPlayer>();
        activeQuest ??= new List<QuestData>();
        completedQuest ??= new List<QuestData>();
        dailyQuestNextAvailableTime ??= new Dictionary<string, DateTime>();
    }
    /// <summary>
    /// Gọi khi bắt đầu 1 game mới (chưa từng có save data cũ) để tự động nhận
    /// Quest chính đầu tiên (startingMainQuest). Không làm gì nếu đã có/đã hoàn thành.
    /// </summary>
    public void GiveStartingQuestIfNeeded()
    {
        if (startingMainQuest == null)
            return;

        AcceptQuest(startingMainQuest);
    }

    /// <summary>
    /// Hoàn thành ngay lập tức 1 Quest đang active (dùng cho Dialogue - Done Quest).
    /// Không cấp phát reward item/tiền tự động (Quest chính hoàn thành thông qua Dialogue,
    /// không thông qua cơ chế TryGiveRewardFrom). Không làm gì nếu quest không active.
    /// </summary>
    public bool CompleteQuestManually(QuestDataSO questDataSO)
    {
        if (questDataSO == null)
            return false;

        QuestData questData = activeQuest.Find(q => q != null && q.questDataSO == questDataSO);
        if (questData == null)
            return false;

        CompletedQuest(questData);
        return true;
    }

    public void TryGiveRewardFrom(RewardType npcType)
    {
        List<QuestData> getRewardQuests = new List<QuestData>();
        foreach (var quest in activeQuest)
        {
            // MainQuest chỉ được hoàn thành thông qua Dialogue (Done Quest ở DialogueLineSO),
            // không tự động hoàn thành/nhận thưởng qua tương tác NPC như SideQuest/DailyQuest.
            if (quest.questDataSO.questType == QuestType.MainQuest)
                continue;

            // Nhiệm vụ giao hàng
            if (quest.questDataSO.missionType == QuestCategories.Delivery)
            {
                var requiredItem = quest.questDataSO.itemToDelivery;
                var requiredAmount = quest.questDataSO.requiredAmount;

                if (inventory.HasItemAmount(requiredItem, requiredAmount)) // == false
                {
                    inventory.RemoveItemAmount(requiredItem, requiredAmount);
                    quest.AddQuestProgress(requiredAmount);
                }
            }



            // Nhận hoàn thành nhiệm vụ
            if (quest.CanGetReward() && quest.questDataSO.rewardType == npcType)
                getRewardQuests.Add(quest);

        }

        foreach (var quest in getRewardQuests)
        {
            GiveQuestReward(quest.questDataSO);
            CompletedQuest(quest);
        }
    }

    private void GiveQuestReward(QuestDataSO questDataSO)
    {
        foreach (var item in questDataSO.rewardItems)
        {
            if (item == null || item.itemData == null) continue;

            for (int i = 0; i < item.stackSize; i++)
            {
                dropManager.CreateItemDrop(item.itemData);
            }
        }

        if (questDataSO.moneyAmount > 0)
        {
            inventory.AddMoney(questDataSO.moneyType, questDataSO.moneyAmount);
        }
    }

    public void AddProgress(string questTargetID, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(questTargetID) || amount <= 0)
            return;

        string targetID = questTargetID.Trim();

        foreach (var quest in activeQuest)
        {
            if (quest == null || quest.questDataSO == null)
                continue;

            string questID = quest.questDataSO.questTargetID;

            if (string.IsNullOrWhiteSpace(questID) ||
                !string.Equals(
                    questID.Trim(),
                    targetID,
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            quest.AddQuestProgress(amount);
        }
    }


    public bool AcceptQuest(QuestDataSO questDataSO)
    {
        if (CanAcceptQuest(questDataSO) == false)
            return false;

        activeQuest.Add(new QuestData(questDataSO));
        onQuestListChanged?.Invoke();
        return true;
    }

    public bool CanAcceptMoreQuest() => activeQuest.Count < maxActiveQuest;

    /// <summary>
    /// Quest có đang được phép nhận hay không (dùng cho cả NPC lẫn UI để quyết định có hiển thị/cho nhận quest hay không).
    /// - Quest đang làm dở (active) -> không cho nhận lại.
    /// - MainQuest/SideQuest đã hoàn thành -> biến mất vĩnh viễn, không cho nhận lại.
    /// - DailyQuest đã hoàn thành -> vẫn còn, nhưng phải chờ đủ thời gian cooldown (mặc định 24h) mới nhận lại được.
    /// </summary>
    public bool CanAcceptQuest(QuestDataSO questDataSO)
    {
        if (questDataSO == null)
            return false;

        if (QuestIsActive(questDataSO))
            return false;

        if (CanAcceptMoreQuest() == false)
            return false;

        if (questDataSO.questType == QuestType.DailyQuest)
            return IsDailyQuestOnCooldown(questDataSO) == false;

        return IsQuestCompleted(questDataSO) == false;
    }

    public void CompletedQuest(QuestData questData)
    {
        activeQuest.Remove(questData);

        if (questData.questDataSO.questType == QuestType.DailyQuest)
        {
            // Không xoá vĩnh viễn, chỉ đánh dấu thời điểm được nhận lại
            dailyQuestNextAvailableTime[questData.questDataSO.questSaveID] =
                DateTime.UtcNow.AddHours(dailyQuestCooldownHours);
        }
        else
        {
            completedQuest.Add(questData);
        }

        onQuestListChanged?.Invoke();
    }

    public bool QuestIsActive(QuestDataSO questToCheck)
    {
        if (questToCheck == null)
            return false;
        return activeQuest.Find(q => q.questDataSO == questToCheck) != null;
    }

    public bool IsQuestCompleted(QuestDataSO questToCheck)
    {
        if (questToCheck == null)
            return false;
        return completedQuest.Find(q => q.questDataSO == questToCheck) != null;
    }

    public bool IsDailyQuestOnCooldown(QuestDataSO questToCheck)
    {
        if (questToCheck == null)
            return false;

        if (dailyQuestNextAvailableTime.TryGetValue(questToCheck.questSaveID, out DateTime nextTime))
            return DateTime.UtcNow < nextTime;

        return false;
    }

    /// <summary>Thời gian còn lại (nếu có) trước khi DailyQuest có thể nhận lại. TimeSpan.Zero nếu đã sẵn sàng.</summary>
    public TimeSpan GetDailyQuestRemainingTime(QuestDataSO questToCheck)
    {
        if (questToCheck == null)
            return TimeSpan.Zero;

        if (dailyQuestNextAvailableTime.TryGetValue(questToCheck.questSaveID, out DateTime nextTime))
        {
            TimeSpan remaining = nextTime - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }

        return TimeSpan.Zero;
    }

    public void LoadData(GameData data)
    {
        activeQuest.Clear();
        completedQuest.Clear();
        dailyQuestNextAvailableTime.Clear();

        foreach (var entry in data.activeQuest)
        {
            string questSaveID = entry.Key;
            int progress = entry.Value;
            QuestDataSO questDataSO = questDatabase.GetQuestByID(questSaveID);

            if (questDataSO == null)
                continue;

            QuestData questToLoad = new QuestData(questDataSO);
            questToLoad.currentAmount = progress;

            activeQuest.Add(questToLoad);
        }

        foreach (var entry in data.completedQuest)
        {
            if (entry.Value == false)
                continue;

            QuestDataSO questDataSO = questDatabase.GetQuestByID(entry.Key);
            if (questDataSO == null)
                continue;

            completedQuest.Add(new QuestData(questDataSO));
        }

        foreach (var entry in data.dailyQuestNextAvailableTime)
        {
            if (DateTime.TryParse(
                    entry.Value,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.RoundtripKind,
                    out DateTime nextTime))
            {
                dailyQuestNextAvailableTime[entry.Key] = nextTime;
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        data.activeQuest.Clear();
        data.completedQuest.Clear();
        data.dailyQuestNextAvailableTime.Clear();

        foreach (var quest in activeQuest)
        {
            data.activeQuest.Add(quest.questDataSO.questSaveID, quest.currentAmount);
        }
        foreach (var quest in completedQuest)
        {
            data.completedQuest.Add(quest.questDataSO.questSaveID, true);
        }
        foreach (var entry in dailyQuestNextAvailableTime)
        {
            data.dailyQuestNextAvailableTime.Add(
                entry.Key,
                entry.Value.ToString("o", System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
