using UnityEngine;

public class Mushroom : Enemy, ICounterable
{
    bool ICounterable.canBeCounter { get => canBeStun; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        battleState = new Enemy_BattleState(this, stateMachine, "battle");
        dieState = new Enemy_DieState(this, stateMachine, "die");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stun");

    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    [ContextMenu("Stun Enemy")]
    public void HandleCounter()
    {
        if (canBeStun == false)
            return;
        stateMachine.ChangeState(stunnedState);
    }

}
