using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI instance;

    // ================================================================
    // NHÓM PANEL LOẠI TRỪ LẪN NHAU.
    // Mở 1 cái trong nhóm này sẽ TỰ ĐỘNG đóng tất cả cái còn lại trong
    // cùng nhóm, tránh chồng UI + đảm bảo StopPlayerControls luôn đúng.
    //
    // Kéo vào Inspector: UI_Character (UI_Inventory), UI_CharacterLevel (UI_Level),
    // UI_Merchant, UI_Craft, UI_MakePosion (khi có), UI_Quest, UI_PlayerQuest,
    // UI_Storage, UI_Option, UI_Dialogue, UI_Hack, UI_Death.
    //
    // KHÔNG cho vào đây: UI_InGame, UI_SkillDetail/UI_ItemDetail/UI_InforDetail/
    // UI_MoneyDetail (tooltip), UI_LoadScreen (do GameManager/SaveManager tự fade),
    // UI_Audio (tab con bên trong UI_Option, không phải panel cấp cao nhất).
    // ================================================================
    [Header("Cac panel loai tru lan nhau (mo 1 dong het cai con lai)")]
    [SerializeField] private GameObject[] uiElements;

    // Panel nào trong nhóm trên đang mở (null = không có cái nào mở).
    private GameObject currentOpenPanel;

    public bool alternativeInput { get; private set; }
    private PlayerInputSet input;

    #region UI
    public UI_SkillText skillToolTip { get; private set; }
    public UI_ItemTooltip itemToolTip { get; private set; }
    public UI_PlayerInfor inforToolTip { get; private set; }
    public UI_MoneyInfor moneyToolTip { get; private set; }
    public SkillPoint skillTreeUI { get; private set; }
    public UI_Inventory inventoryUI { get; private set; }
    public UI_Storage storageUI { get; private set; }
    public UI_Craft craftUI { get; private set; }
    public UI_Merchant merchantUI { get; private set; }
    public UI_InGame inGameUI { get; private set; }
    public UI_Skill skillUI { get; private set; }
    public UI_Options optionUI { get; private set; }
    public UI_Hack hackUI { get; private set; }
    public UI_DeathGame deathGameUI { get; private set; }
    public UI_LoadScreen loadScreenUI { get; private set; }
    public UI_Quest questUI { get; private set; }
    public UI_PlayerQuest playerQuestUI { get; private set; }
    public UI_Dialogue dialogueUI { get; private set; }
    public UI_Level levelUI { get; private set; }
    public UI_Map mapUI { get; private set; }
    #endregion

    private void Awake()
    {
        instance = this;

        skillToolTip = GetComponentInChildren<UI_SkillText>();
        itemToolTip = GetComponentInChildren<UI_ItemTooltip>();
        inforToolTip = GetComponentInChildren<UI_PlayerInfor>();
        moneyToolTip = GetComponentInChildren<UI_MoneyInfor>();

        skillTreeUI = GetComponentInChildren<SkillPoint>(true);
        inventoryUI = GetComponentInChildren<UI_Inventory>(true);
        storageUI = GetComponentInChildren<UI_Storage>(true);
        craftUI = GetComponentInChildren<UI_Craft>(true);
        merchantUI = GetComponentInChildren<UI_Merchant>(true);
        inGameUI = GetComponentInChildren<UI_InGame>(true);
        skillUI = GetComponentInChildren<UI_Skill>(true);
        optionUI = GetComponentInChildren<UI_Options>(true);
        hackUI = GetComponentInChildren<UI_Hack>(true);
        deathGameUI = GetComponentInChildren<UI_DeathGame>(true);
        loadScreenUI = GetComponentInChildren<UI_LoadScreen>(true);
        questUI = GetComponentInChildren<UI_Quest>(true);
        playerQuestUI = GetComponentInChildren<UI_PlayerQuest>(true);
        dialogueUI = GetComponentInChildren<UI_Dialogue>(true);
        levelUI = GetComponentInChildren<UI_Level>(true);
        mapUI = GetComponentInChildren<UI_Map>(true);

        // Đảm bảo trạng thái ban đầu nhất quán: không panel loại trừ nào được coi là "đang mở"
        // trừ khi nó thật sự active trong scene lúc khởi động.
        foreach (var element in uiElements)
        {
            if (element != null && element.activeSelf)
            {
                currentOpenPanel = element;
                break;
            }
        }
    }

    public void SetupControlsUI(PlayerInputSet inputSet)
    {
        input = inputSet;
        input.UI.SkillTreeUI.performed += ctx => ToggleSkillTreeUI();
        input.UI.InventoryUI.performed += ctx => ToggleInventoryUI();
        input.UI.QuestUI.performed += ctx => TogglePlayerQuestUI();
        input.UI.LevelUI.performed += ctx => ToggleLevelUI();
        input.UI.MapUI.performed += ctx => ToggleMapUI();

        input.UI.AlternativeInput.performed += ctx => alternativeInput = true;
        input.UI.AlternativeInput.canceled += ctx => alternativeInput = false;

        input.UI.OptionUI.performed += ctx =>
        {
            // Nếu đang có bất kỳ panel loại trừ nào mở (kể cả Option) -> đóng hết, về InGame.
            // Nếu chưa có gì mở -> mở Option.
            if (currentOpenPanel != null)
                SwitchToInGame();
            else
                OpenOptionUI();
        };

        input.UI.DialogueUI.performed += ctx =>
        {
            if (dialogueUI.gameObject.activeInHierarchy)
                dialogueUI.DialogueInteraction();
        };
        input.UI.DialogueNavigation.performed += ctx =>
        {
            int direction = Mathf.RoundToInt(ctx.ReadValue<float>());

            if (dialogueUI.gameObject.activeInHierarchy)
                dialogueUI.NavigateChoice(direction);
        };
    }

    // ================================================================
    // LÕI CƠ CHẾ LOẠI TRỪ LẪN NHAU
    // ================================================================

    /// <summary>
    /// Mở 1 panel trong nhóm loại trừ, tự động đóng mọi panel loại trừ khác,
    /// khóa điều khiển Player, ẩn tooltip. UI_InGame không bị đụng tới (luôn bật).
    /// </summary>
    private void OpenExclusivePanel(GameObject panel)
    {
        if (panel == null)
            return;

        foreach (var element in uiElements)
        {
            if (element == null)
                continue;

            element.SetActive(element == panel);
        }

        currentOpenPanel = panel;

        HideAllTooltips();
        StopPlayerControls(true);
    }

    /// <summary>
    /// Đóng toàn bộ panel trong nhóm loại trừ, trả lại quyền điều khiển cho Player.
    /// UI_InGame không bị đụng tới (luôn bật).
    /// </summary>
    private void CloseAllExclusivePanels()
    {
        foreach (var element in uiElements)
        {
            if (element != null)
                element.SetActive(false);
        }

        currentOpenPanel = null;

        HideAllTooltips();
        StopPlayerControls(false);
    }

    public void OpenDeathUI()
    {
        OpenExclusivePanel(deathGameUI.gameObject);
        input.Disable(); // nếu sử dụng gamepad thì dead game
    }

    public void OpenOptionUI()
    {
        Time.timeScale = 0f;
        OpenExclusivePanel(optionUI.gameObject);
    }

    public void OpenHackUI()
    {
        // Được gọi từ UI_Options khi nhập đúng mã hack.
        OpenExclusivePanel(hackUI.gameObject);
    }

    public void SwitchToInGame()
    {
        CloseAllExclusivePanels();
        Time.timeScale = 1f;

        // UI_InGame không thuộc nhóm loại trừ nên không bị vòng lặp ở trên tắt đi,
        // nhưng vẫn đảm bảo nó luôn bật tường minh ở đây.
        inGameUI.gameObject.SetActive(true);
    }

    public void HideAllTooltips()
    {
        itemToolTip.ShowToolTip(false, null);
        skillToolTip.ShowToolTip(false, null);
        inforToolTip.ShowToolTip(false, null);
        moneyToolTip.ShowToolTip(false, null);
    }

    public void ToggleSkillTreeUI()
    {
        if (skillUI.gameObject.activeSelf)
            CloseAllExclusivePanels();
        else
            OpenExclusivePanel(skillUI.gameObject);
    }

    public void ToggleInventoryUI()
    {
        if (inventoryUI.gameObject.activeSelf)
            CloseAllExclusivePanels();
        else
            OpenExclusivePanel(inventoryUI.gameObject);
    }

    public void ToggleLevelUI()
    {
        if (levelUI.gameObject.activeSelf)
        {
            CloseAllExclusivePanels();
        }
        else
        {
            OpenExclusivePanel(levelUI.gameObject);
            levelUI.RefreshUI();
        }
    }

    public void ToggleMapUI()
    {
        if (mapUI.gameObject.activeSelf)
            CloseAllExclusivePanels();
        else
            OpenExclusivePanel(mapUI.gameObject);
    }

    public void CloseMapUI()
    {
        CloseAllExclusivePanels();
    }

    public void TogglePlayerQuestUI()
    {
        if (playerQuestUI.gameObject.activeSelf)
            CloseAllExclusivePanels();
        else
            OpenExclusivePanel(playerQuestUI.gameObject);
    }

    public void OpenDialogueUI(DialogueLineSO firstLine)
    {
        OpenExclusivePanel(dialogueUI.gameObject);
        dialogueUI.PlayDialogueLine(firstLine);
    }

    public void OpenQuestUI(QuestDataSO[] questToShow)
    {
        OpenExclusivePanel(questUI.gameObject);
        questUI.SetupQuestUI(questToShow);
    }

    public void CloseQuestUI()
    {
        CloseAllExclusivePanels();
    }

    public void OpenStorageUI(bool openStorageUI)
    {
        if (openStorageUI)
            OpenExclusivePanel(storageUI.gameObject);
        else
            CloseAllExclusivePanels();
    }

    public void OpenMerchantUI(bool openMerchantUI)
    {
        if (openMerchantUI)
            OpenExclusivePanel(merchantUI.gameObject);
        else
            CloseAllExclusivePanels();
    }

    public void OpenCraftUI(bool openCraftUI)
    {
        if (openCraftUI)
            OpenExclusivePanel(craftUI.gameObject);
        else
            CloseAllExclusivePanels();
    }

    private void StopPlayerControls(bool stopControls)
    {
        if (stopControls)
            input.Player.Disable();
        else
            input.Player.Enable();
    }
}