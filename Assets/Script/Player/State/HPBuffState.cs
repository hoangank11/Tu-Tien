using UnityEngine;

public class HPBuffState : PlayerState
{
    public HPBuffState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        skillManager.lotus.TryUseSkill();
        player.sfx.PlayHPBuff();

    }
    public override void Update()
    {
        base.Update();
        player.SetVelocity(0, 0);
        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
