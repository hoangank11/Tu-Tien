using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPlayer : InventoryBase
{
    public event Action<int, InventoryItem> OnQuickItemSlot;

    [Header("Moneys")]
    public int Copper = 0;
    public int LinhThach = 0;
    public int TienThach = 0;

    private Player player;
    public List<InventoryEquipmentSlot> equipList;
    public InventoryStorage storage { get; private set; }

    [Header("Quick Item Slots")]
    [SerializeField] private InventoryItem[] quickItem = new InventoryItem[2];

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
        storage = FindFirstObjectByType<InventoryStorage>();
    }

    public void SetQuickItemSlot(int slotNumber, InventoryItem itemToSet)
    {
        quickItem[slotNumber - 1] = itemToSet;
        OnQuickItemSlot?.Invoke(slotNumber - 1, itemToSet);
    }

    public void TryUseQuickItemInSlot(int slotNumber)
    {
        int finalSlotNumber = slotNumber - 1;
        var itemToUSe = quickItem[finalSlotNumber];
        if (itemToUSe == null)
            return;
        if (itemToUSe.itemEffect.CanBeUse() == false) // test
            return;
        TryUseItem(itemToUSe);
        if (FindItem(itemToUSe) == null)
        {
            quickItem[finalSlotNumber] = FindSameItem(itemToUSe);
        }
        OnQuickItemSlot?.Invoke(finalSlotNumber, quickItem[finalSlotNumber]);
    }

    // Dùng bởi UI_Hack: nhập itemName -> tìm trong itemDataBase -> thêm vào túi đồ.
    // Nếu túi đồ đầy (và item không stack được vào slot có sẵn) thì sẽ không nhận được.
    public bool AddItemByName(string itemName)
    {
        if (itemDataBase == null)
        {
            Debug.LogWarning("[InventoryPlayer] Chưa gán Item Data Base, không thể tìm item.");
            return false;
        }

        ItemDataSO itemData = itemDataBase.GetItemDataByName(itemName);
        if (itemData == null)
        {
            Debug.LogWarning($"[InventoryPlayer] Không tìm thấy vật phẩm có tên: {itemName}");
            return false;
        }

        InventoryItem itemToAdd = new InventoryItem(itemData);

        if (CanAddItem(itemToAdd) == false)
        {
            Debug.Log("[InventoryPlayer] Túi đồ đã đầy, không thể nhận thêm vật phẩm.");
            return false;
        }

        AddItem(itemToAdd);
        return true;
    }

    public int GetMoney(MoneyType moneyType)
    {
        switch (moneyType)
        {
            case MoneyType.Copper: return Copper;
            case MoneyType.LinhThach: return LinhThach;
            case MoneyType.TienThach: return TienThach;
            default: return 0;
        }
    }

    public bool HasEnoughMoney(MoneyType moneyType, int amount) => GetMoney(moneyType) >= amount;

    public void AddMoney(MoneyType moneyType, int amount)
    {
        switch (moneyType)
        {
            case MoneyType.Copper: Copper = Mathf.Max(0, Copper + amount); break;
            case MoneyType.LinhThach: LinhThach = Mathf.Max(0, LinhThach + amount); break;
            case MoneyType.TienThach: TienThach = Mathf.Max(0, TienThach + amount); break;
        }

        // LinhThach chính là CurrentExp trong Player_Level, nên cần báo cho UI (EXP bar, nút Tiến Giai...) cập nhật lại.
        TriggerUpdateUI();
    }

    public void TryEquipItem(InventoryItem item)
    {
        var inventoryItem = FindItem(item);
        var matchingSlots = equipList.FindAll(slot => slot.slotType == item.itemData.itemType);

        // Bước 1: tìm Empty Slot và sử dụng Item
        foreach (var slot in matchingSlots)
        {
            if (slot.HasItem() == false)
            {
                EquipItem(inventoryItem, slot);
                return;
            }
        }

        /* Bước 2: không empty slot 
         * {Bước này khá quan trong cơ bản là trong ItemEffectDataSo có Subcribe và Unsubcribe
         * vậy nên phải UnequipItem rồi tới EquipItem nếu thay đổi vị trí sẽ crash game} */
        var slotToReplace = matchingSlots[0];
        var itemToUnequip = slotToReplace.equipedItem;
        UnequipItem(itemToUnequip, slotToReplace != null);
        EquipItem(inventoryItem, slotToReplace);

    }

    private void EquipItem(InventoryItem itemToEquip, InventoryEquipmentSlot slot)
    {
        float saveHealthPercent = player.health.GetHPPercent();
        slot.equipedItem = itemToEquip;
        slot.equipedItem.AddModifiers(player.start);
        slot.equipedItem.AddItemEffect(player);

        player.health.SetHPToPercent(saveHealthPercent);
        RemoveOneItem(itemToEquip);

        UI_SFX.instance?.PlayEquipItem();
    }

    public void UnequipItem(InventoryItem itemToUnequip, bool replacingItem = false)
    {
        if (CanAddItem(itemToUnequip) == false && replacingItem == false)
        {
            Debug.Log("Không đủ slot! :)");
            return;
        }

        float saveHealthPercent = player.health.GetHPPercent();

        var slotToUnequip = equipList.Find(slot => slot.equipedItem == itemToUnequip);

        if (slotToUnequip != null)
            slotToUnequip.equipedItem = null;

        itemToUnequip.RemoveModifiers(player.start);
        itemToUnequip.RemoveItemEffect();
        player.health.SetHPToPercent(saveHealthPercent);
        AddItem(itemToUnequip);

        UI_SFX.instance?.PlayUnequipItem();
    }


    #region Save/Load Moneys

    public override void SaveData(ref GameData data)
    {
        data.Copper = Copper;
        data.LinhThach = LinhThach;
        data.TienThach = TienThach;

        data.inventory.Clear();
        data.equipItem.Clear();
        foreach (var item in itemList)
        {
            if (item != null && item.itemData != null)
            {
                string saveID = item.itemData.saveID;

                if (data.inventory.ContainsKey(saveID) == false)
                    data.inventory[saveID] = 0;
                data.inventory[saveID] += item.stackSize;
            }
        }

        foreach (var slot in equipList)
        {
            if (slot.HasItem())
                data.equipItem[slot.equipedItem.itemData.saveID] = slot.slotType;
        }
    }

    public override void LoadData(GameData data)
    {
        Copper = data.Copper;
        LinhThach = data.LinhThach;
        TienThach = data.TienThach;

        foreach (var entry in data.inventory)
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

        foreach (var entry in data.equipItem)
        {
            string saveID = entry.Key;
            ItemType loadSlotType = entry.Value;

            ItemDataSO itemData = itemDataBase.GetItemData(saveID);
            InventoryItem itemToLoad = new InventoryItem(itemData);

            var slot = equipList.Find(slot => slot.slotType == loadSlotType && slot.HasItem() == false);

            slot.equipedItem = itemToLoad;
            slot.equipedItem.AddModifiers(player.start);
            slot.equipedItem.AddItemEffect(player);
        }

        TriggerUpdateUI();
    }

    #endregion
}