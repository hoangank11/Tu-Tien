using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftPreviewMaterialSlots : MonoBehaviour
{
    [SerializeField] private Image materialIcon;
    [SerializeField] private TextMeshProUGUI materialName;
    [SerializeField] private TextMeshProUGUI materialAmount;


    public void SetupMaterialSlot(ItemDataSO itemData, int avalibaleAmount, int requiredAmount)
    {
        materialIcon.sprite = itemData.itemIcon;
        materialName.text = itemData.itemName;
        materialAmount.text = avalibaleAmount + "/" + requiredAmount;


    }

}
