using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Dialogue Data/New Line", fileName = "Line - ")]
public class DialogueLineSO : ScriptableObject
{
    [Header("Dialogue Infor")]
    public string dialogueGroupName;
    public DialogueSpeakerSO speaker;

    [Space]
    [Header("Text Options")]
    [TextArea] public string[] textLine;

    [Space]
    [Header("Dialogue Action ")]
    [TextArea] public string actionLine;
    public DialogueActionType actionType;
    public DialogueLineSO[] choiceLine;

    [Space]
    [Header("Quest Infor")]
    public QuestDataSO doneQuest;
    public QuestDataSO canExceptQuest;
    public bool canDeleteThisDialogueLineChoice;

    [Space]
    [Header("Điều Kiện Hiển Thị Cho Từng Lựa Chọn (đúng thứ tự với Choice Line)")]
    [Tooltip("Ứng với từng phần tử trong Choice Line theo đúng thứ tự. " +
        "Nếu để trống (hoặc phần tử null) thì lựa chọn đó luôn hiển thị. " +
        "Nếu gán 1 Quest, lựa chọn đó CHỈ hiển thị khi Player đang có Quest đó ở trạng thái active " +
        "(quest đã hoàn thành hoặc chưa nhận sẽ tự động ẩn lựa chọn này).")]
    public QuestDataSO[] choiceRequireActiveQuest;


    public string GetFirstLine() => textLine[0];

    public string GetRandomLine()
    {
        return textLine[Random.Range(0, textLine.Length)];
    }

}
