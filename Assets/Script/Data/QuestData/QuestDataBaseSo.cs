using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Quest Data/Quest Data Base", fileName = "All Quest")]
public class QuestDataBaseSo : ScriptableObject
{
    public QuestDataSO[] allQuests;


    public QuestDataSO GetQuestByID(string id)
    {
        return allQuests.FirstOrDefault(quest => quest != null && quest.questSaveID == id);
    }


#if UNITY_EDITOR
    [ContextMenu("Tự động điền toàn bộ các Quest đang có")]
    public void CollectQuestsData()
    {
        string[] guids = AssetDatabase.FindAssets("t:QuestDataSO");
        allQuests = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<QuestDataSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(quest => quest != null)
            .ToArray();
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
