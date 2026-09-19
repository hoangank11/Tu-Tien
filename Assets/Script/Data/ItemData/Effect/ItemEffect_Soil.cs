using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Item/Item Effect/Soil", fileName = "Item Effect - Soil Armor")]
public class ItemEffect_Soil : ItemEffectDataSO
{
    [SerializeField] private float healthPercentTrigger = .3f;
    [SerializeField] private float regenHealth = .15f;
    [SerializeField] private float cooldown = 50;
    [Header("VFX")]
    [SerializeField] private GameObject healthVFX;
    private float lastTimeUse = -999;
    public override void ExecuteEffect()
    {
        bool noCooldown = Time.time >= lastTimeUse + cooldown;
        bool reachthreshod = player.health.GetHPPercent() <= healthPercentTrigger;
        Debug.Log($"HP%: {player.health.GetHPPercent() * 100} | noCooldown: {noCooldown} | reachThreshold: {reachthreshod}");
        if (noCooldown && reachthreshod)
        {
            lastTimeUse = Time.time;
            RegenHP();
            if (healthVFX != null)
                player.playerVFX.CreateEffectOf(healthVFX, player.transform);
        }
    }

    private void RegenHP()
    {
        player.health.RegenHealth(player.start.GetMaxHeatlh() * regenHealth);
        
    }

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        lastTimeUse = -999f;
        player.health.OnTakingDamage += ExecuteEffect;
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        player.health.OnTakingDamage -= ExecuteEffect;
        player = null;
    }
}
