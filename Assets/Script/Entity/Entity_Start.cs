using UnityEngine;

public class Entity_Start : MonoBehaviour
{
    #region Base Valuation
    public Start_SetUpSO defaultSetup;
    public Start_NormalValue normalGroup;
    public Start_BaseValue baseGroup;
    public Start_IncreaseValue incGroup;
    #endregion

    private float temporaryMaxArmorReduction;

    protected virtual void Awake()
    {

    }


    public void SetTemporaryArmorReduction(float reduction)
    {
        temporaryMaxArmorReduction = Mathf.Clamp01(reduction);
    }

    public void ClearTemporaryArmorReduction()
    {
        temporaryMaxArmorReduction = 0f;
    }


    public AttackData GetAttackData(ScaleFactor scaleFactor, ElementalType elementalType = ElementalType.None)
    {
        return new AttackData(this, scaleFactor, elementalType);
    }

    // Elemental Damage dùng chung cho tất cả nguyên tố.
    // Chỉ maxElementalDmg + incElementalDmg được dùng để tính sát thương.
    public float GetElementalDamage(ElementalType elementalType, float scaleDamage = 1f)
    {
        if (elementalType == ElementalType.None)
            return 0f;

        float baseElementalDamage = baseGroup.maxElementalDmg.GetValue();
        float bonusElementalDamage = incGroup.incElementalDmg.GetValue();
        float finalDamage = baseElementalDamage + bonusElementalDamage;

        return Mathf.Max(0f, finalDamage) * scaleDamage;
    }

    public float GetElementalResistance(ElementalType elementalType)
    {
        float baseResistance = 0;
        float bonusResIce = incGroup.incIceRes.GetValue();
        float bonusResFire = incGroup.incFireRes.GetValue();
        float bonusResToxic = incGroup.incToxicRes.GetValue();
        float bonusResElec = incGroup.incElectricRes.GetValue();
        float bonusResWind = incGroup.incWindRes.GetValue();
        float bonusResEarth = incGroup.incEarthRes.GetValue();
        float bonusResWood = incGroup.incWoodRes.GetValue();
        float bonusResWater = incGroup.incWaterRes.GetValue();

        switch (elementalType)
        {
            case ElementalType.Fire:
                baseResistance = baseGroup.maxFireRes.GetValue() + bonusResFire;
                break;
            case ElementalType.Ice:
                baseResistance = baseGroup.maxIceRes.GetValue() + bonusResIce;
                break;
            case ElementalType.Toxic:
                baseResistance = baseGroup.maxToxicRes.GetValue() + bonusResToxic;
                break;
            case ElementalType.Electric:
                baseResistance = baseGroup.maxElectricRes.GetValue() + bonusResElec;
                break;
            case ElementalType.Wind:
                baseResistance = baseGroup.maxWindRes.GetValue() + bonusResWind;
                break;
            case ElementalType.Earth:
                baseResistance = baseGroup.maxEarthRes.GetValue() + bonusResEarth;
                break;
            case ElementalType.Wood:
                baseResistance = baseGroup.maxWoodRes.GetValue() + bonusResWood;
                break;
            case ElementalType.Water:
                baseResistance = baseGroup.maxWaterRes.GetValue() + bonusResWater;
                break;
        }

        float resistance = baseResistance;
        float resCap = baseGroup.capElementalRes.GetValue();  // 60 là chỉ số đẹp nhất tức 60%
        float finalRes = Mathf.Clamp(resistance, 0f, resCap) / 100;

        return finalRes;
    }

    public float GetDamage(out bool isCrit, float scaleDamage)
    {
        float baseDamage = GetBaseDamage();
        float baseCritChance = GetCritChange();
        float baseCritDmg = GetCritPower() / 100f;

        isCrit = UnityEngine.Random.Range(0, 100) < baseCritChance;
        float finalDamage = isCrit ? baseDamage * baseCritDmg : baseDamage;

        return finalDamage * scaleDamage;
    }
    public float GetBaseDamage() => baseGroup.maxDmg.GetValue() + incGroup.incDmg.GetValue();
    public float GetCritChange() => baseGroup.maxCritChance.GetValue() + (incGroup.incCritChance.GetValue() * .25f);
    public float GetCritPower() => baseGroup.maxCritDmg.GetValue() + incGroup.incCritDmg.GetValue() * .15f;

    public float GetArmor(float armorReduction)
    {
        float baseArmor = GetMaxArmor();

        float reductionMutliplier = Mathf.Clamp01(1 - armorReduction); //float reductionMutliplier = Mathf.Clamp(1 - armorReduction, 0, 1);
        float effectPhysArmor = baseArmor * reductionMutliplier;

        float mitigation = effectPhysArmor / (effectPhysArmor + 100);
        float capMitigation = baseGroup.capMitigation.GetValue(); // 0.85 là chỉ số đẹp nhất tức 85%
        float finalMitigaion = Mathf.Clamp(mitigation, 0, capMitigation);
        /* mitigation: giảm thiểu.... độ giảm thiểu dmg nhận vào dựa vào armor vd: nếu có 100 armor thì độ giảm thiểu sẽ là 100/(100+100) = 50%
        vậy 50% chính là số dmg sẽ đc giảm đi khi nhận vào HP của player
        giới hạn của mitigation sẽ được người chơi tự đặt
         */

        return finalMitigaion;

    }
    public float GetMaxArmor()
    {
        float rawArmor = baseGroup.maxArmor.GetValue() + incGroup.incArmor.GetValue();
        return rawArmor * (1f - temporaryMaxArmorReduction);
    }


    public float GetArmorPenetration()
    {
        float finalReduction = baseGroup.maxArmorPenetration.GetValue() / 100;
        return finalReduction;
    }


    public float GetMaxHeatlh()
    {
        float baseHP = normalGroup.maxHP.GetValue();
        float earnHP = incGroup.incHPvsMP.GetValue() * 5;
        float finalHP = baseHP + earnHP;
        return finalHP;
    }

    public float GetMaxMP()
    {
        return normalGroup.maxMP.GetValue();
    }

    public float GetMPRegen()
    {
        return normalGroup.MPRegen.GetValue();
    }

    public float GetEvasion()
    {
        float baseEvation = baseGroup.maxEvasion.GetValue();
        float bonusEvation = incGroup.incEvasion.GetValue() * .2f;
        float totalEvasion = baseEvation + bonusEvation;
        float evasionMax = 80f; // chỉ số max né tránh có thể đạt được trong game
        float finalEvasion = Mathf.Clamp(totalEvasion, 0, evasionMax);

        return finalEvasion;

    }

    public StartValuation GetValueByType(StartType type)
    {
        switch (type)
        {
            // Tổng hợp chỉ số về HP và MP
            case StartType.HP: return normalGroup.maxHP;
            case StartType.HPRegen: return normalGroup.HPRegen;
            case StartType.MP: return normalGroup.maxMP;
            case StartType.MPRegen: return normalGroup.MPRegen;

            // Tổng hợp chỉ số về các Armor
            case StartType.Armor: return baseGroup.maxArmor;

            // Tổng hợp các chỉ số về Damage
            case StartType.Damage: return baseGroup.maxDmg;
            case StartType.ArmorPenetration: return baseGroup.maxArmorPenetration;
            case StartType.CritChance: return baseGroup.maxCritChance;
            case StartType.CritDmg: return baseGroup.maxCritDmg;
            case StartType.AttackSpeed: return baseGroup.attackSpeed;

            // Tổng hợp chỉ số né
            case StartType.Evasion: return baseGroup.maxEvasion;

            // Tổng hợp chỉ số về Elemental Damage 
            case StartType.ElementalDmg:
                return baseGroup.maxElementalDmg;
            case StartType.ElectricChargeDmg:
                return baseGroup.maxElectricChargeDmg;

            // Tổng hợp chỉ số về Elemental Resistance
            case StartType.FireRes: return baseGroup.maxFireRes;
            case StartType.IceRes: return baseGroup.maxIceRes;
            case StartType.ToxicRes: return baseGroup.maxToxicRes;
            case StartType.ElectricRes: return baseGroup.maxElectricRes;
            case StartType.WindRes: return baseGroup.maxWindRes;
            case StartType.EarthRes: return baseGroup.maxEarthRes;
            case StartType.WoodRes: return baseGroup.maxWoodRes;
            case StartType.WaterRes: return baseGroup.maxWaterRes;

            default:
                return null;
        }
    }

    // Setup các Database trong game cho các nhân vật
    [ContextMenu("Load lại Default Setup")]
    public void ApplyDefaultSetup()
    {
        if (defaultSetup == null)
        {
            Debug.Log("Không có Default Setup!");
            return;
        }

        // Dòng HP/MP Regen
        normalGroup.maxHP.SetBaseValue(defaultSetup.maxHP);
        normalGroup.maxMP.SetBaseValue(defaultSetup.maxMP);
        normalGroup.MPRegen.SetBaseValue(defaultSetup.mpRegen);
        normalGroup.HPRegen.SetBaseValue(defaultSetup.hpRegen);

        // Dòng Damage/Crit/ArmorPenetration
        baseGroup.maxDmg.SetBaseValue(defaultSetup.maxDmg);
        baseGroup.maxCritChance.SetBaseValue(defaultSetup.maxCritChance);
        baseGroup.maxCritDmg.SetBaseValue(defaultSetup.maxCritDmg);
        baseGroup.maxArmorPenetration.SetBaseValue(defaultSetup.maxArmorPenetration);
        baseGroup.attackSpeed.SetBaseValue(defaultSetup.maxAttackSpeed);

        // Dòng Elemental Damage dùng chung cho mọi nguyên tố
        baseGroup.maxElementalDmg.SetBaseValue(defaultSetup.maxElementalDmg);
        baseGroup.maxElectricChargeDmg.SetBaseValue(defaultSetup.maxElectricChargeDmg);

        // Dòng Armor
        baseGroup.maxArmor.SetBaseValue(defaultSetup.maxArmor);
        baseGroup.capMitigation.SetBaseValue(defaultSetup.capMitigation);

        // Dòng Elemental Res
        baseGroup.maxFireRes.SetBaseValue(defaultSetup.maxFireRes);
        baseGroup.maxIceRes.SetBaseValue(defaultSetup.maxIceRes);
        baseGroup.maxToxicRes.SetBaseValue(defaultSetup.maxToxicRes);
        baseGroup.maxElectricRes.SetBaseValue(defaultSetup.maxElectricRes);
        baseGroup.maxWindRes.SetBaseValue(defaultSetup.maxWindRes);
        baseGroup.maxEarthRes.SetBaseValue(defaultSetup.maxEarthRes);
        baseGroup.maxWoodRes.SetBaseValue(defaultSetup.maxWoodRes);
        baseGroup.maxWaterRes.SetBaseValue(defaultSetup.maxWaterRes);

        // Dòng Dodge
        baseGroup.maxEvasion.SetBaseValue(defaultSetup.maxEvasion);


        // ----------------------------------------------------------------------------------------------------

        // Dòng nâng cấp HP/MP
        incGroup.incHPvsMP.SetBaseValue(defaultSetup.incHPvsMP);

        // Dòng nâng cấp Armor
        incGroup.incArmor.SetBaseValue(defaultSetup.incArmor);

        // Dòng nâng cấp Damage
        incGroup.incDmg.SetBaseValue(defaultSetup.incDmg);

        // Dòng nâng cấp Elemental Damage dùng chung cho mọi nguyên tố
        incGroup.incElementalDmg.SetBaseValue(defaultSetup.incElementalDmg);

        // Dòng nâng cấp Crit
        incGroup.incCritDmg.SetBaseValue(defaultSetup.incCritDmg);
        incGroup.incCritChance.SetBaseValue(defaultSetup.incCritChance);
    }


}
