using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private FileDataHandler dataHandler;
    private GameData gameData;
    private List<ISaveable> allSaveables;
    private bool hasSaveData;

    [SerializeField] private string fileName = "SaveGame.json";
    [SerializeField] private bool encryptData = true;

    private void Awake()
    {
        instance = this;
    }

    private IEnumerator Start()
    {
        Debug.Log(Application.persistentDataPath);
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        allSaveables = FindISaveable();

        yield return null;
        LoadGame();
    }

    private void LoadGame()
    {
        gameData = dataHandler.LoadData();
        if (gameData == null)
        {
            Debug.Log("Chưa có dữ liệu cũ");
            gameData = new GameData();
            hasSaveData = false;

            // Vẫn phải cho tất cả ISaveable (vd: GameManager) load qua dữ liệu rỗng này
            // để chúng khởi tạo trạng thái mặc định và báo hiệu đã load xong
            // (vd: GameManager.isSaved = true, nếu không UI_LoadScreen sẽ treo mãi không tắt).
            foreach (var saveable in allSaveables)
                saveable.LoadData(gameData);

            // Cấp Quest mặc định SAU khi các ISaveable đã load/reset xong,
            // để không bị Player_QuestManager.LoadData() (chạy ở trên) xóa mất.
            GiveNewGameDefaults();
            return;
        }
        hasSaveData = true;
        foreach (var saveable in allSaveables)
            saveable.LoadData(gameData);
    }

    /// <summary>
    /// Thiết lập những gì Player cần có sẵn khi bắt đầu 1 game mới (chưa từng có save data cũ).
    /// Vd: tự động nhận Quest chính đầu tiên.
    /// </summary>
    private void GiveNewGameDefaults()
    {
        if (Player.instance != null && Player.instance.questManager != null)
            Player.instance.questManager.GiveStartingQuestIfNeeded();
    }

    public void SaveGame()
    {
        foreach (var saveable in allSaveables)
            saveable.SaveData(ref gameData);
        dataHandler.SaveData(gameData);
        hasSaveData = true;
    }

    public GameData GetGameData() => gameData;

    public bool HasSaveData() => hasSaveData;


    [ContextMenu("***** Xóa Dữ Liệu Game *****")]
    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();
        gameData = new GameData();
        hasSaveData = false;
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<ISaveable> FindISaveable()
    {
        return
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<ISaveable>()
            .ToList();
    }

}