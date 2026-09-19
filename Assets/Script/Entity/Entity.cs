using System;
using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public event Action OnFlip;
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    private Entity_SFX _sfx;
    public Entity_SFX sfx { get => _sfx; private set => _sfx = value; }
    public Entity_Start start;
    protected StateMachine stateMachine;
    private bool facingRight = true;
    public int facingDir { get; private set; } = 1;


    [Header("Collision detection")]
    public LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform primaryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;
    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }

    // knockback valuation
    private Coroutine knockbackCo;
    private bool isKnock;
    private Coroutine slowdownCo;
    private Coroutine airborneCo;

    // Wood Elemental - Root (trói): không thể di chuyển, chỉ có thể quay trái/phải.
    public bool IsRooted { get; private set; }

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sfx = GetComponent<Entity_SFX>();
        start = GetComponent<Entity_Start>();
        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    public void CallAnimationTrigger()
    {
        stateMachine.currentState.AnimationTrigger();
    }

    public virtual void EntityDeath()
    {

    }

    public virtual void SlowdownEntity(float duration, float slowAnim, bool canOverrideSlow = false)
    {
        if (slowdownCo != null)
        {
            if (canOverrideSlow == false)
                return;

            // QUAN TRỌNG: khi 1 hiệu ứng chậm (VD: Toxic) override 1 hiệu ứng chậm khác đang
            // chạy (VD: Ice), phải luôn RESET VỀ TỐC ĐỘ GỐC trước khi bắt đầu hiệu ứng mới.
            // Nếu chỉ StopCoroutine() mà không reset, đoạn code khôi phục tốc độ ở cuối
            // coroutine cũ sẽ không bao giờ chạy -> tốc độ giảm bị bỏ lại vĩnh viễn, và hiệu
            // ứng mới lại tiếp tục nhân thêm vào tốc độ ĐÃ GIẢM đó -> cộng dồn (compounding)
            // càng bị đánh càng chậm, và không hồi lại khi hết duration.
            StopCoroutine(slowdownCo);
            StopSlowdown();
        }
        slowdownCo = StartCoroutine(SlowdownEntityCo(duration, slowAnim));
    }

    protected virtual IEnumerator SlowdownEntityCo(float duration, float slowAnim)
    {
        yield return null;
    }
    public virtual void StopSlowdown()
    {
        slowdownCo = null;
    }
    public void ReciveKnockback(Vector2 knockback, float duration)
    {
        if (knockbackCo != null)
            StopCoroutine(knockbackCo);

        knockbackCo = StartCoroutine(KnockbackCo(knockback, duration));
    }

    private IEnumerator KnockbackCo(Vector2 knockback, float duration)
    {
        isKnock = true;
        rb.linearVelocity = knockback;
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = Vector2.zero;
        isKnock = false;
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnock)
            return;

        if (IsRooted)
        {
            // Đang bị trói (Wood): không cho di chuyển (kể cả nhảy), nhưng vẫn được phép
            // quay mặt theo hướng input để player/enemy còn có thể quay trái/phải mà đánh.
            HandleFlip(xVelocity);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    /// <summary>
    /// Bật/tắt trạng thái bị trói (Wood Elemental). Trong lúc bị trói, SetVelocity sẽ chặn
    /// mọi di chuyển ngang/nhảy nhưng vẫn cho phép Flip (quay trái phải) và tấn công bình thường
    /// nếu mục tiêu đang trong tầm đánh. Không ảnh hưởng tới Knockback/LaunchAirborne vì 2 cơ chế
    /// đó set thẳng rb.linearVelocity, không đi qua SetVelocity.
    /// </summary>
    public void SetRooted(bool rooted)
    {
        IsRooted = rooted;
    }

    /// <summary>
    /// Launches the entity upward by approximately 'height' world units.
    /// State movement is locked until the entity lands.
    /// </summary>
    public void LaunchAirborne(float height, System.Action onLanded = null)
    {
        if (rb == null || height <= 0f)
        {
            onLanded?.Invoke();
            return;
        }

        if (airborneCo != null)
            StopCoroutine(airborneCo);

        if (knockbackCo != null)
            StopCoroutine(knockbackCo);

        airborneCo = StartCoroutine(LaunchAirborneCo(height, onLanded));
    }

    private IEnumerator LaunchAirborneCo(float height, System.Action onLanded)
    {
        isKnock = true;

        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        if (gravity < 0.01f)
            gravity = 9.81f;

        float launchVelocity = Mathf.Sqrt(2f * gravity * height);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, launchVelocity);

        yield return null;

        while (true)
        {
            if (groundDetected && rb.linearVelocity.y <= 0.05f)
                break;

            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isKnock = false;
        airborneCo = null;
        onLanded?.Invoke();
    }

    public void HandleFlip(float xVelcoity)
    {
        if (xVelcoity > 0 && facingRight == false)
            Flip();
        else if (xVelcoity < 0 && facingRight)
            Flip();
    }

    public void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDir = facingDir * -1;

        OnFlip?.Invoke();
    }

    private void HandleCollisionDetection()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround)
                    && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }


    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));
        Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDir, 0));
    }
}