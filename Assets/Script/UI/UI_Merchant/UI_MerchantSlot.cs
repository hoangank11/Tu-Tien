using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MerchantSlot : UI_ItemSlot
{
    private InventoryMerchant merchant;
    public enum MerchantSlotType { MerchantSlot, PlayerSlot }
    public MerchantSlotType slotType;

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null)
            return;
        bool rightButton = eventData.button == PointerEventData.InputButton.Right;
        bool leftButton = eventData.button == PointerEventData.InputButton.Left;
        if (slotType == MerchantSlotType.PlayerSlot)
        {
            if (rightButton)
            {
                bool sellFullStack = Input.GetKey(KeyCode.LeftControl);
                merchant.TrySellItem(itemInSlot, sellFullStack);
                UI_SFX.instance?.PlayMerchantTrade();
            }
            else if (leftButton)
            {
                base.OnPointerDown(eventData);
            }
        }
        else if (slotType == MerchantSlotType.MerchantSlot)
        {
            if (leftButton)
            {

            }
            else if (rightButton)
            {
                bool buyFullStack = Input.GetKey(KeyCode.LeftControl);
                merchant.TryBuyItem(itemInSlot, buyFullStack);
                UI_SFX.instance?.PlayMerchantTrade();
            }
        }
        ui.itemToolTip.ShowToolTip(false, null);

    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (itemInSlot == null) return;
        if (slotType == MerchantSlotType.MerchantSlot)
            ui.itemToolTip.ShowToolTip(true, rect, itemInSlot, true);
        else
            ui.itemToolTip.ShowToolTip(true, rect, itemInSlot, false);
    }

    public void SetupMerchantUI(InventoryMerchant merchant) => this.merchant = merchant;
}
