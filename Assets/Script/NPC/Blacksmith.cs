using UnityEngine;

public class Blacksmith : NPC, IInteractable
{

    private InventoryPlayer inventory;
    [SerializeField] private InventoryStorage storage;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Interact()
    {
        ui.craftUI.SetupCraftUI(storage);
        ui.OpenDialogueUI(firstDialogueLine);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        inventory = player.GetComponent<InventoryPlayer>();
        storage.SetInventory(inventory);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        if (ui == null) return;
        ui.HideAllTooltips();
        if (storage != null)
            ui.OpenCraftUI(false);
    }

}
