using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class UI_Options : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float mixerMultiplier = 25;

    [Header("BGM Volume Setting")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private string bgmParameter;

    [Header("SFX Volume Setting")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private string sfxParameter;

    [Header("Hack")]
    [SerializeField] private string hackCode = "26081998";

    private TMP_InputField hackInput;

    private void Start()
    {
        hackInput = transform.Find("Hack")?.GetComponent<TMP_InputField>();
        if (hackInput != null)
        {
            hackInput.contentType = TMP_InputField.ContentType.IntegerNumber;
            hackInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
            hackInput.characterLimit = hackCode.Length;

            // Chỉ kiểm tra khi submit bằng Enter, không dùng onEndEdit.
            hackInput.onSubmit.RemoveListener(CheckHackCode);
            hackInput.onSubmit.AddListener(CheckHackCode);
        }
    }

    private void CheckHackCode(string value)
    {
        if (value == hackCode && UI.instance != null)
        {
            // Đi qua UI.OpenHackUI() thay vì SetActive tay, để cơ chế
            // loại trừ lẫn nhau trong UI.cs quản lý đúng (đóng Option, mở Hack,
            // giữ nguyên StopPlayerControls đang khóa từ trước).
            UI.instance.OpenHackUI();
        }

        if (hackInput != null)
            hackInput.text = string.Empty;
    }

    public void BGMSliderValue(float value)
    {
        float newValue = Mathf.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(bgmParameter, newValue);
        PlayerPrefs.SetFloat(bgmParameter, value);
    }

    public void SFXSliderValue(float value)
    {
        float newValue = Mathf.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(sfxParameter, newValue);
        PlayerPrefs.SetFloat(sfxParameter, value);
    }

    private void OnEnable()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParameter, .6f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParameter, .6f);
    }

    // Không gọi PlayerPrefs.Save() khi đóng Option.
    // Save() là thao tác I/O đồng bộ và có thể gây khựng frame.

    public void MainMenuButton() => GameManager.instance.ChangeScenes("Main Menu", RespawnType.None);
    public void SaveButon() => SaveManager.instance.SaveGame();

    public void LoadUpVolume()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParameter, .6f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParameter, .6f);
    }
}