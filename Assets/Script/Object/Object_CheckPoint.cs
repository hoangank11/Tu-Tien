using UnityEngine;

public class Object_CheckPoint : MonoBehaviour, ISaveable
{
    [SerializeField] private string checkPointID;
    [SerializeField] private Transform respawnPoint;
    private AudioSource fireAudio;
    public bool isActive { get; private set; }
    private Animator anim;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        fireAudio = GetComponent<AudioSource>();
    }
    public string GetCheckPointID() => checkPointID;
    public Vector3 GetPosition() => respawnPoint == null ? transform.position : respawnPoint.position;

    public void ActivateCheckpoint(bool activate)
    {
        isActive = activate;
        anim.SetBool("isActive", activate);
        if(isActive && fireAudio.isPlaying == false)
            fireAudio.Play();
        if(isActive == false)
            fireAudio.Stop();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ActivateCheckpoint(true);
    }

    public void LoadData(GameData data)
    {
        bool active = data.unlockCheckPoint.TryGetValue(checkPointID, out active);
        ActivateCheckpoint(active);
    }

    public void SaveData(ref GameData data)
    {
        if (isActive == false)
            return;
        if (data.unlockCheckPoint.ContainsKey(checkPointID) == false)
            data.unlockCheckPoint.Add(checkPointID, true);
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(checkPointID))
        {
            checkPointID = System.Guid.NewGuid().ToString();
        }
#endif
    }
}
