using UnityEngine;

public class EnemyHealth : Entity_Health
{
    private Enemy enemy;
    private Player_QuestManager questManager;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
    }
    protected override void Start()
    {
        base.Start();
        questManager = Player.instance != null ? Player.instance.questManager : null; // FIX: Lấy QuestManager an toàn nếu Player đã tồn tại
    }

    public override bool TakeDamage(float damage, float eleDamage, ElementalType elementalType, Transform damageDealer)
    {
        if (canTakeDamage == false)
            return false;

        bool wasHit = base.TakeDamage(damage, eleDamage, elementalType, damageDealer);

        if (wasHit == false)
            return false;

        if (damageDealer != null &&
            damageDealer.GetComponent<Player>() != null)
        {
            enemy.TryEnterBattleState(damageDealer);
        }

        return true;
    }

    protected override void Die()
    {
        Player.instance.questManager.AddProgress(enemy.questTargetID, 1);

        base.Die();
    }
}