using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MoneySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rect;
    private UI ui;

    [SerializeField] private MoneyType moneyType;

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.moneyToolTip.ShowToolTip(true, rect, moneyType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.moneyToolTip.ShowToolTip(false, null);
    }
}

