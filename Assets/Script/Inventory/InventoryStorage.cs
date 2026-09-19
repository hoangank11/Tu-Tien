using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryStorage : InventoryBase
{
    public InventoryPlayer playerInventory { get; private set; }
    public List<InventoryItem> materialStash;

    public void ConsumeMaterial(InventoryItem itemToCraft)
    {
        foreach (var requiredItem in itemToCraft.itemData.craftRecipe)
        {
            int amountToConsume = requiredItem.stackSize;
            amountToConsume = amountToConsume - ConsumeMaterialAmount(playerInventory.itemList, requiredItem);
            if (amountToConsume > 0)
                amountToConsume = amountToConsume - ConsumeMaterialAmount(itemList, requiredItem);

            if (amountToConsume > 0)
                amountToConsume = amountToConsume - ConsumeMaterialAmount(materialStash, requiredItem);

        }
    }

    private int ConsumeMaterialAmount(List<InventoryItem> itemList, InventoryItem needItem)
    {
        int amountNeeded = needItem.stackSize;
        int consumeAmount = 0;
        foreach (var item in itemList)
        {
            if (item.itemData != needItem.itemData)
                continue;

            int removeAmount = Mathf.Min(item.stackSize, amountNeeded - consumeAmount);
            item.stackSize -= removeAmount;
            consumeAmount += removeAmount;
            if (item.stackSize <= 0)
                itemList.Remove(item);
            if (consumeAmount >= amountNeeded)
                break;
        }

        return consumeAmount;
    }

    public bool HasEnoughtMaterial(InventoryItem itemToCraft)
    {
        foreach (var requiredMaterial in itemToCraft.itemData.craftRecipe)
        {
            if (GetAvailableAmountOf(requiredMaterial.itemData) < requiredMaterial.stackSize)
                return false;
        }
        return true;
    }
    public int GetAvailableAmountOf(ItemDataSO requiredItem)
    {
        int amount = 0;
        foreach (var item in playerInventory.itemList)
        {
            if (item.itemData == requiredItem)
                amount += item.stackSize;
        }

        foreach (var item in itemList)
        {
            if (item.itemData == requiredItem)
                amount += item.stackSize;
        }

        foreach (var item in materialStash)
        {
            if (item.itemData == requiredItem)
                amount += item.stackSize;
        }

        return amount;

    }

    public void AddMaterialToStash(InventoryItem itemToAdd)
    {
        var stackableItem = StackableInStash(itemToAdd);
        if (stackableItem != null)
            stackableItem.AddStack();
        else
        {
            var newItemToAdd = new InventoryItem(itemToAdd.itemData);
            materialStash.Add(newItemToAdd);
        }

        TriggerUpdateUI();
        materialStash = materialStash.OrderBy(item => item.itemData.name).ToList();
    }

    public InventoryItem StackableInStash(InventoryItem itemToAdd)
    {
        return materialStash.Find(item => item.itemData == itemToAdd.itemData && item.CanAddStack());
    }
    public void RemoveMaterialFromStash(InventoryItem itemToRemove)
    {
        InventoryItem itemInStash = materialStash.Find(item => item == itemToRemove);
        if (itemInStash == null)
            return;

        if (itemInStash.stackSize > 1)
            itemInStash.RemoveStack();
        else
            materialStash.Remove(itemToRemove);

        TriggerUpdateUI();
    }
    public void SetInventory(InventoryPlayer inventory) => this.playerInventory = inventory;

    public void FromPlayerToStorage(InventoryItem item, bool transferFullStack)
    {
        bool isMaterial = item.itemData.itemType == ItemType.Material;
        int transferAmount = transferFullStack ? item.stackSize : 1;
        for (int i = 0; i < transferAmount; i++)
        {
            if (isMaterial || CanAddItem(item))
            {
                var itemToAdd = new InventoryItem(item.itemData);
                playerInventory.RemoveOneItem(item);
                if (isMaterial)
                    AddMaterialToStash(itemToAdd);
                else
                    AddItem(itemToAdd);
            }

        }

        TriggerUpdateUI();
    }

    public void FromStorageToPlayer(InventoryItem item, bool transferFullStack)
    {
        bool isMaterial = item.itemData.itemType == ItemType.Material;
        int transferAmount = transferFullStack ? item.stackSize : 1;
        for (int i = 0; i < transferAmount; i++)
        {
            if (playerInventory.CanAddItem(item))
            {
                var itemToAdd = new InventoryItem(item.itemData);
                if (isMaterial)
                    RemoveMaterialFromStash(item);
                else
                    RemoveOneItem(item);
                playerInventory.AddItem(itemToAdd);
            }

        }
        TriggerUpdateUI();
    }


    #region Save/Load System
    public override void SaveData(ref GameData data)
    {
        base.SaveData(ref data);

        data.storageItems.Clear();

        foreach (var item in itemList)
        {
            if (item != null && item.itemData != null)
            {
                string saveID = item.itemData.saveID;

                if (data.storageItems.ContainsKey(saveID) == false)
                    data.storageItems[saveID] = 0;

                data.storageItems[saveID] += item.stackSize;
            }
        }

        data.storageMaterials.Clear();

        foreach (var item in materialStash)
        {
            if (item != null && item.itemData != null)
            {
                string saveID = item.itemData.saveID;

                if (data.storageMaterials.ContainsKey(saveID) == false)
                    data.storageMaterials[saveID] = 0;

                data.storageMaterials[saveID] += item.stackSize;
            }
        }
    }

    public override void LoadData(GameData data)
    {
        itemList.Clear();
        materialStash.Clear();

        // storage item
        foreach (var entry in data.storageItems)
        {
            string saveID = entry.Key;
            int stack = entry.Value;

            ItemDataSO itemData = itemDataBase.GetItemData(saveID);
            if (itemData == null)
            {
                Debug.LogWarning("Không tìm thấy item: " + saveID);
                continue;
            }

            for (int i = 0; i < stack; i++)
            {
                InventoryItem itemToLoad = new InventoryItem(itemData);
                AddItem(itemToLoad);
            }
        }

        // storage material
        foreach (var entry in data.storageMaterials)
        {
            string saveID = entry.Key;
            int stack = entry.Value;

            ItemDataSO itemData = itemDataBase.GetItemData(saveID);
            if (itemData == null)
            {
                Debug.LogWarning("Không tìm thấy item: " + saveID);
                continue;
            }

            for (int i = 0; i < stack; i++)
            {
                InventoryItem itemToLoad = new InventoryItem(itemData);
                AddMaterialToStash(itemToLoad);
            }
        }
    }

    #endregion
}
