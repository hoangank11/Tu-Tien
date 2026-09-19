using UnityEngine;

public class SkillObject_SwordSpin : SkillObject_SwordKi
{
    private float attackPerSecond;
    private float attackTimer;


    public override void SetupSword(Skill_ThrowSword swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);
        
        attackPerSecond = swordManager.attackPerSecond;
    }

    protected override void Update()
    {
        HandleAttack();
        DestroySpinSword();

    }
    private void DestroySpinSword()
    {
        Destroy(gameObject, swordManager.spinDestroy);
    }

    private void HandleAttack()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer < 0)
        {
            DamageEnemiesInRadius(transform, 1, ElementalType.None);
            attackTimer = 1/attackPerSecond;
        }
    }


    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        rb.simulated = false;
        anim?.SetTrigger("spin");
    }

}
