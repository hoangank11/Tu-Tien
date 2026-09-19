using UnityEngine;

public class GroundedState : PlayerState
{
    public GroundedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.canDoubleJump = true;
    }
    public override void Update()
    {
        base.Update();

        /* Đại khái là chỉ có thể sài skill trên mặt đất.
         Lí do khá đơn giản là k có anim sài đc trên không.
        Hoặc không thích */

        if (rb.linearVelocity.y < 0 && player.groundDetected == false)
            stateMachine.ChangeState(player.fallState);

        if (input.Player.Jump.WasPressedThisFrame())
            stateMachine.ChangeState(player.jumpState);

        if (input.Player.Attack.WasPressedThisFrame())
            stateMachine.ChangeState(player.basicAttackState);

        if (input.Player.Def.WasPressedThisFrame() && skillManager.def.CanUseSkill())
            stateMachine.ChangeState(player.counterAttackState);

        if (input.Player.Skill01.WasPressedThisFrame() && skillManager.fire.CanUseSkill())
        {
            stateMachine.ChangeState(player.throwState01State);
            skillManager.fire.SetSkillOnCooldown();
        }

        if (input.Player.Skill02.WasPressedThisFrame() && skillManager.sword.CanUseSkill())
            stateMachine.ChangeState(player.throwSwordState);

        if (input.Player.Skill03.WasPressedThisFrame() && skillManager.echo.CanUseSkill())
        {
            stateMachine.ChangeState(player.echoState);
            skillManager.echo.SetSkillOnCooldown();
        }

        if (input.Player.Skill04.WasPressedThisFrame() && skillManager.domain.CanUseSkill())
        {
            stateMachine.ChangeState(player.domainState);
            skillManager.domain.SetSkillOnCooldown();
        }

        if (input.Player.Skill05.WasPressedThisFrame() && skillManager.lotus.CanUseSkill())
        {
            stateMachine.ChangeState(player.hpBuffState);
            skillManager.lotus.SetSkillOnCooldown();
        }





        //test
        if (input.Player.Shoot.WasPressedThisFrame() && skillManager.hand01.CanUseSkill())
        {
            stateMachine.ChangeState(player.handState);
            skillManager.hand01.SetSkillOnCooldown();
        }


    }

}
