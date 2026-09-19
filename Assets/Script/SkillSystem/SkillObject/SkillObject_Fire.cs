using UnityEngine;

public class SkillObject_Fire : SkillObject_Base
{
    protected Skill_ThrowFire throwFireManager;
    [SerializeField] private GameObject groundHitVFX;


    public virtual void SetupFire(Skill_ThrowFire throwFireManager, float throwPower)
    {
        if (throwFireManager.player.facingDir == 1)
            rb.linearVelocity = new Vector3(throwPower, 0, 0);
        else if (throwFireManager.player.facingDir == -1)
        {
            gameObject.transform.Rotate(0, 180, 0);
            rb.linearVelocity = new Vector3(-throwPower, 0, 0);
        }

        this.throwFireManager = throwFireManager;

        start = throwFireManager.player.start;
        scaleFactor = throwFireManager.scaleFactor;
    }

    protected void StopFire(Collider2D collision)
    {
        rb.simulated = false;
        transform.parent = collision.transform;
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        bool groundHit = collision.gameObject.layer == LayerMask.NameToLayer("Ground");
        if (groundHit)
        {
            Instantiate(groundHitVFX, transform.position, Quaternion.identity);
            StopFire(collision);
            return;
        }
        DamageEnemiesInRadius(transform, 1, ElementalType.Fire);
    }

}
