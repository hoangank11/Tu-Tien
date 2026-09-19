using UnityEngine;

public class CombatState : Entity_Combat
{
    [Header("Counter Attack Details")]
    [SerializeField] private float counterCooldown;


    public bool CounterAttackPer()
    {
        bool hasCounter = false;

        foreach (var target in GetDetectedColliders())
        {
            ICounterable counter = target.GetComponent<ICounterable>();

            if (counter == null) 
                continue;
            if (counter.canBeCounter)
            {
                counter.HandleCounter();
                hasCounter = true;
            }

        }
        return hasCounter;
    }

    public float CounterAttackCooldown() => counterCooldown;
}
