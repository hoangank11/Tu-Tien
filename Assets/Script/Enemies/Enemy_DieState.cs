using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_DieState : EnemyState
{
    private Collider2D col;
    public Enemy_DieState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        col = enemy.GetComponent<Collider2D>();
    }

    public override void Enter()
    {
        base.Enter();

        enemy.sfx.PlayDie();
        enemy.SetVelocity(0, 5);

        anim.enabled = false;
        col.enabled = false;

        rb.gravityScale = 12;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -10);

        stateMachine.SwitchOffState();
        enemy.DestroyEnemy(3);

    }

}
