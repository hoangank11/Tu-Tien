using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ItemTooltip : UI_Tooltip
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image moneyType;
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private TextMeshProUGUI itemInfo;
    [SerializeField] private TextMeshProUGUI itemDes;
    [SerializeField] private MoneyTypeIcon[] moneyTypeIcons;

    public void ShowToolTip(bool show, RectTransform targetRect, InventoryItem itemToShow, bool buyPrice = false)
    {
        base.ShowToolTip(show, targetRect);

        itemType.text = GetItemType(itemToShow.itemData.itemType);
        itemIcon.sprite = itemToShow.itemData.itemIcon;
        itemDes.text = GetItemDes(itemToShow);
        itemInfo.text = itemToShow.GetItemInfo();

        price.text = buyPrice? 
            "Mua: " + (itemToShow.buyPrice).ToString() : "Bán: " + (Mathf.FloorToInt(itemToShow.sellPrice)).ToString();

        moneyType.sprite = GetMoneyTypeIcon(itemToShow.itemData.moneyType);
        string color = getColorByRarity(itemToShow.itemData.itemRarity);
        itemName.text = GetColoredText(color, itemToShow.itemData.itemName);
    }
    public string GetItemDes(InventoryItem item)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Mô tả vật phẩm:\n" + item.itemData.description);
        return sb.ToString();
    }


    private string GetItemType(ItemType type)
    {
        switch (type)
        {
            case ItemType.Material: return "Linh Tài";
            case ItemType.Weapon: return "Pháp Bảo";
            case ItemType.Armor: return "Hộ Giáp";
            case ItemType.Assessory: return "Bội Sức";
            case ItemType.MagicItem: return "Pháp Khí";
            case ItemType.Consumable: return "Tiêu Hao";
            case ItemType.Scroll: return "Phù Lục";
            case ItemType.Mission: return "Nhiệm Vụ";
            default: 
                return "Không rõ thông tin";
        }
    }


    private Sprite GetMoneyTypeIcon(MoneyType type)
    {
        foreach (var entry in moneyTypeIcons)
        {
            if (entry.moneyType == type)
                return entry.icon;
        }
        return null;
    }

    private string getColorByRarity(int rarity)
    {
        if (rarity < 100) return "white";   
        if (rarity < 300) return "green";
        if (rarity < 600) return "blue";
        if (rarity < 850) return "yellow";
        if (rarity >= 850) return "red";
        return "orange";
    }

    private string GetColoredText(string color, string text)
    {
        return $"<color={color}>{text}</color>";
    }

}


[System.Serializable]
public class MoneyTypeIcon
{
    public MoneyType moneyType;
    public Sprite icon;
}
