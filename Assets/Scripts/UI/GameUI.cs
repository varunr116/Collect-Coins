using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameUI : MonoBehaviour
{
    [Header("Game UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button backToMenuButton;
    
    [Header("Pause Panel References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseRestartButton;
    [SerializeField] private Button pauseMenuButton;
    
    [Header("Game Over Panel References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button gameOverRestartButton;
    [SerializeField] private Button gameOverMenuButton;
    
    [Header("Animation Settings")]
    [SerializeField] private float scorePopScale = 1.2f;
    [SerializeField] private float scorePopDuration = 0.2f;
    
    private bool isPaused = false;
    private int lastDisplayedScore = 0;
    private bool isScoreAnimating = false;  // FIX: Prevent multiple score animations
    private bool isTimerPulsing = false;    // FIX: Prevent multiple timer pulses
    
    void Start()
    {
        SetupButtons();
        SubscribeToGameEvents();
        InitializeUI();
        SetupAndroidBackButton();
        
        // FIX: Start the game when GameUI is ready!
        StartCoroutine(StartGameWhenReady());
    }
    
    // FIX: Start game after UI is fully set up
    IEnumerator StartGameWhenReady()
    {
        // Wait a brief moment for everything to initialize
        yield return new WaitForSeconds(0.1f);
        
        // Now start the actual game
        if (GameManager.Instance != null)
        {
            Debug.Log("GameUI starting the game!");
            GameManager.Instance.StartGame();
        }
    }
    
    void OnDestroy()
    {
        UnsubscribeFromGameEvents();
    }
    
    void SetupButtons()
    {
        // Game UI buttons
        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseClicked);
        else
            Debug.LogError("GameUI: pauseButton not assigned!");
            
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);
        else
            Debug.LogError("GameUI: restartButton not assigned!");
            
        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(OnBackToMenuClicked);
        else
            Debug.LogError("GameUI: backToMenuButton not assigned!");
        
        // Pause panel buttons
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);
        else
            Debug.LogError("GameUI: resumeButton not assigned!");
            
        if (pauseRestartButton != null)
            pauseRestartButton.onClick.AddListener(OnRestartClicked);
        else
            Debug.LogError("GameUI: pauseRestartButton not assigned!");
            
        if (pauseMenuButton != null)
            pauseMenuButton.onClick.AddListener(OnBackToMenuClicked);
        else
            Debug.LogError("GameUI: pauseMenuButton not assigned!");
        
        // Game over panel buttons
        if (gameOverRestartButton != null)
            gameOverRestartButton.onClick.AddListener(OnRestartClicked);
        else
            Debug.LogError("GameUI: gameOverRestartButton not assigned!");
            
        if (gameOverMenuButton != null)
            gameOverMenuButton.onClick.AddListener(OnBackToMenuClicked);
        else
            Debug.LogError("GameUI: gameOverMenuButton not assigned!");
    }
    
    void SubscribeToGameEvents()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged += UpdateScore;
            GameManager.Instance.OnTimerChanged += UpdateTimer;
            GameManager.Instance.OnGameEnded += ShowGameOver;
            GameManager.Instance.OnGameStarted += OnGameStarted;
        }
        else
        {
            Debug.LogError("GameUI: GameManager.Instance is null!");
        }
    }
    
    void UnsubscribeFromGameEvents()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnScoreChanged -= UpdateScore;
            GameManager.Instance.OnTimerChanged -= UpdateTimer;
            GameManager.Instance.OnGameEnded -= ShowGameOver;
            GameManager.Instance.OnGameStarted -= OnGameStarted;
        }
    }
    
    void InitializeUI()
    {
        // Initialize displays
        UpdateScore(0);
        UpdateTimer(30f);
        
        // Hide panels
        if (pausePanel != null)
            pausePanel.SetActive(false);
            
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
    
    void SetupAndroidBackButton()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(HandleAndroidBackButton());
        }
    }
    
    IEnumerator HandleAndroidBackButton()
    {
        while (gameObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    OnResumeClicked();
                }
                else if (GameManager.Instance != null && GameManager.Instance.IsGameActive)
                {
                    OnPauseClicked();
                }
            }
            yield return null;
        }
    }

    void OnGameStarted()
    {
        isPaused = false;
        lastDisplayedScore = 0;
        isScoreAnimating = false;  // FIX: Reset animation flags
        isTimerPulsing = false;
        AudioManager.Instance?.PlaySFX("game_start"); 
    }
    
    void UpdateScore(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + newScore.ToString();
            
            // FIX: Only animate if score actually increased AND not already animating
            if (newScore > lastDisplayedScore && !isScoreAnimating)
            {
                StartCoroutine(ScorePopAnimation());
            }
            
            lastDisplayedScore = newScore;
        }
    }
    
    IEnumerator ScorePopAnimation()
    {
        if (scoreText == null || isScoreAnimating) yield break;
        
        isScoreAnimating = true;  // FIX: Prevent multiple animations
        
        Vector3 originalScale = scoreText.transform.localScale;
        Vector3 popScale = originalScale * scorePopScale;
        
        // Pop out
        float elapsed = 0;
        while (elapsed < scorePopDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (scorePopDuration / 2);
            scoreText.transform.localScale = Vector3.Lerp(originalScale, popScale, t);
            yield return null;
        }
        
        // Pop back
        elapsed = 0;
        while (elapsed < scorePopDuration / 2)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (scorePopDuration / 2);
            scoreText.transform.localScale = Vector3.Lerp(popScale, originalScale, t);
            yield return null;
        }
        
        scoreText.transform.localScale = originalScale;
        isScoreAnimating = false;  // FIX: Reset flag when done
    }
    
    void UpdateTimer(float timeRemaining)
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            
            // Change color based on remaining time
            if (timeRemaining <= 10f)
            {
                // Red warning for last 10 seconds
                float intensity = 1f - (timeRemaining / 10f);
                timerText.color = Color.Lerp(Color.white, Color.red, intensity);
                
                // FIX: Only pulse once per second in last 5 seconds
                if (timeRemaining <= 5f && !isTimerPulsing)
                {
                    StartCoroutine(TimerPulseEffect());
                }
            }
            else
            {
                timerText.color = Color.white;
                isTimerPulsing = false;  // Reset pulse flag when timer > 5
            }
        }
    }
    
    IEnumerator TimerPulseEffect()
    {
        if (timerText == null || isTimerPulsing) yield break;
        
        isTimerPulsing = true;  // FIX: Prevent multiple pulses
        
        Vector3 originalScale = timerText.transform.localScale;
        Vector3 pulseScale = originalScale * 1.1f;
        
        // Quick pulse
        float elapsed = 0;
        float pulseDuration = 0.1f;
        
        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pulseDuration;
            timerText.transform.localScale = Vector3.Lerp(originalScale, pulseScale, t);
            yield return null;
        }
        
        elapsed = 0;
        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / pulseDuration;
            timerText.transform.localScale = Vector3.Lerp(pulseScale, originalScale, t);
            yield return null;
        }
        
        timerText.transform.localScale = originalScale;
        
        // FIX: Wait 1 second before allowing next pulse
        yield return new WaitForSeconds(0.8f);
        isTimerPulsing = false;
    }
    
    void OnPauseClicked()
    {
        Debug.Log("Pause clicked!");
         AudioManager.Instance?.PlaySFX("button_click"); 
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseGame();
            isPaused = true;
            ShowPausePanel();
        }
    }
    
    void OnResumeClicked()
    {
        Debug.Log("Resume clicked!");
        AudioManager.Instance?.PlaySFX("button_click"); 
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
            isPaused = false;
            HidePausePanel();
        }
    }
    
    void OnRestartClicked()
    {
        Debug.Log("Restart clicked!");
        AudioManager.Instance?.PlaySFX("button_click");
        isPaused = false;
        HidePausePanel();
        HideGameOverPanel();
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
    
    void OnBackToMenuClicked()
    {
        Debug.Log("Back to Menu clicked!");
        AudioManager.Instance?.PlaySFX("button_click");
        // Load main menu scene
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadMainMenu();
        }
        else if (LoadingScreen.Instance != null)
        {
            LoadingScreen.Instance.LoadScene("MainMenu");
        }
        else
        {
            Debug.LogWarning("No scene loader found!");
        }
    }
    
    void ShowPausePanel()
    {
        if (pausePanel != null)
        {
            // Check if we have enhanced panel transitions
            if (FindObjectOfType<PanelTransition>() != null)
            {
                PanelTransition.ShowPanel(pausePanel);
            }
            else
            {
                // Fallback - simple activation
                pausePanel.SetActive(true);
            }
        }
    }
    
    void HidePausePanel()
    {
        if (pausePanel != null)
        {
            // Check if we have enhanced panel transitions
            if (FindObjectOfType<PanelTransition>() != null)
            {
                PanelTransition.HidePanel(pausePanel);
            }
            else
            {
                // Fallback - simple deactivation
                pausePanel.SetActive(false);
            }
        }
    }
    
    void ShowGameOver()
    {
        Debug.Log("Game Over!");
       AudioManager.Instance?.PlaySFX("game_over");
        isPaused = false;
        
        if (gameOverPanel != null && finalScoreText != null)
        {
            // Update final score
            if (GameManager.Instance != null)
            {
                finalScoreText.text = "Final Score: " + GameManager.Instance.CurrentScore.ToString();
            }
            
            // Show game over panel
            if (FindObjectOfType<PanelTransition>() != null)
            {
                PanelTransition.ShowPanel(gameOverPanel);
            }
            else
            {
                gameOverPanel.SetActive(true);
            }
        }
    }
    
    void HideGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            if (FindObjectOfType<PanelTransition>() != null)
            {
                PanelTransition.HidePanel(gameOverPanel);
            }
            else
            {
                gameOverPanel.SetActive(false);
            }
        }
    }
    
    // Public methods for button events (alternative to code setup)
    public void PauseButtonClicked()
    {
        OnPauseClicked();
    }
    
    public void ResumeButtonClicked()
    {
        OnResumeClicked();
    }
    
    public void RestartButtonClicked()
    {
        OnRestartClicked();
    }
    
    public void BackToMenuButtonClicked()
    {
        OnBackToMenuClicked();
    }
}