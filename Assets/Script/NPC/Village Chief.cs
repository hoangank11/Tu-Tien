using UnityEngine;

public class VillageChief : NPC, IInteractable
{

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();

    }

    public override void Interact()
    {
        ui.OpenDialogueUI(firstDialogueLine);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
    private void CloseUI()
    {
        ui.HideAllTooltips();
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        
    }
}
