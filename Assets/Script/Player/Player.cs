using System;
using System.Collections;
using UnityEngine;

public class Player : Entity, ISaveable
{
    public static Player instance;
    public static event Action onPlayerDeath;
    public UI ui { get; private set; }
    public PlayerInputSet input { get; private set; }
    public Player_SkillManager skillManager { get; private set; }
    public PlayerVFX playerVFX { get; private set; }
    public Entity_Health health { get; private set; }
    public Entity_Status status { get; private set; }
    public CombatState combat { get; private set; }
    public InventoryPlayer inventory { get; private set; }
    public new Player_SFX sfx { get; private set; }
    public Player_QuestManager questManager { get; private set; }

    #region Call State
    // normal state
    public IdleState idleState { get; private set; }
    public FallState fallState { get; private set; }
    public DeadState deadState { get; private set; }
    public MoveState moveState { get; private set; }
    public JumpState jumpState { get; private set; }
    public WallSlideState wallSlideState { get; private set; }
    public WallJumpState wallJumpState { get; private set; }
    public BasicAttackState basicAttackState { get; private set; }
    public JumpAttackState jumpAttackState { get; private set; }
    public DoubleJumpState doubleJumpState { get; private set; }
    public CounterAttackState counterAttackState { get; private set; }

    // skill state
    public DashState dashState { get; private set; }
    public ThrowSwordState throwSwordState { get; private set; }
    public DomainState domainState { get; private set; }
    public EchoState echoState { get; private set; }
    public ThrowFire01State throwState01State { get; private set; }
    public HPBuffState hpBuffState { get; private set; }
    public HandState handState { get; private set; }

    #endregion


    #region Valuation
    [Header("Attack details")]
    public Vector2[] attackVelocity;
    public Vector2 jumpAttackVelocity;
    public float attackVelocityDuration = .1f;
    public float comboResetTime = 1;
    private Coroutine queuedAttackCo;

    [Header("Domain Detail")]
    public float riseMaxDistance = 3f;

    [Header("Movement details")]
    public float moveSpeed;
    public float jumpForce = 5;
    public Vector2 wallJumpForce;

    [Range(0, 1)]
    public float inAirMoveMultiplier = .7f;
    [Range(0, 1)]
    public float wallSlideSlowMultiplier = .7f;

    private float baseMoveSpeed;
    private float baseJumpForce;
    private float baseAnimSpeed;
    private Vector2 baseWallJumpForce;
    private Vector2 baseJumpAttackVelocity;
    private Vector2[] baseAttackVelocity;
    [Space]
    public float dashDuration = .25f;
    public float dashSpeed = 20;
    public bool canDoubleJump;

    public Vector2 mousePosition { get; private set; }
    public Vector2 moveInput { get; private set; }



    #endregion


    #region Core
    protected override void Awake()
    {
        base.Awake();
        instance = this;

        ui = FindAnyObjectByType<UI>();
        playerVFX = GetComponent<PlayerVFX>();
        health = GetComponent<Entity_Health>();
        skillManager = GetComponent<Player_SkillManager>();
        status = GetComponent<Entity_Status>();
        combat = GetComponent<CombatState>();
        inventory = GetComponent<InventoryPlayer>();
        sfx = GetComponent<Player_SFX>();
        questManager = GetComponent<Player_QuestManager>();

        input = new PlayerInputSet();
        ui.SetupControlsUI(input);

        idleState = new IdleState(this, stateMachine, "idle");
        moveState = new MoveState(this, stateMachine, "move");
        jumpState = new JumpState(this, stateMachine, "jumpFall");
        fallState = new FallState(this, stateMachine, "jumpFall");
        wallSlideState = new WallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new WallJumpState(this, stateMachine, "jumpFall");
        dashState = new DashState(this, stateMachine, "dash");
        basicAttackState = new BasicAttackState(this, stateMachine, "basicAttack");
        jumpAttackState = new JumpAttackState(this, stateMachine, "jumpAttack");
        doubleJumpState = new DoubleJumpState(this, stateMachine, "doubleJump");
        deadState = new DeadState(this, stateMachine, "die");
        counterAttackState = new CounterAttackState(this, stateMachine, "counterStart");
        throwSwordState = new ThrowSwordState(this, stateMachine, "throwSword");
        domainState = new DomainState(this, stateMachine, "domain");
        echoState = new EchoState(this, stateMachine, "echo");
        throwState01State = new ThrowFire01State(this, stateMachine, "throwFire01");
        hpBuffState = new HPBuffState(this, stateMachine, "hpBuff");
        handState = new HandState(this, stateMachine, "hand");

    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);

        CaptureBaseMovementValues();

    }

    #endregion

    public void TeleportPlayer(Vector3 position) => transform.position = position;

    private void CaptureBaseMovementValues()
    {
        baseMoveSpeed = moveSpeed;
        baseJumpForce = jumpForce;
        baseAnimSpeed = anim.speed;
        baseWallJumpForce = wallJumpForce;
        baseJumpAttackVelocity = jumpAttackVelocity;
        baseAttackVelocity = new Vector2[attackVelocity.Length];
        Array.Copy(attackVelocity, baseAttackVelocity, attackVelocity.Length);
    }

    protected override IEnumerator SlowdownEntityCo(float duration, float slowAnim)
    {
        float speedMul = 1 - slowAnim;

        moveSpeed = baseMoveSpeed * speedMul;
        jumpForce = baseJumpForce * speedMul;
        anim.speed = baseAnimSpeed * speedMul;
        wallJumpForce = baseWallJumpForce * speedMul;
        jumpAttackVelocity = baseJumpAttackVelocity * speedMul;
        for (int i = 0; i < attackVelocity.Length; i++)
            attackVelocity[i] = baseAttackVelocity[i] * speedMul;

        yield return new WaitForSeconds(duration);

        StopSlowdown();
    }

    public override void StopSlowdown()
    {

        moveSpeed = baseMoveSpeed;
        jumpForce = baseJumpForce;
        anim.speed = baseAnimSpeed;
        wallJumpForce = baseWallJumpForce;
        jumpAttackVelocity = baseJumpAttackVelocity;
        for (int i = 0; i < attackVelocity.Length; ++i)
            attackVelocity[i] = baseAttackVelocity[i];

        base.StopSlowdown();
    }


    public override void EntityDeath()
    {
        base.EntityDeath();
        onPlayerDeath?.Invoke();
        stateMachine.ChangeState(deadState);
    }

    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
            StopCoroutine(queuedAttackCo);

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }

    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }

    private void TryInteract()
    {
        Transform closest = null;
        float closesDistance = Mathf.Infinity;
        Collider2D[] objectAround = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (var target in objectAround)
        {
            IInteractable interactable = target.GetComponent<IInteractable>();
            if (interactable == null) continue;
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance < closesDistance)
            {
                closesDistance = distance;
                closest = target.transform;
            }
        }
        if (closest == null)
            return;
        closest.GetComponent<IInteractable>().Interact();


    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.Mouse.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;

        // NPC
        input.Player.Interact.performed += ctx => TryInteract();

        // SlotItem
        input.Player.SlotItem01.performed += ctx => inventory.TryUseQuickItemInSlot(1);
        input.Player.SlotItem02.performed += ctx => inventory.TryUseQuickItemInSlot(2);
    }

    private void OnDisable()
    {
        input.Disable();
    }

    #region Save/Load HP MP
    // Cùng lưu HP và MP theo dạng % (currentHPPercent/currentMPPercent) để tránh
    // lệch giá trị khi maxHP/maxMP thay đổi (VD: người chơi lên cấp) giữa các lần save/load.

    public void SaveData(ref GameData data)
    {
        data.currentHPPercent = health.GetHPPercent();
        data.currentMPPercent = health.GetMPPercent();
    }

    public void LoadData(GameData data)
    {
        StartCoroutine(LoadHPNextFrame(data.currentHPPercent));
        StartCoroutine(LoadMPNextFrame(data.currentMPPercent));
    }

    private IEnumerator LoadHPNextFrame(float percent)
    {
        yield return null;

        float safePercent = percent <= 0f ? 1f : percent;
        health.SetHPToPercent(safePercent);
    }

    private IEnumerator LoadMPNextFrame(float percent)
    {
        yield return null;

        // Khác HP: MP = 0 là hợp lệ (đã dùng hết mana khi save), không cần ép về đầy.
        health.SetMPToPercent(Mathf.Clamp01(percent));
    }

    #endregion

}