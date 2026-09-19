using UnityEngine;


[CreateAssetMenu(menuName = "Wuxia Setup/Item/Item Effect/SkillPoint Hack", fileName = "Item Effect - SkillPoint")]
public class ItemEffectLinhThach : ItemEffectDataSO
{
    [SerializeField] private int skillPointsToAdd;


    public override void ExecuteEffect()
    {
        UI ui = FindFirstObjectByType<UI>();
        ui.skillTreeUI.AddSkillPoint(skillPointsToAdd);
    }
}
