using UnityEngine;

public class Enemy_AnimationTrigger : Entity_AnimationTriggers
{
    private Enemy enemy;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponentInParent<Enemy>();
    }

    private void EnableCounter()
    {
        enemy.EnableCounter(true);
    }

    private void DisableCounter()
    {
        enemy.EnableCounter(false);
    }
}
