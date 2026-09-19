using UnityEngine;

public class Treasurer : NPC, IInteractable
{
    private InventoryPlayer inventory;
    [SerializeField] private InventoryStorage storage;

    protected override void Awake()
    {
        base.Awake();
        storage = GetComponent<InventoryStorage>();
    }
    public override void Interact()
    {
        ui.storageUI.SetupStorage(inventory, storage);
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
            ui.OpenStorageUI(false);
    }

}
