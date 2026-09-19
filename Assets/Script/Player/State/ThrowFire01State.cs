using UnityEngine;

public class ThrowFire01State : PlayerState
{
    public ThrowFire01State(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.sfx.PlayThrowFire();
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(0, 0);
        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
