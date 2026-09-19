using TMPro;
using UnityEngine;

public class UI_PlayerInfor : UI_Tooltip
{
    private PlayerInformation infor;
    private TextMeshProUGUI text;

    protected override void Awake()
    {
        base.Awake();

        infor = FindFirstObjectByType<PlayerInformation>();
        text = GetComponentInChildren<TextMeshProUGUI>();

    }

    public void ShowToolTip(bool show, RectTransform targetRect, StartType startType)
    {
        base.ShowToolTip(show, targetRect);
        text.text = GetInforUI(startType);
    }

    public string GetInforUI(StartType type)
    {
        switch (type)
        {
            // hp, mp, regen, armor
            case StartType.HP:
                return infor.GetMaxHeatlh() + " - Khí Huyết:"
                    + "\n Khí huyết quyết định khả năng chịu đựng";
            case StartType.HPRegen:
                return infor.normalGroup.HPRegen.GetValue() + " - Khôi Phục Khí Huyết:"
                    + "\n Tốc độ khôi phục khí huyết theo thời gian";
            case StartType.MP:
                return infor.normalGroup.maxMP.GetValue() + " - Linh Lực:"
                    + "\n Linh lực dùng để thi triển công pháp";
            case StartType.MPRegen:
                return infor.normalGroup.MPRegen.GetValue() + " - Khôi Phục Linh Lực:"
                    + "\n Tốc độ hồi phục linh lực theo thời gian";
            case StartType.Armor:
                return infor.GetMaxArmor() + " - Hộ Thể Cương Khí:"
                    + "\n Hộ thể cương khí giảm sát thương nhận vào";

            // dmg, armor pen, crit, speed
            case StartType.Damage:
                return infor.GetBaseDamage() + " - Công Lực:"
                    + "\n Uy lực công kích cơ bản";
            case StartType.ArmorPenetration:
                return infor.GetArmorPenetration() * 100 + "% - Phá Cương:"
                    + "\n Xuyên phá phòng ngự của đối thủ";
            case StartType.CritChance:
                return infor.GetCritChange() + "% - Tỷ Lệ Bộc Phát:"
                    + "\n Xác suất bộc phát một kích chí mạng";
            case StartType.CritDmg:
                return infor.GetCritPower() + "% - Uy Lực Bộc Phát:"
                    + "\n Gia tăng uy lực của đòn bộc phát";
            case StartType.AttackSpeed:
                return infor.baseGroup.attackSpeed.GetValue() * 100 + "% - Tốc Độ Xuất Chiêu:"
                    + "\n Tốc độ xuất chiêu và thi triển công pháp";

            // evasion
            case StartType.Evasion:
                return infor.GetEvasion() + "% - Linh Hoạt:"
                    + "\n Thân pháp linh hoạt, tăng khả năng né tránh";

            // elemental Dmg
            case StartType.ElementalDmg:
                return infor.baseGroup.maxElementalDmg.GetValue() + " - Lực nguyên Tố:"
                    + "\n Gia tăng uy lực cho các Nguyên Tố";
            case StartType.ElectricChargeDmg:
                return infor.baseGroup.maxElectricChargeDmg.GetValue() + "% - Lôi Phạt Bạo Lực:"
                    + "\n Gia tăng sát thương Lôi Phạt khi bộc phát";

            // ele res
            case StartType.FireRes:
                return infor.GetElementalResistance(ElementalType.Fire) + "% - Hỏa Kháng:"
                    + "\n Giảm sát thương từ hỏa hệ";
            case StartType.IceRes:
                return infor.GetElementalResistance(ElementalType.Ice) + "% - Thủy Kháng:"
                    + "\n Giảm sát thương từ thủy hệ";
            case StartType.ToxicRes:
                return infor.GetElementalResistance(ElementalType.Toxic) + "% - Độc Kháng:"
                    + "\n Giảm sát thương từ mộc hệ";
            case StartType.ElectricRes:
                return infor.GetElementalResistance(ElementalType.Electric) + "% - Lôi Kháng:"
                    + "\n Giảm sát thương từ lôi hệ";
            case StartType.WoodRes:
                return infor.GetElementalResistance(ElementalType.Wood) + "% - Mộc Kháng:"
                    + "\n Giảm sát thương từ Mộc hệ";
            case StartType.WaterRes:
                return infor.GetElementalResistance(ElementalType.Water) + "% - Thủy Kháng:"
                    + "\n Giảm sát thương từ Thủy hệ";
            case StartType.EarthRes:
                return infor.GetElementalResistance(ElementalType.Earth) + "% - Thổ Kháng:"
                    + "\n Giảm sát thương từ Thổ hệ";
            case StartType.WindRes:
                return infor.GetElementalResistance(ElementalType.Wind) + "% - Phong Kháng:"
                    + "\n Giảm sát thương từ Phong hệ";


            default:
                return "null :6";
        }
    }

}
