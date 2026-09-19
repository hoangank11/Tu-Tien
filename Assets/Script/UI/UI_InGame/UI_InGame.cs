using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_InGame : MonoBehaviour
{
    private Player player;
    private InventoryPlayer inventory;
    private UI_SkillSlot[] skillSlots;
    private UI_QuickItemSlotOption[] quickItemOption;
    private UI_QuickItemSlot[] quickItemSlot;
    private Player_Level playerLevel;
    private Player_BodyLvl playerBodyLvl;

    [Header("HP Detail")]
    [SerializeField] private RectTransform HPRect;
    [SerializeField] private Slider HPSlider;
    [SerializeField] private TextMeshProUGUI HPText;
    [Space]
    [Header("MP Detail")]
    [SerializeField] private RectTransform MPRect;
    [SerializeField] private Slider MPSlider;
    [SerializeField] private TextMeshProUGUI MPText;
    [Space]
    [Header("EXP Detail")]
    [SerializeField] private RectTransform EXPRect;
    [SerializeField] private Slider EXPSlider;
    [SerializeField] private TextMeshProUGUI EXPText;
    [Space]
    [Header("Level Detail")]
    [Tooltip("7 ảnh theo thứ tự nhóm cảnh giới lớn (LevelTypeExtensions.GetIconIndex).")]
    [SerializeField] private Image levelImg;
    [SerializeField] private Sprite[] levelIcons;
    [Space]
    [Header("Body Level Detail")]
    [Tooltip("5 ảnh theo thứ tự nhóm cảnh giới luyện thể (BodyTypeExtensions.GetIconIndex).")]
    [SerializeField] private Image bodyImg;
    [SerializeField] private Sprite[] bodyIcons;
    [Space]
    [Tooltip("Hiện tên currentLevel và currentBody của player dạng chữ.")]
    [SerializeField] private TextMeshProUGUI showText;
    [Space]
    [Header("Quick Item Slot")]
    [SerializeField] private float yOffsetQuickItemParent = 150;
    [SerializeField] private Transform quickItemOptionParent;


    private void Start()
    {
        quickItemSlot = GetComponentsInChildren<UI_QuickItemSlot>();
        player = FindFirstObjectByType<Player>();
        player.health.OnHeal += UpdateHealthBar;
        player.health.OnMPChange += UpdateManaBar;
        skillSlots = GetComponentsInChildren<UI_SkillSlot>(true);
        inventory = player.inventory;
        inventory.OnQuickItemSlot += UpdateQuickSlotUI;

        playerLevel = player.GetComponent<Player_Level>();
        playerBodyLvl = player.GetComponent<Player_BodyLvl>();

        if (playerLevel != null)
        {
            playerLevel.OnExpChanged += UpdateExpBar;
            UpdateExpBar();
        }

        if (playerBodyLvl != null)
        {
            playerBodyLvl.OnBodyChanged += UpdateBodyLevel;
            UpdateBodyLevel();
        }
    }
    public void UpdateQuickSlotUI(int slotNumber, InventoryItem itemInSlot)
    {
        quickItemSlot[slotNumber].UpdateQuickSlotUI(itemInSlot);
    }

    public void OpenQuickItemOption(UI_QuickItemSlot quickItemSlot, RectTransform targetRect)
    {
        if (quickItemOption == null)
            quickItemOption = quickItemOptionParent.GetComponentsInChildren<UI_QuickItemSlotOption>(true);

        List<InventoryItem> consumables = inventory.itemList.FindAll(item => item.itemData.itemType == ItemType.Consumable);

        for (int i = 0; i < quickItemOption.Length; i++)
        {
            if (i < consumables.Count)
            {
                quickItemOption[i].gameObject.SetActive(true);
                quickItemOption[i].SetupOption(quickItemSlot, consumables[i]);
            }
            else
                quickItemOption[i].gameObject.SetActive(false);
        }
        quickItemOptionParent.position = targetRect.position + Vector3.up * yOffsetQuickItemParent;
    }

    public void HideQuickItemOption() => quickItemOptionParent.position = new Vector3(0, 9999);

    public void AssignSkillToSlot(SkillDataSO data, SkillBase skill)
    {
        UI_SkillSlot slot = GetSkillSlot(data.skillByName);
        if (slot == null)
        {
            return;
        }

        slot.SetupSkillSlot(data, skill);
    }

    public UI_SkillSlot GetSkillSlot(SkillName skillType)
    {
        foreach (var slot in skillSlots)
        {
            if (slot.skillType == skillType)
                return slot;
        }
        return null;
    }

    private void UpdateHealthBar()
    {
        float currentHealth = Mathf.RoundToInt(player.health.GetCurrentHealth());
        float maxHealth = player.start.GetMaxHeatlh();
        HPText.text = "Khí Huyết: " + currentHealth + " / " + maxHealth;
        HPSlider.value = player.health.GetHPPercent();
    }

    private void UpdateManaBar()
    {
        float currentMana = Mathf.RoundToInt(player.health.GetCurrentMP());
        float maxMana = player.start.GetMaxMP();
        MPText.text = "Linh Lực: " + currentMana + " / " + maxMana;
        MPSlider.value = player.health.GetMPPercent();
    }

    // Giống HP/MP: Slider dùng để chỉ CurrentExp/NeedExp.
    private void UpdateExpBar()
    {
        if (playerLevel == null)
            return;

        float currentExp = playerLevel.CurrentExp;
        float needExp = playerLevel.NeedExp;

        if (EXPSlider != null)
            EXPSlider.value = needExp > 0f ? Mathf.Clamp01(currentExp / needExp) : 1f;

        if (EXPText != null)
            EXPText.text = needExp > 0f
                ? $"Tu Vi (Chuyển thể từ linh thạch): {currentExp:0.#} / {needExp:0.#}"
                : "Tu Vi (Chuyển thể từ linh thạch): Đã đầy đủ, Tu sĩ cần phải tiến giai";

        UpdateLevelIcon();
        UpdateShowText();
    }

    // Ảnh Level dùng để chỉ level hiện tại theo thứ tự enum LevelType (nhiều LevelType dùng chung 1 ảnh).
    private void UpdateLevelIcon()
    {
        if (playerLevel == null || levelImg == null || levelIcons == null || levelIcons.Length == 0)
            return;

        int index = Mathf.Clamp(playerLevel.CurrentLevel.GetIconIndex(), 0, levelIcons.Length - 1);
        Sprite icon = levelIcons[index];
        if (icon != null)
            levelImg.sprite = icon;
    }

    // Tương tự Level nhưng cho BodyType (5 ảnh dùng chung cho các tầng nhỏ).
    private void UpdateBodyLevel()
    {
        if (playerBodyLvl == null || bodyImg == null || bodyIcons == null || bodyIcons.Length == 0)
        {
            UpdateShowText();
            return;
        }

        int index = Mathf.Clamp(playerBodyLvl.CurrentBody.GetIconIndex(), 0, bodyIcons.Length - 1);
        Sprite icon = bodyIcons[index];
        if (icon != null)
            bodyImg.sprite = icon;

        UpdateShowText();
    }

    // ShowText hiện currentLevel và currentBody của player dạng chữ.
    private void UpdateShowText()
    {
        if (showText == null)
            return;

        string levelName = playerLevel != null ? UI_Level.GetLevelUI(playerLevel.CurrentLevel) : "";
        string bodyName = playerBodyLvl != null ? UI_Level.GetBodyUI(playerBodyLvl.CurrentBody) : "";

        showText.text = $"Cảnh Giới Tu Sĩ: {levelName}\nCảnh Giới Thể Tu: {bodyName}";
    }

}
