using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Background Music")]
    [SerializeField] private AudioClip menuBGM;
    [SerializeField] private AudioClip gameBGM;
    
    [Header("Sound Effects")]
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip coinCollectSFX;
    [SerializeField] private AudioClip gameStartSFX;
    [SerializeField] private AudioClip gameOverSFX;
    [SerializeField] private AudioClip errorSFX;
    [SerializeField] private AudioClip successSFX;
    
    [Header("Audio Settings")]
    [SerializeField] private float bgmVolume = 0.3f;
    [SerializeField] private float sfxVolume = 0.7f;
    [SerializeField] private float fadeDuration = 1f;
    
    private Dictionary<string, AudioClip> sfxClips;
    private bool isMuted = false;
    private bool isBGMMuted = false;
    private bool isSFXMuted = false;
    private string currentBGMName = "";
    
    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeAudio();
    }
    
    void Start()
    {
        // Start with menu BGM
        PlayBGM("menu");
    }
    
    private void InitializeAudio()
    {
        // Create AudioSources if not assigned
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.volume = bgmVolume;
            bgmSource.playOnAwake = false;
        }
        
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.volume = sfxVolume;
            sfxSource.playOnAwake = false;
        }
        
        // Initialize SFX dictionary
        sfxClips = new Dictionary<string, AudioClip>
        {
            { "button_click", buttonClickSFX },
            { "coin_collect", coinCollectSFX },
            { "game_start", gameStartSFX },
            { "game_over", gameOverSFX },
            { "error", errorSFX },
            { "success", successSFX }
        };
        
        // Load saved audio settings
        LoadAudioSettings();
        
        Debug.Log("AudioManager initialized");
    }
    
    #region BGM Control
    
    public void PlayBGM(string bgmName)
    {
        if (isBGMMuted || isMuted) return;
        
        AudioClip clipToPlay = null;
        
        switch (bgmName.ToLower())
        {
            case "menu":
                clipToPlay = menuBGM;
                break;
            case "game":
                clipToPlay = gameBGM;
                break;
        }
        
        if (clipToPlay != null && currentBGMName != bgmName)
        {
            StartCoroutine(CrossfadeBGM(clipToPlay, bgmName));
        }
    }
    
    private IEnumerator CrossfadeBGM(AudioClip newClip, string newBGMName)
    {
        // Fade out current BGM
        if (bgmSource.isPlaying)
        {
            yield return StartCoroutine(FadeBGM(bgmVolume, 0f));
        }
        
        // Switch to new clip
        bgmSource.clip = newClip;
        bgmSource.Play();
        currentBGMName = newBGMName;
        
        // Fade in new BGM
        yield return StartCoroutine(FadeBGM(0f, bgmVolume));
        
        Debug.Log($"BGM switched to: {newBGMName}");
    }
    
    private IEnumerator FadeBGM(float fromVolume, float toVolume)
    {
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaled time for pause compatibility
            bgmSource.volume = Mathf.Lerp(fromVolume, toVolume, elapsed / fadeDuration);
            yield return null;
        }
        
        bgmSource.volume = toVolume;
        
        if (toVolume <= 0f)
        {
            bgmSource.Stop();
        }
    }
    
    public void StopBGM()
    {
        StartCoroutine(FadeBGM(bgmSource.volume, 0f));
        currentBGMName = "";
    }
    
    #endregion
    
    #region SFX Control
    
    public void PlaySFX(string sfxName)
    {
        if (isSFXMuted || isMuted) return;
        
        if (sfxClips.ContainsKey(sfxName) && sfxClips[sfxName] != null)
        {
            sfxSource.PlayOneShot(sfxClips[sfxName]);
            Debug.Log($"Playing SFX: {sfxName}");
        }
        else
        {
            Debug.LogWarning($"SFX not found: {sfxName}");
        }
    }
    
    public void PlaySFXWithPitch(string sfxName, float pitch)
    {
        if (isSFXMuted || isMuted) return;
        
        if (sfxClips.ContainsKey(sfxName) && sfxClips[sfxName] != null)
        {
            // Create temporary AudioSource for pitch variation
            GameObject tempSFX = new GameObject("TempSFX");
            AudioSource tempSource = tempSFX.AddComponent<AudioSource>();
            tempSource.clip = sfxClips[sfxName];
            tempSource.volume = sfxVolume;
            tempSource.pitch = pitch;
            tempSource.Play();
            
            // Destroy after clip finishes
            Destroy(tempSFX, sfxClips[sfxName].length / pitch);
        }
    }
    
    #endregion
    
    #region Volume Control
    
    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        AudioListener.volume = volume;
        SaveAudioSettings();
    }
    
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        if (bgmSource != null && !isBGMMuted && !isMuted)
        {
            bgmSource.volume = bgmVolume;
        }
        SaveAudioSettings();
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
        SaveAudioSettings();
    }
    
    #endregion
    
    #region Mute Control
    
    public void ToggleMute()
    {
        isMuted = !isMuted;
        ApplyMuteSettings();
        SaveAudioSettings();
    }
    
    public void ToggleBGMMute()
    {
        isBGMMuted = !isBGMMuted;
        ApplyMuteSettings();
        SaveAudioSettings();
    }
    
    public void ToggleSFXMute()
    {
        isSFXMuted = !isSFXMuted;
        ApplyMuteSettings();
        SaveAudioSettings();
    }
    
    public void SetMute(bool mute)
    {
        isMuted = mute;
        ApplyMuteSettings();
        SaveAudioSettings();
    }
    
    private void ApplyMuteSettings()
    {
        // Apply BGM mute
        if (bgmSource != null)
        {
            bgmSource.volume = (isMuted || isBGMMuted) ? 0f : bgmVolume;
        }
        
        // Apply SFX mute
        if (sfxSource != null)
        {
            sfxSource.volume = (isMuted || isSFXMuted) ? 0f : sfxVolume;
        }
        
        Debug.Log($"Audio Mute - Master: {isMuted}, BGM: {isBGMMuted}, SFX: {isSFXMuted}");
    }
    
    #endregion
    
    #region Scene-Specific Audio
    
    public void OnSceneChanged(string sceneName)
    {
        switch (sceneName.ToLower())
        {
            case "entry":
            case "login":
            case "register":
            case "mainmenu":
                PlayBGM("menu");
                break;
            case "gamescene":
                PlayBGM("game");
                PlaySFX("game_start");
                break;
            default:
                PlayBGM("menu");
                break;
        }
    }
    
    #endregion
    
    #region Settings Persistence
    
    private void SaveAudioSettings()
    {
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.SetInt("IsBGMMuted", isBGMMuted ? 1 : 0);
        PlayerPrefs.SetInt("IsSFXMuted", isSFXMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    private void LoadAudioSettings()
    {
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.3f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        isBGMMuted = PlayerPrefs.GetInt("IsBGMMuted", 0) == 1;
        isSFXMuted = PlayerPrefs.GetInt("IsSFXMuted", 0) == 1;
        
        ApplyMuteSettings();
    }
    
    #endregion
    
    #region Public Getters
    
    public bool IsMuted => isMuted;
    public bool IsBGMMuted => isBGMMuted;
    public bool IsSFXMuted => isSFXMuted;
    public float BGMVolume => bgmVolume;
    public float SFXVolume => sfxVolume;
    
    #endregion
    
    #region Editor Helpers
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void TestPlaySFX(string sfxName)
    {
        PlaySFX(sfxName);
    }
    
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void TestPlayBGM(string bgmName)
    {
        PlayBGM(bgmName);
    }
    
    #endregion
}
