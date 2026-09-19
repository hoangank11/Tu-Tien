using System;
using UnityEngine;

[Serializable]
public class AttackData
{
    public float damage;
    public float eleDamage;
    public bool isCrit;
    public ElementalType element;

    public ElementalDataEffect effectData;


    public AttackData(Entity_Start start, ScaleFactor scaleFactor, ElementalType elementalType = ElementalType.None)
    {
        damage = start.GetDamage(out isCrit, scaleFactor.xPhysicalDamageMulti);
        element = elementalType;
        eleDamage = start.GetElementalDamage(elementalType, scaleFactor.xElementalDamageMulti);
        effectData = new ElementalDataEffect(start, scaleFactor);
    }
}
