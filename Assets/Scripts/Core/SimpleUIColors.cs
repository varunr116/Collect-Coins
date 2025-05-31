// ====== ENHANCED SIMPLEUI WITH FEEDBACK ======
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SimpleUIColors : MonoBehaviour
{
    void Start()
    {
        ApplyColors();
        SetupInputFields();
        SetupButtonFeedback();
    }
    
    void ApplyColors()
    {
        Camera.main.backgroundColor = new Color32(30, 30, 30, 255);
        
        ColorPanel("MainPanel", new Color32(50, 50, 50, 255));
        ColorPanel("FormPanel", new Color32(50, 50, 50, 255));
        ColorPanel("PausePanel", new Color32(40, 40, 40, 255));
        ColorPanel("GameOverPanel", new Color32(40, 40, 40, 255));
        
        ColorButton("LoginButton", new Color32(70, 130, 200, 255));
        ColorButton("RegisterButton", new Color32(100, 180, 50, 255));
        ColorButton("PlayGameButton", new Color32(100, 180, 50, 255));
        ColorButton("ExitAppButton", new Color32(80, 80, 80, 255));
        ColorButton("PauseButton", new Color32(70, 130, 200, 255));
        ColorButton("RestartButton", new Color32(100, 180, 50, 255));
        ColorButton("BackToMenuButton", new Color32(80, 80, 80, 255));
        
        ColorText("TitleText", Color.white);
        ColorText("WelcomeText", Color.white);
        ColorText("ScoreText", Color.white);
        ColorText("TimerText", Color.white);
        ColorText("ErrorText", Color.red);
    }
    
    void SetupInputFields()
    {
        SetupInputField("PhoneInput");
        SetupInputField("PasswordInput");
        SetupInputField("ConfirmInput");
    }
    
    void SetupInputField(string name)
    {
        GameObject obj = GameObject.Find(name);
        if (obj == null) return;
        
        TMP_InputField inputField = obj.GetComponent<TMP_InputField>();
        if (inputField == null) return;
        
        Image backgroundImage = inputField.GetComponent<Image>();
        if (backgroundImage != null)
        {
            backgroundImage.color = new Color32(60, 60, 60, 255);
        }
        
        if (inputField.textComponent != null)
        {
            inputField.textComponent.color = Color.white;
        }
        
        if (inputField.placeholder != null)
        {
            TextMeshProUGUI placeholder = inputField.placeholder.GetComponent<TextMeshProUGUI>();
            if (placeholder != null)
            {
                placeholder.color = new Color32(120, 120, 120, 255);
            }
        }
        
        inputField.selectionColor = new Color32(70, 130, 200, 100);
        inputField.caretColor = new Color32(70, 130, 200, 255);
        
        // Enhanced focus effects with animation
        inputField.onSelect.AddListener((string value) => StartCoroutine(InputFocusAnimation(inputField, true)));
        inputField.onDeselect.AddListener((string value) => StartCoroutine(InputFocusAnimation(inputField, false)));
    }
    
    IEnumerator InputFocusAnimation(TMP_InputField inputField, bool focused)
    {
        Image background = inputField.GetComponent<Image>();
        if (background == null) yield break;
        
        Color startColor = background.color;
        Color targetColor = focused ? new Color32(70, 70, 80, 255) : new Color32(60, 60, 60, 255);
        Vector3 startScale = inputField.transform.localScale;
        Vector3 targetScale = focused ? startScale * 1.02f : Vector3.one;
        
        float duration = 0.2f;
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            background.color = Color.Lerp(startColor, targetColor, t);
            inputField.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            
            yield return null;
        }
        
        background.color = targetColor;
        inputField.transform.localScale = targetScale;
    }
    
    void SetupButtonFeedback()
    {
        // Add feedback to all buttons
        AddButtonFeedback("LoginButton");
        AddButtonFeedback("RegisterButton");
        AddButtonFeedback("PlayGameButton");
        AddButtonFeedback("ExitAppButton");
        AddButtonFeedback("PauseButton");
        AddButtonFeedback("RestartButton");
        AddButtonFeedback("BackToMenuButton");
        AddButtonFeedback("PasswordToggleButton");
        AddButtonFeedback("ConfirmToggleButton");
    }
    
    void AddButtonFeedback(string buttonName)
    {
        GameObject obj = GameObject.Find(buttonName);
        if (obj == null) return;
        
        Button button = obj.GetComponent<Button>();
        if (button == null) return;
        
        // Add or get the feedback component
        ButtonFeedback feedback = obj.GetComponent<ButtonFeedback>();
        if (feedback == null)
        {
            feedback = obj.AddComponent<ButtonFeedback>();
        }
        
        // Wire up the button click
        button.onClick.AddListener(() => feedback.OnButtonPressed());
    }
    
    void ColorPanel(string name, Color color)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            Image img = obj.GetComponent<Image>();
            if (img != null) img.color = color;
        }
    }
    
    void ColorButton(string name, Color color)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            Button btn = obj.GetComponent<Button>();
            if (btn != null)
            {
                ColorBlock colors = btn.colors;
                colors.normalColor = color;
                colors.highlightedColor = color * 0.8f;
                colors.pressedColor = color * 0.6f;
                btn.colors = colors;
            }
        }
    }
    
    void ColorText(string name, Color color)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            TextMeshProUGUI text = obj.GetComponent<TextMeshProUGUI>();
            if (text != null) text.color = color;
        }
    }
}


