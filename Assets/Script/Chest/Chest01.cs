using UnityEngine;

public class Chest01 : MonoBehaviour, IDamagable
{
    private Animator anim => GetComponentInChildren<Animator>();
    private Rigidbody2D rb => GetComponentInChildren<Rigidbody2D>();
    private Entity_DropManager dropManager => GetComponent<Entity_DropManager>();

    [Header("Open")]
    [SerializeField] private bool canDropItem = true;


    public bool TakeDamage(float damage, float eleDamage, ElementalType elementalType, Transform damageDealer)
    {
        if (canDropItem == false)
            return false;
        canDropItem = false;
        dropManager?.DropItem();
        anim.SetBool("gold", true);
        return true;
    }
}
