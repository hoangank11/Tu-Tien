using TMPro;
using UnityEngine;

public class UI_QuestSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private QuestType questType;

    public QuestDataSO questInSlot {  get; private set; }
    private UI_QuestPreview questpreview;

    public void SetupQuestSlot(QuestDataSO questData)
    {
        questInSlot = questData;
        questpreview = transform.root.GetComponentInChildren<UI_Quest>().GetQuestPreview();
        questName.text = questData.questName;

    }

    public void UpdateQuestPreview()
    {
        questpreview.SetupQuestPreview(questInSlot);
    }


}
