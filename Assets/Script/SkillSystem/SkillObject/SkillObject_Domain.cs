using UnityEngine;

public class SkillObject_Domain : SkillObject_Base
{
    private Skill_Domain domainManager;
    private float expandSpeed = 2;
    private float duration;
    private float slowPercent = 1;

    private Vector3 targetScale;
    private bool isShrinking;

    public void SetupDomain(Skill_Domain domainManager)
    {
        this.domainManager = domainManager;
        duration = domainManager.duration;
        float maxSize = domainManager.maxSize;
        float expandSpeed = domainManager.expandSpeed;


        targetScale = Vector3.one * maxSize;
        Invoke(nameof(ShrinkDomain), duration);
    }

    private void Update()
    {
        HandleScaling();
    }


    private void HandleScaling()
    {
        float size = Mathf.Abs(transform.localScale.x - targetScale.x);
        bool shouldChangeScale = size > .1f;

        if (shouldChangeScale)
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, expandSpeed * Time.deltaTime);
        if (isShrinking && size < .1f)
            TerminateDomain();

    }

    private void TerminateDomain()
    {
        domainManager.ClearTarget();
        Destroy(gameObject);
    }

    private void ShrinkDomain()
    {
        targetScale = Vector3.zero;
        isShrinking = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null) return;
        domainManager.AddTarget(enemy);
        enemy.SlowdownEntity(duration, slowPercent, true);

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy == null) return;

        enemy.StopSlowdown();
    }

}
