using UnityEngine;

public class SkillObject_AnimationTrigger : MonoBehaviour
{
    private SkillObject_Echo echo;

    private void Awake()
    {
        echo = GetComponentInParent<SkillObject_Echo>();
    }

    private void AttackTrigger()
    {
        echo.PerformAttack();
    }

    private void Tryterminate(int currentAttackIndex)
    {
        if (currentAttackIndex == echo.maxAttack)
            echo.EchoDeath();
    }

}
