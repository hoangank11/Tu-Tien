using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryBase : MonoBehaviour, ISaveable
{
    public event Action OnInventoryChange;

    public int maxInventorySize = 20;
    public List<InventoryItem> itemList = new List<InventoryItem>();

    [Header("Item Data Base")]
    [SerializeField] protected ItemListDataSO itemDataBase;


    protected virtual void Awake()
    {

    }
    public void TryUseItem(InventoryItem itemToUse)
    {
        InventoryItem consumable = itemList.Find(item => item == itemToUse);
        if (consumable == null)
            return;

        consumable.itemEffect.ExecuteEffect();
        if (consumable.stackSize > 1)
            consumable.RemoveStack();
        else
            RemoveOneItem(consumable);
        OnInventoryChange?.Invoke();
    }

    public bool CanAddItem(InventoryItem itemToAdd)
    {
        bool hasStackAble = FindStackable(itemToAdd) != null;
        return hasStackAble || itemList.Count < maxInventorySize;

    }
    public InventoryItem FindStackable(InventoryItem itemToAdd)
    {
        
        return itemList.Find(item => item.itemData == itemToAdd.itemData && item.CanAddStack());
    }

    public void AddItem(InventoryItem itemToAdd)
    {
        InventoryItem itemInInventory = FindStackable(itemToAdd);

        if (itemInInventory != null)
            itemInInventory.AddStack();
        else
            itemList.Add(itemToAdd);

        OnInventoryChange?.Invoke();
    }

    public void RemoveOneItem(InventoryItem itemToRemove)
    {
        InventoryItem itemInInventory = itemList.Find(item => item == itemToRemove);
        if (itemInInventory.stackSize > 1)
            itemInInventory.RemoveStack();
        else
            itemList.Remove(itemToRemove);

        OnInventoryChange?.Invoke();
    }

    public void RemoveFullStack(InventoryItem itemToRemove)
    {
        for (int i = 0; i < itemToRemove.stackSize; i++)
        {
            RemoveOneItem(itemToRemove);
        }
    }

    public void RemoveItemAmount(ItemDataSO itemToRemove, int amount)
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            InventoryItem item = itemList[i];
            if (item.itemData != itemToRemove)
                continue;
            int removeCount = (int)MathF.Min(amount, item.stackSize);

            for (int j = 0; j < removeCount; j++)
            {
                RemoveOneItem(item);
                amount--;

                if(amount <= 0) break;
            }
        }
    }

    public bool HasItemAmount(ItemDataSO itemToCheck, int amount)
    {
        int total = 0;
        foreach (var item in itemList)
        {
            if (item.itemData == itemToCheck)
                total += item.stackSize;

            if (total >= amount)
                return true;
        }
        return false;
    }

    public InventoryItem FindItem(InventoryItem itemToFind)
    {
        return itemList.Find(item => item == itemToFind);
    }

    public InventoryItem FindSameItem(InventoryItem itemToFind)
    {
        return itemList.Find(item => item.itemData == itemToFind.itemData);
    }
    public void TriggerUpdateUI() => OnInventoryChange?.Invoke();

    public virtual void LoadData(GameData data)
    {
        
    }

    public virtual void SaveData(ref GameData data)
    {
        
    }
}
