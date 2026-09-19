using UnityEngine;

public class SkillObject_Echo : SkillObject_Base
{
    [SerializeField] private GameObject onDeathVFX;
    [SerializeField] private LayerMask whatIsGround;
    private Skill_Echo echoManager;
    private TrailRenderer trail;
    public int maxAttack { get; private set; }

    public void SetupEcho(Skill_Echo echoManager)
    {
        this.echoManager = echoManager;
        start = echoManager.player.start;
        scaleFactor = echoManager.scaleFactor;
        maxAttack = echoManager.GetMaxAttack();


        Invoke(nameof(EchoDeath), echoManager.GetEchoDuration());
        FlipToTarget();


        trail = GetComponentInChildren<TrailRenderer>();
        trail.gameObject.SetActive(false);

        anim.SetBool("canAttack", maxAttack > 0);

    }

    private void Update()
    {
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        StopMove();
    }


    private void FlipToTarget()
    {
        Transform target = FindTarget();
        if (target != null && target.position.x < transform.position.x)
            transform.Rotate(0, 180, 0);
    }

    public void PerformAttack()
    {
        DamageEnemiesInRadius(targetCheck, 1, ElementalType.None);
        if (targetGotHit == false)
            return;
        bool canDuplicate = Random.value < echoManager.GetDuplicateChance();
        float xOffset = transform.position.x < lastTarget.position.x ? 1 : -1;
        if (canDuplicate)
            echoManager.CreateEcho(lastTarget.position + new Vector3(xOffset, 0));
    }

    public void EchoDeath()
    {
        Instantiate(onDeathVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void StopMove()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, whatIsGround);
        if (hit.collider != null)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }
}
