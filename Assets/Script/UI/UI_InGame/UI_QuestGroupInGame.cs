using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_QuestGroupInGame : MonoBehaviour
{
    [Header("Icon theo từng loại Quest")]
    [SerializeField] private Sprite mainQuestIcon;
    [SerializeField] private Sprite sideQuestIcon;
    [SerializeField] private Sprite dailyQuestIcon;

    private UI_QuestInGame[] questSlots;
    private Player_QuestManager questManager;

    private void Awake()
    {
        questSlots = GetComponentsInChildren<UI_QuestInGame>(true);
    }

    private void OnEnable()
    {
        if (questManager == null)
            questManager = Player.instance != null ? Player.instance.questManager : null;

        if (questManager == null)
        {
            Debug.LogWarning("[UI_QuestGroupInGame] Không tìm thấy Player_QuestManager, chưa thể hiển thị danh sách quest.");
            return;
        }

        questManager.onQuestListChanged += RefreshQuestGroup;
        RefreshQuestGroup();
    }

    private void OnDisable()
    {
        if (questManager != null)
            questManager.onQuestListChanged -= RefreshQuestGroup;
    }

    public void RefreshQuestGroup()
    {
        if (questManager == null || questSlots == null)
            return;

        // MainQuest (0) luôn đứng đầu, SideQuest (1) kế tiếp, DailyQuest (2) cuối cùng.
        // Thứ tự enum QuestType đã đúng theo yêu cầu nên chỉ cần OrderBy theo giá trị enum.
        List<QuestData> sortedQuests = questManager.activeQuest
            .Where(q => q != null && q.questDataSO != null)
            .OrderBy(q => (int)q.questDataSO.questType)
            .ToList();

        for (int i = 0; i < questSlots.Length; i++)
        {
            if (i < sortedQuests.Count)
                questSlots[i].SetupQuestSlot(sortedQuests[i], GetIconFor(sortedQuests[i].questDataSO.questType));
            else
                questSlots[i].ClearSlot();
        }
    }

    private Sprite GetIconFor(QuestType type)
    {
        switch (type)
        {
            case QuestType.MainQuest: return mainQuestIcon;
            case QuestType.SideQuest: return sideQuestIcon;
            case QuestType.DailyQuest: return dailyQuestIcon;
            default: return null;
        }
    }
}
