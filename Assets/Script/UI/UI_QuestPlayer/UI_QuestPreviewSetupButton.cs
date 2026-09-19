using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Gắn trên từng GameObject "UI_QuestPreviewSetupButton" bên trong
// UI_Canvas/UI_PlayerQuest/UI_QuestListParent (10 slot, do UI_PlayerQuest quản lý).
// Khác với UI_QuestInGame (chỉ hiển thị, nằm trong UI_InGame/OnQuest), object này còn có Button:
// mỗi khi bấm vào sẽ lấy UI_QuestPreview (nằm cùng cấp UI_PlayerQuest, đã có sẵn trong scene)
// để hiện lên và show thông tin quest.
public class UI_QuestPreviewSetupButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI questNameText;

    public QuestData questData { get; private set; }
    private UI_QuestPreview questPreview;

    // Gọi bởi UI_PlayerQuest để gán 1 quest đang active vào slot này
    public void SetupQuestSlot(QuestData data, Sprite icon)
    {
        questData = data;

        if (questData == null || questData.questDataSO == null)
        {
            ClearSlot();
            return;
        }

        questPreview = transform.root.GetComponentInChildren<UI_PlayerQuest>().GetQuestPreview();

        if (questNameText != null)
            questNameText.text = questData.questDataSO.questName;

        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
        }

        gameObject.SetActive(true);
    }

    // Ẩn slot khi không còn quest nào để hiển thị
    public void ClearSlot()
    {
        questData = null;

        if (questNameText != null)
            questNameText.text = "";

        if (iconImage != null)
            iconImage.enabled = false;

        gameObject.SetActive(false);
    }

    // Gán vào Button OnClick của từng slot trong Inspector
    public void ShowQuestPreview()
    {
        if (questData == null || questData.questDataSO == null || questPreview == null)
            return;

        questPreview.SetupQuestPreview(questData.questDataSO);
    }
}
