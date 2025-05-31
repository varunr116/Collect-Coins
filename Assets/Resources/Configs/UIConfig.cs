using UnityEngine;

[CreateAssetMenu(fileName = "UIConfig", menuName = "Game/UI Configuration")]
public class UIConfig : ScriptableObject
{
    [Header("Button Feedback")]
    [Tooltip("Scale amount for button press animation")]
    [Range(0.8f, 1f)]
    public float buttonPressScale = 0.95f;
    
    [Tooltip("Duration of button press animation")]
    [Range(0.05f, 0.3f)]
    public float buttonPressDuration = 0.1f;
    
    [Header("Score Animation")]
    [Tooltip("Scale multiplier for score pop animation")]
    [Range(1.1f, 2f)]
    public float scorePopScale = 1.2f;
    
    [Tooltip("Duration of score pop animation")]
    [Range(0.1f, 0.5f)]
    public float scorePopDuration = 0.2f;
    
    [Header("Timer Effects")]
    [Tooltip("When to start warning color (seconds remaining)")]
    [Range(5f, 15f)]
    public float timerWarningThreshold = 10f;
    
    [Tooltip("When to start pulsing effect (seconds remaining)")]
    [Range(1f, 10f)]
    public float timerPulseThreshold = 5f;
    
    [Tooltip("Timer pulse scale multiplier")]
    [Range(1.05f, 1.3f)]
    public float timerPulseScale = 1.1f;
    
    [Header("Panel Transitions")]
    [Tooltip("Fade duration for panel transitions")]
    [Range(0.1f, 1f)]
    public float panelFadeDuration = 0.3f;
    
    [Tooltip("Scale for panel entrance animation")]
    [Range(0.5f, 1f)]
    public float panelEntranceScale = 0.8f;
    
    [Header("Loading Screen")]
    [Tooltip("Fake loading duration range")]
    public Vector2 loadingDurationRange = new Vector2(1f, 2f);
    
    [Tooltip("Loading fade speed")]
    [Range(0.2f, 2f)]
    public float loadingFadeSpeed = 0.5f;
}