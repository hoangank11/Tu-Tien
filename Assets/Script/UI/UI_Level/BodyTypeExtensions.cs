using System;

public static class BodyTypeExtensions
{
    public const int IconCount = 6;

    /// <summary>
    /// Trả về index (0 - 4) của ảnh tương ứng với BodyType, dùng để
    /// lấy sprite trong mảng ảnh (bodyIcons) trên UI.
    /// </summary>
    public static int GetIconIndex(this BodyType type)
    {
        switch (type)
        {
            case BodyType.PhamThe:
                return 0;

            case BodyType.LuyenBiTang01:
            case BodyType.LuyenBiTang02:
            case BodyType.LuyenBiTang03:
            case BodyType.LuyenBiTang04:
            case BodyType.LuyenBiTang05:
            case BodyType.LuyenBiTang06:
            case BodyType.LuyenBiTang07:
            case BodyType.LuyenBiTang08:
            case BodyType.LuyenBiTang09:
                return 1;

            case BodyType.LuyenNhucTang01:
            case BodyType.LuyenNhucTang02:
            case BodyType.LuyenNhucTang03:
            case BodyType.LuyenNhucTang04:
            case BodyType.LuyenNhucTang05:
            case BodyType.LuyenNhucTang06:
            case BodyType.LuyenNhucTang07:
            case BodyType.LuyenNhucTang08:
            case BodyType.LuyenNhucTang09:
                return 2;

            case BodyType.LuyenCanTang01:
            case BodyType.LuyenCanTang02:
            case BodyType.LuyenCanTang03:
            case BodyType.LuyenCanTang04:
            case BodyType.LuyenCanTang05:
            case BodyType.LuyenCanTang06:
            case BodyType.LuyenCanTang07:
            case BodyType.LuyenCanTang08:
            case BodyType.LuyenCanTang09:
                return 3;

            case BodyType.LuyenCotTang01:
            case BodyType.LuyenCotTang02:
            case BodyType.LuyenCotTang03:
            case BodyType.LuyenCotTang04:
            case BodyType.LuyenCotTang05:
            case BodyType.LuyenCotTang06:
            case BodyType.LuyenCotTang07:
            case BodyType.LuyenCotTang08:
            case BodyType.LuyenCotTang09:
                return 4;

            case BodyType.LuyenTuyTang01:
                return 5;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, "BodyType chưa được map vào icon nào ở BodyTypeExtensions.");
        }
    }
}
