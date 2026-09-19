using UnityEngine;

public class Enemy_MoveState : Enemy_GroundedState
{
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemy.sfx.ResetMoveLoop(); // đảm bảo lần di chuyển mới phát audio ngay lập tức

        if (enemy.groundDetected == false || enemy.wallDetected)
            enemy.Flip();

    }

    public override void Update()
    {
        base.Update();

        enemy.sfx.TryPlayMoveLoop(true); // audio di chuyển lặp lại mỗi 0.5f giây, giống Player

        enemy.SetVelocity(enemy.GetMoveSpeed() * enemy.facingDir, rb.linearVelocity.y);

        if (enemy.groundDetected == false || enemy.wallDetected)
            stateMachine.ChangeState(enemy.idleState);

    }
}
