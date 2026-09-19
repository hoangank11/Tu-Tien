using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Skill : MonoBehaviour
{
    private UI_SkillSlotPreview[] skillSlots;
    private UI_SkillListButton[] skillListButtons;

    [Header("Skill Preview Setup")]
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillInfor;
    [SerializeField] private TextMeshProUGUI skillLevel;
    [SerializeField] private TextMeshProUGUI skillPointCost;

    private void Awake()
    {
        SetupSkillListButtons();
    }

    public void SetupSkillUI() => SetupSkillListButtons();

    

    private void SetupSkillListButtons()
    {
        skillSlots = GetComponentsInChildren<UI_SkillSlotPreview>(true);
        skillListButtons = GetComponentsInChildren<UI_SkillListButton>(true);

        foreach (var slot in skillSlots)
            slot.gameObject.SetActive(false);

        foreach (var button in skillListButtons)
            button.SetSkillSlots(skillSlots);
    }

    public void UpdateSkillPreview(SkillDataSO skillData)
    {
        if (skillData == null)
            return;

        // Panel chi tiết là tuỳ chọn - nếu chưa gán field nào trong Inspector thì bỏ qua, không crash.
        if (skillIcon == null || skillName == null || skillInfor == null || skillLevel == null || skillPointCost == null)
            return;

        gameObject.SetActive(true);

        skillIcon.sprite = skillData.icon;
        skillName.text = skillData.skillName;
        skillInfor.text = skillData.description;
        skillLevel.text = skillData.level.ToString();
        skillPointCost.text = skillData.skillPoint.ToString();
    }
}