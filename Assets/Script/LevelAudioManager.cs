using UnityEngine;

public class LevelAudioManager : MonoBehaviour
{
    [SerializeField] private string musicGroupName;

    private void Start()
    {
        AudioManager.instance.StartBGM(musicGroupName);
    }
}
