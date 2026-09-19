using TMPro;
using UnityEngine;

public class UI_Craft : MonoBehaviour
{
    [SerializeField] private UI_ItemSlotParent inventoryParent;
    [SerializeField] private TextMeshProUGUI copper;
    [SerializeField] private TextMeshProUGUI linhThach;
    [SerializeField] private TextMeshProUGUI tienThach;
    private UI ui;
    private InventoryPlayer inventory;
    private UI_CraftPreview craftPreview;
    private UI_CraftSlot[] craftSlots;
    private UI_CraftListButton[] craftListButtons;


    public void SetupCraftUI(InventoryStorage storage)
    {
        inventory = storage.playerInventory;
        inventory.OnInventoryChange += UpdateUI;
        UpdateUI();

        craftPreview = GetComponentInChildren<UI_CraftPreview>();
        craftPreview.SetupCraftPreview(storage);
        SetupCraftListButtons();
    }

    private void SetupCraftListButtons()
    {
        craftSlots = GetComponentsInChildren<UI_CraftSlot>();
        craftListButtons = GetComponentsInChildren<UI_CraftListButton>();

        foreach (var slot in craftSlots)
            slot.gameObject.SetActive(false);

        foreach (var button in craftListButtons)
            button.SetCraftSlots(craftSlots);

    }

    private void UpdateUI()
    {
        inventoryParent.UpdateSlots(inventory.itemList);
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
            ui.OpenCraftUI(false);
        else
            gameObject.SetActive(false); // fallback neu khong tim thay UI manager
    }

}
