using UnityEngine;

public class Skill_HandBase : SkillBase
{
    private SkillObject_Hand hand;
    [SerializeField] private GameObject handObject;
    [Range(0f, 10f)]
    [SerializeField] private float throwPower = 6f;

    public override ElementalType GetElementalType() => ElementalType.Fire;

    public virtual void ThrowHand()
    {
        GameObject newFire = Instantiate(handObject, HandPosition(), Quaternion.identity);
        hand = newFire.GetComponent<SkillObject_Hand>();

        hand.SetupHand(this, GetThrowPower());
        SetSkillOnCooldown();
    }

    private Vector2 HandPosition()
    {
        Vector2 position = new Vector2();
        position = new Vector2(transform.position.x, transform.position.y + 10);
        return position;
    }

    private float GetThrowPower()
    {
        return throwPower;
    }
}
