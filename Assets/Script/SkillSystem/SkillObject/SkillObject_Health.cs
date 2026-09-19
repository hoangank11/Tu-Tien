using UnityEngine;

public class SkillObject_Health : Entity_Health
{
    protected override void Die()
    {
        SkillObject_Echo echo = GetComponent<SkillObject_Echo>();
        echo.EchoDeath();
    }
} 
