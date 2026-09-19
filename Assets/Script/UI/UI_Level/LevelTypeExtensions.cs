using System;
public static class LevelTypeExtensions
{
    public const int IconCount = 7;

    /// <summary>
    /// Trả về index (0 - 6) của ảnh tương ứng với LevelType, dùng để
    /// lấy sprite trong mảng ảnh (levelIcons) trên UI.
    /// </summary>
    public static int GetIconIndex(this LevelType type)
    {
        switch (type)
        {
            case LevelType.PhamNhan:
                return 0;

            case LevelType.DanKhiNhapThe:
                return 1;

            case LevelType.LuyenKhiTang01:
            case LevelType.LuyenKhiTang02:
            case LevelType.LuyenKhiTang03:
            case LevelType.LuyenKhiTang04:
            case LevelType.LuyenKhiTang05:
            case LevelType.LuyenKhiTang06:
            case LevelType.LuyenKhiTang07:
            case LevelType.LuyenKhiTang08:
            case LevelType.LuyenKhiTang09:
            case LevelType.LuyenKhiTang10:
            case LevelType.LuyenKhiHauKy:
            case LevelType.LuyenKhiHauKyDaiVienMan:
                return 2;

            case LevelType.TrucCoSoKy:
            case LevelType.TrucCoTrungKy:
            case LevelType.TrucCoHauKy:
            case LevelType.TrucCoHauKyDaiVienMan:
                return 3;

            case LevelType.KetDanSoKy:
            case LevelType.KetDanTrungKy:
            case LevelType.KetDanHauKy:
            case LevelType.KetDanHauKyDaiVienMan:
                return 4;

            case LevelType.NguyenAnhSoKy:
            case LevelType.NguyenAnhTrungKy:
            case LevelType.NguyenAnhHauKy:
            case LevelType.NguyenAnhHauKyDaiVienMan:
                return 5;

            case LevelType.HoaThanSoKy:
            case LevelType.HoaThanTrungKy:
            case LevelType.HoaThanHauKy:
            case LevelType.HoaThanHauKyDaiVienMan:
                return 6;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, "LevelType chưa được map vào icon nào ở LevelTypeExtensions.");
        }
    }
}
