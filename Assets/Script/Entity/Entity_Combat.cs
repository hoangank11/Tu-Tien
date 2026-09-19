using System;
using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    public event Action<float, Transform> OnDoingPhysicDamage;
    private Entity_SFX sfx;
    private Entity_VFX vfx;
    private Entity_Start start;
    public ScaleFactor scaleFactor;
    [Header("Target Detection")]
    [SerializeField] private Transform targetCheck;
    [Range(.1f, 5)]
    [SerializeField] private float targetCheckRadius;
    [SerializeField] private LayerMask whatIsTarget;


    private void Awake()
    {
        vfx = GetComponent<Entity_VFX>();
        sfx = GetComponent<Entity_SFX>();
        start = GetComponent<Entity_Start>();
    }

    public void PerformAttack()
    {
        foreach (var target in GetDetectedColliders())
        {
            IDamagable damegable = target.GetComponent<IDamagable>();
            if (damegable == null)
                continue;

            AttackData attackData = start.GetAttackData(scaleFactor);
            Entity_Status status = target.GetComponent<Entity_Status>();

            float damage = attackData.damage;
            float eleDamage = 0f; // Đánh thường chỉ gây Physical Damage, tuyệt đối không gây Elemental Damage.
            ElementalType element = ElementalType.None; // Đánh thường không có nguyên tố.

            bool targetGotHit = damegable.TakeDamage(damage, eleDamage, element, transform);

            // Đánh thường không Apply Elemental Status Effect.

            if (targetGotHit)
            {
                OnDoingPhysicDamage?.Invoke(damage, target.transform);
                vfx.CreateHitVFX(target.transform, attackData.isCrit, element);
            }

        }

    }

    protected Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatIsTarget);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
}
