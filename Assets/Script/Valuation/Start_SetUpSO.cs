using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Start Setup", fileName = "Default Setup - ")]
public class Start_SetUpSO : ScriptableObject
{
    [Header("----------------------------------Dòng Cơ Bản----------------------------------")]
    [Header("HP/MP/Regen")]
    public float maxHP = 100f;
    public float hpRegen = 0f;
    public float maxMP = 10f;
    public float mpRegen = 0f;

    [Header("Damage/Crit/PhysArmorPenetration")]
    public float maxDmg = 5f;
    public float maxCritChance;
    public float maxCritDmg = .2f;
    public float maxArmorPenetration;
    public float maxAttackSpeed = 1f;

    [Header("Elemental Damage")]
    public float maxElementalDmg = 5f;
    public float maxElectricChargeDmg = 1.5f;

    [Header("Armor")]
    public float maxArmor = 0f;
    public float capMitigation = 0.85f;

    [Header("Elemental Res")]
    public float maxFireRes = 0f;
    public float maxIceRes = 0f;
    public float maxToxicRes = 0f;
    public float maxElectricRes = 0f;
    public float maxWindRes = 0f;
    public float maxEarthRes = 0f;
    public float maxWoodRes = 0f;
    public float maxWaterRes = 0f;
    public float capElementalRes = 30f;

    [Header("Dodge")]
    public float maxEvasion = 0f;


    [Header("----------------------------------Dòng Nâng Cấp----------------------------------")]
    [Header("Inc HP/MP")]
    public float incHPvsMP = 0f;
    [Header("Inc Armor/Res")]
    public float incArmor = 0f;
    public float incEvasion = 0f;
    public float incFireRes = 0f;
    public float incIceRes = 0f;
    public float incToxicRes = 0f;
    public float incElectricRes = 0f;
    public float incWindRes = 0f;
    public float incEarthRes = 0f;
    public float incWoodRes = 0f;
    public float incWaterRes = 0f;
    [Header("Inc Damage")]
    public float incDmg = 0f;
    [Header("Inc Elementals")]
    public float incElementalDmg = 0f;
    [Header("Inc Crit")]
    public float incCritChance = 0f;
    public float incCritDmg = 0f;
}
