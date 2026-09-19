using System.Collections;
using UnityEngine;

public class SkillObject_SwordKi : SkillObject_Base
{
    protected Skill_ThrowSword swordManager;
    private float destroy = 3f;
    private int amountToPeirce;


    protected virtual void Update()
    {
        transform.right = rb.linearVelocity;
    }

    public virtual void SetupSword(Skill_ThrowSword swordManager, Vector2 direction)
    {
        rb.linearVelocity = direction;
        this.swordManager = swordManager;

        start = swordManager.player.start;
        scaleFactor = swordManager.scaleFactor;
        amountToPeirce = swordManager.peirceAmount;
    }

    protected void StopSword(Collider2D collision)
    {
        rb.simulated = false;
        transform.parent = collision.transform;
        Destroy(gameObject, destroy);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        bool groundHit = collision.gameObject.layer == LayerMask.NameToLayer("Ground");

        if (amountToPeirce <= 0 || groundHit)
        {
            DamageEnemiesInRadius(transform, 1, ElementalType.None);
            StopSword(collision);
            return;
        }
        amountToPeirce--;
        DamageEnemiesInRadius(transform, 1, ElementalType.None);
    }

}
