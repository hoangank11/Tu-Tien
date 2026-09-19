using System;
using UnityEngine;

public class Player_Level : MonoBehaviour, ISaveable
{
    [Serializable]
    public class LevelExpRequirement
    {
        public LevelType level;
        [Min(0f)] public float needExp = 100f;
    }

    [Header("Player Level")]
    public LevelType currentLevel = LevelType.PhamNhan;

    [SerializeField, Min(0f)]
    public float currentExp = 0f;

    [Header("EXP For Each Level")]
    [Tooltip("Mỗi LevelType có một lượng EXP cần riêng")]
    [SerializeField] private LevelExpRequirement[] levelExpRequirements;

    private float needExpSave = 100f;

    private InventoryPlayer inventory;
    private Entity_Start entityStart;
    private Entity_Health entityHealth;

    public event Action OnExpChanged;

    public LevelType CurrentLevel => currentLevel;

    public float CurrentExp => inventory != null ? inventory.LinhThach : currentExp;

    public float NeedExp => needExpSave;

    public bool TryLevelUpFromUI()
    {
        if (IsMaxLevel())
            return false;

        needExpSave = GetNeedExpForLevel(currentLevel);

        if (needExpSave <= 0f || CurrentExp < needExpSave)
            return false;

        SpendExp(needExpSave);

        // PhamNhan không tăng chỉ số. Mỗi cấp sau đó tăng 25%.
        currentLevel = (LevelType)((int)currentLevel + 1);
        ApplyLevelStats();
        UpdateNeedExp();
        OnExpChanged?.Invoke();

        // maxMP vừa đổi ở ApplyLevelStats() nhưng không đi qua ReduceMP/RegenMana
        // nên phải ép UI (MPBar) làm mới lại thủ công.
        RefreshHealthUI();
        return true;
    }

    private void Awake()
    {
        inventory = GetComponent<InventoryPlayer>();
        entityStart = GetComponent<Entity_Start>();
        entityHealth = GetComponent<Entity_Health>();

        if (inventory != null)
            inventory.OnInventoryChange += HandleInventoryChanged;

        InitializeLevelRequirements();
        UpdateNeedExp();
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnInventoryChange -= HandleInventoryChanged;
    }

    private void HandleInventoryChanged()
    {
        OnExpChanged?.Invoke();
    }

    private void OnValidate()
    {
        InitializeLevelRequirements();
        UpdateNeedExp();
    }


    public void AddExp(float amount)
    {
        if (amount <= 0f || IsMaxLevel())
            return;

        if (inventory != null)
            inventory.AddMoney(MoneyType.LinhThach, Mathf.RoundToInt(amount));
        else
            currentExp += amount;

        OnExpChanged?.Invoke();
    }

    private void SpendExp(float amount)
    {
        if (inventory != null)
            inventory.AddMoney(MoneyType.LinhThach, -Mathf.RoundToInt(amount));
        else
            currentExp = Mathf.Max(0f, currentExp - amount);
    }

    private void UpdateNeedExp()
    {
        needExpSave = IsMaxLevel() ? 0f : GetNeedExpForLevel(currentLevel);
    }

    private float GetNeedExpForLevel(LevelType level)
    {
        if (levelExpRequirements == null)
            return 0f;

        for (int i = 0; i < levelExpRequirements.Length; i++)
        {
            if (levelExpRequirements[i] != null && levelExpRequirements[i].level == level)
                return Mathf.Max(0f, levelExpRequirements[i].needExp);
        }

        return 0f;
    }

    private bool IsMaxLevel()
    {
        return (int)currentLevel >= Enum.GetValues(typeof(LevelType)).Length - 1;
    }

    private void InitializeLevelRequirements()
    {
        int levelCount = Enum.GetValues(typeof(LevelType)).Length;

        if (levelExpRequirements == null || levelExpRequirements.Length != levelCount)
        {
            LevelExpRequirement[] old = levelExpRequirements;
            levelExpRequirements = new LevelExpRequirement[levelCount];

            for (int i = 0; i < levelCount; i++)
            {
                float oldNeedExp = 100f;

                if (old != null)
                {
                    for (int j = 0; j < old.Length; j++)
                    {
                        if (old[j] != null && old[j].level == (LevelType)i)
                        {
                            oldNeedExp = old[j].needExp;
                            break;
                        }
                    }
                }

                levelExpRequirements[i] = new LevelExpRequirement
                {
                    level = (LevelType)i,
                    needExp = oldNeedExp
                };
            }
        }
        else
        {
            for (int i = 0; i < levelCount; i++)
            {
                if (levelExpRequirements[i] == null)
                    levelExpRequirements[i] = new LevelExpRequirement();

                levelExpRequirements[i].level = (LevelType)i;
                levelExpRequirements[i].needExp = Mathf.Max(0f, levelExpRequirements[i].needExp);
            }
        }
    }

    private void ApplyLevelStats()
    {
        if (entityStart == null)
            entityStart = GetComponent<Entity_Start>();

        if (entityStart == null)
            return;

        // Chỉ cấp sau Phàm Nhân mới tăng chỉ số.
        if ((int)currentLevel <= (int)LevelType.PhamNhan)
            return;

        entityStart.normalGroup.maxMP.SetBaseValue(
            Mathf.Round(entityStart.normalGroup.maxMP.GetBaseValue() * 1.25f * 10f) / 10f);

        entityStart.baseGroup.maxDmg.SetBaseValue(
            Mathf.Round(entityStart.baseGroup.maxDmg.GetBaseValue() * 1.25f * 10f) / 10f);

        entityStart.baseGroup.maxElementalDmg.SetBaseValue(
            Mathf.Round(entityStart.baseGroup.maxElementalDmg.GetBaseValue() * 1.25f * 10f) / 10f);
    }

    /// <summary>
    /// Tính lại stat từ giá trị gốc theo từng cấp.
    /// Mỗi cấp tăng 25% và làm tròn còn 1 chữ số sau dấu phẩy.
    /// </summary>
    private float CalculateLevelStat(float baseValue, int levelIndex)
    {
        float value = baseValue;
        for (int i = 1; i <= levelIndex; i++)
            value = Mathf.Round(value * 1.25f * 10f) / 10f;
        return value;
    }

    #region Save / Load

    public void SaveData(ref GameData data)
    {
        data.currentExp = CurrentExp;
        data.currentLevel = currentLevel;

        if (entityStart != null)
        {
            data.playerLevelMaxMP = entityStart.normalGroup.maxMP.GetBaseValue();
            data.playerLevelMaxDmg = entityStart.baseGroup.maxDmg.GetBaseValue();
            data.playerLevelMaxElementalDmg = entityStart.baseGroup.maxElementalDmg.GetBaseValue();
        }
    }

    public void LoadData(GameData data)
    {
        int maxLevelIndex = Enum.GetValues(typeof(LevelType)).Length - 1;
        int savedLevelIndex = (int)data.currentLevel;

        if (savedLevelIndex < 0 || savedLevelIndex > maxLevelIndex)
            savedLevelIndex = 0;

        currentLevel = (LevelType)savedLevelIndex;

        // Fallback: nếu vì lý do gì đó chưa có InventoryPlayer, vẫn giữ tạm giá trị exp cũ.
        if (inventory == null)
            currentExp = Mathf.Max(0f, data.currentExp);

        if (entityStart == null)
            entityStart = GetComponent<Entity_Start>();

        // Khôi phục đúng các chỉ số đã tăng theo cấp.
        // Nếu save cũ chưa có các giá trị này, giữ nguyên stat hiện tại.
        if (entityStart != null)
        {
            if (data.playerLevelMaxMP > 0f)
                entityStart.normalGroup.maxMP.SetBaseValue(data.playerLevelMaxMP);
            else if (entityStart.defaultSetup != null)
                entityStart.normalGroup.maxMP.SetBaseValue(
                    CalculateLevelStat(entityStart.defaultSetup.maxMP, savedLevelIndex));

            if (data.playerLevelMaxDmg > 0f)
                entityStart.baseGroup.maxDmg.SetBaseValue(data.playerLevelMaxDmg);
            else if (entityStart.defaultSetup != null)
                entityStart.baseGroup.maxDmg.SetBaseValue(
                    CalculateLevelStat(entityStart.defaultSetup.maxDmg, savedLevelIndex));

            if (data.playerLevelMaxElementalDmg > 0f)
                entityStart.baseGroup.maxElementalDmg.SetBaseValue(data.playerLevelMaxElementalDmg);
            else if (entityStart.defaultSetup != null)
                entityStart.baseGroup.maxElementalDmg.SetBaseValue(
                    CalculateLevelStat(entityStart.defaultSetup.maxElementalDmg, savedLevelIndex));
        }

        UpdateNeedExp();
        OnExpChanged?.Invoke();
        RefreshHealthUI();
    }

    #endregion

    // Ép Entity_Health bắn lại OnHeal/OnMPChange để HPBar/MPBar cập nhật ngay,
    // vì SetBaseValue() ở ApplyLevelStats()/LoadData() không tự bắn 2 event này.
    private void RefreshHealthUI()
    {
        if (entityHealth == null)
            entityHealth = GetComponent<Entity_Health>();

        entityHealth?.RefreshHealthAndManaUI();
    }
}