using UnityEngine;

public class UI_DeathGame : MonoBehaviour
{
    public void GoToCheckpoint()
    {
        GameManager.instance.RestartScenes();
    }
    public void GoToMainMenu()
    {
        GameManager.instance.ChangeScenes("Main Menu", RespawnType.None);
    }
}
