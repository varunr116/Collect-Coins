using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ButtonFeedback : MonoBehaviour
{
    [Header("Feedback Settings")]
    public float scaleAmount = 0.95f;
    public float scaleDuration = 0.1f;
    public bool useColorFlash = true;
    public bool useScale = true;
    public bool useSound = false; // Set to true if you add AudioSource

    private Vector3 originalScale;
    private Color originalColor;
    private Image buttonImage;
    private AudioSource audioSource;

    void Start()
    {
        originalScale = transform.localScale;
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void OnButtonPressed()
    {
        // AudioManager.Instance?.PlaySFX("button_click");
        if (useScale)
        {
            StartCoroutine(ScaleAnimation());
        }

        if (useColorFlash && buttonImage != null)
        {
            StartCoroutine(ColorFlashAnimation());
        }

        if (useSound && audioSource != null)
        {
            audioSource.Play();
        }

        // Mobile haptic feedback
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Handheld.Vibrate();
        }
    }

    IEnumerator ScaleAnimation()
    {
        Vector3 targetScale = originalScale * scaleAmount;

        // Scale down
        float elapsed = 0;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / scaleDuration);
            yield return null;
        }

        // Scale back up
        elapsed = 0;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / scaleDuration);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    IEnumerator ColorFlashAnimation()
    {
        Color flashColor = Color.white;

        // Flash to white
        float elapsed = 0;
        float flashDuration = 0.05f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            buttonImage.color = Color.Lerp(originalColor, flashColor, elapsed / flashDuration);
            yield return null;
        }

        // Flash back to original
        elapsed = 0;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            buttonImage.color = Color.Lerp(flashColor, originalColor, elapsed / flashDuration);
            yield return null;
        }

        buttonImage.color = originalColor;
    }
}
