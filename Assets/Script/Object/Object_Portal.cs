using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Object_Portal : MonoBehaviour, ISaveable
{
    public static Object_Portal instance;
    public bool isActive { get; private set; }

    [SerializeField] private Vector2 defaultPosition;
    [SerializeField] private string scenesName;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private bool canBeTriggered;
    private string currentScenesName;
    private string changeSceneName;
    private bool returningFromLastScene;
    private void Awake()
    {
        instance = this;
        currentScenesName = SceneManager.GetActiveScene().name;
        transform.position = new Vector3(9999, 9999);
    }
    // Triệu hồi portal tại đúng vị trí gần Player (thay vì vị trí DefaultPosition)
    public void ActivatePortal(Vector3 position, int facingDir = 1)
    {
        isActive = true;
        transform.position = position;
        SaveManager.instance.GetGameData().inScenesPortal.Clear();
        if (facingDir == -1)
            transform.Rotate(0, 180, 0);
    }
    // Triệu hồi portal tại đúng vị trí DefaultPosition đã cấu hình sẵn trong scene (thay vì gần Player)
    public void ActivatePortal(int facingDir = 1)
    {
        ActivatePortal(defaultPosition, facingDir);
    }

    public void DisableIfNeeded()
    {
        // Chỉ disable khi vừa đến scene Home/Hub ("Sân Thanh Vân Môn") qua portal
        if (InHome() == false) return;

        SaveManager.instance.GetGameData().inScenesPortal.Clear();
        isActive = false;
        transform.position = new Vector3(9999, 9999);
    }

    private void UsePortal()
    {
        string destinationScenes = InHome() ? changeSceneName : scenesName;

        GameManager.instance.ChangeScenes(destinationScenes, RespawnType.Portal);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBeTriggered == false) return;
        UsePortal();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        canBeTriggered = true;
    }
    public void SetTrigger(bool trigger) => canBeTriggered = trigger;
    public Vector3 GetPosition() => respawnPoint != null ? respawnPoint.position : transform.position;

    private bool InHome() => currentScenesName == scenesName;

    public void LoadData(GameData data)
    {
        if (InHome() && data.inScenesPortal.Count > 0)
        {
            transform.position = defaultPosition;
            isActive = true;
        }
        else if (data.inScenesPortal.TryGetValue(currentScenesName, out Vector3 portalPosition))
        {
            transform.position = portalPosition;
            isActive = true;
        }
        returningFromLastScene = data.returnningFromSence;
        changeSceneName = data.portalDestinationSceneName;
    }

    public void SaveData(ref GameData data)
    {
        if (isActive && InHome() == false)
        {
            data.inScenesPortal[currentScenesName] = transform.position;
            data.portalDestinationSceneName = currentScenesName;
        }
        else
        {
            data.inScenesPortal.Remove(currentScenesName);
        }

        data.returnningFromSence = InHome();

    }
}
