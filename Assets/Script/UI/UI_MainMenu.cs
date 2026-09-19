using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
    [Header("Scene sẽ load khi CHƯA có save data (game mới)")]
    [SerializeField] private string newGameSceneName;

    private void Start()
    {
        transform.root.GetComponentInChildren<UI_Options>(true).LoadUpVolume();
        transform.root.GetComponentInChildren<UI_LoadScreen>().DoFadeIn();
        AudioManager.instance.StartBGM("Menu");
    }

    public void PlayButton()
    {
        AudioManager.instance.PlayGlobalSFX("Button");
        
        if (SaveManager.instance == null)
        {
            Debug.LogError("UI_MainMenu: Không tìm thấy SaveManager trong scene!");
            return;
        }

        if (SaveManager.instance.HasSaveData())
        {
            GameManager.instance.ContinuePlay();
        }
        else
        {
            if (string.IsNullOrEmpty(newGameSceneName))
            {
                Debug.LogError("UI_MainMenu: Chưa gán 'New Game Scene Name' trong Inspector!");
                return;
            }

            GameManager.instance.ChangeScenes(newGameSceneName, RespawnType.None);
        }
    }

    public void InforButton()
    {
        AudioManager.instance.PlayGlobalSFX("Button");
    }

    public void QuitButton()
    {
        SaveManager.instance.SaveGame();
        Application.Quit();
    }
}
