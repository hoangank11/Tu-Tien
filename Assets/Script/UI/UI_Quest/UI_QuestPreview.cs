using TMPro;
using UnityEngine;

public class UI_QuestPreview : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questDes;
    [SerializeField] private TextMeshProUGUI questGoal;
    [SerializeField] private TextMeshProUGUI questMoney;
    [SerializeField] private UI_QuestRewardSlot[] questReward;

    [SerializeField] private GameObject[] additonalObjects;
    private UI_Quest questUI;
    private QuestDataSO previewQuest;

    public void SetupQuestPreview(QuestDataSO questDataSO)
    {
        questUI = transform.root.GetComponentInChildren<UI_Quest>();
        previewQuest = questDataSO;

        EnableAdditonalObjects(true);
        EnableQuestRewardObjects(false);

        questName.text = questDataSO.questName;
        questDes.text = questDataSO.description;
        questGoal.text = "Yêu Cầu:\n" + questDataSO.questGoal;
        questMoney.text = GetMoneyType(questDataSO);

        for (int i = 0; i < questDataSO.rewardItems.Length; i++)
        {
            InventoryItem rewardItem = new InventoryItem(questDataSO.rewardItems[i].itemData);
            rewardItem.stackSize = questDataSO.rewardItems[i].stackSize;

            questReward[i].gameObject.SetActive(true);
            questReward[i].UpdateSlot(rewardItem);
        }

    }

    private string GetMoneyType(QuestDataSO questDataSO)
    {
        if (questDataSO.moneyType == MoneyType.Copper)
            questMoney.text = "Đồng: " + questDataSO.moneyAmount;
        else if (questDataSO.moneyType == MoneyType.LinhThach)
            questMoney.text = "Linh Thạch: " + questDataSO.moneyAmount;
        return questMoney.text;
    }

    public void AcceptQuestBTN()
    {
        MakeQuestPreviewEmpty();

        questUI.questManager.AcceptQuest(previewQuest);
        questUI.UpdateQuestList();
    }

    public void MakeQuestPreviewEmpty()
    {
        questName.text = "";
        questDes.text = "";

        EnableAdditonalObjects(false);
        EnableQuestRewardObjects(false);
    }

    private void EnableAdditonalObjects(bool enable)
    {
        foreach (var obj in additonalObjects)
            obj.SetActive(enable);
    }

    private void EnableQuestRewardObjects(bool enable)
    {
        foreach (var obj in questReward)
            obj.gameObject.SetActive(enable);
    }

}
