using UnityEngine;

public class FallState : AiredState
{
    public FallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.groundDetected)
        {
            player.sfx.PlayLand();
            stateMachine.ChangeState(player.idleState);
        }

        if (player.wallDetected)
            stateMachine.ChangeState(player.wallSlideState);
    }
}
