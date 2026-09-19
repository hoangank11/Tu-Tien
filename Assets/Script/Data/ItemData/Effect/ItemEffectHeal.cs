using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Item/Item Effect/Heal Effect", fileName = "Item Effect - Heal")]
public class ItemEffectHeal : ItemEffectDataSO
{
    [SerializeField] private float healPercent = .1f;

    public override void ExecuteEffect()
    {
        Player player = FindFirstObjectByType<Player>();

        float healAmount = player.start.GetMaxHeatlh() * healPercent;
        float finalHPHealth = player.start.GetMaxHeatlh() * healPercent;
        player.health.RegenHealth(finalHPHealth);

    }


}
