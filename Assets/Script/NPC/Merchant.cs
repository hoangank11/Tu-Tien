using UnityEngine;

public class Merchant : NPC, IInteractable
{

    private InventoryPlayer inventory;
    private InventoryMerchant merchant;

    protected override void Awake()
    {
        base.Awake();
        merchant = GetComponent<InventoryMerchant>();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.F11))
            merchant.FillShopList();
    }

    public override void Interact()
    {
        if (ui == null || ui.merchantUI == null)
        {
            return;
        }
        ui.merchantUI.SetupMerchantUI(merchant, inventory);
        ui.OpenDialogueUI(firstDialogueLine);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        inventory = player.GetComponent<InventoryPlayer>();
        merchant.SetInventory(inventory);
    }
    private void CloseUI()
    {
        ui.HideAllTooltips();

        if (ui.merchantUI != null)
            ui.OpenMerchantUI(false);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        if (ui == null)
            return;

        ui.HideAllTooltips();

        if (ui.merchantUI != null)
            ui.OpenMerchantUI(false);
    }

}
