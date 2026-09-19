using System;
using UnityEngine;
using UnityEngine.UI;

public class Entity_Health : MonoBehaviour, IDamagable
{
    public event Action OnTakingDamage;
    public event Action OnHeal;
    public event Action OnMPChange;

    private Slider hpBar;
    private Slider mpBar;
    private Entity_VFX entityVFX;
    private Entity entity;
    private Entity_Start entityStart;
    private Entity_DropManager dropManager;
    private Entity_Status entityStatus;



    [SerializeField] protected float currentHP;
    [SerializeField] protected float currentMP;
    [SerializeField] protected bool isDead;
    [SerializeField] protected bool canTakeDamage = true;
    [SerializeField] protected bool infiniteMana = false;

    // Hệ số nhân lên damage nhận vào
    private float incomingDamageMultiplier = 1f;

    [Header("Health Regen")]
    [SerializeField] private float regenHPInterval = 0f;
    [SerializeField] private bool canRegenHealth = true;

    [Header("Mana Regen")]
    [SerializeField] private float regenMPInterval = 0f;
    [SerializeField] private bool canRegenMana = true;

    [Header("On Damage To Knockback")]
    [SerializeField] private float knockbackDuration = .2f;
    [SerializeField] private Vector2 knockBackVector;
    [Header("On Heavy To Knockback")]
    [Range(0f, 1f)]
    [SerializeField] private float HPPercentToHeavyKnockback = .3f;
    [SerializeField] private float heavyKnockbackDuration = .5f;
    [SerializeField] private Vector2 heavyKnockbackVector;

    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        entityVFX = GetComponent<Entity_VFX>();
        AssignBars();
        entityStart = GetComponent<Entity_Start>();
        dropManager = GetComponent<Entity_DropManager>();
        entityStatus = GetComponent<Entity_Status>();

        SetupHealth();
        SetupMana();

    }

    protected virtual void Start()
    {

    }

    // Tìm Slider HP và MP trong children. Ưu tiên tìm theo tên GameObject ("HP"/"MP") để tránh
    // nhầm lẫn khi entity có nhiều hơn 1 Slider con (VD: world-space bar phía trên đầu quái).
    // Nếu không tìm thấy theo tên (entity cũ chỉ có 1 Slider HP) thì giữ nguyên hành vi cũ.
    private void AssignBars()
    {
        Slider[] sliders = GetComponentsInChildren<Slider>(true);
        if (sliders.Length == 0)
            return;

        foreach (Slider slider in sliders)
        {
            string sliderName = slider.name.ToLowerInvariant();
            if (sliderName.Contains("mp") || sliderName.Contains("mana"))
                mpBar = slider;
            else if (sliderName.Contains("hp") || sliderName.Contains("health"))
                hpBar = slider;
        }

        if (hpBar == null && mpBar == null)
            hpBar = sliders[0];
    }

    private void SetupHealth()
    {
        if (entityStart == null)
            return;
        currentHP = entityStart.GetMaxHeatlh();
        OnHeal += UpdateHealthBar;
        UpdateHealthBar();
        InvokeRepeating(nameof(RegenerateHP), 1f, regenHPInterval);
    }

    private void SetupMana()
    {
        if (entityStart == null)
            return;
        currentMP = entityStart.GetMaxMP();
        OnMPChange += UpdateManaBar;
        UpdateManaBar();
        InvokeRepeating(nameof(RegenerateMP), 1f, regenMPInterval);
    }

    #region Dòng Damage
    public void SetCanTakeDamage(bool canTakeDamage) => this.canTakeDamage = canTakeDamage;

    // Dùng bởi các state/skill phòng thủ (VD: giữ nút Counter Attack) để giảm % dmg nhận vào tạm thời.
    // Gọi lại với 1f để trả về bình thường khi hết hiệu ứng.
    public void SetIncomingDamageMultiplier(float multiplier) => incomingDamageMultiplier = Mathf.Max(0f, multiplier);

    public virtual bool TakeDamage(float physicalDamage, float elementalDamage, ElementalType elementalType, Transform damageDealer)
    {
        if (isDead || canTakeDamage == false)
            return false;

        if (AttackEvasion())
            return false;

        // TÁCH RIÊNG 2 loại damage: physical và elemental.
        float finalPhysicalDamage = TakePhysicalDamage(physicalDamage, damageDealer) * incomingDamageMultiplier;
        float finalElementalDamage = TakeElementalDamage(elementalDamage, elementalType) * incomingDamageMultiplier;

        // Chỉ physical damage mới tạo knockback. Nếu đang bị trói (Wood Elemental) thì
        // không bị knockback bởi Physical Damage - Wind Elemental vẫn hất tung bình thường
        // vì đi qua LaunchAirborne, không phải Knockback.
        bool isRooted = entity != null && entity.IsRooted;
        if (finalPhysicalDamage > 0f && !isRooted)
            TakeKnockBack(damageDealer, finalPhysicalDamage);

        // Cộng 2 loại damage lại rồi mới trừ HP.
        float totalDamage = finalPhysicalDamage + finalElementalDamage;
        if (totalDamage > 0f)
        {
            ReduceHP(totalDamage);
            OnTakingDamage?.Invoke();
        }

        return true;
    }

    public float TakePhysicalDamage(float physicalDamage, Transform damageDealer)
    {
        if (physicalDamage <= 0f)
            return 0f; // Không có physical damage thì không tính armor.

        // Water Elemental: trong lúc đang dính hiệu ứng Water, mỗi nhát Physical Damage có tỉ lệ
        // trueDamageChange bỏ qua hoàn toàn Armor (trừ thẳng currentHP), đồng thời hồi HP cho
        // người gây damage bằng % (mặc định 25%) của damage vừa gây ra.
        if (entityStatus != null && entityStatus.TryRollWaterTrueDamage(out float healPercent))
        {
            HealDamageDealer(damageDealer, physicalDamage * healPercent);
            return physicalDamage; // True Damage - bỏ qua Armor hoàn toàn.
        }

        Entity_Start attackerStart = damageDealer != null
            ? damageDealer.GetComponent<Entity_Start>()
            : null;

        float armorReduction = attackerStart != null ? attackerStart.GetArmorPenetration() : 0f;
        float mitigation = entityStart != null ? entityStart.GetArmor(armorReduction) : 0f;

        return physicalDamage * (1f - mitigation); // Physical damage sau khi trừ Armor.
    }

    // Hồi HP cho người gây damage (dùng cho proc True Damage của Water Elemental).
    private void HealDamageDealer(Transform damageDealer, float healAmount)
    {
        if (damageDealer == null || healAmount <= 0f)
            return;

        Entity_Health dealerHealth = damageDealer.GetComponent<Entity_Health>();
        dealerHealth?.RegenHealth(healAmount);
    }

    public float TakeElementalDamage(float elementalDamage, ElementalType elementalType)
    {
        if (elementalDamage <= 0f || elementalType == ElementalType.None)
            return 0f; // Không có elemental damage thì không bị ảnh hưởng bởi default elemental stats.

        float elementalResistance = entityStart != null
            ? entityStart.GetElementalResistance(elementalType)
            : 0f;

        return elementalDamage * (1f - elementalResistance); // Elemental damage sau khi trừ Elemental Resistance.
    }
    public void ReduceHP(float damage)
    {
        currentHP -= damage;

        entityVFX?.PlayOnDamageVFX();
        OnHeal?.Invoke();

        if (currentHP <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        isDead = true;

        entity.EntityDeath();
        dropManager?.DropItem();
    }

    #endregion

    #region Dòng Né
    private bool AttackEvasion()
    {
        if (entityStart == null)
            return false;
        else
            return UnityEngine.Random.Range(0, 100) < entityStart.GetEvasion();
    }


    #endregion


    #region Dòng Hồi HP
    private void RegenerateHP()
    {
        if (canRegenHealth == false)
            return;
        float regenAmount = entityStart.normalGroup.HPRegen.GetValue();
        RegenHealth(regenAmount);
    }

    public void RegenHealth(float healthRegen)
    {
        if (isDead)
            return;
        float newHealth = currentHP + healthRegen;
        float maxHealth = entityStart.GetMaxHeatlh();

        currentHP = Mathf.Min(newHealth, maxHealth);
        OnHeal?.Invoke();
    }



    #endregion


    #region Dòng MP
    // Bật/tắt Infinite Mana (dùng bởi UI_Hack). Khi bật thì luôn đủ MP và không bị trừ MP.
    public void SetInfiniteMana(bool infiniteMana) => this.infiniteMana = infiniteMana;

    // Kiểm tra đủ MP để sài skill hay không. Dùng bởi SkillBase.CanUseSkill().
    public bool HasEnoughMP(float amount) => infiniteMana || currentMP >= amount;

    // Trừ MP khi sài skill. Dùng bởi SkillBase khi skill thực sự được kích hoạt.
    public void ReduceMP(float amount)
    {
        if (infiniteMana)
            return;

        if (amount <= 0f)
            return;

        currentMP = Mathf.Max(0f, currentMP - amount);
        OnMPChange?.Invoke();
    }

    private void RegenerateMP()
    {
        if (canRegenMana == false)
            return;
        float regenAmount = entityStart.normalGroup.MPRegen.GetValue();
        RegenMana(regenAmount);
    }

    public void RegenMana(float manaRegen)
    {
        if (isDead)
            return;
        float newMana = currentMP + manaRegen;
        float maxMana = entityStart.GetMaxMP();

        currentMP = Mathf.Min(newMana, maxMana);
        OnMPChange?.Invoke();
    }
    #endregion


    // Ép UI (HP/MP bar) làm mới lại giá trị + slider mà không thay đổi currentHP/currentMP.
    // Dùng khi maxHP/maxMP thay đổi (VD: lên cấp Player_Level, tiến hóa Player_BodyLvl)
    // nhưng không đi qua các hàm damage/heal/regen bình thường nên UI không tự cập nhật.
    public void RefreshHealthAndManaUI()
    {
        OnHeal?.Invoke();
        OnMPChange?.Invoke();
    }

    #region HP Bar
    public float GetHPPercent() => currentHP / entityStart.GetMaxHeatlh();
    public void SetHPToPercent(float percent)
    {
        currentHP = entityStart.GetMaxHeatlh() * Mathf.Clamp01((float)percent);
        OnHeal?.Invoke();
    }
    public float GetCurrentHealth() => currentHP;

    private void TakeKnockBack(Transform damageDealer, float finalPhysDamage)
    {
        float duration = CalculateDuration(finalPhysDamage);
        Vector2 knocback = CalculateKnockback(finalPhysDamage, damageDealer);
        entity?.ReciveKnockback(knocback, duration);
    }

    private void UpdateHealthBar()
    {
        if (hpBar == null)
            return;
        hpBar.value = currentHP / entityStart.GetMaxHeatlh();
    }

    #endregion


    #region MP Bar
    public float GetMPPercent() => currentMP / entityStart.GetMaxMP();
    public void SetMPToPercent(float percent)
    {
        currentMP = entityStart.GetMaxMP() * Mathf.Clamp01((float)percent);
        OnMPChange?.Invoke();
    }
    public float GetCurrentMP() => currentMP;

    private void UpdateManaBar()
    {
        if (mpBar == null)
            return;
        mpBar.value = currentMP / entityStart.GetMaxMP();
    }

    #endregion



    #region Dòng nhận Damage hiệu ứng knockback
    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;

        Vector2 knockback = IsHeavyDamage(damage) ? heavyKnockbackVector : knockBackVector;
        knockback.x = knockback.x * direction;
        return knockback;
    }

    private float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;

    private bool IsHeavyDamage(float Damage)
    {
        if (entityStart == null) return false;
        else
            return Damage / entityStart.GetMaxHeatlh() > HPPercentToHeavyKnockback;
    }
    #endregion

}