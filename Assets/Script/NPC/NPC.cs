using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    protected Transform player;
    protected UI ui;
    [Header("Quest Infor")]
    protected Player_QuestManager questManager;
    [SerializeField] private RewardType npcReward;
    [Space]
    [SerializeField] private string npcNameID;
    [SerializeField] private Transform npc;
    [SerializeField] private GameObject interactTooltip;
    private bool facingRight = true;

    [Header("Quest & Dialogue")]
    public QuestDataSO[] quests;
    public DialogueLineSO firstDialogueLine;

    [Header("Tinh chỉnh vị trí của Tooltip float")]
    [SerializeField] private float floatSpeed = 8f;
    [SerializeField] private float floatRange = .1f;
    private Vector3 startPosition;

    protected virtual void Awake()
    {
        ui = FindFirstObjectByType<UI>();
        startPosition = interactTooltip.transform.position;
        interactTooltip.SetActive(false);
    }

    protected virtual void Start()
    {
        questManager = Player.instance.questManager;
    }

    protected virtual void Update()
    {
        NPCFlip();
        HandleTooltipFloat();
    }

    private void HandleTooltipFloat()
    {
        if (interactTooltip.activeSelf)
        {
            float yOffSet = Mathf.Sin(Time.time * floatSpeed) * floatRange;
            interactTooltip.transform.position = startPosition + new Vector3(0, yOffSet);
        }
    }

    private void NPCFlip()
    {
        if (player == null || npc == null) return;
        if (npc.position.x > player.position.x && facingRight)
        {
            npc.transform.Rotate(0f, 180f, 0f);
            interactTooltip.transform.Rotate(0, -180, 0);
            facingRight = false;
        }
        else if (npc.position.x < player.position.x && facingRight == false)
        {
            npc.transform.Rotate(0f, 180f, 0f);
            interactTooltip.transform.Rotate(0, -180, 0);
            facingRight = true;
        }
    }


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.transform;
        interactTooltip.SetActive(true);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        interactTooltip.SetActive(false);
    }

    public virtual void Interact()
    {
        // Open UI Dialogue
        ui.OpenDialogueUI(firstDialogueLine);


        questManager.AddProgress(npcNameID);
        questManager.TryGiveRewardFrom(npcReward);
    }
}
