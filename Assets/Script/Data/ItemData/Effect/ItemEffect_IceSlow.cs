using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Item/Item Effect/Ice Slow", fileName = "Item Effect - Ice Slow")]
public class ItemEffect_IceSlow : ItemEffectDataSO
{
    [Header("Slow Chance")]
    [Range(0, 100)]
    [SerializeField] private float slowChance = 8f;
    [Header("VFX")]
    [SerializeField] private GameObject hitVFX;
    [SerializeField] private ElementalDataEffect effectData;

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        player.combat.OnDoingPhysicDamage += TrySlowOnAttack;
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        player.combat.OnDoingPhysicDamage -= TrySlowOnAttack;
        player = null;
    }

    private void TrySlowOnAttack(float damage, Transform target)
    {
        bool slowSuccess = Random.Range(0, 100) < slowChance;
        if (!slowSuccess)
            return;

        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy == null)
            return;

        IDamagable damagable = enemy.GetComponent<IDamagable>();
        if (damagable == null) return;

        bool targetGotHit = damagable.TakeDamage(0, 0, ElementalType.Ice, target.transform);
        Entity_Status status = enemy.GetComponent<Entity_Status>();
        status?.ApplyStatusEffect(ElementalType.Ice, effectData);
        if (targetGotHit)
            player.playerVFX.CreateEffectOf(hitVFX, enemy.transform);
    }
}
