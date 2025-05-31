using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class PanelTransition : MonoBehaviour
{
    public static void ShowPanel(GameObject panel, float duration = 0.3f)
    {
        if (panel == null) return;
        
        PanelTransition transition = panel.GetComponent<PanelTransition>();
        if (transition == null)
        {
            transition = panel.AddComponent<PanelTransition>();
        }
        
        transition.Show(duration);
    }
    
    public static void HidePanel(GameObject panel, float duration = 0.3f)
    {
        if (panel == null) return;
        
        PanelTransition transition = panel.GetComponent<PanelTransition>();
        if (transition == null)
        {
            transition = panel.AddComponent<PanelTransition>();
        }
        
        transition.Hide(duration);
    }
    
    public void Show(float duration = 0.3f)
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeIn(duration));
    }
    
    public void Hide(float duration = 0.3f)
    {
        StartCoroutine(FadeOut(duration));
    }
    
    IEnumerator FadeIn(float duration)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        canvasGroup.alpha = 0;
        transform.localScale = Vector3.one * 0.8f;
        
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            canvasGroup.alpha = Mathf.Lerp(0, 1, t);
            transform.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, t);
            
            yield return null;
        }
        
        canvasGroup.alpha = 1;
        transform.localScale = Vector3.one;
    }
    
    IEnumerator FadeOut(float duration)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            canvasGroup.alpha = Mathf.Lerp(1, 0, t);
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.8f, t);
            
            yield return null;
        }
        
        gameObject.SetActive(false);
    }
}