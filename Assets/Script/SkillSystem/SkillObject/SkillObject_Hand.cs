using UnityEngine;

public class SkillObject_Hand : SkillObject_Base
{
    protected Skill_HandBase handManager;
    [SerializeField] private GameObject groundHitVFX;


    public virtual void SetupHand(Skill_HandBase handManager, float throwPower)
    {
        if (handManager.player.facingDir == 1)
            rb.linearVelocity = new Vector3(throwPower, -2);
        else if (handManager.player.facingDir == -1)
        {
            transform.Rotate(0, 180, 0);
            rb.linearVelocity = new Vector3(-throwPower, -2);
        }

        this.handManager = handManager;

        start = handManager.player.start;
        scaleFactor = handManager.scaleFactor;
    }

    protected void StopHand(Collider2D collision)
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
            DamageEnemiesInRadius(transform, 5, ElementalType.Fire);
            Instantiate(groundHitVFX, transform.position, Quaternion.identity);
            StopHand(collision);
            return;
        }
        DamageEnemiesInRadius(transform, 5, ElementalType.Fire);
        
    }

}
