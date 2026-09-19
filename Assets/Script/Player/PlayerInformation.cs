using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PlayerInformation : Entity_Start
{
    private List<string> activeBuff = new List<string>();
    private InventoryPlayer inventory;

    protected override void Awake()
    {
        base.Awake();
        inventory = GetComponent<InventoryPlayer>();
    }

    public bool CanApplyBuff(string source)
    {
        return activeBuff.Contains(source) == false;
    }

    public void ApplyBuff(BuffEffectData[] buffToApply, float duration, string source)
    {
        StartCoroutine(BuffCo(buffToApply, duration, source));
    }

    private IEnumerator BuffCo(BuffEffectData[] buffToApply, float duration, string source)
    {
        activeBuff.Add(source);

        foreach (var buff in buffToApply)
        {
            GetValueByType(buff.type).AddModifier(buff.value, source);
        }

        yield return new WaitForSeconds(duration);

        foreach (var buff in buffToApply)
        {
            GetValueByType(buff.type).RemoveModifier(source);
        }
        inventory.TriggerUpdateUI();
        activeBuff.Remove(source);
    }

}
