using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Skill Data Setup/Skill Name", fileName = "Skill Data - ")]

public class SkillDataSO : ScriptableObject
{
    [Header("Skill Type")]
    public bool unlockTest;
    public SkillName skillByName;

    [Header("Resource")]
    public float mpCost;
    public float cooldown;
    public ScaleFactor scaleFactor;


    [Header("Skill description")]
    public string skillName;
    public LevelType level;
    public int skillPoint;
    [TextArea]
    public string description;
    public Sprite icon;

}