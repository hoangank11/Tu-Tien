using TMPro;
using UnityEngine;

public class SkillPoint : MonoBehaviour, ISaveable
{
    [SerializeField] private int skillPoint;
    [SerializeField] private UI_TreeConnectHandle[] parentNode;

    [Header("Skill Point UI")]
    [SerializeField] private TextMeshProUGUI skillPointText; // Text (TMP) nằm trong Img/Text -> Image -> Text (TMP)

    public Player_SkillManager skillManager { get; private set; }

    private void Awake()
    {
        skillManager = FindAnyObjectByType<Player_SkillManager>();
    }
    private void Start()
    {
        UpdateAllConnect();
        UpdateSkillPointUI();
    }

    public int GetSkillPoint() => skillPoint;


    [ContextMenu("Reset Skill Tree")]
    public void RefundAllSkill()
    {
        UI_TreeNode[] skillNode = GetComponentsInChildren<UI_TreeNode>();
        foreach (var node in skillNode)
            node.Refund();
    }
    public bool EnoughSkillPoint(int cost) => skillPoint >= cost;


    // Kiểm tra người chơi đã đạt đúng/vượt cảnh giới (LevelType) yêu cầu của skill hay chưa.

    public bool EnoughLevel(LevelType requiredLevel) => skillManager != null && skillManager.HasReachedLevel(requiredLevel);
    public void RemoveSkillPoint(int cost)
    {
        skillPoint -= cost;
        UpdateSkillPointUI();
    }
    public void AddSkillPoint(int point)
    {
        skillPoint += point;
        UpdateSkillPointUI();
    }

    private void UpdateSkillPointUI()
    {
        if (skillPointText == null)
            return;

        skillPointText.text = skillPoint.ToString();
    }

    #region Save/Load Skill Point

    public void SetSkillPoint(int point)
    {
        skillPoint = point;
        UpdateSkillPointUI();
    }

    public void SaveData(ref GameData data)
    {
        data.skillPoint = skillPoint;
    }

    public void LoadData(GameData data)
    {
        SetSkillPoint(data.skillPoint);
    }

    #endregion

    [ContextMenu("UpdateAllNodes")]
    public void UpdateAllConnect()
    {
        foreach (var node in parentNode)
        {
            node.UpdateAllConnect();
        }
    }
}