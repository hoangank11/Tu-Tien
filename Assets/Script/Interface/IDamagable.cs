using UnityEngine;

public interface IDamagable
{
    public bool TakeDamage(float damage, float eleDamage, ElementalType elementalType, Transform damageDealer);
}
