using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    public new Enemy_SFX sfx { get; private set; }

    [Header("Quest Infor")]
    public string questTargetID;

    #region Call State
    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttackState attackState;
    public Enemy_BattleState battleState;
    public Enemy_DieState dieState;
    public Enemy_StunnedState stunnedState;

    #endregion

    protected override void Awake()
    {
        base.Awake();
        sfx = GetComponent<Enemy_SFX>();
    }

    #region Valuation
    [Header("Battle Details")]
    [Range(1, 20)]
    public float battleSpeed;
    [Range(0, 10)]
    public float attackDistance;
    public float battleTime = 5;
    public float minRetreatDistance;
    public Vector2 retreatVelocity;


    [Header("Move Details")]
    [Range(.1f, 10)]
    public float speed;
    [Range(1, 10)]
    public float idleTime;
    [Range(1, 5)]
    public float animSpeed;

    [Header("Player Detection")]
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance;
    public Transform player { get; private set; }

    [Header("Stun Details")]
    public float stunDuration;
    public Vector2 stunVelocity;
    private float pendingStunDuration = -1f;
    protected bool canBeStun;


    [Header("Slow/Speed")]
    public float activeSlowMul { get; private set; } = 1;



    #endregion




    public float GetMoveSpeed() => speed * activeSlowMul;
    public float GetBattleSpeed() => battleSpeed * activeSlowMul;

    public void EnableCounter(bool enable) => canBeStun = enable;

    public void ForceStun()
    {
        if (stunnedState == null)
            return;
        if (stateMachine.currentState == dieState || stateMachine.currentState == stunnedState)
            return;

        stateMachine.ChangeState(stunnedState);
    }

    public void ForceStun(float duration)
    {
        if (stunnedState == null)
            return;
        if (stateMachine.currentState == dieState || stateMachine.currentState == stunnedState)
            return;

        pendingStunDuration = Mathf.Max(0f, duration);
        stateMachine.ChangeState(stunnedState);
    }

    public float ConsumeStunDuration()
    {
        if (pendingStunDuration >= 0f)
        {
            float duration = pendingStunDuration;
            pendingStunDuration = -1f;
            return duration;
        }

        return stunDuration;
    }


    protected override IEnumerator SlowdownEntityCo(float duration, float slowAnim)
    {

        activeSlowMul = 1 - slowAnim;
        anim.speed = activeSlowMul;

        yield return new WaitForSeconds(duration);
        StopSlowdown();

    }

    public override void StopSlowdown()
    {
        activeSlowMul = 1;
        anim.speed = 1;
        base.StopSlowdown();

    }


    public override void EntityDeath()
    {
        base.EntityDeath();
        stateMachine.ChangeState(dieState);

    }

    private void HandlePlayerDeath()
    {
        stateMachine.ChangeState(idleState);
    }

    public void TryEnterBattleState(Transform player)
    {
        if (stateMachine.currentState == battleState || stateMachine.currentState == attackState || stateMachine.currentState == stunnedState)
            return;

        this.player = player;
        stateMachine.ChangeState(battleState);
    }

    public Transform GetPlayerRef()
    {
        if (player == null)
            player = playerDetection().transform;

        return player;
    }
    public void DestroyEnemy(float delay)
    {
        Destroy(gameObject, delay);
    }

    public RaycastHit2D playerDetection()
    {
        RaycastHit2D hit =
            Physics2D.Raycast(playerCheck.position, Vector2.right * facingDir, playerCheckDistance, whatIsPlayer | whatIsGround);
        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
            return default;

        return hit;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * playerCheckDistance), playerCheck.position.y));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * attackDistance), playerCheck.position.y));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * minRetreatDistance), playerCheck.position.y));


    }

    private void OnEnable()
    {
        Player.onPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        Player.onPlayerDeath -= HandlePlayerDeath;
    }
}