using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Item/Item Effect/Heal On Damage", fileName = "Item Effect - Heal On Damage")]
public class ItemEffect_HealOnDamage : ItemEffectDataSO
{
    [SerializeField] private float percentHealOnAttack = .05f;
    [Header("VFX")]
    [SerializeField] private GameObject hitVFX;

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        player.combat.OnDoingPhysicDamage += HealOnDoingDamage;
    }
    public override void Unsubscribe()
    {
        base.Unsubscribe();
        player.combat.OnDoingPhysicDamage -= HealOnDoingDamage;
        player = null;
    }

    private void HealOnDoingDamage(float damage, Transform target)
    {
        player.health.RegenHealth(damage * percentHealOnAttack);
        if (hitVFX != null)
            player.playerVFX.CreateEffectOf(hitVFX, target.transform);
    }

}
