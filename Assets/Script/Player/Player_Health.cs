using System.Runtime.CompilerServices;
using UnityEngine;

public class Player_Health : Entity_Health
{
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }


    public override bool TakeDamage(float damage, float eleDamage, ElementalType elementalType, Transform damageDealer)
    {
        bool tookDamage = base.TakeDamage(damage, eleDamage, elementalType, damageDealer);

        // Chỉ phát audio khi thực sự nhận sát thương và damage là physical damage (damage > 0).
        if (tookDamage && damage > 0)
            player.sfx.PlayTakePhysicalDamage();

        return tookDamage;
    }

    protected override void Die()
    {
        base.Die();


        player.ui.OpenDeathUI();

    }
}
