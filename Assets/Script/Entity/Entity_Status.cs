using System.Collections;
using UnityEngine;

public class Entity_Status : MonoBehaviour
{
    private Entity entity;
    private Entity_VFX entityVFX;
    private Entity_Start entityStart;
    private Entity_Health entityHealth;

    // Mỗi loại effect chạy độc lập -> có thể chịu Fire + Toxic + Ice + Electric cùng lúc.
    // Chỉ khi trùng loại (Fire-Fire, Ice-Ice...) thì mới "làm mới" (refresh) theo luật riêng.
    // Khi 1 effect chạy hết duration -> tự reset (co = null), nên lần bị dính lại kế tiếp (không
    // trùng với effect nào đang active) sẽ dùng nguyên chỉ số gốc của chiêu thức mới, không so sánh gì cả.

    #region Elemental Details

    [Header("Electric Detail")]
    [SerializeField] private GameObject electricVfx;
    [SerializeField] private float currentCharge;
    [Range(1, 10)]
    [SerializeField] private float maxCharge;
    private Coroutine electricCo;
    private float electricEndTime;

    [Header("Fire Detail")]
    [SerializeField] private GameObject fireVfx;
    private Coroutine fireCo;
    private float fireDps;      // dps GỐC (trước khi bị Armor giảm) - dùng để so sánh khi refresh
    private float fireEndTime;  // thời điểm Fire DoT hiện tại sẽ kết thúc

    [Header("Ice Detail")]
    [SerializeField] private GameObject iceVfx;
    private Coroutine iceCo;
    private float iceEndTime;
    private float iceSlowAnim;

    [Header("Toxic Detail")]
    [SerializeField] private GameObject toxicVfx;
    private Coroutine toxicCo;
    private float toxicDps;
    private float toxicEndTime;

    [Header("Wind Detail")]
    [SerializeField] private GameObject windVfx;
    [SerializeField] private float windAirborneCooldown = 10f;
    [SerializeField] private float airBorne = 3f;
    private Coroutine windCo;
    private float nextWindAirborneTime;

    [Header("Earth Detail")]
    [SerializeField] private GameObject earthVfx;
    private Coroutine earthCo;

    [Header("Wood Detail")]
    [SerializeField] private GameObject woodVfx;
    private Coroutine woodCo;
    private float woodEndTime;

    [Header("Water Detail")]
    [SerializeField] private GameObject waterVfx;
    private Coroutine waterCo;
    private float waterEndTime;
    private float waterTrueDamageChance;
    private float waterHealPercent;

    #endregion
    private void Awake()
    {
        entityHealth = GetComponent<Entity_Health>();
        entityStart = GetComponent<Entity_Start>();
        entity = GetComponent<Entity>();
        entityVFX = GetComponent<Entity_VFX>();
    }

    public void ApplyStatusEffect(ElementalType element, ElementalDataEffect dataEffect, Entity_Start attackerStart = null)
    {
        switch (element)
        {
            case ElementalType.Ice:
                ApplyIceEffect(dataEffect.iceDuration, dataEffect.iceSlowAnimation);
                break;

            case ElementalType.Fire:
                ApplyFireEffect(dataEffect.fireDuration, dataEffect.fireDmg, attackerStart);
                break;

            case ElementalType.Electric:
                ApplyElectricEffect(dataEffect.electricDuration, dataEffect.electricDmg, dataEffect.electricCharge);
                break;

            case ElementalType.Toxic:
                ApplyToxicEffect(dataEffect.toxicDuration, dataEffect.toxicDmg, dataEffect.toxicSlowAnimation);
                break;

            case ElementalType.Wind:
                ApplyWindEffect(airBorne, dataEffect.stunTimeWindElemental);
                break;

            case ElementalType.Earth:
                ApplyEarthEffect(dataEffect.stunChange, dataEffect.reduceMaxArmor, dataEffect.earthReduceDuration);
                break;

            case ElementalType.Wood:
                ApplyWoodEffect(dataEffect.woodDuration);
                break;

            case ElementalType.Water:
                ApplyWaterEffect(dataEffect.waterDuration, dataEffect.waterTrueDamageChance, dataEffect.waterHealPercent);
                break;
        }
    }


    // Remove All Negative Effects
    public void RemoveAllNegativeEffect()
    {
        StopAllCoroutines();
        fireCo = null;
        toxicCo = null;
        electricCo = null;
        iceCo = null;
        windCo = null;
        earthCo = null;
        woodCo = null;
        waterCo = null;
        waterTrueDamageChance = 0f;
        entityStart.ClearTemporaryArmorReduction();
        entity.SetRooted(false);
        entityVFX.StopAllVfx();
    }


    #region Electric Effect

    private void ApplyElectricEffect(float duration, float damage, float charge)
    {
        float electricResistance = entityStart.GetElementalResistance(ElementalType.Electric);
        float finalCharge = charge * (1 - electricResistance);

        // Duration: bên nào còn lại nhiều hơn thì dùng cái đó. Các thông số khác (damage, charge)
        // luôn lấy theo lần apply mới nhất - damage được dùng trực tiếp từ tham số hiện tại lúc kích nổ,
        // charge cộng dồn theo từng lần trúng đòn (không so sánh max).
        float finalDuration = duration;
        if (electricCo != null)
        {
            float remaining = Mathf.Max(0f, electricEndTime - Time.time);
            finalDuration = Mathf.Max(remaining, duration);
            StopCoroutine(electricCo);
        }

        currentCharge = currentCharge + finalCharge;
        if (currentCharge >= maxCharge)
        {
            DoElectricEffect(damage);
            StopElectricEffect();
            return;
        }

        electricEndTime = Time.time + finalDuration;
        electricCo = StartCoroutine(ElectricEffect(finalDuration));

    }

    private void StopElectricEffect()
    {
        currentCharge = 0;
        electricCo = null;
        entityVFX.StopVfx(ElementalType.Electric);
    }

    private void DoElectricEffect(float damage)
    {
        Instantiate(electricVfx, transform.position, Quaternion.identity);
        entityHealth.ReduceHP(damage);
    }

    private IEnumerator ElectricEffect(float duration)
    {
        entityVFX.PlayVfx(duration, ElementalType.Electric);

        yield return new WaitForSeconds(duration);

        StopElectricEffect();
    }

    #endregion


    #region Fire Effect

    private void ApplyFireEffect(float duration, float fireDamage, Entity_Start attackerStart)
    {
        float fireRes = entityStart.GetElementalResistance(ElementalType.Fire);
        float reducedDuration = duration * (1 - fireRes);

        // Fire Resistance reduces each tick's damage.
        float incomingTickDamage = fireDamage * (1 - fireRes);

        float finalDuration = reducedDuration;
        float finalTickDamage = incomingTickDamage;

        // Fire đang active -> làm mới (refresh) theo luật:
        // lấy duration còn lại lớn hơn và damage mỗi tick lớn hơn.
        if (fireCo != null)
        {
            float remaining = Mathf.Max(0f, fireEndTime - Time.time);
            finalDuration = Mathf.Max(remaining, reducedDuration);
            finalTickDamage = Mathf.Max(fireDps, incomingTickDamage);
            StopCoroutine(fireCo);
        }

        fireDps = finalTickDamage;
        fireEndTime = Time.time + finalDuration;

        // Giữ nguyên logic Armor hiện tại của Fire:
        // Armor được áp dụng cho damage của MỖI tick.
        float armorReduction = attackerStart != null ? attackerStart.GetArmorPenetration() : 0f;
        float mitigation = entityStart.GetArmor(armorReduction);
        float mitigatedTickDamage = finalTickDamage * (1 - mitigation);

        fireCo = StartCoroutine(FireEffect(finalDuration, mitigatedTickDamage));
    }

    private IEnumerator FireEffect(float duration, float tickDamage)
    {
        entityVFX.PlayVfx(duration, ElementalType.Fire);
        DoFireEffect();

        float tickInterval = .5f;
        float timer = 0f;

        while (timer < duration)
        {
            // fireDmg is now dealt once every 0.5 seconds.
            entityHealth.ReduceHP(tickDamage);
            yield return new WaitForSeconds(tickInterval);
            timer += tickInterval;
        }

        StopFireEffect();
    }

    private void DoFireEffect()
    {
        Instantiate(fireVfx, transform.position, Quaternion.identity);
    }

    private void StopFireEffect()
    {
        fireCo = null;
        entityVFX.StopVfx(ElementalType.Fire);
    }

    #endregion


    #region Ice Effect 
    private void ApplyIceEffect(float duration, float slowAnim)
    {
        float iceRes = entityStart.GetElementalResistance(ElementalType.Ice);
        float reduceDuration = duration * (1 - iceRes);

        float finalDuration = reduceDuration;
        float finalSlowAnim = slowAnim;

        // Ice đang active -> làm mới theo luật: lấy duration còn lại lớn hơn, slowAnim lớn hơn
        if (iceCo != null)
        {
            float remaining = Mathf.Max(0f, iceEndTime - Time.time);
            finalDuration = Mathf.Max(remaining, reduceDuration);
            finalSlowAnim = Mathf.Max(iceSlowAnim, slowAnim);
            StopCoroutine(iceCo);
        }

        iceSlowAnim = finalSlowAnim;
        iceEndTime = Time.time + finalDuration;

        iceCo = StartCoroutine(IceEffect(finalDuration, finalSlowAnim));
    }

    private IEnumerator IceEffect(float duration, float slowAnim)
    {
        DoIceEffect();
        entity.SlowdownEntity(duration, slowAnim, true); // canOverrideSlow: true để luôn apply đúng thông số đã tính (refresh)
        entityVFX.PlayVfx(duration, ElementalType.Ice);

        yield return new WaitForSeconds(duration);
        StopIceEffect();

    }

    private void DoIceEffect()
    {
        Instantiate(iceVfx, transform.position, Quaternion.identity);
    }

    private void StopIceEffect()
    {
        iceCo = null;
        entityVFX.StopVfx(ElementalType.Ice);
    }

    #endregion


    #region Toxic Effect

    private void ApplyToxicEffect(float duration, float toxicDamage, float slowAnim)
    {
        float toxicRes = entityStart.GetElementalResistance(ElementalType.Toxic);
        float reducedDuration = duration * (1 - toxicRes);

        // Keep the existing Toxic bonus based on target maxArmor, but apply it to each tick.
        float incomingTickDamage;
        if (toxicDamage > entityStart.baseGroup.maxArmor.GetValue())
            incomingTickDamage = toxicDamage * (1 - toxicRes);
        else
            incomingTickDamage = toxicDamage * ((1 - toxicRes) + (entityStart.baseGroup.maxArmor.GetValue() * .1f));

        float finalDuration = reducedDuration;
        float finalTickDamage = incomingTickDamage;

        // Toxic đang active -> làm mới (refresh) theo luật:
        // lấy duration còn lại lớn hơn và damage mỗi tick lớn hơn.
        if (toxicCo != null)
        {
            float remaining = Mathf.Max(0f, toxicEndTime - Time.time);
            finalDuration = Mathf.Max(remaining, reducedDuration);
            finalTickDamage = Mathf.Max(toxicDps, incomingTickDamage);
            StopCoroutine(toxicCo);
        }

        toxicDps = finalTickDamage;
        toxicEndTime = Time.time + finalDuration;

        // Toxic DoT KHÔNG bị Armor giảm - đánh thẳng vào currentHP
        toxicCo = StartCoroutine(ToxicEffect(finalDuration, finalTickDamage, slowAnim));
    }

    private IEnumerator ToxicEffect(float duration, float tickDamage, float slowAnim)
    {
        entityVFX.PlayVfx(duration, ElementalType.Toxic);
        DoToxicEffect();
        entity.SlowdownEntity(duration, slowAnim, true); // canOverrideSlow: true để luôn apply đúng thông số đã tính (refresh)

        float tickInterval = .5f;
        float timer = 0f;

        while (timer < duration)
        {
            // toxicDmg is now dealt once every 0.5 seconds.
            entityHealth.ReduceHP(tickDamage); // không qua Armor
            yield return new WaitForSeconds(tickInterval);
            timer += tickInterval;
        }

        StopToxicEffect();
    }

    private void DoToxicEffect()
    {
        Instantiate(toxicVfx, transform.position, Quaternion.identity);
    }

    private void StopToxicEffect()
    {
        toxicCo = null;
        entityVFX.StopVfx(ElementalType.Toxic);
    }

    #endregion

    #region Wind Effect

    private void ApplyWindEffect(float airBorne, float stunTime)
    {
        if (entity == null)
            return;

        if (Time.time < nextWindAirborneTime)
            return;

        float windRes = entityStart.GetElementalResistance(ElementalType.Wind);
        float finalStunTime = Mathf.Max(0f, stunTime * (1f - windRes));

        nextWindAirborneTime = Time.time + Mathf.Max(0f, windAirborneCooldown);

        if (windCo != null)
            StopCoroutine(windCo);

        windCo = StartCoroutine(WindEffect(airBorne, finalStunTime));
    }

    private IEnumerator WindEffect(float airBorne, float stunTime)
    {
        DoWindEffect();
        entityVFX.PlayVfx(Mathf.Max(0.1f, stunTime), ElementalType.Wind);

        bool landed = false;
        entity.LaunchAirborne(airBorne, () => landed = true);

        while (!landed)
            yield return null;

        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null && stunTime > 0f)
        {
            enemy.ForceStun(stunTime);
        }

        windCo = null;
        StopWindEffect();
    }

    private void DoWindEffect()
    {
        Instantiate(windVfx, transform.position, Quaternion.identity);
    }

    private void StopWindEffect()
    {
        windCo = null;
        entityVFX.StopVfx(ElementalType.Wind);
    }

    #endregion

    #region Earth Effect

    private void ApplyEarthEffect(float stunChange, float reduceMaxArmor, float duration)
    {
        float earthRes = entityStart.GetElementalResistance(ElementalType.Earth);

        // Resistance reduces both stun chance and Armor-reduction duration.
        float finalStunChance = Mathf.Clamp01(stunChange * (1f - earthRes));
        float finalDuration = Mathf.Max(0f, duration * (1f - earthRes));

        if (Random.value < finalStunChance)
        {
            Enemy enemy = GetComponent<Enemy>();
            if (enemy != null)
                enemy.ForceStun();
        }

        if (earthCo != null)
            StopCoroutine(earthCo);

        earthCo = StartCoroutine(EarthArmorReduction(finalDuration, reduceMaxArmor));
    }

    private IEnumerator EarthArmorReduction(float duration, float reduceMaxArmor)
    {
        DoEarthEffect();
        entityStart.SetTemporaryArmorReduction(reduceMaxArmor);
        entityVFX.PlayVfx(duration, ElementalType.Earth);

        yield return new WaitForSeconds(duration);

        entityStart.ClearTemporaryArmorReduction();
        earthCo = null;
        entityVFX.StopVfx(ElementalType.Earth);
        StopEarthEffect();
    }

    private void DoEarthEffect()
    {
        Instantiate(earthVfx, transform.position, Quaternion.identity);
    }

    private void StopEarthEffect()
    {
        earthCo = null;
        entityVFX.StopVfx(ElementalType.Earth);
    }

    #endregion


    #region Wood Effect

    // Root: trong lúc duration còn hiệu lực, entity không thể di chuyển (kể cả nhảy),
    // chỉ có thể quay trái/phải và vẫn tấn công bình thường nếu mục tiêu trong tầm đánh
    // (xem Entity.SetVelocity/SetRooted). Physical Damage nhận vào lúc này sẽ không gây
    // Knockback (xem Entity_Health.TakeDamage), nhưng Wind Elemental vẫn hất tung bình
    // thường vì LaunchAirborne không đi qua SetVelocity. Nếu bị hất tung + choáng khi rơi
    // (logic Wind cũ) mà Wood vẫn còn duration thì coroutine này vẫn tiếp tục chạy song song
    // và giữ IsRooted = true cho tới khi hết duration, nên sau khi StunnedState kết thúc,
    // entity trở lại Move/Idle State thì vẫn bị SetVelocity chặn di chuyển như bình thường.

    private void ApplyWoodEffect(float duration)
    {
        float woodRes = entityStart.GetElementalResistance(ElementalType.Wood);
        float reducedDuration = duration * (1 - woodRes);

        float finalDuration = reducedDuration;

        // Wood đang active -> làm mới (refresh) theo luật: lấy duration còn lại lớn hơn (giống Ice/Toxic)
        if (woodCo != null)
        {
            float remaining = Mathf.Max(0f, woodEndTime - Time.time);
            finalDuration = Mathf.Max(remaining, reducedDuration);
            StopCoroutine(woodCo);
        }

        woodEndTime = Time.time + finalDuration;
        woodCo = StartCoroutine(WoodEffect(finalDuration));
    }

    private IEnumerator WoodEffect(float duration)
    {
        DoWoodEffect();
        entity.SetRooted(true);
        entityVFX.PlayVfx(duration, ElementalType.Wood);

        yield return new WaitForSeconds(duration);

        StopWoodEffect();
    }

    private void DoWoodEffect()
    {
        Instantiate(woodVfx, transform.position, Quaternion.identity);
    }

    private void StopWoodEffect()
    {
        woodCo = null;
        entity.SetRooted(false);
        entityVFX.StopVfx(ElementalType.Wood);
    }

    #endregion


    #region Water Effect

    // Trong lúc duration còn hiệu lực, mỗi nhát Physical Damage nhận vào có tỉ lệ
    // trueDamageChange để bỏ qua hoàn toàn Armor (trừ thẳng currentHP). Khi tỉ lệ đó
    // trúng, người gây damage được hồi HP bằng healPercent (mặc định 25%) của damage
    // vừa gây ra. Việc roll tỉ lệ + tính true damage nằm ở Entity_Health.TakePhysicalDamage,
    // gọi qua TryRollWaterTrueDamage(). maxWaterRes làm giảm duration của hiệu ứng.

    private void ApplyWaterEffect(float duration, float trueDamageChance, float healPercent)
    {
        float waterRes = entityStart.GetElementalResistance(ElementalType.Water);
        float reducedDuration = duration * (1 - waterRes);

        float finalDuration = reducedDuration;
        float finalChance = trueDamageChance;

        // Water đang active -> làm mới theo luật: lấy duration còn lại lớn hơn, tỉ lệ lớn hơn
        if (waterCo != null)
        {
            float remaining = Mathf.Max(0f, waterEndTime - Time.time);
            finalDuration = Mathf.Max(remaining, reducedDuration);
            finalChance = Mathf.Max(waterTrueDamageChance, trueDamageChance);
            StopCoroutine(waterCo);
        }

        waterTrueDamageChance = finalChance;
        waterHealPercent = healPercent;
        waterEndTime = Time.time + finalDuration;

        waterCo = StartCoroutine(WaterEffect(finalDuration));
    }

    private IEnumerator WaterEffect(float duration)
    {
        DoWaterEffect();
        entityVFX.PlayVfx(duration, ElementalType.Water);

        yield return new WaitForSeconds(duration);

        StopWaterEffect();
    }

    private void DoWaterEffect()
    {
        Instantiate(waterVfx, transform.position, Quaternion.identity);
    }

    private void StopWaterEffect()
    {
        waterCo = null;
        waterTrueDamageChance = 0f;
        entityVFX.StopVfx(ElementalType.Water);
    }

    // Được Entity_Health gọi khi tính Physical Damage. Trả về true nếu roll trúng
    // trueDamageChange (hit này bỏ qua Armor hoàn toàn); healPercent luôn được trả ra
    // để Entity_Health biết hồi bao nhiêu % damage cho người gây damage khi trúng.
    public bool TryRollWaterTrueDamage(out float healPercent)
    {
        healPercent = waterHealPercent;

        if (waterCo == null || waterTrueDamageChance <= 0f)
            return false;

        return Random.value < waterTrueDamageChance;
    }

    #endregion

}