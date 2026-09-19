using UnityEngine;

// SFX dùng chung cho tất cả Enemy.
public class Enemy_SFX : Entity_SFX
{
    [Header("SFX Name - Combat")]
    [SerializeField] protected string attack;
    [SerializeField] protected string battle;
    [SerializeField] protected string die;

    [Header("SFX Name - Move Loop")]
    [SerializeField] protected string move;
    [SerializeField] protected float moveSfxInterval = 0.5f;

    private float moveSfxTimer;

    [Header("SFX Name - Idle Loop")]
    [SerializeField] protected string idle;
    [SerializeField] protected float idleSfxInterval = 10f;

    private float idleSfxTimer;

    protected override void Awake()
    {
        base.Awake();

        sfxPriority = SFXPriority.Normal;
    }

    #region COMBAT

    public virtual void PlayAttack()
    {
        PlaySFX(attack);
    }

    public virtual void PlayBattle()
    {
        PlaySFX(battle);
    }

    public virtual void PlayDie()
    {
        PlaySFX(die);
    }

    #endregion

    #region MOVE LOOP

    public void TryPlayMoveLoop(bool isMoving)
    {
        PlayLoopingSFX(
            move,
            ref moveSfxTimer,
            moveSfxInterval,
            isMoving
        );
    }

    public void ResetMoveLoop()
    {
        moveSfxTimer = 0f;
    }

    #endregion

    #region IDLE LOOP

    public void TryPlayIdleLoop(bool isIdle)
    {
        PlayLoopingSFX(
            idle,
            ref idleSfxTimer,
            idleSfxInterval,
            isIdle
        );
    }

    public void ResetIdleLoop()
    {
        idleSfxTimer = 0f;
    }

    #endregion
}