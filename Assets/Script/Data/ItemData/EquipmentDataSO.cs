using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Wuxia Setup/Item/Equipment", fileName = "Equipment - ")]
public class EquipmentDataSO : ItemDataSO
{
    [Header("Item Modifiers")]
    public ItemModifier[] modifiers;


}


[Serializable]
public class ItemModifier
{
    public StartType startType;
    public float value;
}