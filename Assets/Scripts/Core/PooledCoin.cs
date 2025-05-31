using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PooledCoin : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float spawnDuration = 0.3f;
    [SerializeField] private float collectDuration = 0.2f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float lifetime = 3f;

    [Header("Coin Settings")]
    [SerializeField] private int coinValue = 1;
    [SerializeField] private Color coinColor = Color.yellow;

    private bool isCollected = false;
    private bool isActive = false;
    private Button coinButton;
    private Image coinImage;
    private CoinPool pool;
    private Coroutine lifetimeCoroutine;
    private Coroutine rotationCoroutine;

    void Awake()
    {
        coinButton = GetComponent<Button>();
        coinImage = GetComponent<Image>();

        if (coinButton != null)
            coinButton.onClick.AddListener(CollectCoin);
    }

    public void SetPool(CoinPool coinPool)
    {
        pool = coinPool;
    }

    void OnEnable()
    {
        // Reset state when coin is activated from pool
        ResetCoin();
        StartCoinBehavior();
    }

    void OnDisable()
    {
        // Stop all coroutines when coin is deactivated
        StopAllCoroutines();
    }

    public void ResetCoin()
    {
        isCollected = false;
        isActive = false;

        // Reset visual state
        if (coinImage != null)
        {
            coinImage.color = coinColor;
            Color color = coinImage.color;
            color.a = 1f;
            coinImage.color = color;
        }

        // Reset button
        if (coinButton != null)
        {
            coinButton.interactable = true;
        }

        // Reset transform
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;

        // Stop existing coroutines
        if (lifetimeCoroutine != null)
        {
            StopCoroutine(lifetimeCoroutine);
            lifetimeCoroutine = null;
        }

        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
            rotationCoroutine = null;
        }
    }

    private void StartCoinBehavior()
    {
        isActive = true;

        // Start animations and behavior
        StartCoroutine(SpawnAnimation());
        rotationCoroutine = StartCoroutine(RotateCoin());
        lifetimeCoroutine = StartCoroutine(CoinLifetime());
    }

    IEnumerator SpawnAnimation()
    {
        // Start small and grow
        transform.localScale = Vector3.zero;

        float elapsed = 0;
        while (elapsed < spawnDuration && isActive)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / spawnDuration;
            float easedProgress = 1 - Mathf.Pow(1 - progress, 3);

            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, easedProgress);
            yield return null;
        }

        if (isActive)
        {
            transform.localScale = Vector3.one;
        }
    }

    IEnumerator RotateCoin()
    {
        while (isActive && !isCollected)
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator CoinLifetime()
    {
        yield return new WaitForSeconds(lifetime);

        if (!isCollected && isActive)
        {
            // Coin expired, fade out and return to pool
            yield return StartCoroutine(FadeOutAndReturn());
        }
    }

    IEnumerator FadeOutAndReturn()
    {
        if (coinImage == null) yield break;

        Color startColor = coinImage.color;
        Color endColor = startColor;
        endColor.a = 0;

        float elapsed = 0;
        float fadeDuration = 0.5f;

        while (elapsed < fadeDuration && isActive)
        {
            elapsed += Time.deltaTime;
            coinImage.color = Color.Lerp(startColor, endColor, elapsed / fadeDuration);
            yield return null;
        }

        ReturnToPool();
    }

    public void CollectCoin()
    {
        if (isCollected || !isActive || GameManager.Instance == null || !GameManager.Instance.IsGameActive)
            return;

        isCollected = true;

        // Add score
        GameManager.Instance.AddScore(coinValue);

        // Play sound
        AudioManager.Instance?.PlaySFX("coin_collect");

        // Play collect animation then return to pool
        StartCoroutine(CollectAndReturn());
    }

    IEnumerator CollectAndReturn()
    {
        // Disable button to prevent multiple clicks
        if (coinButton != null)
            coinButton.interactable = false;

        // Play collect animation
        yield return StartCoroutine(CollectAnimation());

        // Return to pool
        ReturnToPool();
    }

    IEnumerator CollectAnimation()
    {
        if (coinImage == null) yield break;

        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 1.5f;
        Color startColor = coinImage.color;
        Color endColor = startColor;
        endColor.a = 0;

        float elapsed = 0;
        while (elapsed < collectDuration && isActive)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / collectDuration;

            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            coinImage.color = Color.Lerp(startColor, endColor, t);

            yield return null;
        }
    }

    private void ReturnToPool()
    {
        isActive = false;

        if (pool != null)
        {
            pool.ReturnCoin(gameObject);
        }
        else
        {
            // Fallback if no pool reference
            Destroy(gameObject);
        }
    }

    // Public getters for debugging
    public bool IsCollected => isCollected;
    public bool IsActive => isActive;
    public int CoinValue => coinValue;
}
