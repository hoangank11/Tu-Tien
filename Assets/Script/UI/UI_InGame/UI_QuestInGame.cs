using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Gắn trên từng GameObject "UI_QuestPreviewSetupButton" (10 slot trong UI_QuestGroupParent).
// Script này CHỈ hiển thị 1 nhiệm vụ mà player ĐÃ nhận (activeQuest, sau khi AcceptQuest()).
// Việc lấy danh sách quest, sắp xếp và gán vào từng slot do UI_QuestGroupInGame (đặt trên UI_QuestGroupParent) đảm nhiệm.
public class UI_QuestInGame : MonoBehaviour
{
    [SerializeField] private Image iconImage;              // Kéo child "IconType" vào đây
    [SerializeField] private TextMeshProUGUI questNameText; // Kéo child "QuestName" vào đây

    public QuestData questData { get; private set; }

    // Gọi bởi UI_QuestGroupInGame để gán 1 quest đang active vào slot này
    public void SetupQuestSlot(QuestData data, Sprite icon)
    {
        questData = data;

        if (questData == null || questData.questDataSO == null)
        {
            ClearSlot();
            return;
        }

        if (questNameText != null)
            questNameText.text = questData.questDataSO.questName;

        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
        }

        gameObject.SetActive(true);
    }

    // Ẩn slot khi không còn quest nào để hiển thị (ví dụ slot thứ 8, 9, 10 khi player mới nhận 3 quest)
    public void ClearSlot()
    {
        questData = null;

        if (questNameText != null)
            questNameText.text = "";

        if (iconImage != null)
            iconImage.enabled = false;

        gameObject.SetActive(false);
    }
}
