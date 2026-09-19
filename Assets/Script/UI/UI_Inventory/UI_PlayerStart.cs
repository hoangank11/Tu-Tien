using UnityEngine;

public class UI_PlayerStart : MonoBehaviour
{
    private UI_StartSlot[] startSlot;
    private InventoryPlayer inventory;

    private void Awake()
    {
        startSlot = GetComponentsInChildren<UI_StartSlot>();
        inventory = FindFirstObjectByType<InventoryPlayer>();
        inventory.OnInventoryChange += UpdateStartUI;
    }

    private void Start()
    {
        UpdateStartUI();
    }

    private void UpdateStartUI()
    {
        foreach (var slot in startSlot)
            slot.UpdateStartValue();
    }


}
