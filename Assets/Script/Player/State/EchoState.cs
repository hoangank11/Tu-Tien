using UnityEngine;

public class EchoState : PlayerState
{
    public EchoState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        skillManager.echo.TryUseSkill();
        player.sfx.PlayEcho();
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(0, 0);
        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }


}
