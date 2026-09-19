using System;
using UnityEngine;

[Serializable]
public class InventoryEquipmentSlot
{
    public ItemType slotType;
    public InventoryItem equipedItem;

    public bool HasItem() => equipedItem != null && equipedItem.itemData != null;
}
