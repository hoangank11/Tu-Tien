using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Item/Item Effect/Light", fileName = "Item Effect - Light Stun")]
public class ItemEffect_Light : ItemEffectDataSO
{
    [Header("Stun Chance")]
    [Range(0, 100)]
    [SerializeField] private float stunChance = 2f;

    [Header("VFX")]
    [SerializeField] private GameObject stunVFX;

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        player.combat.OnDoingPhysicDamage += TryStunOnAttack;
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        player.combat.OnDoingPhysicDamage -= TryStunOnAttack;
        player = null;
    }

    private void TryStunOnAttack(float damage, Transform target)
    {
        bool stunSuccess = Random.Range(0, 100) < stunChance;
        if (!stunSuccess)
            return;

        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy == null)
            return;

        enemy.ForceStun();

        if (stunVFX != null)
            player.playerVFX.CreateEffectOf(stunVFX, target);
    }
}
