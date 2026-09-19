using UnityEngine;
public class DomainState : PlayerState
{

    public DomainState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.sfx.PlayDomain();
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocity(0, 0);
        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);

    }

    public override void Exit()
    {
        base.Exit();
        Levitate();

    }

    private void Levitate()
    {
        skillManager.domain.CreateDomain();
    }

}
