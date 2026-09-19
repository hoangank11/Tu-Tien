using UnityEngine;

public class SkillObject_Base : MonoBehaviour
{
    [SerializeField] private GameObject OnHitVFX;
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected float checkRadius = 1f;
    protected Rigidbody2D rb;

    protected Animator anim;
    protected Entity_Start start;
    protected ScaleFactor scaleFactor;
    protected ElementalType elemental;
    protected Entity_Combat combat;
    protected bool targetGotHit;
    protected Transform lastTarget;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected void DamageEnemiesInRadius(Transform t, float radius, ElementalType elemental)
    {
        foreach (var target in EnemiesAround(t, radius))
        {
            IDamagable damagable = target.GetComponent<IDamagable>();
            if (damagable == null)
                continue;

            AttackData attackData = start.GetAttackData(scaleFactor, elemental);
            Entity_Status status = target.GetComponent<Entity_Status>();

            float damage = attackData.damage;
            float eleDamage = attackData.eleDamage;

            targetGotHit = damagable.TakeDamage(damage, eleDamage, elemental, transform);

            if (elemental != ElementalType.None)
                status?.ApplyStatusEffect(elemental, attackData.effectData, start);
            if (targetGotHit)
            {
                lastTarget = target.transform;
                Instantiate(OnHitVFX, target.transform.position, Quaternion.identity);
            }
        }
    }

    protected Transform FindTarget()
    {
        Transform target = null;
        float closesDistance = Mathf.Infinity;

        foreach (var Enemy in EnemiesAround(transform, 10f))
        {
            float distance = Vector2.Distance(transform.position, Enemy.transform.position);

            if (distance < closesDistance)
            {
                target = Enemy.transform;
                closesDistance = distance;
            }
        }
        return target;
    }

    protected Collider2D[] EnemiesAround(Transform t, float radius)
    {
        return Physics2D.OverlapCircleAll(t.position, radius, whatIsEnemy);
    }

    protected virtual void OnDrawGizmos()
    {
        if (targetCheck == null)
            targetCheck = transform;

        Gizmos.DrawWireSphere(targetCheck.position, checkRadius);
    }

}
