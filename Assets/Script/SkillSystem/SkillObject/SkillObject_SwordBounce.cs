using System.Collections.Generic;
using UnityEngine;

public class SkillObject_SwordBounce : SkillObject_SwordKi
{
    [SerializeField] private float bounceSpeed = 15;
    private int bounceCount;
    private Collider2D[] enemyTarget;
    private Transform nextTarget;
    private List<Transform> selectedTarget = new List<Transform>();


    public override void SetupSword(Skill_ThrowSword swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);
        anim.SetTrigger("spin");
        bounceSpeed = swordManager.bounceSpeed;
        bounceCount = swordManager.bounceCount;
    }

    protected override void Update()
    {
        HandleBounce();
    }
    private void HandleBounce()
    {
        if (nextTarget == null)
            return;
        transform.position = Vector2.MoveTowards(transform.position, nextTarget.position, bounceSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, nextTarget.position) < .75f)
        {
            DamageEnemiesInRadius(transform, 1, ElementalType.None);
            BounceToNextTarget();
            if (bounceCount == 0 || nextTarget == null)
            {
                nextTarget = null;
                DestroyPrefab();
            }
        }

    }

    private void BounceToNextTarget()
    {
        nextTarget = GetNextTarget();
        bounceCount--;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyTarget == null)
        {
            enemyTarget = EnemiesAround(transform, 7);
            rb.simulated = false;
        }
        DamageEnemiesInRadius(transform, 1, ElementalType.None);

        if (enemyTarget.Length < 1 || bounceCount == 0)
            DestroyPrefab();
        else
            nextTarget = GetNextTarget();
    }

    private void DestroyPrefab()
    {
        Destroy(gameObject, .1f);
    }

    private Transform GetNextTarget()
    {
        List<Transform> validTarget = GetValidTarget();
        int randomIndex = Random.Range(0, validTarget.Count);
        Transform nextTarget = validTarget[randomIndex];
        selectedTarget.Add(nextTarget); 
        return nextTarget;
    }

    private List<Transform> GetValidTarget()
    {
        List<Transform> validTarget = new List<Transform>();
        List<Transform> aliveTarget = GetAliveTarget();
        foreach (var enemy in GetAliveTarget())
        {
            if (enemy != null && selectedTarget.Contains(enemy.transform) == false)
            {
                validTarget.Add(enemy.transform);
            }
        }
        if (validTarget.Count > 0)
            return validTarget;
        else
        {
            selectedTarget.Clear();
            return aliveTarget;
        }

    }
    private List<Transform> GetAliveTarget()
    {
        List<Transform> aliveTargets = new List<Transform>();

        foreach (var enemy in enemyTarget)
        {
            if (enemy != null)
                aliveTargets.Add(enemy.transform);
        }

        return aliveTargets;
    }


    
}
