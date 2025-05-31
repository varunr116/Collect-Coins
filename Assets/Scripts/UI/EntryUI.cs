using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EntryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private CanvasGroup mainCanvasGroup;
    
    [Header("Animation Settings")]
    [SerializeField] private float introAnimDuration = 1f;
    
    void Start()
    {
        InitializeUI();
        PlayIntroAnimation();
    }
    
    void Awake()
    {
        if (loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginClicked);
        }
        else
        {
            Debug.LogError("EntryUI: loginButton not assigned!");
        }
        
        if (registerButton != null)
        {
            registerButton.onClick.AddListener(OnRegisterClicked);
        }
        else
        {
            Debug.LogError("EntryUI: registerButton not assigned!");
        }
    }
    
    private void InitializeUI()
    {
        // Start with UI invisible for intro animation
        if (mainCanvasGroup != null)
        {
            mainCanvasGroup.alpha = 0;
            mainCanvasGroup.transform.localScale = Vector3.one * 0.8f;
        }
    }
    
    private void PlayIntroAnimation()
    {
        StartCoroutine(IntroSequence());
    }
    
    private IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(0.2f); // Brief pause
        
        if (mainCanvasGroup != null)
        {
            // Fade in and scale up simultaneously
            float elapsed = 0;
            while (elapsed < introAnimDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / introAnimDuration;
                
                // Smooth ease-out curve
                float easedProgress = 1 - Mathf.Pow(1 - progress, 3);
                
                mainCanvasGroup.alpha = Mathf.Lerp(0, 1, easedProgress);
                mainCanvasGroup.transform.localScale = Vector3.Lerp(
                    Vector3.one * 0.8f, 
                    Vector3.one, 
                    easedProgress
                );
                
                yield return null;
            }
            
            mainCanvasGroup.alpha = 1;
            mainCanvasGroup.transform.localScale = Vector3.one;
        }
        
        // Add subtle button pulse animation
        if (loginButton != null && registerButton != null)
        {
            StartCoroutine(AlternatePulseButtons());
        }
    }
    
    private IEnumerator AlternatePulseButtons()
    {
        yield return new WaitForSeconds(1f); // Wait after intro
        
        while (gameObject.activeInHierarchy)
        {
            // Pulse login button
            yield return StartCoroutine(ScalePulse(loginButton.transform, 1.05f, 0.8f));
            yield return new WaitForSeconds(2f);
            
            // Pulse register button
            yield return StartCoroutine(ScalePulse(registerButton.transform, 1.05f, 0.8f));
            yield return new WaitForSeconds(2f);
        }
    }
    
    private IEnumerator ScalePulse(Transform target, float scale, float duration)
    {
        Vector3 originalScale = target.localScale;
        Vector3 targetScale = originalScale * scale;
        
        // Scale up
        float elapsed = 0;
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            target.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / (duration / 2));
            yield return null;
        }
        
        // Scale down
        elapsed = 0;
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            target.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / (duration / 2));
            yield return null;
        }
        
        target.localScale = originalScale;
    }
    
    private void OnLoginClicked()
    {
        // Animate button press
        UIManager.Instance?.AnimateButtonPress(loginButton.transform);
        
        // Add loading delay for better UX
        StartCoroutine(LoadLoginWithDelay());
    }
    
    private IEnumerator LoadLoginWithDelay()
    {
        yield return new WaitForSeconds(0.2f); // Wait for button animation
        SceneLoader.Instance?.LoadLogin();
    }
    
    private void OnRegisterClicked()
    {
        // Animate button press
        UIManager.Instance?.AnimateButtonPress(registerButton.transform);
        
        // Add loading delay for better UX
        StartCoroutine(LoadRegisterWithDelay());
    }
    
    private IEnumerator LoadRegisterWithDelay()
    {
        yield return new WaitForSeconds(0.2f); // Wait for button animation
        SceneLoader.Instance?.LoadRegister();
    }
}
