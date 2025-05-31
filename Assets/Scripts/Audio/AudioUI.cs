using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioUI : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    
    [Header("Mute Buttons")]
    [SerializeField] private Button muteAllButton;
    [SerializeField] private Button muteBGMButton;
    [SerializeField] private Button muteSFXButton;
    
    [Header("Volume Labels")]
    [SerializeField] private TextMeshProUGUI bgmVolumeLabel;
    [SerializeField] private TextMeshProUGUI sfxVolumeLabel;
    
    [Header("Mute Button Texts")]
    [SerializeField] private TextMeshProUGUI muteAllText;
    [SerializeField] private TextMeshProUGUI muteBGMText;
    [SerializeField] private TextMeshProUGUI muteSFXText;
    
    void Start()
    {
        SetupUI();
        UpdateUI();
    }
    
    private void SetupUI()
    {
        // Setup volume sliders
        if (bgmVolumeSlider != null)
        {
            bgmVolumeSlider.minValue = 0f;
            bgmVolumeSlider.maxValue = 1f;
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        
        // Setup mute buttons
        if (muteAllButton != null)
            muteAllButton.onClick.AddListener(OnMuteAllClicked);
            
        if (muteBGMButton != null)
            muteBGMButton.onClick.AddListener(OnMuteBGMClicked);
            
        if (muteSFXButton != null)
            muteSFXButton.onClick.AddListener(OnMuteSFXClicked);
    }
    
    private void UpdateUI()
    {
        if (AudioManager.Instance == null) return;
        
        // Update sliders
        if (bgmVolumeSlider != null)
            bgmVolumeSlider.value = AudioManager.Instance.BGMVolume;
            
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = AudioManager.Instance.SFXVolume;
        
        // Update labels
        UpdateVolumeLabels();
        UpdateMuteButtonTexts();
    }
    
    private void OnBGMVolumeChanged(float value)
    {
        AudioManager.Instance?.SetBGMVolume(value);
        UpdateVolumeLabels();
    }
    
    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
        UpdateVolumeLabels();
        
        // Play test sound
        AudioManager.Instance?.PlaySFX("button_click");
    }
    
    private void OnMuteAllClicked()
    {
        AudioManager.Instance?.ToggleMute();
        AudioManager.Instance?.PlaySFX("button_click");
        UpdateMuteButtonTexts();
    }
    
    private void OnMuteBGMClicked()
    {
        AudioManager.Instance?.ToggleBGMMute();
        AudioManager.Instance?.PlaySFX("button_click");
        UpdateMuteButtonTexts();
    }
    
    private void OnMuteSFXClicked()
    {
        AudioManager.Instance?.ToggleSFXMute();
        UpdateMuteButtonTexts();
    }
    
    private void UpdateVolumeLabels()
    {
        if (AudioManager.Instance == null) return;
        
        if (bgmVolumeLabel != null)
            bgmVolumeLabel.text = $"BGM: {(AudioManager.Instance.BGMVolume * 100):F0}%";
            
        if (sfxVolumeLabel != null)
            sfxVolumeLabel.text = $"SFX: {(AudioManager.Instance.SFXVolume * 100):F0}%";
    }
    
    private void UpdateMuteButtonTexts()
    {
        if (AudioManager.Instance == null) return;
        
        if (muteAllText != null)
            muteAllText.text = AudioManager.Instance.IsMuted ? "🔇 Unmute All" : "🔊 Mute All";
            
        if (muteBGMText != null)
            muteBGMText.text = AudioManager.Instance.IsBGMMuted ? "🔇 BGM" : "🎵 BGM";
            
        if (muteSFXText != null)
            muteSFXText.text = AudioManager.Instance.IsSFXMuted ? "🔇 SFX" : "🔊 SFX";
    }
}