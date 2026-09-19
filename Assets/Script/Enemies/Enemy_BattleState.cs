using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;
    private Transform lastTarget;
    private float lastTimeWasInBattle;
    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        UpdateBattleTimer();
        enemy.sfx.PlayBattle();
        player ??= enemy.GetPlayerRef(); // if (player == null)

        if (ShouldRetreat())
        {
            rb.linearVelocity =
                new Vector2((enemy.retreatVelocity.x * enemy.activeSlowMul) * -DirectionToPlayer(), enemy.retreatVelocity.y);
            enemy.HandleFlip(DirectionToPlayer());
        }

    }

    public override void Update()
    {
        base.Update();

        if (enemy.playerDetection())
        {
            UpdateTargetNeed();
            UpdateBattleTimer();
        }

        if (BattleTimeOver())
            stateMachine.ChangeState(enemy.idleState);

        if (WithinAttackRange() && enemy.playerDetection())
            stateMachine.ChangeState(enemy.attackState);
        else
            enemy.SetVelocity(enemy.GetBattleSpeed() * DirectionToPlayer(), rb.linearVelocity.y);

    }


    private void UpdateTargetNeed()
    {
        if (enemy.playerDetection() == false)
            return;

        Transform newTarget = enemy.playerDetection().transform;
        if (newTarget != lastTarget)
        {
            lastTarget = newTarget;
            player = newTarget;
        }
    }

    private void UpdateBattleTimer() => lastTimeWasInBattle = Time.time;
    private bool BattleTimeOver() => Time.time > lastTimeWasInBattle + enemy.battleTime;
    private bool WithinAttackRange() => DistanceToPlayer() < enemy.attackDistance;
    private bool ShouldRetreat() => DistanceToPlayer() < enemy.minRetreatDistance;

    private float DistanceToPlayer()
    {
        if (player == null)
            return float.MaxValue;
        return Mathf.Abs(player.position.x - enemy.transform.position.x);
    }

    private int DirectionToPlayer()
    {
        if (player == null)
            return 0;
        return player.position.x > enemy.transform.position.x ? 1 : -1;
    }

}
