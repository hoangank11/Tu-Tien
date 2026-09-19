
using System.Collections;
using UnityEngine;

public class Skill_HPBuff : SkillBase
{
    [SerializeField] private GameObject lotusVFX;
    [SerializeField] private float immortalTime;
    public float hpPercent;

    
    public override void TryUseSkill()
    {
        GetLotusVFX();
        GetBuff();
    }

    private void GetBuff()
    {
        player.status.RemoveAllNegativeEffect(); //xóa bỏ toàn bộ effect xấu
        player.health.RegenHealth(HPPercent(hpPercent));
        StartCoroutine(ImmortalTime());
    }

    private float HPPercent(float hpPercent)
    {
        hpPercent = this.hpPercent / 100f;
        float finalHPHealth = player.start.GetMaxHeatlh() * hpPercent;
        return finalHPHealth;
    }

    private IEnumerator ImmortalTime()
    {
        player.health.SetCanTakeDamage(false);
        yield return new WaitForSeconds(immortalTime);
        player.health.SetCanTakeDamage(true);
    }

    private void GetLotusVFX()
    {
        Instantiate(lotusVFX, transform.position, Quaternion.identity);
    }

}
