using UnityEditor;
using UnityEngine;


[CreateAssetMenu(menuName = "Wuxia Setup/Item/Item", fileName = "Item - ")]
public class ItemDataSO : ScriptableObject
{
    public string saveID;

    [Header("Merchant Detail")]
    public MoneyType moneyType;
    public int itemPrice = 0;
    public int minStackSizeAtShop = 1;
    public int maxStackSizeAtShop = 10;

    [Header("Drop Detail")]
    [Range(0, 1000)]
    public int itemRarity = 100;
    [Range(0, 100)]
    public float dropChance;
    [Range(0, 100)]
    public float maxDropChance = 50f;
    public Vector3 imgScale;

    [Header("Item Detail")]
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    [TextArea]
    public string description;
    public int maxStackSize = 99;

    [Header("Item Effect")]
    public ItemEffectDataSO itemEffect;

    [Header("Craft Details")]
    public InventoryItem[] craftRecipe;


    private void OnValidate()
    {
        dropChance = GetDropChance();

#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        saveID = AssetDatabase.AssetPathToGUID(path);
#endif

    }

    public float GetDropChance()
    {
        float maxRarity = 1000;
        float chance = (maxRarity - itemRarity) / maxRarity * 100;
        return Mathf.Min(chance, maxDropChance);
    }
}
