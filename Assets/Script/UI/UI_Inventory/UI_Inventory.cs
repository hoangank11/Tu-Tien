using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    private InventoryPlayer inventory;
    private SkillPoint skillTreeUI;
    [SerializeField] private TextMeshProUGUI skillPointText;
    [SerializeField] private TextMeshProUGUI copper;
    [SerializeField] private TextMeshProUGUI linhThach;
    [SerializeField] private TextMeshProUGUI tienThach;
    [SerializeField] private UI_ItemSlotParent uiItemSlotParent;
    [SerializeField] private UI_EquipSlotParent uiEquipSlotParent;

    private void Awake()
    {
        skillTreeUI = GetComponentInParent<UI>(true).skillTreeUI;

        inventory = FindFirstObjectByType<InventoryPlayer>();
        inventory.OnInventoryChange += UpdateUI;

        UpdateUI();
    }

    private void OnEnable()
    {
        if (inventory == null)
            return;
        UpdateUI();
    }

    private void UpdateUI()
    {
        uiItemSlotParent.UpdateSlots(inventory.itemList);
        uiEquipSlotParent.UpdateEquipmentSlots(inventory.equipList);
        skillPointText.text = skillTreeUI.GetSkillPoint().ToString();
        copper.text = inventory.GetMoney(MoneyType.Copper).ToString();
        linhThach.text = inventory.GetMoney(MoneyType.LinhThach).ToString();
        tienThach.text = inventory.GetMoney(MoneyType.TienThach).ToString();
    }

}
