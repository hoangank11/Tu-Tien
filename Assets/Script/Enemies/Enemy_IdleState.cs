using UnityEngine;

public class Enemy_IdleState : Enemy_GroundedState
{
    public Enemy_IdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;
        enemy.sfx.ResetIdleLoop();

        enemy.SetVelocity(0f, rb.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        enemy.sfx.TryPlayIdleLoop(true);

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.moveState);
    }

}