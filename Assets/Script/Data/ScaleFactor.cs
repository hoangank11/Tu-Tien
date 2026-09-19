using System;
using UnityEngine;

[Serializable]
public class ScaleFactor
{
    [Header("Damage")]
    public float xPhysicalDamageMulti = 1f;
    public float xElementalDamageMulti = 1f;

    [Header("Toxic")]
    [Range(1, 10)]
    public float effectToxicDuration = 6f;
    public float toxicScaleDmgDoT = .25f;
    [Range(0, 1)]
    public float toxicSlowAnimation = .15f;

    [Header("Ice")]
    [Range(1, 10)]
    public float effectIceDuration = 3f;
    [Range(0, 1)]
    public float iceSlowAnimation = .3f;

    [Header("Fire")]
    [Range(1, 10)]
    public float effectFireDuration = 6f;
    public float fireScaleDmgDoT = .25f;

    [Header("Electric")]
    [Range(1, 10)]
    public float effectElectricDuration = 4f;
    public float electricScaleDmg = 1f;
    public float electricChargePerHit = 1f;

    [Header("Wind")]
    [Min(0f)]
    public float airBorne = 2.5f;
    [Min(0f)]
    public float stunTimeWindElemental = 1.5f;

    [Header("Earth")]
    [Range(0, 1)]
    public float stunChange = 0.25f;
    [Range(0, 1)]
    public float reduceMaxArmor = 0.15f;
    [Min(0f)]
    public float earthReduceDuration = 4f;

    [Header("Wood")]
    [Min(0f)]
    public float woodRootDuration = 4f;

    [Header("Water")]
    [Min(0f)]
    public float waterDuration = 4f;
    [Range(0, 1)]
    public float waterTrueDamageChance = 0.25f;
    [Range(0, 1)]
    public float waterHealPercent = 0.25f;
}
