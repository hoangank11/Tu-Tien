using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillSlot : MonoBehaviour
{
    private UI ui;
    private Image skillIcon;
    private RectTransform rect;
    private Button button;
    [Header("Skill Slot Setup")]
    [Tooltip("Chọn skill mà ô này đại diện. Khi skill này được unlock trong " +
        "skill tree, icon sẽ tự hiện ra ở đây.")]
    public SkillName skillType;
    public SkillDataSO skillData { get; private set; }
    public SkillBase skillBase { get; private set; }
    [Space]
    [SerializeField] private Image cooldownImg;
    [SerializeField] private string inputKeyName;
    [SerializeField] private TextMeshProUGUI inputKeyText;

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        button = GetComponent<Button>();
        skillIcon = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        inputKeyText.text = inputKeyName;
        ClearSlot();
    }


    // Gắn skill (đã unlock) vào ô này.
    public void SetupSkillSlot(SkillDataSO selectedSkill, SkillBase selectedSkillBase)
    {
        skillData = selectedSkill;
        skillBase = selectedSkillBase;

        skillIcon.color = Color.white;
        skillIcon.sprite = selectedSkill.icon;

        Color cdColor = Color.black; cdColor.a = 0.6f; // hết cooldown -> icon rõ hoàn toàn
        cooldownImg.color = cdColor;
    }

    // Bỏ skill khỏi ô, đưa ô về trạng thái trống.
    public void ClearSlot()
    {
        skillData = null;
        skillBase = null;

        skillIcon.sprite = null;
        skillIcon.color = Color.clear;
    }

    private void OnValidate()
    {
        gameObject.name = "UI_SkillSlot - " + skillType.ToString();
    }
    public void ResetCooldown() => cooldownImg.fillAmount = 0f;
    public void StartCooldown(float cooldown)
    {
        cooldownImg.fillAmount = 1f;
        StartCoroutine(CooldownCo(cooldown));
    }

    private IEnumerator CooldownCo(float duration)
    {
        float timePased = 0f;
        while (timePased < duration)
        {
            timePased += Time.deltaTime;
            cooldownImg.fillAmount = 1f - (timePased / duration);
            yield return null;
        }
        cooldownImg.fillAmount = 0;
    }


}
