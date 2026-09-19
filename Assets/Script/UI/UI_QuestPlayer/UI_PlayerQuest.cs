using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Gắn trên UI_Canvas/UI_PlayerQuest.
// Giống UI_Quest nhưng CHỈ hiển thị các quest mà player ĐÃ nhận (activeQuest),
// tối đa Player_QuestManager.maxActiveQuest (10) quest.
// Cách lấy/refresh danh sách quest tương tự UI_QuestGroupInGame (subscribe onQuestListChanged).
public class UI_PlayerQuest : MonoBehaviour
{
    [Header("Icon theo từng loại Quest")]
    [SerializeField] private Sprite mainQuestIcon;
    [SerializeField] private Sprite sideQuestIcon;
    [SerializeField] private Sprite dailyQuestIcon;

    [SerializeField] private UI_QuestPreview questPreview;

    private UI_QuestPreviewSetupButton[] questSlots;
    private Player_QuestManager questManager;

    private void Awake()
    {
        questSlots = GetComponentsInChildren<UI_QuestPreviewSetupButton>(true);
    }

    private void OnEnable()
    {
        if (questManager == null)
            questManager = Player.instance != null ? Player.instance.questManager : null;

        if (questManager == null)
        {
            Debug.LogWarning("[UI_PlayerQuest] Không tìm thấy Player_QuestManager, chưa thể hiển thị danh sách quest.");
            return;
        }

        questManager.onQuestListChanged += RefreshPlayerQuestList;
        RefreshPlayerQuestList();

        if (questPreview != null)
            questPreview.MakeQuestPreviewEmpty();
    }

    private void OnDisable()
    {
        if (questManager != null)
            questManager.onQuestListChanged -= RefreshPlayerQuestList;
    }

    public void RefreshPlayerQuestList()
    {
        if (questManager == null || questSlots == null)
            return;

        // MainQuest (0) luôn đứng đầu, SideQuest (1) kế tiếp, DailyQuest (2) cuối cùng.
        // Giới hạn tối đa maxActiveQuest (10) quest, bằng đúng số slot trong UI_QuestListParent.
        List<QuestData> sortedQuests = questManager.activeQuest
            .Where(q => q != null && q.questDataSO != null)
            .OrderBy(q => (int)q.questDataSO.questType)
            .Take(Player_QuestManager.maxActiveQuest)
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

    public UI_QuestPreview GetQuestPreview() => questPreview;

    public void CloseButton()
    {
        UI.instance.TogglePlayerQuestUI();
    }
}
