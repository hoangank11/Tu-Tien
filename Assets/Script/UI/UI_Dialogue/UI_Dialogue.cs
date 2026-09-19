
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Dialogue : MonoBehaviour
{
    private UI ui;

    [SerializeField] private Image speakerPortrait;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI[] dialogueChoices;
    [Space]
    [SerializeField] private float textSpeed = .1f;
    private DialogueLineSO currentLine;
    private DialogueLineSO[] currentChoice;
    private DialogueLineSO selectedChoice;
    private int selectedChoiceIndex;

    private string fullTextToShow;
    private Coroutine typeTextCo;
    private bool waitingToConfirm;
    private bool canInteract;
    private Player_QuestManager questManager;


    private void Awake()
    {
        ui = GetComponentInParent<UI>();
    }

    public void PlayDialogueLine(DialogueLineSO line, bool showActionLine = false)
    {
        currentLine = line;
        currentChoice = GetVisibleChoices(line);
        canInteract = false;
        HideAllChoices();

        speakerPortrait.sprite = line.speaker.speakerPortait;
        speakerName.text = line.speaker.speakerName;

        fullTextToShow = (showActionLine && !string.IsNullOrEmpty(line.actionLine)) ?
            line.actionLine : line.GetRandomLine();

        typeTextCo = StartCoroutine(TypeTextCo(fullTextToShow));
        StartCoroutine(EnableInteractionCo());
    }

    public void ExitButton()
    {
        StopAllCoroutines();
        waitingToConfirm = false;
        canInteract = false;
        selectedChoice = null;
        selectedChoiceIndex = 0;
        ui.SwitchToInGame();
    }

    private void HandleNextAction()
    {
        switch (currentLine.actionType)
        {
            case DialogueActionType.OpenShop: ProcessDialogueQuestActions(currentLine); ui.SwitchToInGame(); ui.OpenMerchantUI(true); break;
            case DialogueActionType.OpenCraft: ProcessDialogueQuestActions(currentLine); ui.SwitchToInGame(); ui.OpenCraftUI(true); break;
            case DialogueActionType.OpenThuQuy: ProcessDialogueQuestActions(currentLine); ui.SwitchToInGame(); ui.OpenStorageUI(true); break;
            case DialogueActionType.CloseDialogue: ProcessDialogueQuestActions(currentLine); ui.SwitchToInGame(); break;
            case DialogueActionType.PlayerMakeChoice:
                if (selectedChoice == null)
                {
                    selectedChoiceIndex = 0;
                    ShowChoice();
                    waitingToConfirm = true;
                }
                else
                {
                    DialogueLineSO nextLine = selectedChoice;
                    selectedChoice = null;
                    selectedChoiceIndex = 0;
                    PlayDialogueLine(nextLine, true);
                }
                break;

        }
    }
    private void ShowChoice()
    {
        if (currentChoice == null || currentChoice.Length == 0)
        {
            HideAllChoices();
            selectedChoice = null;
            return;
        }

        selectedChoiceIndex = Mathf.Clamp(selectedChoiceIndex, 0, currentChoice.Length - 1);

        for (int i = 0; i < dialogueChoices.Length; i++)
        {
            if (i < currentChoice.Length)
            {
                DialogueLineSO choice = currentChoice[i];
                string choiceText = choice.GetFirstLine();

                dialogueChoices[i].gameObject.SetActive(true);
                dialogueChoices[i].text = selectedChoiceIndex == i ?
                    $"<color=yellow>{choiceText}" : $"{choiceText}";
            }
            else
            {
                dialogueChoices[i].gameObject.SetActive(false);
            }
        }
        selectedChoice = currentChoice[selectedChoiceIndex];
    }

    private void HideAllChoices()
    {
        foreach (var obj in dialogueChoices)
        {
            obj.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Lọc lại Choice Line của 1 DialogueLineSO theo điều kiện Quest (choiceRequireActiveQuest).
    /// Lựa chọn nào yêu cầu 1 Quest mà Player hiện KHÔNG đang active Quest đó (chưa nhận hoặc đã hoàn thành)
    /// sẽ tự động bị ẩn khỏi danh sách hiển thị.
    /// </summary>
    private DialogueLineSO[] GetVisibleChoices(DialogueLineSO line)
    {
        if (line == null || line.choiceLine == null || line.choiceLine.Length == 0)
            return line?.choiceLine;

        Player_QuestManager qManager = GetQuestManager();
        var visibleChoices = new System.Collections.Generic.List<DialogueLineSO>();

        for (int i = 0; i < line.choiceLine.Length; i++)
        {
            QuestDataSO requiredQuest = (line.choiceRequireActiveQuest != null && i < line.choiceRequireActiveQuest.Length)
                ? line.choiceRequireActiveQuest[i]
                : null;

            bool isAllowed = requiredQuest == null ||
                (qManager != null && qManager.QuestIsActive(requiredQuest));

            if (isAllowed)
                visibleChoices.Add(line.choiceLine[i]);
        }

        return visibleChoices.ToArray();
    }

    /// <summary>
    /// Xử lý Done Quest / Can Except Quest gắn trên 1 DialogueLineSO khi dialogue của dòng đó kết thúc:
    /// - doneQuest: hoàn thành ngay Quest được gán (Quest sẽ không thể nhận lại, biến mất khỏi UI_PlayerQuest).
    /// - canExceptQuest: cho Player nhận ngay Quest kế tiếp được gán.
    /// Logic này áp dụng chung cho mọi DialogueLineSO có gán doneQuest/canExceptQuest trong tương lai.
    /// </summary>
    private void ProcessDialogueQuestActions(DialogueLineSO line)
    {
        if (line == null)
            return;

        Player_QuestManager qManager = GetQuestManager();
        if (qManager == null)
            return;

        if (line.doneQuest != null)
            qManager.CompleteQuestManually(line.doneQuest);

        if (line.canExceptQuest != null)
            qManager.AcceptQuest(line.canExceptQuest);
    }

    private Player_QuestManager GetQuestManager()
    {
        if (questManager == null && Player.instance != null)
            questManager = Player.instance.questManager;

        return questManager;
    }

    public void NavigateChoice(int direction)
    {
        if (currentChoice == null || currentChoice.Length <= 1)
            return;

        selectedChoiceIndex += direction;
        selectedChoiceIndex = Mathf.Clamp(selectedChoiceIndex, 0, currentChoice.Length - 1);
        ShowChoice();
    }

    public void DialogueInteraction()
    {
        if (canInteract == false)
            return;

        if (typeTextCo != null)
        {
            CompleteTyping();
            waitingToConfirm = true;
            OnFullTextShown();
            return;
        }
        if (waitingToConfirm)
        {
            waitingToConfirm = false;
            HandleNextAction();
        }
    }

    private void CompleteTyping()
    {
        if (typeTextCo != null)
        {
            StopCoroutine(typeTextCo);
            dialogueText.text = fullTextToShow;
            typeTextCo = null;
        }
    }

    private IEnumerator TypeTextCo(string text)
    {
        dialogueText.text = "";

        foreach (char letter in text)
        {
            dialogueText.text = dialogueText.text + letter;
            yield return new WaitForSeconds(textSpeed);
        }
        waitingToConfirm = true;
        typeTextCo = null;
        OnFullTextShown();
    }

    private void OnFullTextShown()
    {
        if (currentLine != null && currentLine.actionType == DialogueActionType.CloseDialogue)
            StartCoroutine(AutoCloseDialogueCo());
    }

    private IEnumerator AutoCloseDialogueCo()
    {
        yield return new WaitForSeconds(1f);

        if (currentLine != null && currentLine.actionType == DialogueActionType.CloseDialogue)
        {
            waitingToConfirm = false;
            HandleNextAction();
        }
    }

    private IEnumerator EnableInteractionCo()
    {
        yield return null;
        canInteract = true;
    }

}
