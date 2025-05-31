using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Game Configuration")]
public class GameConfig : ScriptableObject
{
    [Header("Game Settings")]
    [Tooltip("Duration of the game in seconds")]
    [Range(10f, 120f)]
    public float gameDuration = 30f;
    
    [Tooltip("Time between coin spawns in seconds")]
    [Range(0.5f, 5f)]
    public float coinSpawnInterval = 1.5f;
    
    [Tooltip("Default points awarded per coin")]
    [Range(1, 10)]
    public int defaultPointsPerCoin = 1;
    
    [Header("Coin Pooling")]
    [Tooltip("Initial number of coins in the pool")]
    [Range(5, 50)]
    public int initialPoolSize = 10;
    
    [Tooltip("Maximum number of coins that can exist")]
    [Range(10, 100)]
    public int maxPoolSize = 20;
    
    [Tooltip("How long coins stay on screen before disappearing")]
    [Range(1f, 10f)]
    public float coinLifetime = 3f;
    
    [Header("Animation Settings")]
    [Tooltip("Duration of coin spawn animation")]
    [Range(0.1f, 1f)]
    public float coinSpawnDuration = 0.3f;
    
    [Tooltip("Duration of coin collection animation")]
    [Range(0.1f, 1f)]
    public float coinCollectDuration = 0.2f;
    
    [Tooltip("Speed of coin rotation")]
    [Range(0f, 360f)]
    public float coinRotationSpeed = 180f;
    
    [Header("Spawn Area")]
    [Tooltip("Spawn boundaries as percentage of screen")]
    [Range(0f, 0.5f)]
    public float leftBoundary = 0.1f;
    
    [Range(0.5f, 1f)]
    public float rightBoundary = 0.9f;
    
    [Range(0f, 0.5f)]
    public float bottomBoundary = 0.2f;
    
    [Range(0.5f, 1f)]
    public float topBoundary = 0.8f;
    
    [Header("Difficulty Progression")]
    [Tooltip("Should spawn rate increase over time?")]
    public bool enableDifficultyProgression = false;
    
    [Tooltip("Minimum spawn interval at maximum difficulty")]
    [Range(0.1f, 2f)]
    public float minSpawnInterval = 0.8f;
    
    [Tooltip("Time to reach maximum difficulty")]
    [Range(5f, 30f)]
    public float difficultyRampTime = 15f;
    
    [Header("Score Multipliers")]
    [Tooltip("Score multiplier for consecutive collections")]
    public bool enableComboSystem = false;
    
    [Range(1.1f, 3f)]
    public float maxComboMultiplier = 2f;
    
    [Range(2, 10)]
    public int coinsForMaxCombo = 5;
    
    [Tooltip("Time window for maintaining combo")]
    [Range(1f, 5f)]
    public float comboTimeWindow = 2f;
}
