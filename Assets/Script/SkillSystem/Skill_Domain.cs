using System.Collections.Generic;
using UnityEngine;

public class Skill_Domain : SkillBase
{
    [SerializeField] private GameObject domainPrefab;
    [Header("Domain Detail")]
    public float maxSize = 1.5f;
    public float expandSpeed = 2f;

    [Header("Slowing down")]
    public float slowPercent = 1f;
    [Range(5, 20f)]
    public float duration = 5f;

    private List<Enemy> trappedTarget = new List<Enemy>();
    private Transform currentTarget;
    public bool InstantDomain()
    {
        return skillType == SkillName.ĐịnhThânThuật;
    }

    public void CreateDomain()
    {
        GameObject domain = Instantiate(domainPrefab, transform.position, Quaternion.identity);
        domain.GetComponent<SkillObject_Domain>().SetupDomain(this);
    }

    public void AddTarget(Enemy targetToAdd)
    {
        trappedTarget.Add(targetToAdd);
    }
    public void ClearTarget()
    {
        foreach (var enemy in trappedTarget)
        {
            enemy.StopSlowdown();
        }
        trappedTarget = new List<Enemy>();
    }
}
