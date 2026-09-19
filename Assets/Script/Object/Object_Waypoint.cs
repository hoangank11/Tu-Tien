using UnityEngine;
using UnityEngine.SceneManagement;

public class Object_Waypoint : MonoBehaviour
{
    [SerializeField] private string transferToScene;
    [Space]
    [SerializeField] private RespawnType waypointType;
    [SerializeField] private RespawnType connectedWaypoint;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private bool canBeTrigger = true;

    public void SetCanBeTrigger(bool canBeTrigger) => this.canBeTrigger = canBeTrigger;

    public RespawnType GetWaypointType() => waypointType;

    public Vector3 GetPosition()
    {
        return respawnPoint == null ? transform.position : respawnPoint.position;
    }

    private void OnValidate()
    {
        gameObject.name = "Waypoint _ " + waypointType.ToString() + " - " + transferToScene;

        if (waypointType == RespawnType.Enter)
            connectedWaypoint = RespawnType.Exit;

        if (waypointType == RespawnType.Exit)
            connectedWaypoint = RespawnType.Enter;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBeTrigger == false)
            return;

        GameManager.instance.ChangeScenes(transferToScene, connectedWaypoint);
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canBeTrigger = true;
    }

}
