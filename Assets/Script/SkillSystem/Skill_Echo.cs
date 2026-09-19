using UnityEngine;

public class Skill_Echo : SkillBase
{
    [SerializeField] private GameObject echoPrefab;
    [SerializeField] private float echoDuration;
    [SerializeField] private int maxAttack = 3;
    [SerializeField] private float duplicateChance = .05f; //max là 70%. chỉ số ban đầu sẽ là 5%

    private void Start()
    {
        EchoRotate();
    }

    private void EchoRotate()
    {
        if (player.facingDir == 1)
            gameObject.transform.Rotate(0, 0, 0);
        else if (player.facingDir == -1)
            gameObject.transform.Rotate(0, 180, 0);
    }

    public float GetDuplicateChance()
    {
        return duplicateChance;
    }


    public int GetMaxAttack()
    {
        if (skillType == SkillName.ThiênHuyễnTànẢnhQuyết)
            return maxAttack;
        return 0;
    }
    public float GetEchoDuration()
    {
        return echoDuration;
    }

    public override void TryUseSkill()
    {
        if (CanUseSkill() == false)
            return;
        //CreateEcho(Vector3.zero);  khá thú vị nếu cho đây là nâng cấp cấp 1 vì nó khá đuối
        CreateEcho();
    }

    public void CreateEcho(Vector3? targetPosition = null)
    {
        Vector2 position2 = EchoPosition();
        Vector3 position = targetPosition ?? position2;
        GameObject echo = Instantiate(echoPrefab, position, Quaternion.identity);
        echo.GetComponent<SkillObject_Echo>().SetupEcho(this);
    }

    private Vector2 EchoPosition()
    {
        Vector2 position2 = new Vector2();
        if (player.facingDir == 1)
            position2 = new Vector2(transform.position.x + 2, transform.position.y);
        else if (player.facingDir == -1)
            position2 = new Vector2(transform.position.x - 2, transform.position.y);
        return position2;
    }
}
