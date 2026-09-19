using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StorageSlot : UI_ItemSlot
{
    private InventoryStorage storage;
    public enum StorageSlotType { StorageSlot, InventorySlot}
    public StorageSlotType slotType;

    public void SetStorage(InventoryStorage storage) => this.storage = storage;

    public override void OnPointerDown(PointerEventData eventData)
    {
        bool transferFullStack = Input.GetKey(KeyCode.LeftControl);
        if (itemInSlot == null) return;
        if (slotType == StorageSlotType.StorageSlot)
            storage.FromStorageToPlayer(itemInSlot, transferFullStack);
        if (slotType == StorageSlotType.InventorySlot)
            storage.FromPlayerToStorage(itemInSlot, transferFullStack);
        ui.itemToolTip.ShowToolTip(false, null);
    }

}
