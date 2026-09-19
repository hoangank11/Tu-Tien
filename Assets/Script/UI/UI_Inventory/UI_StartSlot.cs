using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StartSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private PlayerInformation playerInfor;
    private RectTransform rect;
    private UI ui;

    [SerializeField] private StartType startSlotType;
    [SerializeField] private TextMeshProUGUI startName;
    [SerializeField] private TextMeshProUGUI startValue;

    private void OnValidate()
    {
        gameObject.name = "Start - " + GetStartNameByType(startSlotType);
        startName.text = GetStartNameByType(startSlotType) + ":";
    }
    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        playerInfor = FindFirstObjectByType<PlayerInformation>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.inforToolTip.ShowToolTip(true, rect, startSlotType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.inforToolTip.ShowToolTip(false, null);
    }

    public void UpdateStartValue()
    {
        StartValuation startToUpdate = playerInfor.GetValueByType(startSlotType);
        if (startToUpdate == null) return;
        float value = 0;
        switch (startSlotType)
        {
            // HP & MP & Armor
            case StartType.HP:
                value = playerInfor.GetMaxHeatlh();
                break;
            case StartType.MP:
                value = playerInfor.normalGroup.maxMP.GetValue();
                break;
            case StartType.Armor:
                value = playerInfor.GetMaxArmor();
                break;

            // Dmg & Armor Pen & Crit
            case StartType.Damage:
                value = playerInfor.GetBaseDamage();
                break;
            case StartType.ArmorPenetration:
                value = playerInfor.GetArmorPenetration() * 100;
                break;
            case StartType.CritChance:
                value = playerInfor.GetCritChange();
                break;
            case StartType.CritDmg:
                value = playerInfor.GetCritPower();
                break;
            case StartType.AttackSpeed:
                value = playerInfor.baseGroup.attackSpeed.GetValue() * 100;
                break;

            // Evasion & Regen
            case StartType.Evasion:
                value = playerInfor.GetEvasion();
                break;
            case StartType.HPRegen:
                value = playerInfor.normalGroup.HPRegen.GetValue();
                break;
            case StartType.MPRegen:
                value = playerInfor.normalGroup.MPRegen.GetValue();
                break;

            // Elemental Damage
            case StartType.ElementalDmg:
                value = playerInfor.baseGroup.maxElementalDmg.GetValue();
                break;
            case StartType.ElectricChargeDmg:
                value = playerInfor.baseGroup.maxElectricChargeDmg.GetValue();
                break;

            // Elemental Res
            case StartType.FireRes:
                value = playerInfor.GetElementalResistance(ElementalType.Fire);
                break;
            case StartType.IceRes:
                value = playerInfor.GetElementalResistance(ElementalType.Ice);
                break;
            case StartType.ToxicRes:
                value = playerInfor.GetElementalResistance(ElementalType.Toxic);
                break;
            case StartType.ElectricRes:
                value = playerInfor.GetElementalResistance(ElementalType.Electric);
                break;

        }

        startValue.text = IsPercentageStart(startSlotType) ? value + "%" : value.ToString();
    }

    private string GetStartNameByType(StartType type)
    {
        switch (type)
        {
            case StartType.HP: return "Khí Huyết";
            case StartType.MP: return "Linh Lực";
            case StartType.HPRegen: return "Hồi Phục Khí Huyết";
            case StartType.MPRegen: return "Hồi Phục Linh Lực";
            case StartType.Armor: return "Hộ thể cương khí";
            case StartType.Damage: return "Công Lực";
            case StartType.AttackSpeed: return "Tốc Độ Xuất Chiêu";
            case StartType.ArmorPenetration: return "Phá Cương";
            case StartType.CritChance: return "Tỷ Lệ Bộc Phát";
            case StartType.CritDmg: return "Uy Lực Bộc Phát";
            case StartType.Evasion: return "Linh Hoạt";
            case StartType.ElementalDmg: return "Nguyên Tố Chi Lực";
            case StartType.ElectricChargeDmg: return "Lôi Phạt Bạo Lực";
            case StartType.FireRes: return "Hỏa Kháng";
            case StartType.IceRes: return "Hàn Kháng";
            case StartType.ToxicRes: return "Độc Kháng";
            case StartType.ElectricRes: return "Lôi Kháng";
            case StartType.WoodRes: return "Mộc Kháng";
            case StartType.WaterRes: return "Thủy Kháng";
            case StartType.WindRes: return "Phong Kháng";
            case StartType.EarthRes: return "Thổ Kháng";

            default: return "Không rõ thông tin";
        }
    }

    private bool IsPercentageStart(StartType type)
    {
        switch (type)
        {
            case StartType.CritChance:
            case StartType.CritDmg:
            case StartType.ArmorPenetration:
            case StartType.AttackSpeed:
            case StartType.FireRes:
            case StartType.IceRes:
            case StartType.ToxicRes:
            case StartType.ElectricRes:
            case StartType.Evasion:
            case StartType.ElectricChargeDmg:
            case StartType.WoodRes:
            case StartType.WaterRes:
            case StartType.WindRes:
            case StartType.EarthRes:
                return true;
            default:
                return false;
        }
    }


}
