using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Item/Item Effect/Refund Skill", fileName = "Item Effect - Refund Skill")]
public class ItemEffectRefundSkill : ItemEffectDataSO
{
    public override void ExecuteEffect()
    {
        UI ui = FindFirstObjectByType<UI>();
        ui.skillTreeUI.RefundAllSkill();
    }
}
