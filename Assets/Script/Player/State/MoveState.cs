using UnityEngine;

public class MoveState : GroundedState
{
    public MoveState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.sfx.ResetMoveLoop(); // đảm bảo lần di chuyển mới phát audio ngay lập tức
    }

    public override void Update()
    {
        base.Update();

        player.sfx.TryPlayMoveLoop(player.moveInput.x != 0); // audio bước chân lặp lại mỗi 0.5f giây

        if (player.moveInput.x == 0 || player.wallDetected)
            stateMachine.ChangeState(player.idleState);


        player.SetVelocity(player.moveInput.x * player.moveSpeed, rb.linearVelocity.y);
    }
}
