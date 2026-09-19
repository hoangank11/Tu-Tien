using UnityEngine;

public class UI_SkillListButton : MonoBehaviour
{
    [SerializeField] private SkillListDataSO skillData;
    private UI_SkillSlotPreview[] skillSlots;

    public void SetSkillSlots(UI_SkillSlotPreview[] skillSlots) => this.skillSlots = skillSlots;

    public void UpdateSkillSlots()
    {
        if (skillData == null) { Debug.Log("Chưa gắn SkillList"); return; }

        foreach (var slot in skillSlots)
        {
            slot.gameObject.SetActive(false);
        }

        for (int i = 0; i < skillData.skillList.Length; i++)
        {
            SkillDataSO skill = skillData.skillList[i];
            skillSlots[i].gameObject.SetActive(true);
            skillSlots[i].SetupButton(skill);
        }
    }
}
