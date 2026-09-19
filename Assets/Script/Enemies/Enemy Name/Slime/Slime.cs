using UnityEngine;

public class Slime : Enemy, ICounterable
{
    bool ICounterable.canBeCounter { get => canBeStun; }

    [SerializeField] private GameObject childSlime;
    [SerializeField] private int spawnSlimeNumber;
    [SerializeField] private Vector2 velocitySpawn;
    public Slime_DieState slimeDieState { get; set; }

    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        battleState = new Enemy_BattleState(this, stateMachine, "battle");
        slimeDieState = new Slime_DieState(this, stateMachine, "die");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "stun");

    }
    public override void EntityDeath()
    {
        stateMachine.ChangeState(slimeDieState);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    public void CreateSlime()
    {
        if(childSlime == null)
            return;

        for (int i = 0; i < spawnSlimeNumber; i++)
        {
            GameObject newSlime = Instantiate(childSlime, transform.position, Quaternion.identity);
            if (newSlime == null)
                continue;
            newSlime.GetComponent<Slime>().SetupSlime(velocitySpawn);
        }
    }

    public void SetupSlime(Vector2 velocity)
    {
        rb.linearVelocity = new Vector2(velocity.x * -facingDir, velocity.y);
    }


    [ContextMenu("Stun Enemy")]
    public void HandleCounter()
    {
        if (canBeStun == false)
            return;
        stateMachine.ChangeState(stunnedState);
    }



}
