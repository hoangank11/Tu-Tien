using System.Collections.Generic;
using UnityEngine;

public class Player_DropManager : Entity_DropManager
{
    [Header("Player Drop Detail")]
    [Range(0f, 100f)]
    [SerializeField] private float changeToLoseItem = 0f;
    private InventoryPlayer inventory;

    private void Awake()
    {
        inventory = GetComponent<InventoryPlayer>();
    }

    public override void DropItem()
    {
        
        List<InventoryItem> inventoryCopy = new List<InventoryItem>(inventory.itemList);

        foreach (var item in inventoryCopy)
        {
            if (Random.Range(0, 100) < changeToLoseItem)
            {
                CreateItemDrop(item.itemData);
                inventory.RemoveOneItem(item);
            }
        } // ok khi chết sẽ mất 1 item bất kỳ trong inventory

    }

}
