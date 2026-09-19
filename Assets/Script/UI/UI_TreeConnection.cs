using UnityEngine;

public class UI_TreeConnection : MonoBehaviour
{
    [SerializeField] private RectTransform rotationtPoint;
    [SerializeField] private RectTransform connectLength;
    [SerializeField] private RectTransform nodeConnectPoint;

    public void DirectConnection(NodeDirectionType type, float length)
    {
        bool shouldBeActive = type != NodeDirectionType.None;
        float finalLength = shouldBeActive ? length : 0f;
        float angle = GetDirectionAngle(type);

        rotationtPoint.localRotation = Quaternion.Euler(0f, 0f, angle);
        connectLength.sizeDelta = new Vector2(finalLength, connectLength.sizeDelta.y);

    }

    public Vector2 GetConnectPoint(RectTransform rect)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle
            (
                rect.parent as RectTransform,
                nodeConnectPoint.position,
                null,
                out var localPosition
            );
        return localPosition;
    }


    private float GetDirectionAngle(NodeDirectionType type)
    {
        switch (type)
        {
            case NodeDirectionType.Up:             return 90f;
            case NodeDirectionType.UpLeft:         return 135f;
            case NodeDirectionType.UpRight:        return 45f;
            case NodeDirectionType.Down:           return -90f;
            case NodeDirectionType.DownLeft:       return -135f;
            case NodeDirectionType.DownRight:      return -45f;
            case NodeDirectionType.Left:           return 180f;
            case NodeDirectionType.Right:          return 0f;
            default:                               return 0f;
        }

    }
}

public enum NodeDirectionType
{
    None,
    Up,
    UpLeft,
    UpRight,
    Left,
    Right,
    Down,
    DownLeft,
    DownRight
}