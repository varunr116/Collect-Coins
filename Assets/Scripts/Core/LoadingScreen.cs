using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Image fadePanel;   // full-screen panel Image
    [SerializeField] private Slider progressBar; // slider [0..1]

    [Header("Timing Settings")]
    [Tooltip("Seconds for fade in/out")]
    [SerializeField] private float fadeDuration = 0.5f;
    [Tooltip("Min/max seconds for fake load bar")]
    [SerializeField] private float minFakeTime = 1f, maxFakeTime = 2f;

    void Awake()
    {
        // singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // hide UI
        fadePanel.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);
    }
    private void Start() {
        LoadScene("Entry");
    }

    /// <summary>
    /// Call this to swap to another scene with fade + fake bar.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(DoLoad(sceneName));
    }

    private IEnumerator DoLoad(string sceneName)
    {
        // 1) Show UI
        fadePanel.gameObject.SetActive(true);
       // progressBar.gameObject.SetActive(true);

        // init
        SetPanelAlpha(0);
        //progressBar.value = 0;

        // 2) Fade to opaque
        yield return StartCoroutine(Fade(0f, 1f));

        // 3) Fake loading bar
        float fakeTime = Random.Range(minFakeTime, maxFakeTime);
        float t = 0f;
        while (t < fakeTime)
        {
            t += Time.deltaTime;
            progressBar.value = Mathf.Clamp01(t / fakeTime);
            yield return null;
        }
        progressBar.value = 1f;

        // 4) Load new scene
        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = true;
        while (!op.isDone) yield return null;

        // 5) Fade back to clear
        yield return StartCoroutine(Fade(1f, 0f));

        // 6) Hide UI
        fadePanel.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        Color c = fadePanel.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }

        c.a = to;
        fadePanel.color = c;
    }

    private void SetPanelAlpha(float a)
    {
        var c = fadePanel.color;
        c.a = a;
        fadePanel.color = c;
    }
}
