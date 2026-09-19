using UnityEngine;

public class CounterAttackState : PlayerState
{
    private CombatState combatState;
    private bool counterEnemy;
    private bool isHolding;

    private const float dmgReduce = 0.25f; // Giảm 25% dmg nhận vào khi giữ nút.

    public CounterAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        combatState = player.GetComponent<CombatState>();
    }

    public override void Enter()
    {
        base.Enter();
        player.sfx.PlayDef();
        isHolding = false;

        skillManager.def.OnStartEffectt();
        skillManager.def.SetSkillOnCooldown();

        counterEnemy = combatState.CounterAttackPer();
        stateTimer = combatState.CounterAttackCooldown();
        anim.SetBool("counterEnd", counterEnemy);
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(0, 0);

        // Bấm 1 lần trúng enemy -> counterEnem
        if (counterEnemy == false && input.Player.Def.IsPressed())
        {
            if (isHolding == false)
                StartHold();

            // Hủy hold để chuyển sang state khác theo input, giống hệt GroundedState.
            if (input.Player.Jump.WasPressedThisFrame())
            {
                stateMachine.ChangeState(player.jumpState);
                return;
            }

            if (input.Player.Attack.WasPressedThisFrame())
            {
                stateMachine.ChangeState(player.basicAttackState);
                return;
            }

            return; // Giữ nút -> ở lại state này, anim "counterStart" tiếp tục loop.
        }

        // Thả nút trong lúc đang hold -> về idle.
        if (isHolding && input.Player.Def.IsPressed() == false)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        if (triggerCalled)
            stateMachine.ChangeState(player.idleState);

        if (stateTimer < 0 && counterEnemy == false)
            stateMachine.ChangeState(player.idleState);
    }

    public override void Exit()
    {
        base.Exit();

        EndHold();
        skillManager.def.OnEndEffect();
    }

    private void StartHold()
    {
        isHolding = true;
        player.health.SetIncomingDamageMultiplier(1f - dmgReduce);
    }

    private void EndHold()
    {
        if (isHolding == false)
            return;

        isHolding = false;
        player.health.SetIncomingDamageMultiplier(1f);
    }
}
