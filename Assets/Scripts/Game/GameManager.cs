// ====== FIXED GAMEMANAGER.CS - Proper Pause/Resume ======
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 30f;
    [SerializeField] private float coinSpawnInterval = 1.5f;
    [SerializeField] private int pointsPerCoin = 1;
    
    [Header("Game State")]
    public bool IsGameActive { get; private set; }
    public int CurrentScore { get; private set; }
    public float TimeRemaining { get; private set; }
    
    // Events for UI updates
    public System.Action<int> OnScoreChanged;
    public System.Action<float> OnTimerChanged;
    public System.Action OnGameStarted;
    public System.Action OnGamePaused;
    public System.Action OnGameResumed;
    public System.Action OnGameEnded;
    
    private bool gameStarted = false;
    private bool isPaused = false;  // FIX: Track pause state separately
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    void Start()
    {
        InitializeGame();
    }
    
    private void LoadConfigSettings()
    {
        if (ConfigManager.Instance?.Game != null)
        {
            var config = ConfigManager.Instance.Game;
            gameDuration = config.gameDuration;
            coinSpawnInterval = config.coinSpawnInterval;
            pointsPerCoin = config.defaultPointsPerCoin;
            
            Debug.Log($"GameManager: Loaded settings from config - Duration: {gameDuration}s, Spawn Interval: {coinSpawnInterval}s");
        }
    }
    
    private void InitializeGame()
    {
        LoadConfigSettings();
        
        IsGameActive = false;
        CurrentScore = 0;
        TimeRemaining = gameDuration;
        isPaused = false;  // FIX: Initialize pause state

        Debug.Log("GameManager initialized, waiting for Game scene to start...");
    }
    
    public void StartGame()
    {
        if (gameStarted)
        {
            StopAllCoroutines();
            ClearAllCoins();
        }
        
        IsGameActive = true;
        CurrentScore = 0;
        TimeRemaining = gameDuration;
        gameStarted = true;
        isPaused = false;  // FIX: Reset pause state
        
        Debug.Log($"Game Starting! Duration: {gameDuration}, Time Remaining: {TimeRemaining}");
        
        OnGameStarted?.Invoke();
        OnScoreChanged?.Invoke(CurrentScore);
        OnTimerChanged?.Invoke(TimeRemaining);
        
        StartCoroutine(GameTimer());
        StartCoroutine(CoinSpawning());
    }
    
    public void AddScore(int points = -1)
    {
        if (!IsGameActive || isPaused) return;  // FIX: Check pause state
        
        if (points == -1) points = pointsPerCoin;
        
        CurrentScore += points;
        OnScoreChanged?.Invoke(CurrentScore);
        
        Debug.Log($"Score: {CurrentScore}");
        
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Handheld.Vibrate();
        }
    }
    
    public void PauseGame()
    {
        if (!IsGameActive || isPaused) return;  // FIX: Prevent double pause
        
        isPaused = true;  // FIX: Set pause state
        OnGamePaused?.Invoke();
        Debug.Log("Game Paused");
    }
    
    public void ResumeGame()
    {
        if (!IsGameActive || !isPaused) return;  // FIX: Only resume if paused
        
        isPaused = false;  // FIX: Clear pause state
        OnGameResumed?.Invoke();
        Debug.Log("Game Resumed");
    }
    
    public void RestartGame()
    {
        Debug.Log("Restarting Game");
        StopAllCoroutines();
        ClearAllCoins();
        StartGame();
    }
    
    public void EndGame()
    {
        IsGameActive = false;
        isPaused = false;  // FIX: Clear pause state
        StopAllCoroutines();
        
        ClearAllCoins();
        
        Debug.Log($"Game Ended! Final Score: {CurrentScore}");
        OnGameEnded?.Invoke();
    }
    
    private void ClearAllCoins()
    {
        if (CoinSpawner.Instance != null)
        {
            CoinSpawner.Instance.ClearAllCoins();
        }
        else
        {
            GameObject coinsContainer = GameObject.Find("CoinsContainer");
            if (coinsContainer != null)
            {
                foreach (Transform child in coinsContainer.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
    
    IEnumerator GameTimer()
    {
        Debug.Log("Timer Started");
        
        while (TimeRemaining > 0 && IsGameActive)
        {
            yield return new WaitForSeconds(0.1f);
            
            // FIX: Only update timer when not paused
            if (IsGameActive && !isPaused)
            {
                TimeRemaining -= 0.1f;
                TimeRemaining = Mathf.Max(0, TimeRemaining);
                OnTimerChanged?.Invoke(TimeRemaining);
                
                if (Mathf.FloorToInt(TimeRemaining) != Mathf.FloorToInt(TimeRemaining + 0.1f))
                {
                    Debug.Log($"Timer: {Mathf.FloorToInt(TimeRemaining)} seconds remaining");
                }
            }
        }
        
        if (TimeRemaining <= 0)
        {
            Debug.Log("Timer reached 0 - Ending game");
            EndGame();
        }
    }
    
    IEnumerator CoinSpawning()
    {
        yield return new WaitForSeconds(1f);
        
        Debug.Log("Coin spawning started");
        
        while (IsGameActive && TimeRemaining > 0)
        {
            // FIX: Only spawn coins when not paused
            if (IsGameActive && !isPaused)
            {
                CoinSpawner.Instance?.SpawnCoin();
            }
            
            yield return new WaitForSeconds(coinSpawnInterval);
        }
        
        Debug.Log("Coin spawning stopped");
    }
    
    // FIX: Add getter for pause state
    public bool IsPaused => isPaused;
}