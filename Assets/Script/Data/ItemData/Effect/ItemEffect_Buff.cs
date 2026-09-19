using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Item/Item Effect/Buff Effect", fileName = "Item Effect - Buff")]
public class ItemEffect_Buff : ItemEffectDataSO
{
    [SerializeField] private BuffEffectData[] buffToApply;
    [SerializeField] private float duration;
    [SerializeField] private string source = Guid.NewGuid().ToString();
    private PlayerInformation playerInfor;

    public override bool CanBeUse()
    {
        if (playerInfor == null)
            playerInfor = FindAnyObjectByType<PlayerInformation>();
        if (playerInfor.CanApplyBuff(source))
            return true;
        else
        {
            Debug.Log("Same buff");
            return false;
        }
    }

    public override void ExecuteEffect()
    {
        if (playerInfor == null)
            playerInfor = FindAnyObjectByType<PlayerInformation>();
        playerInfor.ApplyBuff(buffToApply, duration, source);
    }
}
