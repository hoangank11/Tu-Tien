using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SFXPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Important = 3
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Database")]
    [SerializeField] private AudioDataBaseSO audioDataBase;

    [Header("BGM")]
    [SerializeField] private AudioSource bgmMixerSource;
    [SerializeField] private bool bgmShouldPlay;

    [Header("SFX Pool")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField, Min(1)] private int maxConcurrentSFX = 24;

    [Tooltip("Nếu true, khi pool đầy, SFX mới có Priority cao hơn sẽ đẩy SFX Priority thấp hơn.")]
    [SerializeField] private bool allowHigherPriorityToStealVoice = true;

    [Header("Debug")]
    [SerializeField] private bool showDebugLog;

    private readonly List<SFXVoice> voices = new List<SFXVoice>();

    private AudioClip lastMusicPlayed;
    private Transform player;

    private string currentBgmGroupName;
    private Coroutine currentBgmCo;

    private class SFXVoice
    {
        public AudioSource source;
        public SFXPriority priority;
        public float distance;
        public bool isUI;
        public float startTime;

        public bool IsPlaying()
        {
            return source != null && source.isPlaying;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePool();
    }

    private void Update()
    {
        CleanupFinishedVoices();

        UpdateBGM();
    }

    #region INITIALIZE

    private void InitializePool()
    {
        if (maxConcurrentSFX < 1)
            maxConcurrentSFX = 1;

        for (int i = 0; i < maxConcurrentSFX; i++)
        {
            CreateVoice();
        }
    }

    private SFXVoice CreateVoice()
    {
        GameObject obj = new GameObject($"SFX_Voice_{voices.Count}");
        obj.transform.SetParent(transform);

        AudioSource source = obj.AddComponent<AudioSource>();

        source.outputAudioMixerGroup = sfxMixerGroup;
        source.playOnAwake = false;
        source.loop = false;
        source.spatialBlend = 0f;

        SFXVoice voice = new SFXVoice
        {
            source = source,
            priority = SFXPriority.Low,
            distance = 0f,
            isUI = false,
            startTime = 0f
        };

        voices.Add(voice);

        return voice;
    }

    #endregion

    #region BGM

    private void UpdateBGM()
    {
        if (bgmMixerSource == null)
            return;

        if (!bgmMixerSource.isPlaying && bgmShouldPlay)
        {
            if (!string.IsNullOrEmpty(currentBgmGroupName))
            {
                NextBGM(currentBgmGroupName);
            }
        }

        if (bgmMixerSource.isPlaying && !bgmShouldPlay)
        {
            StopBGM();
        }
    }

    public void StartBGM(string musicGroup)
    {
        if (string.IsNullOrEmpty(musicGroup))
            return;

        bgmShouldPlay = true;

        if (musicGroup == currentBgmGroupName && bgmMixerSource.isPlaying)
            return;

        NextBGM(musicGroup);
    }

    public void NextBGM(string musicGroup)
    {
        if (string.IsNullOrEmpty(musicGroup))
            return;

        bgmShouldPlay = true;
        currentBgmGroupName = musicGroup;

        if (currentBgmCo != null)
        {
            StopCoroutine(currentBgmCo);
        }

        currentBgmCo = StartCoroutine(SwitchMusicCo(musicGroup));
    }

    public void StopBGM()
    {
        bgmShouldPlay = false;

        if (bgmMixerSource == null)
            return;

        if (currentBgmCo != null)
        {
            StopCoroutine(currentBgmCo);
            currentBgmCo = null;
        }

        StartCoroutine(FadeVolumeCo(bgmMixerSource, 0f, 1f));
    }

    private System.Collections.IEnumerator SwitchMusicCo(string musicGroup)
    {
        if (audioDataBase == null)
            yield break;

        AudioClipData data = audioDataBase.Get(musicGroup);

        if (data == null || data.clips == null || data.clips.Count == 0)
            yield break;

        AudioClip nextMusic = data.GetRandomClip();

        if (nextMusic == null)
            yield break;

        if (data.clips.Count > 1)
        {
            int safety = 0;

            while (nextMusic == lastMusicPlayed && safety < 20)
            {
                nextMusic = data.GetRandomClip();
                safety++;
            }
        }

        if (bgmMixerSource == null)
            yield break;

        if (bgmMixerSource.isPlaying)
        {
            yield return FadeVolumeCo(bgmMixerSource, 0f, 1f);
        }

        lastMusicPlayed = nextMusic;

        bgmMixerSource.clip = nextMusic;
        bgmMixerSource.volume = 0f;
        bgmMixerSource.Play();

        StartCoroutine(
            FadeVolumeCo(
                bgmMixerSource,
                data.maxVolume,
                1f
            )
        );
    }

    private System.Collections.IEnumerator FadeVolumeCo(
        AudioSource source,
        float targetVolume,
        float duration)
    {
        if (source == null)
            yield break;

        if (duration <= 0f)
        {
            source.volume = targetVolume;
            yield break;
        }

        float time = 0f;
        float startVolume = source.volume;

        while (time < duration)
        {
            time += Time.deltaTime;

            source.volume = Mathf.Lerp(
                startVolume,
                targetVolume,
                time / duration
            );

            yield return null;
        }

        source.volume = targetVolume;
    }

    #endregion

    #region ENTITY SFX

    /// <summary>
    /// Phát SFX của Player/Enemy.
    /// Vị trí được lấy từ Transform của Entity.
    /// </summary>
    public void PlaySFX(
        string soundName,
        Transform emitter,
        float maxDistance,
        SFXPriority priority)
    {
        if (string.IsNullOrEmpty(soundName))
            return;

        if (audioDataBase == null)
            return;

        if (emitter == null)
            return;

        EnsurePlayerReference();

        AudioClipData data = audioDataBase.Get(soundName);

        if (data == null)
            return;

        AudioClip clip = data.GetRandomClip();

        if (clip == null)
            return;

        float distance = 0f;

        if (player != null)
        {
            distance = Vector2.Distance(
                emitter.position,
                player.position
            );
        }

        // Player luôn nằm trong khoảng nghe.
        // Enemy/Entity thì kiểm tra khoảng cách.
        if (priority != SFXPriority.Important)
        {
            if (maxDistance <= 0f)
                return;

            if (distance > maxDistance)
            {
                if (showDebugLog)
                {
                    Debug.Log(
                        $"[AudioManager] SFX bỏ vì quá xa: {soundName}"
                    );
                }

                return;
            }
        }

        float volumeMultiplier = 1f;

        if (priority != SFXPriority.Important)
        {
            float t = Mathf.Clamp01(
                1f - distance / maxDistance
            );

            // Âm thanh càng xa càng nhỏ.
            volumeMultiplier = t * t;
        }

        float finalVolume =
            data.maxVolume * volumeMultiplier;

        SFXVoice voice = GetVoice(
            priority,
            distance,
            false
        );

        if (voice == null)
        {
            if (showDebugLog)
            {
                Debug.Log(
                    $"[AudioManager] SFX bị bỏ do pool đầy: {soundName}"
                );
            }

            return;
        }

        PlayOnVoice(
            voice,
            clip,
            finalVolume,
            priority,
            distance,
            false
        );
    }

    #endregion

    #region UI SFX

    /// <summary>
    /// UI SFX không có khoảng cách.
    /// UI được ưu tiên cao.
    /// </summary>
    public void PlayUISFX(string soundName)
    {
        if (string.IsNullOrEmpty(soundName))
            return;

        if (audioDataBase == null)
            return;

        AudioClipData data = audioDataBase.Get(soundName);

        if (data == null)
            return;

        AudioClip clip = data.GetRandomClip();

        if (clip == null)
            return;

        SFXVoice voice = GetVoice(
            SFXPriority.Important,
            0f,
            true
        );

        if (voice == null)
        {
            if (showDebugLog)
            {
                Debug.Log(
                    $"[AudioManager] UI SFX bị bỏ do pool đầy: {soundName}"
                );
            }

            return;
        }

        PlayOnVoice(
            voice,
            clip,
            data.maxVolume,
            SFXPriority.Important,
            0f,
            true
        );
    }

    /// <summary>
    /// Giữ lại API cũ để UI_MainMenu và script cũ của bạn
    /// vẫn có thể gọi AudioManager.instance.PlayGlobalSFX(...).
    /// Global SFX được xem như UI/Important SFX.
    /// </summary>
    public void PlayGlobalSFX(string soundName)
    {
        PlayUISFX(soundName);
    }

    #endregion

    #region VOICE MANAGEMENT

    private SFXVoice GetVoice(
        SFXPriority newPriority,
        float newDistance,
        bool isUI)
    {
        CleanupFinishedVoices();

        // 1. Tìm voice đang rảnh.
        for (int i = 0; i < voices.Count; i++)
        {
            if (!voices[i].IsPlaying())
            {
                return voices[i];
            }
        }

        // 2. Pool đang đầy.
        if (!allowHigherPriorityToStealVoice)
        {
            return null;
        }

        SFXVoice weakestVoice = null;

        for (int i = 0; i < voices.Count; i++)
        {
            SFXVoice voice = voices[i];

            if (!voice.IsPlaying())
                return voice;

            if (weakestVoice == null)
            {
                weakestVoice = voice;
                continue;
            }

            if (IsVoiceWeaker(
                voice,
                weakestVoice,
                newPriority,
                newDistance))
            {
                weakestVoice = voice;
            }
        }

        if (weakestVoice == null)
            return null;

        // Không được phép đè Important bằng Important
        // nếu cả hai đều quan trọng.
        if (weakestVoice.priority == SFXPriority.Important &&
            newPriority == SFXPriority.Important)
        {
            return null;
        }

        // Chỉ steal nếu SFX mới thực sự quan trọng hơn.
        if (newPriority > weakestVoice.priority)
        {
            weakestVoice.source.Stop();
            return weakestVoice;
        }

        // Cùng priority:
        // SFX gần Player hơn được ưu tiên.
        if (newPriority == weakestVoice.priority &&
            newDistance < weakestVoice.distance)
        {
            weakestVoice.source.Stop();
            return weakestVoice;
        }

        return null;
    }

    private bool IsVoiceWeaker(
        SFXVoice candidate,
        SFXVoice currentWeakest,
        SFXPriority newPriority,
        float newDistance)
    {
        if (candidate.priority != currentWeakest.priority)
        {
            return candidate.priority <
                   currentWeakest.priority;
        }

        // Nếu cùng priority:
        // xa Player hơn = yếu hơn.
        if (!candidate.isUI &&
            !currentWeakest.isUI)
        {
            return candidate.distance >
                   currentWeakest.distance;
        }

        // UI cùng priority với UI:
        // voice cũ hơn được xem là yếu hơn.
        return candidate.startTime <
               currentWeakest.startTime;
    }

    private void PlayOnVoice(
        SFXVoice voice,
        AudioClip clip,
        float volume,
        SFXPriority priority,
        float distance,
        bool isUI)
    {
        if (voice == null || voice.source == null)
            return;

        voice.source.Stop();

        voice.priority = priority;
        voice.distance = distance;
        voice.isUI = isUI;
        voice.startTime = Time.time;

        voice.source.clip = null;
        voice.source.pitch = Random.Range(
            0.95f,
            1.1f
        );

        voice.source.volume = volume;

        // Quan trọng:
        // Không gán source.clip = clip.
        // Chỉ dùng PlayOneShot.
        voice.source.PlayOneShot(clip);
    }

    private void CleanupFinishedVoices()
    {
        for (int i = 0; i < voices.Count; i++)
        {
            if (!voices[i].IsPlaying())
            {
                voices[i].priority = SFXPriority.Low;
                voices[i].distance = 0f;
                voices[i].isUI = false;
            }
        }
    }

    #endregion

    #region PLAYER

    private void EnsurePlayerReference()
    {
        if (player != null)
            return;

        if (Player.instance != null)
        {
            player = Player.instance.transform;
        }
    }

    #endregion
}