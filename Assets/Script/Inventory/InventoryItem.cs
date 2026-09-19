using System;
using System.Text;
using UnityEngine;


[Serializable]
public class InventoryItem
{
    private string itemID;
    public ItemDataSO itemData;
    public int stackSize = 1;
    public ItemModifier[] modifiers {  get; private set; }
    public ItemEffectDataSO itemEffect;

    public int buyPrice {  get; private set; }
    public float sellPrice { get; private set; }
    private float TiGia = .50f; // Tỉ giá là số tiền sẽ nhận được sau khi bán một vật phẩm nhất định theo % số tiền mua từ shop

    public InventoryItem(ItemDataSO itemData)
    {
        this.itemData = itemData;
        modifiers = EquipmentData()?.modifiers;
        buyPrice = itemData.itemPrice;
        sellPrice = itemData.itemPrice * TiGia;

        itemEffect = itemData.itemEffect;
        itemID = itemData.itemName + " - " + Guid.NewGuid();
    }
    public void AddModifiers(Entity_Start playerStart)
    {
        foreach (var mod in modifiers)
        {
            StartValuation startToModifiers = playerStart.GetValueByType(mod.startType);
            startToModifiers.AddModifier(mod.value, itemID);
        }
    }

    public void RemoveModifiers(Entity_Start playerStart)
    {
        foreach (var mod in modifiers)
        {
            StartValuation startToModifier = playerStart.GetValueByType(mod.startType);
            startToModifier.RemoveModifier(itemID);
        }
    }

    public void AddItemEffect(Player player) => itemEffect?.Subscribe(player);
    public void RemoveItemEffect() => itemEffect?.Unsubscribe();

    private EquipmentDataSO EquipmentData()
    {
        if (itemData is EquipmentDataSO equipment)
            return equipment;
        return null;
    }
    public bool CanAddStack() => stackSize < itemData.maxStackSize;
    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;



    #region Item Informaton
    public string GetItemInfo()
    {
        if (itemData.itemType == ItemType.Material)
            return "Thông tin vật phẩm:\n" + "  Vật phẩm dùng để chế tạo hoặc trao đổi, buôn bán.";
        if (itemData.itemType == ItemType.Mission)
            return "Thông tin vật phẩm:\n" + "  Vật phẩm nhiệm vụ";

        if (itemData.itemType == ItemType.Consumable)
            return itemData.itemEffect.effectDes;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Thông tin vật phẩm:");
        foreach (var mod in modifiers)
        {
            string modType = GetStartNameByType(mod.startType);
            string modValue = IsPercentageStart(mod.startType) ?
                mod.startType == StartType.AttackSpeed ? (mod.value * 100).ToString() + "%" : mod.value.ToString() + "%" : mod.value.ToString();

            sb.AppendLine(" + " + modValue + " " + modType);
        }
        if (itemEffect != null)
        {
            sb.AppendLine("\n");
            sb.AppendLine("Hiệu ứng: ");
            sb.AppendLine(itemEffect.effectDes);
        }

        return sb.ToString();

    }

    private string GetStartNameByType(StartType type)
    {
        switch (type)
        {
            case StartType.HP: return "Khí Huyết";
            case StartType.MP: return "Linh Lực";
            case StartType.HPRegen: return "Hồi Phục Khí Huyết";
            case StartType.MPRegen: return "Hồi Phục Linh Lực";
            case StartType.Armor: return "Phòng Ngự";
            case StartType.Damage: return "Công Kích";
            case StartType.AttackSpeed: return "Tốc Độ Công Kích";
            case StartType.ArmorPenetration: return "Phá Giáp";
            case StartType.CritChance: return "Tỷ Lệ Bạo Kích";
            case StartType.CritDmg: return "Sát Thương Bạo Kích";
            case StartType.Evasion: return "Né Tránh";
            case StartType.ElementalDmg: return "Nguyên Tố Công Kích";
            case StartType.ElectricChargeDmg: return "Lôi Phạt Bạo Kích";
            case StartType.FireRes: return "Hỏa Kháng";
            case StartType.IceRes: return "Hàn Kháng";
            case StartType.ToxicRes: return "Độc Kháng";
            case StartType.ElectricRes: return "Lôi Kháng";

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
                return true;
            default:
                return false;
        }
    }
    #endregion

}
