using UnityEngine;

public class Player_SkillManager : MonoBehaviour, ISaveable
{
    public Skill_Dash dash { get; private set; }
    public Skill_ThrowSword sword { get; private set; }
    public Skill_Echo echo { get; private set; }
    public Skill_Domain domain { get; private set; }
    public Skill_ThrowFire fire { get; private set; }
    public Skill_HPBuff lotus { get; private set; }
    public Skill_Hand01 hand01 { get; private set; }
    public Skill_Hand02 hand02 { get; private set; }
    public Skill_Hand03 hand03 { get; private set; }
    public Skill_Def def { get; private set; }

    [Header("Skill Data Base")]
    [Tooltip("Kéo hết các asset 'List Skill - ...' vào đây để LoadData tra được SkillDataSO theo SkillName.")]
    [SerializeField] private SkillListDataSO[] skillCategories;

    private UI ui;
    private SkillBase[] allSkills;
    private Player_Level playerLevel;

    public LevelType CurrentLevel => playerLevel != null ? playerLevel.CurrentLevel : LevelType.PhamNhan;

    /// <summary>
    /// Kiểm tra xem người chơi đã đạt tới (hoặc vượt qua) cảnh giới (LevelType) yêu cầu hay chưa.
    /// Dùng để làm điều kiện unlock skill, tương tự EnoughSkillPoint().
    /// </summary>
    public bool HasReachedLevel(LevelType requiredLevel)
    {
        if (playerLevel == null)
            return false;

        return (int)playerLevel.CurrentLevel >= (int)requiredLevel;
    }

    private void Awake()
    {
        ui = FindAnyObjectByType<UI>();
        playerLevel = GetComponent<Player_Level>();

        dash = GetComponentInChildren<Skill_Dash>();
        sword = GetComponentInChildren<Skill_ThrowSword>();
        echo = GetComponentInChildren<Skill_Echo>();
        domain = GetComponentInChildren<Skill_Domain>();
        fire = GetComponentInChildren<Skill_ThrowFire>();
        lotus = GetComponentInChildren<Skill_HPBuff>();
        hand01 = GetComponentInChildren<Skill_Hand01>();
        hand02 = GetComponentInChildren<Skill_Hand02>();
        hand03 = GetComponentInChildren<Skill_Hand03>();
        def = GetComponentInChildren<Skill_Def>();


        allSkills = GetComponentsInChildren<SkillBase>();
    }

    public void ReduceAllSkillCooldown(float amount)
    {
        foreach (var skill in allSkills)
            skill.ResetCooldownBy(amount);
    }

    public SkillBase GetSkillByName(SkillName name)
    {
        switch (name)
        {
            case SkillName.LưuẢnhBộ: return dash;
            case SkillName.NgựKiếmThiênPhongDẫn: return sword;
            case SkillName.ThiênHuyễnTànẢnhQuyết: return echo;
            case SkillName.ĐịnhThânThuật: return domain;
            case SkillName.PhầnViêmChưởng: return fire;
            case SkillName.ThanhLiênHồiNguyênQuyết: return lotus;
            case SkillName.ThiênMaNhấtChỉ: return hand01;
            case SkillName.ThiênMaSongChỉ: return hand02;
            case SkillName.ThiênMaThủẤn: return hand03;
            case SkillName.HộLinhNgưngThuẫn: return def;



            default:
                return null;
        }
    }

    private SkillDataSO GetSkillData(SkillName name)
    {
        foreach (var category in skillCategories)
        {
            if (category == null) continue;

            SkillDataSO data = category.GetSkillData(name);
            if (data != null)
                return data;
        }
        return null;
    }

    #region Save/Load Skill

    public void SaveData(ref GameData data)
    {
        data.skillName.Clear();

        foreach (var skill in allSkills)
        {
            if (skill.IsUnlocked && skill.skillData != null)
                data.skillName[skill.skillData.skillByName] = skill.skillData.skillName;
        }
    }

    public void LoadData(GameData data)
    {
        foreach (var entry in data.skillName)
        {
            SkillName name = entry.Key;

            SkillBase skill = GetSkillByName(name);
            if (skill == null)
            {
                continue;
            }

            SkillDataSO skillData = GetSkillData(name);
            if (skillData == null)
            {
                continue;
            }

            skill.SetSkillUpgrade(skillData);


            ui.inGameUI.AssignSkillToSlot(skillData, skill);
        }
    }

    #endregion
}