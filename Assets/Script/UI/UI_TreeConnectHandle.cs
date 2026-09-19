using System;
using UnityEngine;


[Serializable]
public class UI_TreeConnectDetail
{
    public UI_TreeConnectHandle childNode;
    public NodeDirectionType direction;
    [Range(1f, 1000f)] public float length;
}



public class UI_TreeConnectHandle : MonoBehaviour
{
    private RectTransform rect => GetComponent<RectTransform>();
    [SerializeField] private UI_TreeConnectDetail[] ConnectDetails;
    [SerializeField] private UI_TreeConnection[] connections;


    private void OnValidate()
    {
        if (ConnectDetails.Length <= 0)
            return;

        if (ConnectDetails.Length != connections.Length)
            return;
        UpdateConnection();
    }
    public void UpdateConnection()
    {
        for (int i = 0; i < ConnectDetails.Length; i++)
        {
            var detail = ConnectDetails[i];
            var connection = this.connections[i];
            Vector2 targetPosition = connection.GetConnectPoint(rect);

            connection.DirectConnection(detail.direction, detail.length);

            if (detail.childNode == null)
                continue;
            detail.childNode.SetPosition(targetPosition);
            detail.childNode.transform.SetAsLastSibling();

        }
    }

    public void UpdateAllConnect()
    {
        UpdateConnection();
        foreach (var node in ConnectDetails)
        {
            if (node.childNode == null) continue;
            node.childNode?.UpdateConnection();
        }
    }

    public void SetPosition(Vector2 position) => rect.anchoredPosition = position;


}
