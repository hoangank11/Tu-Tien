using System.Linq;
using UnityEngine;

public class UI_Quest : MonoBehaviour
{
    private UI_QuestSlot[] questSlots;
    [SerializeField] private UI_ItemSlotParent inventorySlots;
    [SerializeField] private UI_QuestPreview questPreview;
    public Player_QuestManager questManager { get; private set; }
    private void Awake()
    {
        questSlots = GetComponentsInChildren<UI_QuestSlot>(true);
        questManager = Player.instance.questManager;
    }

    public void SetupQuestUI(QuestDataSO[] questToSetup)
    {
        foreach (var quest in questSlots)
            quest.gameObject.SetActive(false);

        // MainQuest không bao giờ hiển thị ở UI_Quest (chỉ được nhận tự động khi bắt đầu game mới
        // hoặc thông qua Dialogue - Can Except Quest), nên luôn lọc bỏ khỏi danh sách hiển thị ở đây.
        QuestDataSO[] displayableQuests = questToSetup
            .Where(quest => quest != null && quest.questType != QuestType.MainQuest)
            .ToArray();

        for (int i = 0; i < displayableQuests.Length; i++)
        {
            questSlots[i].gameObject.SetActive(true);
            questSlots[i].SetupQuestSlot(displayableQuests[i]);
        }

        questPreview.MakeQuestPreviewEmpty();
        inventorySlots.UpdateSlots(Player.instance.inventory.itemList);
        UpdateQuestList();
    }


    public void UpdateQuestList()
    {
        foreach (var slot in questSlots)
        {
            if (slot.questInSlot == null) continue;

            if (slot.gameObject.activeSelf && CanTakeQuest(slot.questInSlot) == false)
                slot.gameObject.SetActive(false);
        }
    }
    private bool CanTakeQuest(QuestDataSO questToCheck)
    {
        return questManager.CanAcceptQuest(questToCheck);
    }

    public UI_QuestPreview GetQuestPreview() => questPreview;

    public void CloseButton()
    {
        UI.instance.CloseQuestUI();
    }

}
