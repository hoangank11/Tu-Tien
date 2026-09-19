using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Hack : MonoBehaviour
{
    private const int MaxAmount = 1000000;

    private Player player;
    private InventoryPlayer inventoryPlayer;
    private SkillPoint skillPoint;

    private TMP_InputField copperInput;
    private TMP_InputField linhThachInput;
    private TMP_InputField tienThachInput;
    private TMP_InputField ngoDaoThachInput;
    private TMP_InputField itemInput;
    private Toggle immortalToggle;
    private Toggle manaToggle;
    private Button xButton;

    private void Awake()
    {
        player = Player.instance != null ? Player.instance : FindFirstObjectByType<Player>();
        inventoryPlayer = FindFirstObjectByType<InventoryPlayer>();

        Transform skillTree = transform.parent != null ? transform.parent.Find("UI_SkillTree") : null;
        if (skillTree != null)
            skillPoint = skillTree.GetComponentInChildren<SkillPoint>(true);

        copperInput = GetInputField("Đồng");
        linhThachInput = GetInputField("Linh Thạch");
        tienThachInput = GetInputField("Tiên Thạch");
        ngoDaoThachInput = GetInputField("Ngộ Đạo Thạch");
        itemInput = GetInputField("Item");

        SetupInput(copperInput, AddCopper);
        SetupInput(linhThachInput, AddLinhThach);
        SetupInput(tienThachInput, AddTienThach);
        SetupInput(ngoDaoThachInput, AddSkillPoint);
        SetupTextInput(itemInput, AddItem);

        immortalToggle = transform.Find("Bất Tử")?.GetComponent<Toggle>();
        if (immortalToggle != null)
        {
            immortalToggle.onValueChanged.RemoveListener(OnImmortalToggleChanged);
            immortalToggle.onValueChanged.AddListener(OnImmortalToggleChanged);
        }

        manaToggle = transform.Find("Mana")?.GetComponent<Toggle>();
        if (manaToggle != null)
        {
            manaToggle.onValueChanged.RemoveListener(OnManaToggleChanged);
            manaToggle.onValueChanged.AddListener(OnManaToggleChanged);
        }

        xButton = transform.Find("XButton")?.GetComponent<Button>();
        if (xButton != null)
            xButton.onClick.AddListener(Close);
    }

    private TMP_InputField GetInputField(string parentName)
    {
        Transform parent = transform.Find(parentName);
        return parent != null ? parent.Find("InputField (TMP)")?.GetComponent<TMP_InputField>() : null;
    }

    private void SetupInput(TMP_InputField input, UnityEngine.Events.UnityAction<string> callback)
    {
        if (input == null)
            return;

        input.contentType = TMP_InputField.ContentType.IntegerNumber;
        input.characterValidation = TMP_InputField.CharacterValidation.Integer;
        input.characterLimit = 5;

        // Chỉ xử lý khi người chơi submit bằng Enter.
        // Không dùng onEndEdit vì tắt UI cũng có thể làm InputField mất focus
        // và kích hoạt callback ngoài ý muốn.
        input.onSubmit.RemoveListener(callback);
        input.onSubmit.AddListener(callback);
    }

    // Riêng cho ô Item: nhập tên vật phẩm (text) chứ không phải số, nên không dùng chung SetupInput.
    private void SetupTextInput(TMP_InputField input, UnityEngine.Events.UnityAction<string> callback)
    {
        if (input == null)
            return;

        input.contentType = TMP_InputField.ContentType.Standard;
        input.characterValidation = TMP_InputField.CharacterValidation.None;

        input.onSubmit.RemoveListener(callback);
        input.onSubmit.AddListener(callback);
    }

    private bool TryGetAmount(string text, out int amount)
    {
        amount = 0;

        if (string.IsNullOrWhiteSpace(text))
            return false;

        if (!int.TryParse(text, out amount))
            return false;

        return amount >= 1 && amount <= MaxAmount;
    }

    private void AddCopper(string text)
    {
        if (TryGetAmount(text, out int amount))
        {
            inventoryPlayer ??= FindFirstObjectByType<InventoryPlayer>();
            if (inventoryPlayer != null)
                inventoryPlayer.AddMoney(MoneyType.Copper, amount);
        }

        ClearInput(copperInput);
    }

    private void AddLinhThach(string text)
    {
        if (TryGetAmount(text, out int amount))
        {
            inventoryPlayer ??= FindFirstObjectByType<InventoryPlayer>();
            if (inventoryPlayer != null)
                inventoryPlayer.AddMoney(MoneyType.LinhThach, amount);
        }

        ClearInput(linhThachInput);
    }

    private void AddTienThach(string text)
    {
        if (TryGetAmount(text, out int amount))
        {
            inventoryPlayer ??= FindFirstObjectByType<InventoryPlayer>();
            if (inventoryPlayer != null)
                inventoryPlayer.AddMoney(MoneyType.TienThach, amount);
        }

        ClearInput(tienThachInput);
    }

    private void AddSkillPoint(string text)
    {
        if (TryGetAmount(text, out int amount) && skillPoint != null)
            skillPoint.AddSkillPoint(amount);

        ClearInput(ngoDaoThachInput);
    }

    private void AddItem(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            inventoryPlayer ??= FindFirstObjectByType<InventoryPlayer>();
            inventoryPlayer?.AddItemByName(text);
        }

        ClearInput(itemInput);
    }

    private void OnImmortalToggleChanged(bool isOn)
    {
        player ??= FindFirstObjectByType<Player>();
        if (player == null || player.health == null)
            return;

        // isOn = true -> Bất Tử -> không nhận damage. isOn = false -> khôi phục nhận damage bình thường.
        player.health.SetCanTakeDamage(!isOn);
    }

    private void OnManaToggleChanged(bool isOn)
    {
        player ??= FindFirstObjectByType<Player>();
        if (player == null || player.health == null)
            return;

        player.health.SetInfiniteMana(isOn);
    }

    private void ClearInput(TMP_InputField input)
    {
        if (input != null)
            input.text = string.Empty;
    }

    public void Close()
    {
        // Quan trọng: UI_Options được mở bằng UI.OpenOptionUI(),
        // method này đã disable PlayerInput và đặt Time.timeScale = 0.
        // Vì vậy khi đóng UI_Hack phải khôi phục gameplay.
        EventSystem.current?.SetSelectedGameObject(null);

        copperInput?.DeactivateInputField();
        linhThachInput?.DeactivateInputField();
        tienThachInput?.DeactivateInputField();
        ngoDaoThachInput?.DeactivateInputField();
        itemInput?.DeactivateInputField();

        if (UI.instance != null)
        {
            UI.instance.SwitchToInGame();
        }
        else
        {
            // Fallback nếu UI.instance chưa tồn tại.
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}