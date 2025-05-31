using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ErrorAnimator : MonoBehaviour
{
    public static void ShowError(string errorText, TextMeshProUGUI textComponent)
    {
        if (textComponent == null) return;
        
        // Add animator if not exists
        ErrorAnimator animator = textComponent.GetComponent<ErrorAnimator>();
        if (animator == null)
        {
            animator = textComponent.gameObject.AddComponent<ErrorAnimator>();
        }
        
        animator.DisplayError(errorText);
    }
    
    public void DisplayError(string message)
    {
        TextMeshProUGUI textComponent = GetComponent<TextMeshProUGUI>();
        if (textComponent == null) return;
        
        textComponent.text = message;
        StartCoroutine(ErrorSequence(textComponent));
    }
    
    IEnumerator ErrorSequence(TextMeshProUGUI textComponent)
    {
        // Shake animation
        Vector3 originalPosition = textComponent.transform.localPosition;
        
        for (int i = 0; i < 10; i++)
        {
            float offsetX = Random.Range(-5f, 5f);
            textComponent.transform.localPosition = originalPosition + Vector3.right * offsetX;
            yield return new WaitForSeconds(0.03f);
        }
        
        textComponent.transform.localPosition = originalPosition;
        
        // Fade in
        Color textColor = textComponent.color;
        textColor.a = 0;
        textComponent.color = textColor;
        
        float elapsed = 0;
        float duration = 0.3f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            textColor.a = Mathf.Lerp(0, 1, elapsed / duration);
            textComponent.color = textColor;
            yield return null;
        }
        
        textColor.a = 1;
        textComponent.color = textColor;
    }
}
