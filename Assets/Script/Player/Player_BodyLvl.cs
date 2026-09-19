using System;
using UnityEngine;

public class Player_BodyLvl : MonoBehaviour, ISaveable
{
    [Serializable]
    public class BodyRequirement
    {
        public BodyType body;
        [Tooltip("ItemListDataSO chứa các item và số lượng cần để tiến hóa từ BodyType này lên cấp kế tiếp.")]
        public ItemListDataSO requiredItems;
    }

    [Header("Body Level")]
    public BodyType currentBody = BodyType.PhamThe;

    [Header("Required Items For Each BodyType")]
    [Tooltip("Mỗi BodyType có một ItemListDataSO riêng. BodyType cuối cùng LuyenCotTang09 không cần ItemListDataSO.")]
    [SerializeField] private BodyRequirement[] bodyRequirements;

    private InventoryPlayer inventory;
    private Entity_Start entityStart;
    private Entity_Health entityHealth;

    /// <summary>
    /// Bắn ra mỗi khi CurrentBody thay đổi.
    /// UI (vd UI_InGame) lắng nghe event này để cập nhật ảnh BodyLevel.
    /// </summary>
    public event Action OnBodyChanged;

    public BodyType CurrentBody => currentBody;

    private void Awake()
    {
        inventory = GetComponent<InventoryPlayer>();
        entityStart = GetComponent<Entity_Start>();
        entityHealth = GetComponent<Entity_Health>();
        if (inventory == null)
            inventory = FindAnyObjectByType<InventoryPlayer>();

        InitializeBodyRequirements();
    }

    private void OnValidate()
    {
        InitializeBodyRequirements();
    }

    public bool IsMaxBody()
    {
        return currentBody == BodyType.LuyenTuyTang01;
    }

    private void InitializeBodyRequirements()
    {
        int count = Enum.GetValues(typeof(BodyType)).Length;

        if (bodyRequirements == null || bodyRequirements.Length != count)
        {
            BodyRequirement[] old = bodyRequirements;
            bodyRequirements = new BodyRequirement[count];

            for (int i = 0; i < count; i++)
            {
                ItemListDataSO oldData = null;

                if (old != null)
                {
                    for (int j = 0; j < old.Length; j++)
                    {
                        if (old[j] != null && old[j].body == (BodyType)i)
                        {
                            oldData = old[j].requiredItems;
                            break;
                        }
                    }
                }

                bodyRequirements[i] = new BodyRequirement
                {
                    body = (BodyType)i,
                    requiredItems = oldData
                };
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                if (bodyRequirements[i] == null)
                    bodyRequirements[i] = new BodyRequirement();

                bodyRequirements[i].body = (BodyType)i;
            }
        }

        // Cấp cuối không có tiến hóa tiếp.
        if (count > 0 && bodyRequirements[count - 1] != null)
            bodyRequirements[count - 1].requiredItems = null;
    }

    public ItemListDataSO GetCurrentRequiredItems()
    {
        if (IsMaxBody())
            return null;

        if (bodyRequirements == null || (int)currentBody >= bodyRequirements.Length)
            return null;

        return bodyRequirements[(int)currentBody]?.requiredItems;
    }

    public bool CanEvolve()
    {
        if (IsMaxBody() || inventory == null)
            return false;

        ItemListDataSO required = GetCurrentRequiredItems();

        // Chưa cấu hình ItemListDataSO thì không cho tiến hóa,
        // tránh trường hợp thiếu dữ liệu nhưng vẫn lên cấp.
        if (required == null || required.itemList == null || required.itemList.Length == 0)
            return false;

        for (int i = 0; i < required.itemList.Length; i++)
        {
            ItemDataSO item = required.itemList[i];
            if (item == null)
                continue;

            int amount = required.GetRequiredAmount(i);
            if (amount <= 0)
                continue;

            if (!inventory.HasItemAmount(item, amount))
                return false;
        }

        return true;
    }

    public bool TryEvolve()
    {
        if (!CanEvolve())
            return false;

        ItemListDataSO required = GetCurrentRequiredItems();

        // Trừ toàn bộ nguyên liệu sau khi kiểm tra đủ.
        for (int i = 0; i < required.itemList.Length; i++)
        {
            ItemDataSO item = required.itemList[i];
            if (item == null)
                continue;

            int amount = required.GetRequiredAmount(i);
            if (amount > 0)
                inventory.RemoveItemAmount(item, amount);
        }

        currentBody = (BodyType)((int)currentBody + 1);
        ApplyBodyStats();
        inventory.TriggerUpdateUI();
        OnBodyChanged?.Invoke();

        RefreshHealthUI();
        return true;
    }

    private void ApplyBodyStats()
    {
        if (entityStart == null)
            entityStart = GetComponent<Entity_Start>();

        if (entityStart == null)
            return;

        // Phàm Thể không tăng chỉ số. Mỗi BodyType sau đó tăng HP 25%
        // và Armor cộng thẳng 5.
        if ((int)currentBody <= (int)BodyType.PhamThe)
            return;

        entityStart.normalGroup.maxHP.SetBaseValue(
            Mathf.Round(entityStart.normalGroup.maxHP.GetBaseValue() * 1.25f * 10f) / 10f);

        entityStart.baseGroup.maxArmor.SetBaseValue(
            entityStart.baseGroup.maxArmor.GetBaseValue() + 5f);
    }

    public void SaveData(ref GameData data)
    {
        data.currentBody = currentBody;

        if (entityStart != null)
        {
            data.playerBodyMaxHP = entityStart.normalGroup.maxHP.GetBaseValue();
            data.playerBodyMaxArmor = entityStart.baseGroup.maxArmor.GetBaseValue();
        }
    }

    private float CalculateBodyHP(float baseValue, int bodyIndex)
    {
        float value = baseValue;
        for (int i = 1; i <= bodyIndex; i++)
            value = Mathf.Round(value * 1.25f * 10f) / 10f;
        return value;
    }

    public void LoadData(GameData data)
    {
        int maxIndex = Enum.GetValues(typeof(BodyType)).Length - 1;
        int savedIndex = (int)data.currentBody;

        if (savedIndex < 0 || savedIndex > maxIndex)
            savedIndex = 0;

        currentBody = (BodyType)savedIndex;

        if (entityStart == null)
            entityStart = GetComponent<Entity_Start>();

        // Khôi phục đúng các chỉ số đã tăng theo BodyType.
        // Save cũ chưa có các giá trị này thì giữ stat hiện tại.
        if (entityStart != null)
        {
            if (data.playerBodyMaxHP > 0f)
                entityStart.normalGroup.maxHP.SetBaseValue(data.playerBodyMaxHP);
            else if (entityStart.defaultSetup != null)
                entityStart.normalGroup.maxHP.SetBaseValue(
                    CalculateBodyHP(entityStart.defaultSetup.maxHP, savedIndex));

            // Armor: Phàm Thể = 0, mỗi cấp sau +5.
            // Save cũ chưa có playerBodyMaxArmor thì tính lại từ BodyType đã lưu.
            if (data.playerBodyMaxArmor > 0f)
                entityStart.baseGroup.maxArmor.SetBaseValue(data.playerBodyMaxArmor);
            else if (entityStart.defaultSetup != null)
                entityStart.baseGroup.maxArmor.SetBaseValue(
                    entityStart.defaultSetup.maxArmor + (savedIndex * 5f));
        }

        OnBodyChanged?.Invoke();
        RefreshHealthUI();
    }

    private void RefreshHealthUI()
    {
        if (entityHealth == null)
            entityHealth = GetComponent<Entity_Health>();

        entityHealth?.RefreshHealthAndManaUI();
    }
}