using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wuxia Setup/Audio/Audio DataBase")]
public class AudioDataBaseSO : ScriptableObject
{
    public List<AudioClipData> player;
    public List<AudioClipData> mushroom;
    public List<AudioClipData> uiAudio;
    public List<AudioClipData> objectItem;

    [Header("Music List")]
    public List<AudioClipData> mainMenuMusic;
    public List<AudioClipData> levelMusic;
    public List<AudioClipData> asmrMusic;

    private Dictionary<string, AudioClipData> clipCollection;

    private void OnEnable()
    {
        clipCollection = new Dictionary<string, AudioClipData>();
        AddToCollection(player);
        AddToCollection(objectItem);
        AddToCollection(uiAudio);
        AddToCollection(levelMusic);
        AddToCollection(mainMenuMusic);
        AddToCollection(asmrMusic);
    }

    public AudioClipData Get(string groupName)
    {
        return clipCollection.TryGetValue(groupName, out var data) ? data : null;
    }

    private void AddToCollection(List<AudioClipData> listToAdd)
    {
        foreach (var data in listToAdd)
        {
            if (data != null && clipCollection.ContainsKey(data.audioName) == false)
            {
                clipCollection.Add(data.audioName, data);
            }
        }
    }
}

[Serializable]
public class AudioClipData
{
    public string audioName;
    public List<AudioClip> clips = new List<AudioClip>();
    [Range(0f, 1f)] public float maxVolume = 1f;

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Count == 0)
            return null;
        return clips[UnityEngine.Random.Range(0, clips.Count)];
    }
}