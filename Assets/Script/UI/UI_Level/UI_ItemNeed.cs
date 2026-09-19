using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ItemNeed : MonoBehaviour
{
    [Header("========== ITEM UI ==========")]

    [SerializeField] private Image itemIcon;

    [SerializeField] private TextMeshProUGUI amountText;

    public void SetItem(
        ItemDataSO itemData,
        int currentAmount,
        int requiredAmount)
    {
        if (itemData == null)
            return;

        // Icon
        if (itemIcon != null)
        {
            itemIcon.sprite = itemData.itemIcon;
        }

        // Số lượng
        if (amountText != null)
        {
            amountText.text =
                currentAmount + " / " + requiredAmount;
        }
    }
}