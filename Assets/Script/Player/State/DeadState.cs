using UnityEngine;

public class DeadState : PlayerState
{
    public DeadState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        input.Disable();
        rb.simulated = false;
    }
}
