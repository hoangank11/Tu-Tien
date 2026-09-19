using UnityEngine;

public class JumpState : AiredState
{
    public JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.sfx.PlayJump();
        player.SetVelocity(rb.linearVelocity.x, player.jumpForce);
    }


    public override void Update()
    {
        base.Update();

        if (rb.linearVelocity.y < 0 && stateMachine.currentState != player.jumpAttackState)
            stateMachine.ChangeState(player.fallState);
    }
}
