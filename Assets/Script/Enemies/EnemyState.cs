using UnityEngine;

public class EnemyState : EntityState
{
    protected Enemy enemy;


    public EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.enemy = enemy;
        rb = enemy.rb;
        anim = enemy.anim;
        start = enemy.start;
    }

    public override void UpdateAnimParameter()
    {
        base.UpdateAnimParameter();

        float battleAnimSpeed = enemy.battleSpeed / enemy.speed;

        anim.SetFloat("animSpeed", enemy.animSpeed);
        anim.SetFloat("battleAnimSpeed", battleAnimSpeed);
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
    }

}
