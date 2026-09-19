using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Skill_ThrowSword : SkillBase
{
    private SkillObject_SwordKi swordKi; 

    [Header("Sword")]
    [SerializeField] private GameObject swordPrefab;
    [Range(0f, 10f)]
    [SerializeField] private float throwPower = 6;
    private float swordGravity;

    [Header("Peirce Sword")]
    [SerializeField] private GameObject peirceSwordPrefab;
    [Range(1, 5)]
    public int peirceAmount = 2;

    [Header("Spin Sword")]
    [SerializeField] private GameObject spinSwordPrefab;
    public float spinDestroy = 3f;
    [Range(1, 5)]
    public float attackPerSecond = 2;

    [Header("Bounce Sword")]
    [SerializeField] private GameObject bounceSwordPrefab;
    [Range(1, 10)]
    public int bounceCount = 5;
    [Range(5, 20)]
    public float bounceSpeed = 15;

    [Header("Dot")]
    [SerializeField] private GameObject dotPrefab;
    [Range(0, 30)]
    [SerializeField] private int numberOfDot = 20;
    [SerializeField] private float spaceBetweenDots = .05f;
    private Transform[] dots;
    private Vector2 confirmDirection;

    [SerializeField] private GameObject bq;
    [SerializeField] private GameObject kk;
    

    protected override void Awake()
    {
        base.Awake();
        swordGravity = swordPrefab.GetComponent<Rigidbody2D>().gravityScale;
        dots = GenerateDots();
    }

    /*public override bool CanUseSkill()
    {
        if (swordKi != null)
            return false;

        return base.CanUseSkill();

    }*/

    public void ThrowSword()
    {
        GameObject swordPre = GetSwordPrefab();
        GameObject newSword = Instantiate(swordPre, dots[1].position, Quaternion.identity);
        swordKi = newSword.GetComponent<SkillObject_SwordKi>();

        swordKi.SetupSword(this, GetThrowPower());
        SetSkillOnCooldown();
    }

    private GameObject GetSwordPrefab()
    {
        if (Unlocked(SkillName.NgựKiếmThiênPhongDẫn))
            return peirceSwordPrefab;
        if (Unlocked(SkillName.VạnKiếmQuyTông))
            //return spinSwordPrefab;
            return bounceSwordPrefab;

        return null;
    }

    private Vector2 GetThrowPower() => confirmDirection * (throwPower * 10);

    #region Dot

    public void PredictTrajectory(Vector2 direction)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].position = GetTrajectorPoint(direction, i * spaceBetweenDots);
        }
    }


    private Vector2 GetTrajectorPoint(Vector2 direction, float t)
    {
        float scaleThrowPower = throwPower * 10f;

        Vector2 initialVelocity = direction * scaleThrowPower;

        Vector2 gravityEffect = 0.5f * Physics2D.gravity * swordGravity * (t * t);

        Vector2 predictedPoint = (initialVelocity * t) + gravityEffect;

        Vector2 playerPosition = transform.root.position;

        return playerPosition + predictedPoint;
    }


    public void ComfirmTrajectory(Vector2 direction) => confirmDirection = direction;

    public void EnableDots(bool enable)
    {
        foreach (Transform t in dots)
            t.gameObject.SetActive(enable);
    }


    private Transform[] GenerateDots()
    {
        Transform[] newDots = new Transform[numberOfDot];

        for (int i = 0; i < numberOfDot; i++)
        {
            newDots[i] = Instantiate(dotPrefab, transform.position, Quaternion.identity, transform).transform;
            newDots[i].gameObject.SetActive(false);

        }

        return newDots;

    }
    #endregion

}
