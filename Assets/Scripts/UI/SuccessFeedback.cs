using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SuccessFeedback : MonoBehaviour
{
    public static void ShowSuccess(Button button, System.Action onComplete = null)
    {
        if (button == null) return;
        
        SuccessFeedback feedback = button.GetComponent<SuccessFeedback>();
        if (feedback == null)
        {
            feedback = button.gameObject.AddComponent<SuccessFeedback>();
        }
        
        feedback.PlaySuccessAnimation(onComplete);
    }
    
    public void PlaySuccessAnimation(System.Action onComplete = null)
    {
        StartCoroutine(SuccessSequence(onComplete));
    }
    
    IEnumerator SuccessSequence(System.Action onComplete)
    {
        Image buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            Color originalColor = buttonImage.color;
            Color successColor = Color.green;
            
            // Flash green
            float elapsed = 0;
            float duration = 0.3f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                buttonImage.color = Color.Lerp(originalColor, successColor, elapsed / duration);
                yield return null;
            }
            
            yield return new WaitForSeconds(0.2f);
            
            // Back to original
            elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                buttonImage.color = Color.Lerp(successColor, originalColor, elapsed / duration);
                yield return null;
            }
            
            buttonImage.color = originalColor;
        }
        
        onComplete?.Invoke();
    }
}
