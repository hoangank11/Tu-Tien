using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftPreview : MonoBehaviour
{
    private InventoryItem itemToCraft;
    private InventoryStorage storage;
    private UI_CraftPreviewMaterialSlots[] craftRecipeSlots;

    [Header("Item Preview Setup")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemInfor;
    [SerializeField] private TextMeshProUGUI buttonText;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void SetupCraftPreview(InventoryStorage storage)
    {
        this.storage = storage;
        craftRecipeSlots = GetComponentsInChildren<UI_CraftPreviewMaterialSlots>();
        foreach (var slot in craftRecipeSlots)
            slot.gameObject.SetActive(false);

    }

    public void ConfirmCraft()
    {
        if (itemToCraft == null)
        {
            buttonText.text = "Nguyên Liệu Đầy Đủ";
            return;
        }
        if (storage.HasEnoughtMaterial(itemToCraft) && storage.playerInventory.CanAddItem(itemToCraft))
        {
            storage.ConsumeMaterial(itemToCraft);
            storage.playerInventory.AddItem(itemToCraft);
            UI_SFX.instance?.PlayConfirmCraft();
        }

        UpdateCraftPreviewSlot();

    }


    public void UpdateCraftPreview(ItemDataSO itemData)
    {
        gameObject.SetActive(true);
        itemToCraft = new InventoryItem(itemData);

        itemIcon.sprite = itemData.itemIcon;
        itemName.text = itemData.itemName;
        itemInfor.text = itemToCraft.GetItemInfo();
        UpdateCraftPreviewSlot();

    }

    private void UpdateCraftPreviewSlot()
    {
        foreach (var slot in craftRecipeSlots)
            slot.gameObject.SetActive(false);

        for (int i = 0; i < itemToCraft.itemData.craftRecipe.Length; i++)
        {
            InventoryItem requiredItem = itemToCraft.itemData.craftRecipe[i];
            int availableAmount = storage.GetAvailableAmountOf(requiredItem.itemData);
            int requiredAmount = requiredItem.stackSize;
            craftRecipeSlots[i].gameObject.SetActive(true);
            craftRecipeSlots[i].SetupMaterialSlot(requiredItem.itemData, availableAmount, requiredAmount);
        }
    }
}
