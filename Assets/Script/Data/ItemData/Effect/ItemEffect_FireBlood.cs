using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Item/Item Effect/Fire Blood", fileName = "Item Effect - Fire Blood")]
public class ItemEffect_FireBlood : ItemEffectDataSO
{
    [Tooltip("Attack Speed cộng thêm cho mỗi 1% HP mất đi. 0.005 = 0.5%")]
    [SerializeField] private float attackSpeedPerHPPercentLost = .005f;

    // Source riêng cho từng instance SO, dùng để add/remove modifier đúng chỗ
    private string Source => $"FireBlood_{GetInstanceID()}";

    public override void ExecuteEffect()
    {
        UpdateAttackSpeedBonus();
    }

    private void UpdateAttackSpeedBonus()
    {
        StartValuation attackSpeedStat = player.start.GetValueByType(StartType.AttackSpeed);
        if (attackSpeedStat == null)
            return;

        float missingHPPercent = (1f - player.health.GetHPPercent()) * 100f; // % HP đã mất (0 - 100)
        float bonusAttackSpeed = missingHPPercent * attackSpeedPerHPPercentLost;

        // Xoá modifier cũ trước khi add lại giá trị mới (vì hiệu ứng thay đổi liên tục theo HP hiện tại)
        attackSpeedStat.RemoveModifier(Source);
        attackSpeedStat.AddModifier(bonusAttackSpeed, Source);

        Debug.Log($"HP%: {player.health.GetHPPercent() * 100} | lost HP%: {missingHPPercent} | Bonus AtkSpd: +{bonusAttackSpeed * 100}%");
    }

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        player.health.OnTakingDamage += ExecuteEffect;
        player.health.OnHeal += ExecuteEffect; // recalc lại khi được hồi máu (skill, potion, regen...)
        // Tính ngay lúc trang bị, phòng trường hợp player đã mất máu sẵn từ trước
        UpdateAttackSpeedBonus();
    }

    public override void Unsubscribe()
    {
        player.start.GetValueByType(StartType.AttackSpeed)?.RemoveModifier(Source);

        base.Unsubscribe();
        player.health.OnTakingDamage -= ExecuteEffect;
        player.health.OnHeal -= ExecuteEffect;
        player = null;
    }
}
