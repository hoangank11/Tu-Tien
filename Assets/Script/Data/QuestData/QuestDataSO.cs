using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Quest Data/Quest", fileName = "Quest - ")]
public class QuestDataSO : ScriptableObject
{
    public string questSaveID;
    [Space]
    [Header("Thông Tin Quest")]
    public QuestType questType;
    public QuestCategories missionType;
    public string questName;
    [TextArea] public string description;
    [TextArea] public string questGoal;
    // cần có cả required Level ở đây
    public string questTargetID;
    public int requiredAmount;

    [Space]
    [Header("Chỉ dành cho nhiệm vụ là Delivery")]
    public ItemDataSO itemToDelivery;

    [Space]
    [Header("Chỉ dành cho nhiệm vụ là Collect")]
    public ItemDataSO itemNeedToCollect;


    [Space]
    [Header("Phần Thưởng")]
    public RewardType rewardType;
    public InventoryItem[] rewardItems;
    public MoneyType moneyType;
    public int moneyAmount;

    private void OnValidate()
    {

#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        questSaveID = AssetDatabase.AssetPathToGUID(path);

#endif

    }

}
