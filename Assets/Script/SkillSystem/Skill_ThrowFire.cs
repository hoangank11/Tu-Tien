using UnityEngine;

public class Skill_ThrowFire : SkillBase
{
    private SkillObject_Fire fireKi;
    [Header("Fire")]
    [SerializeField] private GameObject firePrefab01;
    [SerializeField] private GameObject firePrefab02;
    [Range(0f, 10f)]
    [SerializeField] private float throwPower = 6f;

    public override ElementalType GetElementalType() => ElementalType.Fire;

    public void ThrowFire()
    {
        GameObject firePre = GetFirePrefab();
        GameObject newFire = Instantiate(firePre, FirePosition(), Quaternion.identity);
        fireKi = newFire.GetComponent<SkillObject_Fire>();

        fireKi.SetupFire(this, GetThrowPower());
        SetSkillOnCooldown();
    }

    private Vector2 FirePosition()
    {
        Vector2 position2 = new Vector2();
        if (player.facingDir == 1)
            position2 = new Vector2(transform.position.x + 2, transform.position.y);
        else if (player.facingDir == -1)
            position2 = new Vector2(transform.position.x - 2, transform.position.y);
        return position2;
    }

    private float GetThrowPower()
    {
        if (Unlocked(SkillName.PhầnViêmChưởng))
            return throwPower;
        if (Unlocked(SkillName.PhầnThiênDiệtViêmQuyết))
            return throwPower * 2;

        return throwPower;
    }

    private GameObject GetFirePrefab()
    {
        if (Unlocked(SkillName.PhầnViêmChưởng))
            return firePrefab01;
        if (Unlocked(SkillName.PhầnThiênDiệtViêmQuyết))
            return firePrefab02;

        return null;
    }
}
