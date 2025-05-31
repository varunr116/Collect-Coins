using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string entryScene = "Entry";
    [SerializeField] private string loginScene = "Login";
    [SerializeField] private string registerScene = "Register";
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string gameScene = "GameScene";

    public bool IsLoading { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Load entry scene on start
        LoadSceneWithTransition(entryScene);
    }

    public void LoadEntry() => LoadSceneWithTransition(entryScene);
    public void LoadLogin() => LoadSceneWithTransition(loginScene);
    public void LoadRegister() => LoadSceneWithTransition(registerScene);
    public void LoadMainMenu() => LoadSceneWithTransition(mainMenuScene);
    public void LoadGame() => LoadSceneWithTransition(gameScene);

    public void LoadSceneWithTransition(string sceneName)
    {
        if (IsLoading) return;

        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
{
    IsLoading = true;
    
    // Play button sound
    AudioManager.Instance?.PlaySFX("button_click");
    
    // Trigger loading screen
    UIManager.Instance?.ShowLoadingScreen();
    
    // Wait for loading screen animation
    yield return new WaitForSeconds(0.5f);
    
    // Load scene
    var operation = SceneManager.LoadSceneAsync(sceneName);
    operation.allowSceneActivation = false;
    
    // Fake loading progress for better UX
    float fakeProgress = 0;
    while (fakeProgress < 0.9f)
    {
        fakeProgress += Random.Range(0.1f, 0.3f);
        fakeProgress = Mathf.Clamp01(fakeProgress);
        UIManager.Instance?.UpdateLoadingProgress(fakeProgress);
        yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
    }
    
    // Wait for actual scene load
    while (operation.progress < 0.9f)
    {
        UIManager.Instance?.UpdateLoadingProgress(operation.progress);
        yield return null;
    }
    
    UIManager.Instance?.UpdateLoadingProgress(1f);
    yield return new WaitForSeconds(0.2f);
    
    // Activate scene
    operation.allowSceneActivation = true;
    
    // Wait for scene activation
    while (!operation.isDone)
    {
        yield return null;
    }
    
    // AUDIO: Notify AudioManager of scene change
    AudioManager.Instance?.OnSceneChanged(sceneName);
    
    // Hide loading screen
    UIManager.Instance?.HideLoadingScreen();
    
    IsLoading = false;
}

    
}