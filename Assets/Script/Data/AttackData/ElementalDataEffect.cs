using System;
using UnityEngine;


[Serializable]
public class ElementalDataEffect
{
    [Header("Status Effect Details")]
    public float toxicDuration;
    public float toxicDmg;
    public float toxicSlowAnimation;

    public float iceDuration;
    public float iceSlowAnimation;

    public float fireDuration;
    public float fireDmg;

    public float electricDuration;
    public float electricDmg;
    public float electricCharge;

    public float airBorne;
    public float stunTimeWindElemental;

    public float stunChange;
    public float reduceMaxArmor;
    public float earthReduceDuration;

    // Wood
    public float woodDuration;

    // Water
    public float waterDuration;
    public float waterTrueDamageChance;
    public float waterHealPercent;

    public ElementalDataEffect(Entity_Start entityStart, ScaleFactor scaleFactor)
    {
        // Ice
        iceDuration = scaleFactor.effectIceDuration;
        iceSlowAnimation = scaleFactor.iceSlowAnimation;

        // Fire
        fireDuration = scaleFactor.effectFireDuration;
        fireDmg = (entityStart.baseGroup.maxElementalDmg.GetValue() 
            + entityStart.incGroup.incElementalDmg.GetValue())
            * scaleFactor.fireScaleDmgDoT;

        // Toxic
        toxicDuration = scaleFactor.effectToxicDuration;
        toxicDmg = (entityStart.baseGroup.maxElementalDmg.GetValue()
            + entityStart.incGroup.incElementalDmg.GetValue())
            * scaleFactor.toxicScaleDmgDoT;
        toxicSlowAnimation = scaleFactor.toxicSlowAnimation;

        //Electric
        electricDuration = scaleFactor.effectElectricDuration;
        electricDmg = (entityStart.baseGroup.maxElementalDmg.GetValue()
            + entityStart.incGroup.incElementalDmg.GetValue())
            * scaleFactor.electricScaleDmg;
        electricCharge = scaleFactor.electricChargePerHit;

        // Wind
        airBorne = scaleFactor.airBorne;
        stunTimeWindElemental = scaleFactor.stunTimeWindElemental;

        // Earth
        stunChange = scaleFactor.stunChange;
        reduceMaxArmor = scaleFactor.reduceMaxArmor;
        earthReduceDuration = scaleFactor.earthReduceDuration;

        // Wood
        woodDuration = scaleFactor.woodRootDuration;

        // Water
        waterDuration = scaleFactor.waterDuration;
        waterTrueDamageChance = scaleFactor.waterTrueDamageChance;
        waterHealPercent = scaleFactor.waterHealPercent;

    }

}
