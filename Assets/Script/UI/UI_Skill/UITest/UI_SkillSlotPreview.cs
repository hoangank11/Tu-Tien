using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillSlotPreview : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;
    private SkillPoint skillTree;
    private Player_SkillManager skillManager;

    private SkillDataSO skillToPreview;
    public SkillDataSO skillData => skillToPreview;
    [SerializeField] private UI_Skill skillPreview;
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private string lockColorHex = "#3D3D3D";

    public bool isUnlock { get; private set; }
    private Color lastColor;

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = ui.GetComponentInChildren<SkillPoint>(true);
        skillManager = FindAnyObjectByType<Player_SkillManager>();
    }

    // Gán data cho ô này khi UI_SkillListButton hiện danh sách skill của 1 nhóm.
    public void SetupButton(SkillDataSO skillData)
    {
        this.skillToPreview = skillData;

        skillIcon.sprite = skillData.icon;
        skillName.text = skillData.skillName;

        // Đọc trạng thái unlock thật từ SkillBase (nguồn dữ liệu duy nhất),
        // thay vì luôn set false -> tránh mất trạng thái unlock mỗi khi
        // panel này bị setup lại (đóng/mở UI_SkillTree, đổi tab category...).
        SkillBase skill = skillManager.GetSkillByName(skillData.skillByName);
        isUnlock = skill != null && skill.IsUnlocked && skill.skillData == skillData;

        UpdateIconColor(isUnlock ? Color.white : GetColorByHex(lockColorHex));
    }

    public void UpdateSkillPreview() => skillPreview.UpdateSkillPreview(skillToPreview);

    private bool CanBeUnlock()
    {
        if (isUnlock)
            return false;

        return skillTree.EnoughSkillPoint(skillToPreview.skillPoint);
    }

    private void LearnSkill()
    {
        if (isUnlock)
            return;

        SkillBase skill = skillManager.GetSkillByName(skillToPreview.skillByName);
        if (skill == null)
        {
            Debug.LogWarning($"UI_SkillSlotPreview: không tìm thấy SkillBase cho {skillToPreview.skillByName} trên Player_SkillManager, kiểm tra lại skillByName trong SkillDataSO.");
            return;
        }

        isUnlock = true;
        UpdateIconColor(Color.white);

        skillTree.RemoveSkillPoint(skillToPreview.skillPoint);

        skill.SetSkillUpgrade(skillToPreview);
        ui.inGameUI.AssignSkillToSlot(skillToPreview, skill);
    }

    // học skill khi bấm vào ô
    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanBeUnlock())
            LearnSkill();
        else
            Debug.Log("Can't be unlock");
    }

    // hiện tooltip skill khi trỏ vào
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true, rect, this);

        if (isUnlock)
            return;
        ToggleSlotHighlight(true);
    }

    // ẩn tooltip khi rời chuột
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, rect);

        if (isUnlock)
            return;
        ToggleSlotHighlight(false);
    }

    private void ToggleSlotHighlight(bool highlight)
    {
        Color highlightColor = Color.white * .7f;
        Color colorApply = highlight ? highlightColor : lastColor;
        UpdateIconColor(colorApply);
    }

    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null)
            return;

        lastColor = skillIcon.color;
        skillIcon.color = color;
    }

    private Color GetColorByHex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }
}
