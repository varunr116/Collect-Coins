using UnityEngine;

[CreateAssetMenu(fileName = "AudioConfig", menuName = "Game/Audio Configuration")]
public class AudioConfig : ScriptableObject
{
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float defaultBGMVolume = 0.3f;
    
    [Range(0f, 1f)]
    public float defaultSFXVolume = 0.7f;
    
    [Range(0f, 2f)]
    public float masterVolumeMultiplier = 1f;
    
    [Header("Fade Settings")]
    [Tooltip("Duration for BGM crossfade between scenes")]
    [Range(0.5f, 3f)]
    public float bgmFadeDuration = 1f;
    
    [Tooltip("Should BGM pause when game is paused?")]
    public bool pauseBGMWithGame = false;
    
    [Header("Sound Effects")]
    [Tooltip("Pitch variation range for coin collection")]
    public Vector2 coinSFXPitchRange = new Vector2(0.9f, 1.1f);
    
    [Tooltip("Should button sounds have pitch variation?")]
    public bool varyButtonSoundPitch = false;
    
    [Range(0f, 0.2f)]
    public float buttonPitchVariation = 0.1f;
    
    [Header("3D Audio")]
    [Tooltip("Enable positional audio for coins (experimental)")]
    public bool enable3DAudio = false;
    
    [Range(1f, 50f)]
    public float audioRange = 10f;
}