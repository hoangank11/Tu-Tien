using UnityEngine;

public class UI_QuestListToggle : MonoBehaviour
{
    [SerializeField] private GameObject questListParent;

    public void ToggleQuestList()
    {
        if (questListParent == null)
        {
            Debug.LogWarning("[UI_QuestListToggle] Chưa gán questListParent trong Inspector.");
            return;
        }

        questListParent.SetActive(!questListParent.activeSelf);
    }
}
