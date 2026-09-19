using UnityEngine;

public class Skill_Hand01 : Skill_HandBase
{
    public override void ThrowHand()
    {
        player.sfx.PlayHand();
        base.ThrowHand();
    }
}
