using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Item/Item Effect/Ice", fileName = "Item Effect - Ice")]
public class ItemEffect_IceOnDamage : ItemEffectDataSO
{
    [SerializeField] private ElementalDataEffect effectData;
    [SerializeField] private float iceDamageMul = 1.5f;
    [SerializeField] private float damageRadius = 1.5f;
    [SerializeField] private LayerMask whatIsEnemy;

    [Space]
    [SerializeField] private float healthPercentTrigger = .15f;
    [SerializeField] private float cooldown = 40;
    private float lastTimeUse = -999;
    [Header("VFX")]
    [SerializeField] private GameObject iceBlastVFX;
    [SerializeField] private GameObject onHitVFX;

    public override void ExecuteEffect()
    {
        bool noCooldown = Time.time >= lastTimeUse + cooldown;
        bool reachthreshod = player.health.GetHPPercent() <= healthPercentTrigger;
        Debug.Log($"HP%: {player.health.GetHPPercent() * 100} | noCooldown: {noCooldown} | reachThreshold: {reachthreshod}");
        if (noCooldown && reachthreshod)
        {
            player.playerVFX.CreateEffectOf(iceBlastVFX, player.transform);
            lastTimeUse = Time.time;
            DamageEnemyByIce();
        }
    }

    private void DamageEnemyByIce()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position, damageRadius, whatIsEnemy);
        float iceDamage = player.start.GetBaseDamage() * iceDamageMul;
        foreach (var enemy in enemies)
        {
            IDamagable damagable = enemy.GetComponent<IDamagable>();
            if (damagable == null) continue;

            bool targetGotHit = damagable.TakeDamage(0, iceDamage, ElementalType.Ice, player.transform);
            Entity_Status status = enemy.GetComponent<Entity_Status>();
            status?.ApplyStatusEffect(ElementalType.Ice, effectData);
            if (targetGotHit)
                player.playerVFX.CreateEffectOf(onHitVFX, enemy.transform);
        }
    }

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        lastTimeUse = -999f;
        player.health.OnTakingDamage += ExecuteEffect;
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        player.health.OnTakingDamage -= ExecuteEffect;
        player = null;
    }

}
