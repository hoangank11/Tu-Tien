using UnityEngine;

public class UI_Storage : MonoBehaviour
{
    private UI ui;
    private InventoryStorage storage;
    private InventoryPlayer inventory;
    [SerializeField] private UI_ItemSlotParent inventoryParent;
    [SerializeField] private UI_ItemSlotParent storageParent;
    [SerializeField] private UI_ItemSlotParent materialStashParent;


    public void SetupStorage(InventoryPlayer inventory, InventoryStorage storage)
    {
        this.inventory = inventory;
        this.storage = storage;
        storage.OnInventoryChange += UpdateUI;
        UpdateUI();

        UI_StorageSlot[] storageSlots = GetComponentsInChildren<UI_StorageSlot>();
        foreach (var slot in storageSlots)
            slot.SetStorage(storage);
    }

    private void UpdateUI()
    {
        inventoryParent.UpdateSlots(inventory.itemList);
        storageParent.UpdateSlots(storage.itemList);
        materialStashParent.UpdateSlots(storage.materialStash);
    }
    public void CloseButton()
    {
        if (gameObject.activeSelf == false)
            return;

        if (ui == null)
            ui = GetComponentInParent<UI>(true);

        if (ui != null)
            ui.OpenStorageUI(false);
        else
            gameObject.SetActive(false); // fallback neu khong tim thay UI manager
    }
}
