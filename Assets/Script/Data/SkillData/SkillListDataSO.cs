using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Skill Data Setup/Skill List", fileName = "List Skill - ")]
public class SkillListDataSO : ScriptableObject
{
    public SkillDataSO[] skillList;
    public SkillDataSO GetSkillData(SkillName name)
    {
        return skillList.FirstOrDefault(skill => skill != null && skill.skillByName == name);
    }
}
