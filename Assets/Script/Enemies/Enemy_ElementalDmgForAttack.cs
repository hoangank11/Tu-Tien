using UnityEngine;


[DisallowMultipleComponent]
[RequireComponent(typeof(Entity_Combat))]
public class Enemy_ElementalDmgForAttack : MonoBehaviour
{
    [Header("=== Elemental Type riêng của loài Enemy này ===")]
    [Tooltip("Nguyên tố sẽ TỰ ĐỘNG được cộng thêm vào MỌI đòn đánh thường của Enemy này. " +
             "VD: Mushroom -> Toxic. Để None nếu Enemy này đánh thường thuần Physical.")]
    [SerializeField] private ElementalType elementalType = ElementalType.None;

    [Header("Tuỳ chọn")]
    [Tooltip("Bật: đòn đánh thường sẽ gây thêm Elemental Damage (dựa theo maxElementalDmg/incElementalDmg của chính Enemy).")]
    [SerializeField] private bool applyElementalDamage = true;

    [Tooltip("Bật: đòn đánh thường sẽ Apply Status Effect (Ice làm chậm, Fire/Toxic DoT, Wind hất tung,...) lên mục tiêu bị đánh trúng.")]
    [SerializeField] private bool applyStatusEffect = true;

    [Tooltip("Hệ số nhân Elemental Damage, tương tự xElementalDamageMulti của Skill.")]
    [SerializeField] private float elementalDamageMultiplier = 1f;

    [Header("Debug")]
    [Tooltip("In log ra Console mỗi khi Enemy này gây thêm Elemental Damage/Status Effect.")]
    [SerializeField] private bool logDebug = false;

    private Entity_Combat combat;
    private Entity_Start start;
    private bool isSubscribed;

    private void Awake()
    {
        combat = GetComponent<Entity_Combat>();
        start = GetComponent<Entity_Start>();
    }

    // Start() luôn chạy SAU khi mọi Awake() trong scene đã chạy xong -> combat/start chắc chắn đã sẵn sàng.
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

        if (combat == null)
            combat = GetComponent<Entity_Combat>();

        if (combat == null)
            return;

        combat.OnDoingPhysicDamage += OnNormalAttackHit;
        isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!isSubscribed)
            return;

        if (combat != null)
            combat.OnDoingPhysicDamage -= OnNormalAttackHit;

        isSubscribed = false;
    }

    // Được gọi mỗi khi đòn đánh thường của Enemy này trúng mục tiêu (mục tiêu đã nhận Physical Damage).
    private void OnNormalAttackHit(float physicalDamage, Transform target)
    {
        if (elementalType == ElementalType.None || target == null || start == null)
            return;

        IDamagable damagable = target.GetComponent<IDamagable>();
        if (damagable == null)
            return;

        // 1. Cộng thêm Elemental Damage cho đòn đánh thường (test riêng, KHÔNG đụng vào Physical Damage gốc).
        if (applyElementalDamage)
        {
            float eleDamage = start.GetElementalDamage(elementalType, elementalDamageMultiplier);
            if (eleDamage > 0f)
            {
                damagable.TakeDamage(0f, eleDamage, elementalType, transform);

                if (logDebug)
                    Debug.Log($"[Enemy_ElementalDmgForAttack] {name} gây thêm {eleDamage} {elementalType} Damage lên {target.name}");
            }
        }

        // 2. Apply Status Effect tương ứng lên mục tiêu (Ice/Fire/Toxic/Electric/Wind/Earth/Wood/Water).
        if (applyStatusEffect)
        {
            Entity_Status targetStatus = target.GetComponent<Entity_Status>();
            if (targetStatus != null)
            {
                ElementalDataEffect effectData = new ElementalDataEffect(start, combat.scaleFactor);
                targetStatus.ApplyStatusEffect(elementalType, effectData, start);

                if (logDebug)
                    Debug.Log($"[Enemy_ElementalDmgForAttack] {name} đã apply {elementalType} Status Effect lên {target.name}");
            }
        }
    }
}