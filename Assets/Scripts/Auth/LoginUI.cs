using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class LoginUI : MonoBehaviour
{
    [Header("Field References")]
    [SerializeField] private TMP_InputField phoneInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button passwordToggleButton;
    [SerializeField] private Image passwordToggleIcon;
    
    [Header("Password Toggle Icons")]
    [SerializeField] private Sprite showPasswordIcon;
    [SerializeField] private Sprite hidePasswordIcon;
    
    [Header("Animation")]
    [SerializeField] private CanvasGroup formCanvasGroup;
    
    private bool isPasswordVisible = false;
    
    void Start()
    {
        InitializeUI();
        PlayIntroAnimation();
    }
    
    void Awake()
    {
        // Wire up buttons
        if (loginButton != null)
            loginButton.onClick.AddListener(HandleLogin);
        else
            Debug.LogError("LoginUI: loginButton not assigned!", this);

        if (passwordToggleButton != null)
            passwordToggleButton.onClick.AddListener(TogglePasswordVisibility);
        else
            Debug.LogError("LoginUI: passwordToggleButton not assigned!", this);

        InitializePasswordField();
        SetupInputFieldAnimations();
    }
    
    private void InitializeUI()
    {
        // Clear error text
        if (errorText != null) 
        {
            errorText.text = "";
            Color color = errorText.color;
            color.a = 0;
            errorText.color = color;
        }
        
        // Initialize form for animation
        if (formCanvasGroup != null)
        {
            formCanvasGroup.alpha = 0;
            formCanvasGroup.transform.localPosition += Vector3.up * 50;
        }
    }
    
    private void PlayIntroAnimation()
    {
        StartCoroutine(FormIntroAnimation());
    }
    
    private IEnumerator FormIntroAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        
        if (formCanvasGroup != null)
        {
            Vector3 startPos = formCanvasGroup.transform.localPosition;
            Vector3 targetPos = startPos - Vector3.up * 50;
            
            float elapsed = 0;
            float duration = 0.6f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                float easedProgress = 1 - Mathf.Pow(1 - progress, 3);
                
                formCanvasGroup.alpha = Mathf.Lerp(0, 1, easedProgress);
                formCanvasGroup.transform.localPosition = Vector3.Lerp(startPos, targetPos, easedProgress);
                
                yield return null;
            }
            
            formCanvasGroup.alpha = 1;
            formCanvasGroup.transform.localPosition = targetPos;
        }
    }
    
    private void InitializePasswordField()
    {
        if (passwordInput != null)
        {
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            passwordInput.ForceLabelUpdate();
        }
        
        UpdatePasswordToggleIcon();
    }
    
    private void SetupInputFieldAnimations()
    {
        // Add focus animations to input fields
        if (phoneInput != null)
        {
            phoneInput.onSelect.AddListener((string value) => AnimateInputFocus(phoneInput.transform));
        }
        
        if (passwordInput != null)
        {
            passwordInput.onSelect.AddListener((string value) => AnimateInputFocus(passwordInput.transform));
        }
    }
    
    private void AnimateInputFocus(Transform inputTransform)
    {
        StartCoroutine(InputFocusAnimation(inputTransform));
    }
    
    private IEnumerator InputFocusAnimation(Transform inputTransform)
    {
        Vector3 originalScale = inputTransform.localScale;
        Vector3 focusScale = originalScale * 1.02f;
        
        float elapsed = 0;
        float duration = 0.2f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            inputTransform.localScale = Vector3.Lerp(originalScale, focusScale, elapsed / duration);
            yield return null;
        }
        
        // Hold briefly then return
        yield return new WaitForSeconds(0.1f);
        
        elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            inputTransform.localScale = Vector3.Lerp(focusScale, originalScale, elapsed / duration);
            yield return null;
        }
        
        inputTransform.localScale = originalScale;
    }
    
    private void TogglePasswordVisibility()
    {
        // Animate button press
        UIManager.Instance?.AnimateButtonPress(passwordToggleButton.transform);
        
        // Play sound
           AudioManager.Instance?.PlaySFX("button_click");
        
        // Toggle visibility
        isPasswordVisible = !isPasswordVisible;
        passwordInput.contentType = isPasswordVisible ? 
            TMP_InputField.ContentType.Standard : 
            TMP_InputField.ContentType.Password;
        passwordInput.ForceLabelUpdate();
        
        // Update icon with animation
        StartCoroutine(AnimateIconChange());
    }
    
    private IEnumerator AnimateIconChange()
    {
        if (passwordToggleIcon != null)
        {
            // Rotate icon during change
            float elapsed = 0;
            float duration = 0.2f;
            Vector3 startRotation = passwordToggleIcon.transform.eulerAngles;
            Vector3 endRotation = startRotation + Vector3.forward * 180;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                passwordToggleIcon.transform.eulerAngles = Vector3.Lerp(startRotation, endRotation, elapsed / duration);
                yield return null;
            }
            
            // Update icon at midpoint
            UpdatePasswordToggleIcon();
            
            // Complete rotation
            elapsed = 0;
            startRotation = endRotation;
            endRotation = startRotation + Vector3.forward * 180;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                passwordToggleIcon.transform.eulerAngles = Vector3.Lerp(startRotation, endRotation, elapsed / duration);
                yield return null;
            }
            
            passwordToggleIcon.transform.eulerAngles = Vector3.zero;
        }
    }
    
    private void UpdatePasswordToggleIcon()
    {
        if (passwordToggleIcon != null)
        {
            passwordToggleIcon.sprite = isPasswordVisible ? hidePasswordIcon : showPasswordIcon;
        }
    }
    
    private void HandleLogin()
    {
        // Animate button press
        UIManager.Instance?.AnimateButtonPress(loginButton.transform);
        
        string phone = phoneInput.text.Trim();
        string pwd = passwordInput.text;

        // Clear previous error
        ClearError();

        // Validation with animated feedback
        if (phone.Length != 10 || !long.TryParse(phone, out _))
        {
            ShowErrorWithAnimation("Please enter a valid 10-digit phone number.");
             AudioManager.Instance?.PlaySFX("error");
            AnimateFieldError(phoneInput.transform);
            return;
        }
        if (string.IsNullOrEmpty(pwd))
        {
            ShowErrorWithAnimation("Password cannot be empty.");
             AudioManager.Instance?.PlaySFX("error");
            AnimateFieldError(passwordInput.transform);
            return;
        }

        // Check credentials
        bool valid = AuthManager.Validate(phone, pwd);
        if (!valid)
        {
            ShowErrorWithAnimation("Invalid phone or password.");
             AudioManager.Instance?.PlaySFX("error");
            AnimateFieldError(phoneInput.transform);
            AnimateFieldError(passwordInput.transform);
            return;
        }

        // Success - add success feedback
         AudioManager.Instance?.PlaySFX("success");
        StartCoroutine(LoginSuccessSequence());
    }
    
    private IEnumerator LoginSuccessSequence()
    {
        // Brief success indication
        if (loginButton != null)
        {
            Image buttonImage = loginButton.GetComponent<Image>();
            Color originalColor = buttonImage.color;
            buttonImage.color = Color.green;
            
            yield return new WaitForSeconds(0.3f);
            
            buttonImage.color = originalColor;
        }
        
        // Load main menu
        SceneLoader.Instance?.LoadMainMenu();
    }
    
    private void ShowErrorWithAnimation(string message)
    {
        if (errorText != null)
        {
            UIManager.Instance?.ShowErrorMessage(errorText, message);
            
            // Subtle shake animation for error text
            StartCoroutine(ShakeTransform(errorText.transform, 0.3f, 5f));
        }
    }
    
    private void ClearError()
    {
        if (errorText != null)
        {
            errorText.text = "";
            Color color = errorText.color;
            color.a = 0;
            errorText.color = color;
        }
    }
    
    private void AnimateFieldError(Transform fieldTransform)
    {
        StartCoroutine(ShakeTransform(fieldTransform, 0.4f, 10f));
    }
    
    private IEnumerator ShakeTransform(Transform target, float duration, float strength)
    {
        Vector3 originalPosition = target.localPosition;
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-1f, 1f) * strength * (1 - elapsed / duration);
            target.localPosition = originalPosition + Vector3.right * x;
            yield return null;
        }
        
        target.localPosition = originalPosition;
    }
}