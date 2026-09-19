using UnityEngine;

// SFX dành cho Player.
public class Player_SFX : Entity_SFX
{
    [Header("SFX Name - Basic Attack (3 combo)")]
    [SerializeField] private string attackCombo1;
    [SerializeField] private string attackCombo2;
    [SerializeField] private string attackCombo3;

    [Header("SFX Name - Di chuyển")]
    [SerializeField] private string jump;
    [SerializeField] private string dash;
    [SerializeField] private string wallJump;
    [SerializeField] private string wallSlide;
    [SerializeField] private string land;

    [Header("SFX Name - Move Loop")]
    [SerializeField] private string move;
    [SerializeField] private float moveSfxInterval = 0.5f;

    private float moveSfxTimer;

    [Header("SFX Name - Skill")]
    [SerializeField] private string domain;
    [SerializeField] private string echo;
    [SerializeField] private string hand;
    [SerializeField] private string hpBuff;
    [SerializeField] private string throwFire;
    [SerializeField] private string throwSword;
    [SerializeField] private string def;

    [Header("SFX Name - Combat")]
    [SerializeField] private string takePhysicalDamage;

    protected override void Awake()
    {
        base.Awake();

        // Player luôn được ưu tiên cao nhất.
        sfxPriority = SFXPriority.Important;
    }

    #region BASIC ATTACK

    public void PlayAttackCombo(int comboIndex)
    {
        switch (comboIndex)
        {
            case 1:
                PlaySFX(attackCombo1);
                break;

            case 2:
                PlaySFX(attackCombo2);
                break;

            case 3:
                PlaySFX(attackCombo3);
                break;

            default:
                PlaySFX(attackCombo3);
                break;
        }
    }

    #endregion

    #region MOVEMENT

    public void PlayJump()
    {
        PlaySFX(jump);
    }

    public void PlayDash()
    {
        PlaySFX(dash);
    }

    public void PlayWallJump()
    {
        PlaySFX(wallJump);
    }

    public void PlayWallSlide()
    {
        PlaySFX(wallSlide);
    }

    public void PlayLand()
    {
        PlaySFX(land);
    }

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

    #region SKILL

    public void PlayDomain()
    {
        PlaySFX(domain);
    }

    public void PlayEcho()
    {
        PlaySFX(echo);
    }

    public void PlayHand()
    {
        PlaySFX(hand);
    }

    public void PlayHPBuff()
    {
        PlaySFX(hpBuff);
    }

    public void PlayThrowFire()
    {
        PlaySFX(throwFire);
    }

    public void PlayThrowSword()
    {
        PlaySFX(throwSword);
    }

    public void PlayDef()
    {
        PlaySFX(def);
    }

    #endregion

    #region COMBAT

    public void PlayTakePhysicalDamage()
    {
        PlaySFX(takePhysicalDamage);
    }

    #endregion
}