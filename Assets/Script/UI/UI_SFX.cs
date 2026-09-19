using UnityEngine;

// SFX dành cho toàn bộ UI.
// UI không phụ thuộc khoảng cách với Player.
public class UI_SFX : MonoBehaviour
{
    public static UI_SFX instance;

    [Header("SFX Name - Inventory")]
    [SerializeField] private string equipItem;
    [SerializeField] private string unequipItem;

    [Header("SFX Name - Merchant")]
    [SerializeField] private string merchantTrade;

    [Header("SFX Name - Craft")]
    [SerializeField] private string confirmCraft;

    [Header("SFX Name - Button")]
    [SerializeField] private string buttonClick;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    #region INVENTORY

    public void PlayEquipItem()
    {
        PlaySFX(equipItem);
    }

    public void PlayUnequipItem()
    {
        PlaySFX(unequipItem);
    }

    #endregion

    #region MERCHANT

    public void PlayMerchantTrade()
    {
        PlaySFX(merchantTrade);
    }

    #endregion

    #region CRAFT

    public void PlayConfirmCraft()
    {
        PlaySFX(confirmCraft);
    }

    #endregion

    #region BUTTON

    public void PlayButtonClick()
    {
        PlaySFX(buttonClick);
    }

    #endregion

    #region INTERNAL

    private void PlaySFX(string sfxName)
    {
        if (string.IsNullOrEmpty(sfxName))
            return;

        if (AudioManager.instance == null)
            return;

        // UI không có distance attenuation.
        AudioManager.instance.PlayUISFX(sfxName);
    }

    #endregion
}