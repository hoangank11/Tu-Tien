using TMPro;
using UnityEngine;

public class UI_MoneyInfor : UI_Tooltip
{
    private TextMeshProUGUI text;

    protected override void Awake()
    {
        base.Awake();

        text = GetComponentInChildren<TextMeshProUGUI>();

    }

    public void ShowToolTip(bool show, RectTransform targetRect, MoneyType moneyType)
    {
        base.ShowToolTip(show, targetRect);
        text.text = GetInforUI(moneyType);
    }

    public string GetInforUI(MoneyType type)
    {
        switch (type)
        {
            
            case MoneyType.Copper: 
                return "Tiền Đồng, một loại tiền phổ thông dùng để chi tiêu trong thế giới người phàm";
            case MoneyType.LinhThach:
                return "Linh Thạch, một loại thạch quý giá dùng để giao thương phổ thông trong tu tiên giả";
            case MoneyType.TienThach:
                return "Tiên Thạch, một loại đá quý hiếm có và cực kỳ quý giá trong giới tu tiên giả";
            case MoneyType.NgoDaoThach:
                return "Một viên linh thạch hấp thụ vô số đạo vận của thiên địa. Khi hấp thu, người tu luyện có thể gia tăng lĩnh ngộ đối với công pháp và thần thông, nhận được lượng lớn Kinh Nghiệm Kỹ Năng";

            default:
                return "null :6";

        }

    }
}
