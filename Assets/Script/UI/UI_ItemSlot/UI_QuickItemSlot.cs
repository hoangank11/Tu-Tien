using UnityEngine;
using UnityEngine.EventSystems;

public class UI_QuickItemSlot : UI_ItemSlot
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private int slotNumber; 

    public void SetupQuickSlotItem(InventoryItem itemToPass)
    {
        inventory.SetQuickItemSlot(slotNumber, itemToPass);
    }
    public void UpdateQuickSlotUI(InventoryItem currentItemInSlot)
    {
        if (currentItemInSlot == null || currentItemInSlot.itemData == null)
        {
            itemIcon.sprite = defaultSprite;
            itemStackSize.text = "";
            return;
        }
        itemIcon.sprite = currentItemInSlot.itemData.itemIcon;
        itemStackSize.text = currentItemInSlot.stackSize.ToString();

    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        ui.inGameUI.OpenQuickItemOption(this, rect);
    }

}
