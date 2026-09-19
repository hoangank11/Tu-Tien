using UnityEngine;

public class SkillBase : MonoBehaviour
{
    public Player_SkillManager skillManager { get; private set; }
    public Player player { get; private set; }
    public ScaleFactor scaleFactor { get; private set; }

    [Header("Detail")]
    [SerializeField] protected SkillName skillType;
    [SerializeField] protected float cooldown;
    private float lastTimeUse;

    private Entity_Health entityHealth;

    public SkillDataSO skillData { get; private set; }
    public bool IsUnlocked { get; private set; }

    protected virtual void Awake()
    {
        skillManager = GetComponentInParent<Player_SkillManager>();
        player = GetComponentInParent<Player>();
        entityHealth = GetComponentInParent<Entity_Health>();
        lastTimeUse -= cooldown;
        scaleFactor = new ScaleFactor();
    }

    public virtual void TryUseSkill()
    {

    }

    // Loại Elemental thực tế mà skill sử dụng khi gây Elemental Damage/Status.
    // Mặc định là None để các skill Physical không bị hiểu nhầm là Elemental.
    public virtual ElementalType GetElementalType() => ElementalType.None;

    public void SetSkillUpgrade(SkillDataSO data)
    {
        skillData = data;

        cooldown = data.cooldown;
        scaleFactor = data.scaleFactor;

        IsUnlocked = true;

        ResetCooldown();
    }

    public virtual bool CanUseSkill()
    {
        if (skillType == SkillName.None) return false;
        if (IsUnlocked == false) return false;
        if (OnCooldown())
            return false;
        if (HasEnoughMP() == false)
            return false;
        return true;
    }

    public bool Unlocked(SkillName name) => IsUnlocked && skillType == name;

    protected bool OnCooldown() => Time.time < lastTimeUse + cooldown;

    public void SetSkillOnCooldown()
    {
        UI_SkillSlot slot = GetSkillSlotSafe();
        if (slot != null)
            slot.StartCooldown(cooldown);

        lastTimeUse = Time.time;
        ConsumeMP();
    }

    public void ResetCooldownBy(float cooldownReduction) => lastTimeUse += cooldownReduction;

    public void ResetCooldown()
    {
        UI_SkillSlot slot = GetSkillSlotSafe();
        if (slot != null)
            slot.ResetCooldown();

        lastTimeUse = Time.time - cooldown;
    }

    // Không để game crash + gãy StateMachine giữa chừng nếu thiếu UI trong scene (vd scene test).
    // Thay vào đó chỉ log cảnh báo để dễ debug, còn cooldown vẫn được set đúng.
    private UI_SkillSlot GetSkillSlotSafe()
    {
        if (player == null || player.ui == null || player.ui.inGameUI == null)
        {
            Debug.LogWarning($"[{name}] Không tìm thấy UI trong scene hiện tại nên không cập nhật được UI cooldown cho skill '{skillType}'. " +
                              $"Kiểm tra scene có UI Canvas chưa.", this);
            return null;
        }

        return player.ui.inGameUI.GetSkillSlot(skillType);
    }

    #region Dòng MP
    // Nếu thiếu Entity_Health hoặc skillData (skill chưa unlock) thì không chặn - tránh crash/bug ngược.
    protected bool HasEnoughMP()
    {
        if (entityHealth == null || skillData == null)
            return true;

        return entityHealth.HasEnoughMP(skillData.mpCost);
    }

    // Trừ MP theo mpCost của skillData. Được gọi chung trong SetSkillOnCooldown() nên mọi skill kế
    // thừa từ SkillBase đều tự động có xử lý trừ MP mà không cần viết lại ở từng skill con.
    private void ConsumeMP()
    {
        if (entityHealth == null || skillData == null)
            return;
        if (skillData.mpCost <= 0f)
            return;

        entityHealth.ReduceMP(skillData.mpCost);
    }
    #endregion
}