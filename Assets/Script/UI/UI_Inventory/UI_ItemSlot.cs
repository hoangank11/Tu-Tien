using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    protected UI ui;
    protected RectTransform rect;
    public InventoryItem itemInSlot { get; private set; }
    protected InventoryPlayer inventory;
    [Header("UI Slot Setup")]
    [SerializeField] protected Image itemIcon;
    [SerializeField] protected TextMeshProUGUI itemStackSize;

    protected virtual void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        inventory = FindAnyObjectByType<InventoryPlayer>();
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        // Nguyên Liệu
        if (itemInSlot == null || itemInSlot.itemData.itemType == ItemType.Material)
            return;
        // nhiệm vụ
        if(itemInSlot == null || itemInSlot.itemData.itemType == ItemType.Mission)
            return;
        // Thuốc 
        if (itemInSlot.itemData.itemType == ItemType.Consumable)
        {
            if (itemInSlot.itemEffect.CanBeUse() == false)
                return;

            inventory.TryUseItem(itemInSlot);
        }
        // Trang bị
        else
            inventory.TryEquipItem(itemInSlot);
        // Null
        if (itemInSlot == null)
            ui.itemToolTip.ShowToolTip(false, null);
    }

    public void UpdateSlot(InventoryItem item)
    {
        itemInSlot = item;

        if (itemInSlot == null)
        {
            itemStackSize.text = "";
            itemIcon.color = Color.clear;
            return;
        }

        Color color = Color.white;
        color.a = 1f; // độ mờ icon trong slot. 
        itemIcon.color = color;
        itemIcon.sprite = itemInSlot.itemData.itemIcon;
        itemStackSize.text = item.stackSize > 1 ?item.stackSize.ToString() : "";
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null) return;
        ui.itemToolTip.ShowToolTip(true, rect, itemInSlot); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.itemToolTip.ShowToolTip(false, null);
    }
}
