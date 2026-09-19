using UnityEngine;

public class ThrowSwordState : PlayerState
{
    private Camera mainCamera;
    public ThrowSwordState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        skillManager.sword.EnableDots(true);

        if (mainCamera != Camera.main)
            mainCamera = Camera.main;

    }

    public override void Update()
    {
        base.Update();

        Vector2 dirToMouse = DirectionToMouse();

        player.SetVelocity(0, rb.linearVelocity.y);
        player.HandleFlip(dirToMouse.x);
        skillManager.sword.PredictTrajectory(dirToMouse);

        if (input.Player.Attack.WasPressedThisFrame())
        {
            anim.SetBool("throwSwordPerform", true);
            skillManager.sword.EnableDots(false);
            skillManager.sword.ComfirmTrajectory(dirToMouse);
            player.sfx.PlayThrowSword();
        }

        if (input.Player.Skill02.WasPressedThisFrame() || triggerCalled)
            stateMachine.ChangeState(player.idleState);

        if (input.Player.Def.WasPressedThisFrame())
            stateMachine.ChangeState(player.counterAttackState);

        if (input.Player.Dash.WasPressedThisFrame())
            stateMachine.ChangeState(player.dashState);

        if (input.Player.Jump.WasPressedThisFrame())
            stateMachine.ChangeState(player.jumpState);

    }

    public override void Exit()
    {
        base.Exit();

        anim.SetBool("throwSwordPerform", false);
        skillManager.sword.EnableDots(false);
    }

    private Vector2 DirectionToMouse()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 worldMousePosition = mainCamera.ScreenToWorldPoint(player.mousePosition);

        Vector2 direction = worldMousePosition - playerPosition;

        return direction.normalized;
    }

}
