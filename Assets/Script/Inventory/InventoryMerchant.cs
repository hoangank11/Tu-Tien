using System.Collections.Generic;
using UnityEngine;

public class InventoryMerchant : InventoryBase
{
    private InventoryPlayer inventory;
    [SerializeField] private ItemListDataSO shopData;
    [SerializeField] private int minItemAmount = 5;
    [SerializeField] private int maxItemAmount = 10; // max và min này là số item sẽ đc random để bán trong shop

    protected override void Awake()
    {
        base.Awake();
        FillShopList();
    }

    public void TryBuyItem(InventoryItem itemToBuy, bool buyFullStack)
    {
        MoneyType moneyType = itemToBuy.itemData.moneyType;
        int amountToBuy = buyFullStack ? itemToBuy.stackSize : 1;
        for (int i = 0; i < amountToBuy; i++)
        {
            if (inventory.HasEnoughMoney(moneyType, itemToBuy.buyPrice) == false)
            {
                Debug.Log("Not enought money");
                return;
            }
            if (inventory.CanAddItem(itemToBuy))
            {
                var itemToAdd = new InventoryItem(itemToBuy.itemData);
                inventory.AddItem(itemToAdd);
            }
            inventory.AddMoney(moneyType, -itemToBuy.buyPrice);
            RemoveOneItem(itemToBuy);
        }
        TriggerUpdateUI();
    }

    public void TrySellItem(InventoryItem itemToSell, bool sellFullStack)
    {
        MoneyType moneyType = itemToSell.itemData.moneyType;
        int amountToSell = sellFullStack ? itemToSell.stackSize : 1;
        for (int i = 0; i < amountToSell; i++)
        {
            int sellPrice = Mathf.FloorToInt(itemToSell.sellPrice);
            inventory.AddMoney(moneyType, sellPrice);
            inventory.RemoveOneItem(itemToSell);
        }
        TriggerUpdateUI();
    }


    public void FillShopList()
    {
        itemList.Clear();
        List<InventoryItem> possibleItems = new List<InventoryItem>();
        foreach (var itemData in shopData.itemList)
        {
            int randomSizeStack = Random.Range(itemData.minStackSizeAtShop, itemData.maxStackSizeAtShop + 1);
            int finalStack = Mathf.Clamp(randomSizeStack, 1, itemData.maxStackSize);

            InventoryItem itemToAdd = new InventoryItem(itemData);
            itemToAdd.stackSize = finalStack;
            possibleItems.Add(itemToAdd);
        }
        int randomItemAmount = Random.Range(minItemAmount, maxItemAmount + 1);
        int finalAmount = Mathf.Clamp(randomItemAmount, 1, possibleItems.Count);

        for (int i = 0; i < finalAmount; i++)
        {
            var randomIndex = Random.Range(0, possibleItems.Count);
            var item = possibleItems[randomIndex];
            if (CanAddItem(item))
            {
                possibleItems.Remove(item);
                AddItem(item);
            }
        }
        TriggerUpdateUI();

    }


    public void SetInventory(InventoryPlayer inventory) => this.inventory = inventory;
}
