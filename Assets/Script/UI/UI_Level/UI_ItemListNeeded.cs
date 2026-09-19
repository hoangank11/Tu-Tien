using System.Collections.Generic;
using UnityEngine;

public class UI_ItemListNeeded : MonoBehaviour
{
    private InventoryPlayer inventory;
    private Player_BodyLvl playerBodyLvl;
    private UI_ItemNeed template;
    private readonly List<UI_ItemNeed> spawnedItems = new List<UI_ItemNeed>();

    private void Awake()
    {
        inventory = FindAnyObjectByType<InventoryPlayer>();
        playerBodyLvl = FindAnyObjectByType<Player_BodyLvl>();

        template = GetComponentInChildren<UI_ItemNeed>(true);

        if (template == null)
        {
            Debug.LogWarning("UI_ItemListNeeded: Không tìm thấy ItemNeed làm template.");
            return;
        }

        // Xóa các ItemNeed placeholder còn lại. Template đầu tiên sẽ được clone khi cần.
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child != template.transform)
                Destroy(child.gameObject);
        }

        template.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (inventory == null)
            inventory = FindAnyObjectByType<InventoryPlayer>();

        if (playerBodyLvl == null)
            playerBodyLvl = FindAnyObjectByType<Player_BodyLvl>();

        ClearSpawnedItems();

        if (template == null || playerBodyLvl == null)
            return;

        ItemListDataSO required = playerBodyLvl.GetCurrentRequiredItems();

        if (required == null || required.itemList == null)
            return;

        for (int i = 0; i < required.itemList.Length; i++)
        {
            ItemDataSO item = required.itemList[i];
            if (item == null)
                continue;

            int requiredAmount = required.GetRequiredAmount(i);
            int currentAmount = inventory != null
                ? GetCurrentAmount(item)
                : 0;

            UI_ItemNeed uiItem = Instantiate(template, transform);
            uiItem.gameObject.SetActive(true);
            uiItem.SetItem(item, currentAmount, requiredAmount);
            spawnedItems.Add(uiItem);
        }
    }

    private int GetCurrentAmount(ItemDataSO item)
    {
        int total = 0;

        foreach (InventoryItem inventoryItem in inventory.itemList)
        {
            if (inventoryItem != null && inventoryItem.itemData == item)
                total += inventoryItem.stackSize;
        }

        return total;
    }

    private void ClearSpawnedItems()
    {
        for (int i = spawnedItems.Count - 1; i >= 0; i--)
        {
            if (spawnedItems[i] != null)
                Destroy(spawnedItems[i].gameObject);
        }

        spawnedItems.Clear();
    }
}
