using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Level : MonoBehaviour
{
    private Player_Level playerLevel;
    private Player_BodyLvl playerBodyLvl;

    [Header("Tu Vi UI")]
    [SerializeField] private TextMeshProUGUI tuViText;
    [SerializeField] private Button tuViButton;

    [Header("Luyen The UI")]
    [SerializeField] private TextMeshProUGUI luyenTheText;
    [SerializeField] private Button luyenTheButton;
    [SerializeField] private UI_ItemListNeeded itemListNeeded;

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        RefreshPlayerReferences();
        RefreshTuVi();
        RefreshLuyenThe();
        RefreshButtons();
        RefreshItemListNeeded();
    }

    private void RefreshPlayerReferences()
    {
        playerLevel = FindAnyObjectByType<Player_Level>();
        playerBodyLvl = FindAnyObjectByType<Player_BodyLvl>();
    }

    private void RefreshTuVi()
    {
        if (playerLevel == null || tuViText == null)
            return;

        float needExp = playerLevel.NeedExp;
        string needText = needExp > 0f ? needExp.ToString("0.#") : "Đầy Đủ";

        tuViText.text =
            $"<color=yellow>Cảnh Giới Tu Sĩ</color>\n" +
            $"<color=red>{GetLevelUI(playerLevel.currentLevel)}</color>\n" +
            $"Linh Thạch hiện có: {playerLevel.CurrentExp:0.#}\n" +
            $"Linh Thạch cần để tiến giai: {needText}";
    }

    private void RefreshLuyenThe()
    {
        if (playerBodyLvl == null || luyenTheText == null)
            return;

        luyenTheText.text =
            $"<color=yellow>Thể Tu Cảnh Giới\n</color>" +
            $"<color=red>{GetBodyUI(playerBodyLvl.CurrentBody)}</color>";
    }

    private void RefreshButtons()
    {
        if (tuViButton != null && playerLevel != null)
        {
            tuViButton.interactable =
                !IsPlayerLevelMax() &&
                playerLevel.CurrentExp >= playerLevel.NeedExp;
        }

        if (luyenTheButton != null && playerBodyLvl != null)
        {
            luyenTheButton.interactable =
                !playerBodyLvl.IsMaxBody() &&
                playerBodyLvl.CanEvolve();
        }
    }

    private void RefreshItemListNeeded()
    {
        if (itemListNeeded == null)
            return;

        itemListNeeded.Refresh();
    }

    private bool IsPlayerLevelMax()
    {
        return playerLevel == null || playerLevel.NeedExp <= 0f;
    }

    public void OnTuViButtonClick()
    {
        RefreshPlayerReferences();

        if (playerLevel == null || IsPlayerLevelMax())
            return;

        if (playerLevel.CurrentExp < playerLevel.NeedExp)
            return;

        if (playerLevel.TryLevelUpFromUI())
            RefreshUI();
    }

    public void OnLuyenTheButtonClick()
    {
        RefreshPlayerReferences();

        if (playerBodyLvl == null || playerBodyLvl.IsMaxBody())
            return;

        if (playerBodyLvl.TryEvolve())
            RefreshUI();
    }

    public void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
    }

    // Có thể gọi từ Inventory/Player script khi EXP hoặc item thay đổi.
    public void UpdateLevelUI()
    {
        RefreshUI();
    }

    public static string GetLevelUI(LevelType type)
    {
        switch (type)
        {
            case LevelType.PhamNhan: return "Phàm Nhân";
            case LevelType.DanKhiNhapThe: return "Dẫn Khí Nhập Thể";
            case LevelType.LuyenKhiTang01: return "Luyện Khí Nhất Trọng";
            case LevelType.LuyenKhiTang02: return "Luyện Khí Nhị Trọng";
            case LevelType.LuyenKhiTang03: return "Luyện Khí Tam Trọng";
            case LevelType.LuyenKhiTang04: return "Luyện Khí Tứ Trọng";
            case LevelType.LuyenKhiTang05: return "Luyện Khí Ngũ Trọng";
            case LevelType.LuyenKhiTang06: return "Luyện Khí Lục Trọng";
            case LevelType.LuyenKhiTang07: return "Luyện Khí Thất Trọng";
            case LevelType.LuyenKhiTang08: return "Luyện Khí Bát Trọng";
            case LevelType.LuyenKhiTang09: return "Luyện Khí Cửu Trọng";
            case LevelType.LuyenKhiTang10: return "Luyện Khí Thập Trọng";
            case LevelType.LuyenKhiHauKy: return "Luyện Khí Hậu Ký";
            case LevelType.LuyenKhiHauKyDaiVienMan: return "Luyện Khí Hậu Kỳ Đại Viên Mãn";
            case LevelType.TrucCoSoKy: return "Trúc Cơ Sơ Kỳ";
            case LevelType.TrucCoTrungKy: return "Trúc Cơ Trung Kỳ";
            case LevelType.TrucCoHauKy: return "Trúc Cơ Hậu Kỳ";
            case LevelType.TrucCoHauKyDaiVienMan: return "Trúc Cơ Hậu Kỳ Đại Viên Mãn";
            case LevelType.KetDanSoKy: return "Kết Đan Sơ Kỳ";
            case LevelType.KetDanTrungKy: return "Kết Đan Trung Kỳ";
            case LevelType.KetDanHauKy: return "Kết Đan Hậu Kỳ";
            case LevelType.KetDanHauKyDaiVienMan: return "Kết Đan Hậu Kỳ Đại Viên Mãn";
            case LevelType.NguyenAnhSoKy: return "Nguyên Anh So Kỳ";
            case LevelType.NguyenAnhTrungKy: return "Nguyên Anh Trung Kỳ";
            case LevelType.NguyenAnhHauKy: return "Nguyên Anh Hậu Kỳ";
            case LevelType.NguyenAnhHauKyDaiVienMan: return "Nguyên Anh Hậu Kỳ Đại Viên Mãn";
            case LevelType.HoaThanSoKy: return "Hóa Thần Sơ Kỳ";
            case LevelType.HoaThanTrungKy: return "Hóa Thần Trung Kỳ";
            case LevelType.HoaThanHauKy: return "Hóa Thần Hậu Kỳ";
            case LevelType.HoaThanHauKyDaiVienMan: return "Hóa Thần Hậu Kỳ Đại Viên Mãn";

            default:
                return "Lỗi LevelType Name ở UI_Level";
        }
    }
    public static string GetBodyUI(BodyType type)
    {
        switch (type)
        {
            case BodyType.PhamThe: return "Phàm Thể";
            case BodyType.LuyenBiTang01: return "Luyện Bì Cảnh Nhất Trọng";
            case BodyType.LuyenBiTang02: return "Luyện Bì Cảnh Nhị Trọng";
            case BodyType.LuyenBiTang03: return "Luyện Bì Cảnh Tam Trọng";
            case BodyType.LuyenBiTang04: return "Luyện Bì Cảnh Tứ Trọng";
            case BodyType.LuyenBiTang05: return "Luyện Bì Cảnh Ngũ Trọng";
            case BodyType.LuyenBiTang06: return "Luyện Bì Cảnh Lục Trọng";
            case BodyType.LuyenBiTang07: return "Luyện Bì Cảnh Thất Trọng";
            case BodyType.LuyenBiTang08: return "Luyện Bì Cảnh Viên Mãn";
            case BodyType.LuyenBiTang09: return "Luyện Bì Cảnh Đại Viên Mãn";
            case BodyType.LuyenNhucTang01: return "Luyện Nhục Cảnh Nhất Trọng";
            case BodyType.LuyenNhucTang02: return "Luyện Nhục Cảnh Nhị Trọng";
            case BodyType.LuyenNhucTang03: return "Luyện Nhục Cảnh Tam Trọng";
            case BodyType.LuyenNhucTang04: return "Luyện Nhục Cảnh Tứ Trọng";
            case BodyType.LuyenNhucTang05: return "Luyện Nhục Cảnh Ngũ Trọng";
            case BodyType.LuyenNhucTang06: return "Luyện Nhục Cảnh Lục Trọng";
            case BodyType.LuyenNhucTang07: return "Luyện Nhục Cảnh Thất Trọng";
            case BodyType.LuyenNhucTang08: return "Luyện Nhục Cảnh Viên Mãn";
            case BodyType.LuyenNhucTang09: return "Luyện Nhục Cảnh Đại Viên Mãn";
            case BodyType.LuyenCanTang01: return "Luyện Căn Cảnh Nhất Trọng";
            case BodyType.LuyenCanTang02: return "Luyện Căn Cảnh Nhị Trọng";
            case BodyType.LuyenCanTang03: return "Luyện Căn Cảnh Tam Trọng";
            case BodyType.LuyenCanTang04: return "Luyện Căn Cảnh Tứ Trọng";
            case BodyType.LuyenCanTang05: return "Luyện Căn Cảnh Ngũ Trọng";
            case BodyType.LuyenCanTang06: return "Luyện Căn Cảnh Lục Trọng";
            case BodyType.LuyenCanTang07: return "Luyện Căn Cảnh Thất Trọng";
            case BodyType.LuyenCanTang08: return "Luyện Căn Cảnh Viên Mãn";
            case BodyType.LuyenCanTang09: return "Luyện Căn Cảnh Đại Viên Mãn";
            case BodyType.LuyenCotTang01: return "Luyện Cốt Cảnh Nhất Trọng";
            case BodyType.LuyenCotTang02: return "Luyện Cốt Cảnh Nhị Trọng";
            case BodyType.LuyenCotTang03: return "Luyện Cốt Cảnh Tam Trọng";
            case BodyType.LuyenCotTang04: return "Luyện Cốt Cảnh Tứ Trọng";
            case BodyType.LuyenCotTang05: return "Luyện Cốt Cảnh Ngũ Trọng";
            case BodyType.LuyenCotTang06: return "Luyện Cốt Cảnh Lục Trọng";
            case BodyType.LuyenCotTang07: return "Luyện Cốt Cảnh Thất Trọng";
            case BodyType.LuyenCotTang08: return "Luyện Cốt Cảnh Viên Mãn";
            case BodyType.LuyenCotTang09: return "Luyện Cốt Cảnh Đại Viên Mãn";
            case BodyType.LuyenTuyTang01: return "Luyện Tủy Cảnh Nhất Trọng";

            default:
                return "Lỗi BodyType Name ở UI_Level";
        }
    }
}