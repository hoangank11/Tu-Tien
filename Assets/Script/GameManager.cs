using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager instance;
    public float timeLoadingScenes = .1f;
    private Vector3 lastPlayerPosition;
    private string lastScensePlayed;
    private bool isSaved;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ContinuePlay()
    {
        ChangeScenes(lastScensePlayed, RespawnType.None);
    }

    public void RestartScenes()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        ChangeScenes(sceneName, RespawnType.None);
    }

    public void ChangeScenes(string scenesName, RespawnType respawnType)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // Không lưu game khi đang rời khỏi Main Menu: ở đó chưa có Player/dữ liệu gameplay thật
        // để lưu, nếu lưu sẽ ghi đè thành file save rỗng NGAY TRƯỚC khi scene mới kịp khởi tạo Player,
        // khiến hệ thống hiểu nhầm là "đã có save data" và không cấp Quest mặc định (startingMainQuest)
        // cho game mới.
        if (currentScene != "Main Menu")
            SaveManager.instance.SaveGame();

        Time.timeScale = 1f;
        StartCoroutine(ChangeScenesCo(scenesName, respawnType));
    }

    private IEnumerator ChangeScenesCo(string scenesName, RespawnType respawnType)
    {
        UI_LoadScreen loadScreen = FindFadeScreenUI();
        loadScreen.DoFadeOut();
        yield return loadScreen.fadeEffectCo;

        SceneManager.LoadScene(scenesName);

        isSaved = false;
        yield return null;

        while (isSaved == false)
        {
            yield return null;
        }
        loadScreen = FindFadeScreenUI();
        loadScreen.DoFadeIn();

        Player player = Player.instance;
        if (player == null) yield break;

        Vector3 position = GetNewPlayerPosition(respawnType);

        if (position != Vector3.zero)
            player.TeleportPlayer(position);
    }

    private UI_LoadScreen FindFadeScreenUI()
    {
        if (UI.instance != null)
            return UI.instance.loadScreenUI;
        else
            return FindFirstObjectByType<UI_LoadScreen>();
    }

    private Vector3 GetNewPlayerPosition(RespawnType type)
    {
        if (type == RespawnType.Portal)
        {
            Object_Portal portal = Object_Portal.instance;
            Vector3 position = portal.GetPosition();
            portal.SetTrigger(false);
            portal.DisableIfNeeded();
            return position;
        }



        if (type == RespawnType.None)
        {
            var data = SaveManager.instance.GetGameData();

            // sau khi chết sẽ respawn ở gần checkpoint
            var checkpoint = FindObjectsByType<Object_CheckPoint>(FindObjectsSortMode.None);
            var unlockedCheckPoint = checkpoint
                .Where(cp => data.unlockCheckPoint.TryGetValue(cp.GetCheckPointID(), out bool unlocked) && unlocked)
                .Select(cp => cp.GetPosition())
                .ToList();
            // sau khi chết sẽ respawn ở gần waypoint
            var enterWaypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None)
                .Where(wp => wp.GetWaypointType() == RespawnType.Enter)
                .Select(wp => wp.GetPosition())
                .ToList();
            // căn cứ vào khoảng cách để xem sẽ respawn ở đâu
            var selectedPosition = unlockedCheckPoint.Concat(enterWaypoints).ToList();  // concat dfung để combine 2 list về 1
            if (selectedPosition.Count == 0)
                return Vector3.zero;
            return selectedPosition.OrderBy(Position => Vector3.Distance(Position, lastPlayerPosition)).First();
        }

        return GetWaypointPosition(type);
    }


    private Vector3 GetWaypointPosition(RespawnType type)
    {
        var waypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None);
        foreach (var point in waypoints)
        {
            if (point.GetWaypointType() == type)
            {
                point.SetCanBeTrigger(true);
                return point.GetPosition();
            }
        }
        return Vector3.zero;
    }

    public void LoadData(GameData data)
    {
        lastScensePlayed = data.lastScenePlayed;
        lastPlayerPosition = data.lastPlayerPosition;

        if (string.IsNullOrEmpty(lastScensePlayed))
            lastScensePlayed = "Thôn Thanh Nguyên";
        isSaved = true;
    }

    public void SaveData(ref GameData data)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Main Menu")
            return;
        data.lastPlayerPosition = Player.instance.transform.position;
        data.lastScenePlayed = currentScene;
        isSaved = false;
    }
}