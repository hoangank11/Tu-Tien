using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SkillText : UI_Tooltip
{
    private UI ui;
    private SkillPoint skillTree;
    private Player player;
    private Player_SkillManager skillManager;

    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillRequire;
    [SerializeField] private TextMeshProUGUI skillCooldown;
    [SerializeField] private TextMeshProUGUI skillDes;
    [SerializeField] private TextMeshProUGUI skillInfor;
    [Space]
    [SerializeField] private string metConditionHex;
    [SerializeField] private string notMetConditionHex;
    [SerializeField] private Color exampColor;

    protected override void Awake()
    {
        base.Awake();
        ui = GetComponentInParent<UI>();
        skillTree = ui.GetComponentInChildren<SkillPoint>(true);
        player = FindFirstObjectByType<Player>();
        skillManager = FindFirstObjectByType<Player_SkillManager>();
    }

    public override void ShowToolTip(bool show, RectTransform targetRect)
    {
        base.ShowToolTip(show, targetRect);

        if (!show && skillInfor != null)
            skillInfor.text = string.Empty;
    }

    public void ShowToolTip(bool show, RectTransform targetRect, UI_TreeNode node)
    {
        base.ShowToolTip(show, targetRect);

        if (show == false)
            return;

        skillName.text = node.skillData.skillName;
        skillRequire.text = "Yêu Cầu: \n"
            + " * Cảnh Giới: " + node.skillData.level + "\n"
            + " * Kỹ Năng Cần: \n"
            + GetRequire(node.skillData.skillPoint, node.needNode, node.conflictNode);
        skillCooldown.text = "Thời gian hồi chiêu thức: " + node.skillData.cooldown + "s";
        skillIcon.sprite = node.skillData.icon;
        skillDes.text = node.skillData.description;

        skillInfor.text = GetSkillInformation(node.skillData);
    }

    public void ShowToolTip(bool show, RectTransform targetRect, UI_SkillSlotPreview slot)
    {
        base.ShowToolTip(show, targetRect);

        if (show == false)
            return;

        skillName.text = slot.skillData.skillName;

        string costColor = skillTree.EnoughSkillPoint(slot.skillData.skillPoint)
            ? metConditionHex
            : notMetConditionHex;

        skillRequire.text = "Yêu Cầu: \n"
            + " * Cảnh Giới: " + slot.skillData.level + "\n"
            + $"<color={costColor}>     - {slot.skillData.skillPoint} Ngộ Đạo Linh Thạch </color>";

        skillCooldown.text = "Thời gian hồi chiêu thức: " + slot.skillData.cooldown + "s";
        skillIcon.sprite = slot.skillData.icon;
        skillDes.text = slot.skillData.description;

        skillInfor.text = GetSkillInformation(slot.skillData);
    }

    private string GetSkillInformation(SkillDataSO data)
    {
        if (data == null)
            return string.Empty;

        StringBuilder sb = new StringBuilder();

        SkillBase skill = skillManager != null
            ? skillManager.GetSkillByName(data.skillByName)
            : null;

        ElementalType elementalType = skill != null
            ? skill.GetElementalType()
            : ElementalType.None;

        sb.AppendLine(" Nguyên Tố: " + GetElementalName(elementalType));

        float elementalPower = 0f;

        if (elementalType != ElementalType.None)
        {
            float maxElementalDmg = player != null && player.start != null
                ? player.start.baseGroup.maxElementalDmg.GetValue()
                : 0f;
            float incElementalDmg = player != null && player.start != null
                ? player.start.incGroup.incElementalDmg.GetValue()
                : 0f;

            elementalPower = maxElementalDmg + incElementalDmg;

            sb.AppendLine(" Sát thương chiêu thức: " + FormatValue(maxElementalDmg));
        }

        string effectInfo = GetElementalEffectInformation(elementalType, data.scaleFactor, elementalPower);

        if (!string.IsNullOrEmpty(effectInfo))
        {
            if (sb.Length > 0)
                sb.AppendLine();
            sb.Append(effectInfo);
        }

        return sb.ToString();
    }

    private string GetElementalEffectInformation(ElementalType elementalType, ScaleFactor scale, float elementalPower)
    {
        if (scale == null)
            return string.Empty;

        StringBuilder sb = new StringBuilder();

        switch (elementalType)
        {
            case ElementalType.Fire:
                float fireTickDmg = elementalPower * scale.fireScaleDmgDoT;
                sb.AppendLine(" Gây " + FormatValue(fireTickDmg) 
                    + " sát thương hệ hỏa mỗi 0.5 giây (Sát thương duy trì hệ hỏa bị ảnh hưởng bởi Hộ thể cương khí) trong "
                    + FormatValue(scale.effectFireDuration) + " giây");
                break;

            case ElementalType.Toxic:
                float toxicTickDmg = elementalPower * scale.toxicScaleDmgDoT;
                sb.AppendLine(" Gây " + FormatValue(toxicTickDmg)
                    + " sát thương hệ độc mỗi 0.5 giây (Sát thương duy trì hệ độc không bị ảnh hưởng bởi Hộ thể cương khí) và làm chậm kẻ địch "
                    + FormatPercent(scale.toxicSlowAnimation) + " trong "
                    + FormatValue(scale.effectToxicDuration) + " giây");
                break;

            case ElementalType.Ice:
                sb.AppendLine(" Làm chậm kẻ địch " + FormatPercent(scale.iceSlowAnimation) + " trong "
                + FormatValue(scale.effectIceDuration) + " giây");
                break;

            case ElementalType.Electric:
                sb.AppendLine(" Khi gây sát thương hệ lôi nếu tích tụ đủ 5 tầng trong " 
                    + FormatValue(scale.effectElectricDuration) + " giây sẽ gây lôi phạt với "
                    + (player.start.defaultSetup.maxElectricChargeDmg * 100) + "% sát lực của nguyên tố");
                break;

            case ElementalType.Wind:
                sb.AppendLine(" Hất tung kẻ địch cao và khi rơi xuống kẻ địch sẽ bị choáng trong "
                    + FormatValue(scale.stunTimeWindElemental) 
                    + " giây (với mỗi kẻ địch sẽ chỉ thi triển được mỗi 10 giây mỗi lần)");
                break;

            case ElementalType.Earth:
                sb.AppendLine(" Khi gây sát thương hệ thổ trong " 
                    + FormatValue(scale.earthReduceDuration) + " giây thì có tỉ lệ " 
                    + FormatPercent(scale.stunChange * 100) + "% làm choáng kẻ địch và làm giảm Hộ " 
                    + FormatPercent(scale.reduceMaxArmor * 100) + "% Thể Cương Khí của kẻ địch");
                break;

            case ElementalType.Wood:
                sb.AppendLine(" Khi gây sát thương hệ mộc trong " + FormatValue(scale.woodRootDuration) 
                    + " giây sẽ khiến cho kẻ địch mất hoàn toàn khả năng di chuyển nhưng kẻ địch vẫn có thể tấn công");
                break;

            case ElementalType.Water:
                sb.AppendLine(" Khi gây sát thương hệ Thủy trong " + FormatValue(scale.waterDuration)
                    + " giây sẽ có tỉ lệ " + FormatPercent(scale.waterTrueDamageChance * 100)
                    + "% gây sát thưởng chuẩn bỏ qua hoàn toàn Hộ Thể Cương Khí của kẻ địch và nếu gây sát thương chuẩn sẽ hồi phục "
                    + FormatPercent(scale.waterHealPercent) + "% Khí Huyết theo Sát thương đã gây");
                break;
        }

        return sb.ToString();
    }

    private string GetElementalName(ElementalType type)
    {
        switch (type)
        {
            case ElementalType.Fire: return "Hỏa";
            case ElementalType.Ice: return "Băng";
            case ElementalType.Toxic: return "Độc";
            case ElementalType.Electric: return "Lôi";
            case ElementalType.Wind: return "Phong";
            case ElementalType.Earth: return "Thổ";
            case ElementalType.Wood: return "Mộc";
            case ElementalType.Water: return "Thủy";
            default: return "Không";
        }
    }

    private string FormatValue(float value)
    {
        return value.ToString("0.##");
    }

    private string FormatPercent(float value)
    {
        return (value * 100f).ToString("0.##") + "%";
    }

    private string GetRequire(int skillPoint, UI_TreeNode[] needSkill, UI_TreeNode[] conflictNode)
    {
        StringBuilder sb = new StringBuilder();

        string costColor = skillTree.EnoughSkillPoint(skillPoint)
            ? metConditionHex
            : notMetConditionHex;

        sb.AppendLine($"<color={costColor}>     - {skillPoint} Linh Thạch </color>");

        foreach (var node in needSkill)
        {
            if (node == null) continue;

            string nodeColor = node.isUnlock
                ? metConditionHex
                : notMetConditionHex;

            sb.AppendLine($"<color={nodeColor}>     - {node.skillData.skillName} </color>");
        }

        foreach (var node in conflictNode)
        {
            if (node == null) continue;
        }

        return sb.ToString();
    }
}