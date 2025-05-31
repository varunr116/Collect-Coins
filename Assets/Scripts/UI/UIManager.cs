using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingScreenPanel;
    [SerializeField] private Image fadePanel;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    
    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float buttonScaleAmount = 0.95f;
    [SerializeField] private float buttonAnimDuration = 0.1f;
    
    // Loading text variations
    private string[] loadingTexts = { "Loading...", "Please wait...", "Almost there..." };
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeUI();
    }
    
    private void InitializeUI()
    {
        if (loadingScreenPanel != null)
            loadingScreenPanel.SetActive(false);
            
        if (fadePanel != null)
            SetPanelAlpha(0);
            
        if (progressBar != null)
            progressBar.value = 0;
    }
    
    public void ShowLoadingScreen()
    {
        if (loadingScreenPanel != null)
        {
            loadingScreenPanel.SetActive(true);
            progressBar.value = 0;
            StartCoroutine(FadeIn());
            StartCoroutine(AnimateLoadingText());
        }
    }
    
    public void HideLoadingScreen()
    {
        StartCoroutine(FadeOutAndHide());
    }
    
    public void UpdateLoadingProgress(float progress)
    {
        if (progressBar != null)
        {
            StartCoroutine(SmoothProgressUpdate(progress));
        }
    }
    
    private IEnumerator SmoothProgressUpdate(float targetProgress)
    {
        float currentProgress = progressBar.value;
        float elapsed = 0;
        
        while (elapsed < 0.2f)
        {
            elapsed += Time.deltaTime;
            float newProgress = Mathf.Lerp(currentProgress, targetProgress, elapsed / 0.2f);
            progressBar.value = newProgress;
            yield return null;
        }
        
        progressBar.value = targetProgress;
    }
    
    private IEnumerator FadeIn()
    {
        yield return StartCoroutine(Fade(0f, 1f));
    }
    
    private IEnumerator FadeOutAndHide()
    {
        yield return StartCoroutine(Fade(1f, 0f));
        if (loadingScreenPanel != null)
            loadingScreenPanel.SetActive(false);
    }
    
    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            SetPanelAlpha(alpha);
            yield return null;
        }
        
        SetPanelAlpha(to);
    }
    
    private void SetPanelAlpha(float alpha)
    {
        if (fadePanel != null)
        {
            Color color = fadePanel.color;
            color.a = alpha;
            fadePanel.color = color;
        }
    }
    
    private IEnumerator AnimateLoadingText()
    {
        int textIndex = 0;
        while (loadingScreenPanel != null && loadingScreenPanel.activeInHierarchy)
        {
            if (loadingText != null)
            {
                loadingText.text = loadingTexts[textIndex % loadingTexts.Length];
                textIndex++;
            }
            yield return new WaitForSeconds(1f);
        }
    }
    
    // Button animation helpers
    public void AnimateButtonPress(Transform buttonTransform)
    {
        StartCoroutine(ButtonPressAnimation(buttonTransform));
    }
    
    private IEnumerator ButtonPressAnimation(Transform buttonTransform)
    {
        Vector3 originalScale = buttonTransform.localScale;
        Vector3 pressedScale = originalScale * buttonScaleAmount;
        
        // Scale down
        yield return StartCoroutine(ScaleTo(buttonTransform, pressedScale, buttonAnimDuration));
        
        // Scale back up
        yield return StartCoroutine(ScaleTo(buttonTransform, originalScale, buttonAnimDuration));
    }
    
    private IEnumerator ScaleTo(Transform target, Vector3 targetScale, float duration)
    {
        Vector3 startScale = target.localScale;
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            target.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            yield return null;
        }
        
        target.localScale = targetScale;
    }
    
    // Error message animations
    public void ShowErrorMessage(TextMeshProUGUI errorText, string message)
    {
        StartCoroutine(AnimateErrorMessage(errorText, message));
    }
    
    private IEnumerator AnimateErrorMessage(TextMeshProUGUI errorText, string message)
    {
        errorText.text = message;
        
        // Fade in
        Color color = errorText.color;
        color.a = 0;
        errorText.color = color;
        
        float elapsed = 0;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, elapsed / 0.3f);
            errorText.color = color;
            yield return null;
        }
        
        color.a = 1;
        errorText.color = color;
    }
}