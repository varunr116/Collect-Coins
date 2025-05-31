using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI References - Drag from Hierarchy")]
    [SerializeField] private Button playGameButton;
    [SerializeField] private Button exitAppButton;
    [SerializeField] private GameObject exitConfirmationPanel;
    [SerializeField] private Button confirmExitButton;
    [SerializeField] private Button cancelExitButton;
    [SerializeField] private TextMeshProUGUI welcomeText;
    
    void Start()
    {
        SetupButtons();
        SetupAndroidBackButton();
        PlayWelcomeAnimation();
    }
    
    void SetupButtons()
    {
        // Wire up button clicks
        if (playGameButton != null)
            playGameButton.onClick.AddListener(OnPlayGameClicked);
        else
            Debug.LogError("MainMenuUI: PlayGameButton not assigned!");
            
        if (exitAppButton != null)
            exitAppButton.onClick.AddListener(OnExitAppClicked);
        else
            Debug.LogError("MainMenuUI: ExitAppButton not assigned!");
            
        if (confirmExitButton != null)
            confirmExitButton.onClick.AddListener(OnConfirmExit);
        else
            Debug.LogError("MainMenuUI: ConfirmExitButton not assigned!");
            
        if (cancelExitButton != null)
            cancelExitButton.onClick.AddListener(OnCancelExit);
        else
            Debug.LogError("MainMenuUI: CancelExitButton not assigned!");
    }
    
    void SetupAndroidBackButton()
    {
        // Handle Android back button
        if (Application.platform == RuntimePlatform.Android)
        {
            StartCoroutine(HandleAndroidBackButton());
        }
    }
    
    void PlayWelcomeAnimation()
    {
        // Simple welcome text animation
        if (welcomeText != null)
        {
            StartCoroutine(TypewriterEffect());
        }
    }
    
    IEnumerator TypewriterEffect()
    {
        string fullText = "Welcome Back!";
        welcomeText.text = "";
        
        foreach (char c in fullText)
        {
            welcomeText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
    }
    
    IEnumerator HandleAndroidBackButton()
    {
        while (gameObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (exitConfirmationPanel != null && exitConfirmationPanel.activeInHierarchy)
                {
                    OnCancelExit();
                }
                else
                {
                    OnExitAppClicked();
                }
            }
            yield return null;
        }
    }
    
    void OnPlayGameClicked()
    {
        Debug.Log("Play Game clicked!");
         AudioManager.Instance?.PlaySFX("button_click");
        // Add success feedback if available
        SuccessFeedback.ShowSuccess(playGameButton, () => {
            LoadGameScene();
        });
    }
    
    void LoadGameScene()
    {
        // Try to load game scene
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadGame();
        }
        else if (LoadingScreen.Instance != null)
        {
            LoadingScreen.Instance.LoadScene("GameScene");
        }
        else
        {
            Debug.LogWarning("No scene loader found!");
        }
    }
    
    void OnExitAppClicked()
    {
        Debug.Log("Exit App clicked!");
         AudioManager.Instance?.PlaySFX("button_click");
        ShowExitConfirmation();
    }
    
    void ShowExitConfirmation()
    {
        if (exitConfirmationPanel != null)
        {
            PanelTransition.ShowPanel(exitConfirmationPanel);
        }
        else
        {
            // No confirmation panel - direct exit
            QuitApplication();
        }
    }
    
    void OnConfirmExit()
    {
        Debug.Log("Confirmed exit!");

         AudioManager.Instance?.PlaySFX("button_click"); 
        
        SuccessFeedback.ShowSuccess(confirmExitButton, () => {
            QuitApplication();
        });
    }
    
    void QuitApplication()
    {
        Debug.Log("Quitting application...");
        
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    void OnCancelExit()
    {
        Debug.Log("Cancelled exit!");
        
        AudioManager.Instance?.PlaySFX("button_click");  //
        if (exitConfirmationPanel != null)
        {
            PanelTransition.HidePanel(exitConfirmationPanel);
        }
    }
}