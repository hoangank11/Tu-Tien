using UnityEngine;
using System;

[Serializable]
public class Start_IncreaseValue
{
    [Header("Increase Valuation -------------------------------------")]
    [Header("HP & MP")]
    public StartValuation incHPvsMP;
    [Header("Armor")]
    public StartValuation incArmor;
    public StartValuation incEvasion;
    public StartValuation incFireRes;
    public StartValuation incIceRes;
    public StartValuation incToxicRes;
    public StartValuation incElectricRes;
    public StartValuation incWindRes;
    public StartValuation incEarthRes;
    public StartValuation incWoodRes;
    public StartValuation incWaterRes;
    [Header("Damage")]
    public StartValuation incDmg;
    [Header("Elementals")]
    public StartValuation incElementalDmg;
    [Header("Crit")]
    public StartValuation incCritChance;
    public StartValuation incCritDmg;
}
