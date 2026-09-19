using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UI ui;
    private RectTransform rect;
    private SkillPoint skillTree;

    [Header("Unlock Details")]
    public UI_TreeNode[] needNode;
    public UI_TreeNode[] conflictNode;
    public bool isUnlock;
    public bool isLock;


    [Header("Skill Details")]
    public SkillDataSO skillData;
    [SerializeField] private string skillName;
    [SerializeField] private Image skillIcon;
    [SerializeField] private string lockColorHex = "#3D3D3D";

    private Color lastColor;

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        skillTree = GetComponentInParent<SkillPoint>(true);

        UpdateIconColor(GetColorByHex(lockColorHex));
    }

    private void Start()
    {
        if (skillData.unlockTest)  // just for test
            Unlock();
    }

    public void Refund()
    {
        if (isUnlock == false)
            return;
        isUnlock = false;
        isLock = false;
        UpdateIconColor(GetColorByHex(lockColorHex));

        skillTree.AddSkillPoint(skillData.skillPoint);
    }

    private void Unlock()
    {
        if (isUnlock)
            return;

        SkillBase skill = skillTree.skillManager.GetSkillByName(skillData.skillByName);
        if (skill == null)
        {
            Debug.LogWarning($"UI_TreeNode: không tìm thấy SkillBase cho {skillData.skillByName} trên Player_SkillManager, kiểm tra lại skillByName trong SkillDataSO.");
            return;
        }

        isUnlock = true;
        UpdateIconColor(Color.white);

        skillTree.RemoveSkillPoint(skillData.skillPoint);

        skill.SetSkillUpgrade(skillData);
        ui.inGameUI.AssignSkillToSlot(skillData, skill);
    }

    private bool CanBeUnlock()
    {
        if (isLock || isUnlock)
            return false;

        if (skillTree.EnoughSkillPoint(skillData.skillPoint) == false)
            return false;

        if (skillTree.EnoughLevel(skillData.level) == false)
            return false;

        foreach (var node in needNode)
        {
            if (node.isUnlock == false)
                return false;
        }
        foreach (var node in conflictNode)
        {
            if (node.isUnlock)
                return false;
        }

        return true;
    }
    private void LockConflictNodes()
    {
        foreach (var node in conflictNode)
            node.isLock = true;
    }

    private void UpdateIconColor(Color color)
    {
        if (skillIcon == null)
            return;

        lastColor = skillIcon.color;
        skillIcon.color = color;
    }


    // unlock skill
    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanBeUnlock())
        {
            Unlock();
            return;
        }

        if (isUnlock)
        {
            Debug.Log("Skill này đã được unlock rồi.");
            return;
        }
        if (isLock)
        {
            Debug.Log("Skill này đang bị khóa (xung đột với skill khác).");
            return;
        }
        if (skillTree.EnoughLevel(skillData.level) == false)
        {
            Debug.Log($"Chưa đủ cảnh giới để học skill này. Cần đạt: {skillData.level}, hiện tại: {skillTree.skillManager.CurrentLevel}.");
            return;
        }
        if (skillTree.EnoughSkillPoint(skillData.skillPoint) == false)
        {
            Debug.Log("Không đủ điểm kỹ năng (skill point) để học skill này.");
            return;
        }

        Debug.Log("Can't be unlock");
    }


    // show skill tooltip
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(true, rect, this);

        if (isUnlock || isLock)
            return;
        ToggleNodeHighlight(true);
    }


    // hide tooltip
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillToolTip.ShowToolTip(false, rect);

        if (isUnlock || isLock)
            return;
        ToggleNodeHighlight(false);
    }

    private void ToggleNodeHighlight(bool highlight)
    {
        Color highlightColor = Color.white * .7f;
        Color colorApply = highlight ? highlightColor : lastColor;
        UpdateIconColor(colorApply);
    }

    private Color GetColorByHex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }

    private void OnDisable()
    {
        if (isLock)
            UpdateIconColor(GetColorByHex(lockColorHex));
        if (isUnlock)
            UpdateIconColor(Color.white);
    }

    private void OnValidate()
    {
        if (skillData == null)
            return;
        skillName = skillData.skillName;
        skillIcon.sprite = skillData.icon;
        gameObject.name = "UI_SkillData - " + skillData.skillName;
    }
}