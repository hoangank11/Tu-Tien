using TMPro;
using UnityEngine;

public class UI_Merchant : MonoBehaviour
{
    private InventoryPlayer inventory;
    private InventoryMerchant merchant;
    private UI ui;
    [SerializeField] private UI_ItemSlotParent merchantSlots;
    [SerializeField] private UI_ItemSlotParent inventorySlots;
    [SerializeField] private TextMeshProUGUI copper;
    [SerializeField] private TextMeshProUGUI linhThach;
    [SerializeField] private TextMeshProUGUI tienThach;

    public void SetupMerchantUI(InventoryMerchant merchant, InventoryPlayer inventory)
    {
        this.merchant = merchant;
        this.inventory = inventory;

        this.inventory.OnInventoryChange += UpdateSlotUI;
        this.merchant.OnInventoryChange += UpdateSlotUI;
        UpdateSlotUI();
        UI_MerchantSlot[] merchantSlot = GetComponentsInChildren<UI_MerchantSlot>();

        foreach (var slot in merchantSlot)
            slot.SetupMerchantUI(merchant);

    }

    private void UpdateSlotUI()
    {
        inventorySlots.UpdateSlots(inventory.itemList);
        merchantSlots.UpdateSlots(merchant.itemList);
        copper.text = inventory.GetMoney(MoneyType.Copper).ToString();
        linhThach.text = inventory.GetMoney(MoneyType.LinhThach).ToString();
        tienThach.text = inventory.GetMoney(MoneyType.TienThach).ToString();
    }

    public void CloseButton()
    {
        if (gameObject.activeSelf == false)
            return;

        if (ui == null)
            ui = GetComponentInParent<UI>(true);

        if (ui != null)
            ui.OpenMerchantUI(false);
        else
            gameObject.SetActive(false); // fallback neu khong tim thay UI manager
    }

}
