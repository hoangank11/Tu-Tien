using UnityEngine;
using System;

[Serializable]
public class Start_BaseValue
{
    [Header("Base Valuation -------------------------------------")]
    [Header("Attack Speed")]
    public StartValuation attackSpeed;
    [Header("Armor & Magic Armor")]
    public StartValuation maxArmor; //giới hạn armor nên đặt khuyến khích 567 armor vì giới hạn mitigation là 85%
    public StartValuation capMitigation;
    [Header("Elemental Resistance")]
    public StartValuation maxFireRes;
    public StartValuation maxIceRes;
    public StartValuation maxToxicRes;
    public StartValuation maxElectricRes;
    public StartValuation maxWindRes;
    public StartValuation maxEarthRes;
    public StartValuation maxWoodRes;
    public StartValuation maxWaterRes;
    public StartValuation capElementalRes;  // giới hạn của elemental resistance là 60%
    [Header("Dodge")]
    public StartValuation maxEvasion;
    [Header("Damage")]
    public StartValuation maxDmg;
    public StartValuation maxArmorPenetration; // chỉ số xuyên giáp
    [Header("Elementals")]
    public StartValuation maxElementalDmg;
    public StartValuation maxElectricChargeDmg;
    [Header("Crit")]
    public StartValuation maxCritChance;
    public StartValuation maxCritDmg;
}
