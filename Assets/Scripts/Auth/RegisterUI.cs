using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class RegisterUI : MonoBehaviour
{
    [Header("Field References")]
    [SerializeField] private TMP_InputField phoneInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField confirmInput;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button backToLoginButton;
    
    [Header("Password Toggle")]
    [SerializeField] private Button passwordToggleButton;
    [SerializeField] private Button confirmToggleButton;
    [SerializeField] private Image passwordToggleIcon;
    [SerializeField] private Image confirmToggleIcon;
    
    [Header("Icons")]
    [SerializeField] private Sprite showPasswordIcon;
    [SerializeField] private Sprite hidePasswordIcon;
    
    [Header("Animation")]
    [SerializeField] private CanvasGroup formCanvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    
    private bool isPasswordVisible = false;
    private bool isConfirmVisible = false;
    
    void Start()
    {
        InitializeUI();
        PlayIntroAnimation();
    }
    
    void Awake()
    {
        // Wire up buttons
        if (registerButton != null)
            registerButton.onClick.AddListener(OnRegisterClicked);
        else
            Debug.LogError("RegisterUI: registerButton not assigned!");

        if (backToLoginButton != null)
            backToLoginButton.onClick.AddListener(OnBackToLoginClicked);

        if (passwordToggleButton != null)
            passwordToggleButton.onClick.AddListener(TogglePasswordVisibility);

        if (confirmToggleButton != null)
            confirmToggleButton.onClick.AddListener(ToggleConfirmVisibility);

        InitializePasswordFields();
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
            formCanvasGroup.transform.localPosition += Vector3.up * 60;
        }
    }
    
    private void PlayIntroAnimation()
    {
        StartCoroutine(RegisterIntroSequence());
    }
    
    private IEnumerator RegisterIntroSequence()
    {
        yield return new WaitForSeconds(0.1f);
        
        // Animate title first
        if (titleText != null)
        {
            StartCoroutine(TypewriterEffect(titleText, "Create Account"));
        }
        
        yield return new WaitForSeconds(0.8f);
        
        // Then animate form
        if (formCanvasGroup != null)
        {
            Vector3 startPos = formCanvasGroup.transform.localPosition;
            Vector3 targetPos = startPos - Vector3.up * 60;
            
            float elapsed = 0;
            float duration = 0.8f;
            
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
    
    private IEnumerator TypewriterEffect(TextMeshProUGUI textComponent, string fullText)
    {
        textComponent.text = "";
        
        for (int i = 0; i <= fullText.Length; i++)
        {
            textComponent.text = fullText.Substring(0, i);
            AudioManager.Instance?.PlaySFX("button_click"); // Subtle typing sound
            yield return new WaitForSeconds(0.06f);
        }
    }
    
    private void InitializePasswordFields()
    {
        if (passwordInput != null)
        {
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            passwordInput.ForceLabelUpdate();
        }
        
        if (confirmInput != null)
        {
            confirmInput.contentType = TMP_InputField.ContentType.Password;
            confirmInput.ForceLabelUpdate();
        }
        
        UpdatePasswordToggleIcons();
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
        
        if (confirmInput != null)
        {
            confirmInput.onSelect.AddListener((string value) => AnimateInputFocus(confirmInput.transform));
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
        UIManager.Instance?.AnimateButtonPress(passwordToggleButton.transform);
        AudioManager.Instance?.PlaySFX("button_click");
        
        isPasswordVisible = !isPasswordVisible;
        passwordInput.contentType = isPasswordVisible ? 
            TMP_InputField.ContentType.Standard : 
            TMP_InputField.ContentType.Password;
        passwordInput.ForceLabelUpdate();
        
        StartCoroutine(AnimateIconChange(passwordToggleIcon, isPasswordVisible));
    }
    
    private void ToggleConfirmVisibility()
    {
        UIManager.Instance?.AnimateButtonPress(confirmToggleButton.transform);
        AudioManager.Instance?.PlaySFX("button_click");
        
        isConfirmVisible = !isConfirmVisible;
        confirmInput.contentType = isConfirmVisible ? 
            TMP_InputField.ContentType.Standard : 
            TMP_InputField.ContentType.Password;
        confirmInput.ForceLabelUpdate();
        
        StartCoroutine(AnimateIconChange(confirmToggleIcon, isConfirmVisible));
    }
    
    private IEnumerator AnimateIconChange(Image iconImage, bool isVisible)
    {
        if (iconImage != null)
        {
            // Rotate icon during change
            float elapsed = 0;
            float duration = 0.2f;
            Vector3 startRotation = iconImage.transform.eulerAngles;
            Vector3 endRotation = startRotation + Vector3.forward * 180;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                iconImage.transform.eulerAngles = Vector3.Lerp(startRotation, endRotation, elapsed / duration);
                yield return null;
            }
            
            // Update icon at midpoint
            iconImage.sprite = isVisible ? hidePasswordIcon : showPasswordIcon;
            
            // Complete rotation
            elapsed = 0;
            startRotation = endRotation;
            endRotation = startRotation + Vector3.forward * 180;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                iconImage.transform.eulerAngles = Vector3.Lerp(startRotation, endRotation, elapsed / duration);
                yield return null;
            }
            
            iconImage.transform.eulerAngles = Vector3.zero;
        }
    }
    
    private void UpdatePasswordToggleIcons()
    {
        if (passwordToggleIcon != null)
        {
            passwordToggleIcon.sprite = isPasswordVisible ? hidePasswordIcon : showPasswordIcon;
        }
        
        if (confirmToggleIcon != null)
        {
            confirmToggleIcon.sprite = isConfirmVisible ? hidePasswordIcon : showPasswordIcon;
        }
    }
    
    private void OnRegisterClicked()
    {
        // Animate button press
        UIManager.Instance?.AnimateButtonPress(registerButton.transform);
        
        string phone = phoneInput.text.Trim();
        string pwd = passwordInput.text;
        string confirm = confirmInput.text;

        // Clear previous errors
        ClearError();

        // Enhanced validation with animations
        if (phone.Length != 10 || !long.TryParse(phone, out _))
        {
            ShowErrorWithAnimation("Enter a valid 10-digit phone number.");
            AnimateFieldError(phoneInput.transform);
            return;
        }
        if (string.IsNullOrEmpty(pwd))
        {
            ShowErrorWithAnimation("Password cannot be empty.");
            AnimateFieldError(passwordInput.transform);
            return;
        }
        if (pwd.Length < 6)
        {
            ShowErrorWithAnimation("Password must be at least 6 characters.");
            AnimateFieldError(passwordInput.transform);
            return;
        }
        if (pwd != confirm)
        {
            ShowErrorWithAnimation("Passwords do not match.");
            AnimateFieldError(passwordInput.transform);
            AnimateFieldError(confirmInput.transform);
            return;
        }

        // Attempt registration
        if (!AuthManager.Register(phone, pwd))
        {
            ShowErrorWithAnimation("This phone number is already registered.");
            AnimateFieldError(phoneInput.transform);
            return;
        }

        // Success sequence
        StartCoroutine(RegistrationSuccessSequence());
    }
    
    private IEnumerator RegistrationSuccessSequence()
    {
        // Show success feedback
        if (registerButton != null)
        {
            Image buttonImage = registerButton.GetComponent<Image>();
            Color originalColor = buttonImage.color;
            buttonImage.color = Color.green;
            
            // Show success message
            if (errorText != null)
            {
                errorText.color = Color.green;
                UIManager.Instance?.ShowErrorMessage(errorText, "Account created successfully!");
            }
            
            yield return new WaitForSeconds(1f);
            
            buttonImage.color = originalColor;
        }
        
        // Navigate to login
        SceneLoader.Instance?.LoadLogin();
    }
    
    private void OnBackToLoginClicked()
    {
        UIManager.Instance?.AnimateButtonPress(backToLoginButton.transform);
        AudioManager.Instance?.PlaySFX("button_click");
        
        StartCoroutine(BackToLoginWithDelay());
    }
    
    private IEnumerator BackToLoginWithDelay()
    {
        yield return new WaitForSeconds(0.2f);
        SceneLoader.Instance?.LoadLogin();
    }
    
    private void ShowErrorWithAnimation(string message)
    {
        if (errorText != null)
        {
            errorText.color = Color.red; // Reset to error color
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