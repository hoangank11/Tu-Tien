using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Item/Item List", fileName = "List Item - ")]
public class ItemListDataSO : ScriptableObject
{
    public ItemDataSO[] itemList;

    [Tooltip("Số lượng yêu cầu tương ứng với từng item trong itemList. Nếu thiếu phần tử thì mặc định là 1.")]
    public int[] itemAmount;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (itemList == null)
        {
            itemList = new ItemDataSO[0];
            itemAmount = new int[0];
            return;
        }

        if (itemAmount == null || itemAmount.Length != itemList.Length)
        {
            int[] old = itemAmount;
            itemAmount = new int[itemList.Length];

            for (int i = 0; i < itemAmount.Length; i++)
                itemAmount[i] = (old != null && i < old.Length) ? Mathf.Max(1, old[i]) : 1;
        }
        else
        {
            for (int i = 0; i < itemAmount.Length; i++)
                itemAmount[i] = Mathf.Max(1, itemAmount[i]);
        }
    }
#endif

    public int GetRequiredAmount(int index)
    {
        if (itemAmount == null || index < 0 || index >= itemAmount.Length)
            return 1;

        return Mathf.Max(1, itemAmount[index]);
    }

    public ItemDataSO GetItemData(string saveID)
    {
        return itemList.FirstOrDefault(item => item != null && item.saveID == saveID);
    }

    // Dùng bởi UI_Hack: tìm item theo itemName (không phân biệt hoa thường, bỏ khoảng trắng thừa).
    public ItemDataSO GetItemDataByName(string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName) || itemList == null)
            return null;

        string trimmedName = itemName.Trim();
        return itemList.FirstOrDefault(item => item != null &&
            string.Equals(item.itemName, trimmedName, System.StringComparison.OrdinalIgnoreCase));
    }

#if UNITY_EDITOR
    [ContextMenu("Tự động điền toàn bộ các Item đang có")]
    public void CollectItemsData()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemDataSO");
        itemList = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<ItemDataSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(item => item != null)
            .ToArray();
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
