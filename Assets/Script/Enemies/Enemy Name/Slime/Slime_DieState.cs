using UnityEngine;

public class Slime_DieState : Enemy_DieState
{
    private Slime slime;
    public Slime_DieState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        slime = enemy as Slime;
    }

    public override void Enter()
    {
        base.Enter();

        slime.CreateSlime();
    }
}
