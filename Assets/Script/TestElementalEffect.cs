using UnityEngine;

/// <summary>
/// ================================ SCRIPT TEST - CHỈ DÙNG ĐỂ TEST ================================
/// MỤC ĐÍCH:
/// Cho phép test nhanh TẤT CẢ Elemental Effect (Fire, Ice, Toxic, Electric, Wind, Earth, Wood, Water)
/// thông qua đòn ĐÁNH THƯỜNG của Player, thay vì phải setup Skill mới có gắn nguyên tố.
///
/// VÌ SAO CẦN SCRIPT NÀY:
/// Đánh thường (xem Entity_Combat.PerformAttack) CHỦ Ý luôn dùng eleDamage = 0 và element = None,
/// tức Physical Damage và Elemental Damage được tách biệt hoàn toàn theo thiết kế gốc của game.
/// Script này KHÔNG chỉnh sửa logic gốc đó, mà chỉ lắng nghe (subscribe) sự kiện có sẵn
/// "OnDoingPhysicDamage" của CombatState/Entity_Combat (event tự bắn ra mỗi khi đòn đánh thường
/// trúng mục tiêu) để tự bồi thêm Elemental Damage + Elemental Status Effect theo Elemental Type
/// mà bạn chọn trong Inspector - hoàn toàn giống cách các ItemEffect (VD: ItemEffect_IceSlow) đang làm.
///
/// CÁCH DÙNG:
/// 1. Gắn script này vào Prefab/GameObject Player.
/// 2. Trong Inspector sẽ hiện mục "Elemental Type" -> chọn nguyên tố muốn test (Fire, Ice, Toxic,
///    Electric, Wind, Earth, Wood, Water).
/// 3. Vào game, đánh thường trúng Enemy -> Enemy sẽ nhận thêm Elemental Damage và bị dính Status
///    Effect tương ứng (Ice làm chậm, Fire/Toxic gây DoT, Electric tích điện, Wind hất tung,
///    Earth giảm giáp + có tỉ lệ choáng, Wood trói chân, Water có tỉ lệ gây True Damage).
/// 4. Test xong -> XÓA (Remove Component) script này khỏi Player là xong. Player sẽ trở lại đánh
///    thường thuần Physical Damage như ban đầu, KHÔNG để lại bất kỳ ảnh hưởng gì, vì script chỉ
///    subscribe vào event có sẵn chứ không sửa bất kỳ file gốc nào của hệ thống (Entity_Combat,
///    Entity_Health, Entity_Status...).
///
/// LƯU Ý VỀ EXECUTION ORDER:
/// player.combat chỉ được gán bên trong Player.Awake(). Vì Unity KHÔNG đảm bảo thứ tự Awake/OnEnable
/// giữa các component khác nhau trên cùng GameObject, script này subscribe event ở Start() (Unity
/// đảm bảo Start() luôn chạy sau khi TẤT CẢ Awake() trong scene đã chạy xong) thay vì OnEnable(),
/// để tránh trường hợp subscribe thất bại âm thầm vì player.combat vẫn còn null.
/// ===================================================================================================
/// </summary>
[DisallowMultipleComponent]
public class TestElementalEffect : MonoBehaviour
{
    [Header("=== TEST ONLY: Chọn nguyên tố để test qua đánh thường ===")]
    [Tooltip("Nguyên tố sẽ tự động được cộng thêm vào mỗi đòn đánh thường trúng mục tiêu.")]
    [SerializeField] private ElementalType elementalType = ElementalType.Fire;

    [Header("Tuỳ chọn Test")]
    [Tooltip("Bật: đòn đánh thường sẽ gây thêm Elemental Damage (dựa theo maxElementalDmg/incElementalDmg của Player).")]
    [SerializeField] private bool testElementalDamage = true;

    [Tooltip("Bật: đòn đánh thường sẽ Apply Status Effect (Ice làm chậm, Fire/Toxic DoT, Wind hất tung,...) lên mục tiêu.")]
    [SerializeField] private bool testStatusEffect = true;

    [Tooltip("Hệ số nhân Elemental Damage khi test, tương tự xElementalDamageMulti của Skill.")]
    [SerializeField] private float elementalDamageMultiplier = 1f;

    [Header("Debug")]
    [Tooltip("In log ra Console mỗi khi subscribe thành công / mỗi khi apply hiệu ứng lên mục tiêu.")]
    [SerializeField] private bool logDebug = true;

    private Player player;
    private bool isSubscribed;

    private void Awake()
    {
        player = GetComponent<Player>();

        if (player == null)
            Debug.LogWarning("[TestElementalEffect] Script này chỉ nên gắn vào Player - không tìm thấy component Player trên GameObject này!");
    }

    // Start() luôn chạy SAU khi mọi Awake() trong scene đã chạy xong -> player.combat chắc chắn đã được gán.
    private void Start()
    {
        TrySubscribe();
    }

    // Vẫn thử subscribe ở OnEnable cho trường hợp bật/tắt component lúc runtime sau khi Start() đã chạy.
    private void OnEnable()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void TrySubscribe()
    {
        if (isSubscribed)
            return;

        if (player == null)
            player = GetComponent<Player>();

        if (player == null || player.combat == null)
            return; // Player.Awake() chưa kịp gán combat -> Start() sẽ thử lại sau.

        player.combat.OnDoingPhysicDamage += OnNormalAttackHit;
        isSubscribed = true;

        if (logDebug)
            Debug.Log($"[TestElementalEffect] Đã subscribe vào Player thành công. Elemental Type đang test: {elementalType}");
    }

    private void Unsubscribe()
    {
        if (!isSubscribed)
            return;

        if (player != null && player.combat != null)
            player.combat.OnDoingPhysicDamage -= OnNormalAttackHit;

        isSubscribed = false;
    }

    // Được gọi mỗi khi đòn đánh thường của Player trúng mục tiêu (target đã nhận Physical Damage).
    private void OnNormalAttackHit(float physicalDamage, Transform target)
    {
        if (elementalType == ElementalType.None || target == null)
            return;

        IDamagable damagable = target.GetComponent<IDamagable>();
        Entity_Status targetStatus = target.GetComponent<Entity_Status>();

        // 1. Cộng thêm Elemental Damage cho đòn đánh thường (test riêng, KHÔNG đụng vào Physical Damage gốc).
        if (testElementalDamage && damagable != null && player.start != null)
        {
            float eleDamage = player.start.GetElementalDamage(elementalType, elementalDamageMultiplier);
            if (eleDamage > 0f)
                damagable.TakeDamage(0f, eleDamage, elementalType, transform);
        }

        // 2. Apply Status Effect tương ứng lên mục tiêu (Ice/Fire/Toxic/Electric/Wind/Earth/Wood/Water).
        if (testStatusEffect && targetStatus != null && player.start != null)
        {
            ElementalDataEffect effectData = new ElementalDataEffect(player.start, player.combat.scaleFactor);
            targetStatus.ApplyStatusEffect(elementalType, effectData, player.start);

            if (logDebug)
                Debug.Log($"[TestElementalEffect] Đã apply {elementalType} Status Effect lên {target.name}");
        }
    }
}
