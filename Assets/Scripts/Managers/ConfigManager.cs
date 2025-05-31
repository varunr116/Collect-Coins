using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    public static ConfigManager Instance { get; private set; }
    
    [Header("Configuration Assets")]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private UIConfig uiConfig;
    [SerializeField] private AudioConfig audioConfig;
    [SerializeField] private CoinConfig coinConfig;
    
    [Header("Auto-Load Configs")]
    [SerializeField] private bool autoLoadConfigs = true;
    
    // Public properties for easy access
    public GameConfig Game => gameConfig;
    public UIConfig UI => uiConfig;
    public AudioConfig Audio => audioConfig;
    public CoinConfig Coin => coinConfig;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (autoLoadConfigs)
        {
            LoadDefaultConfigs();
        }
        
        ValidateConfigs();
    }
    
    private void LoadDefaultConfigs()
    {
        if (gameConfig == null)
            gameConfig = Resources.Load<GameConfig>("Configs/GameConfig");
            
        if (uiConfig == null)
            uiConfig = Resources.Load<UIConfig>("Configs/UIConfig");
            
        if (audioConfig == null)
            audioConfig = Resources.Load<AudioConfig>("Configs/AudioConfig");
            
        if (coinConfig == null)
            coinConfig = Resources.Load<CoinConfig>("Configs/CoinConfig");
            
        Debug.Log("ConfigManager: Loaded default configurations");
    }
    
    private void ValidateConfigs()
    {
        if (gameConfig == null)
            Debug.LogError("ConfigManager: GameConfig is missing!");
            
        if (uiConfig == null)
            Debug.LogError("ConfigManager: UIConfig is missing!");
            
        if (audioConfig == null)
            Debug.LogError("ConfigManager: AudioConfig is missing!");
            
        if (coinConfig == null)
            Debug.LogError("ConfigManager: CoinConfig is missing!");
    }
    
    public void LoadConfig<T>(T config) where T : ScriptableObject
    {
        switch (config)
        {
            case GameConfig gc:
                gameConfig = gc;
                break;
            case UIConfig uc:
                uiConfig = uc;
                break;
            case AudioConfig ac:
                audioConfig = ac;
                break;
            case CoinConfig cc:
                coinConfig = cc;
                break;
        }
        
        Debug.Log($"ConfigManager: Loaded {typeof(T).Name}");
    }
    
    public void ReloadAllConfigs()
    {
        LoadDefaultConfigs();
        
        // Notify other systems of config changes
        OnConfigsReloaded();
    }
    
    private void OnConfigsReloaded()
    {
        // Update GameManager with new settings
        if (GameManager.Instance != null && gameConfig != null)
        {
            // GameManager will read from ConfigManager.Instance.Game
        }
        
        // Update AudioManager with new settings
        if (AudioManager.Instance != null && audioConfig != null)
        {
            AudioManager.Instance.SetBGMVolume(audioConfig.defaultBGMVolume);
            AudioManager.Instance.SetSFXVolume(audioConfig.defaultSFXVolume);
        }
        
        Debug.Log("ConfigManager: All systems updated with new configurations");
    }
    
    // Utility methods for common config access
    public float GetGameDuration() => gameConfig != null ? gameConfig.gameDuration : 30f;
    public float GetCoinSpawnInterval() => gameConfig != null ? gameConfig.coinSpawnInterval : 1.5f;
    public float GetButtonPressScale() => uiConfig != null ? uiConfig.buttonPressScale : 0.95f;
    
    #if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Create Default Configs")]
    public static void CreateDefaultConfigs()
    {
        // Create configs folder
        string configPath = "Assets/Resources/Configs";
        if (!System.IO.Directory.Exists(configPath))
        {
            System.IO.Directory.CreateDirectory(configPath);
        }
        
        // Create default configs
        CreateConfigAsset<GameConfig>("GameConfig", configPath);
        CreateConfigAsset<UIConfig>("UIConfig", configPath);
        CreateConfigAsset<AudioConfig>("AudioConfig", configPath);
        CreateConfigAsset<CoinConfig>("CoinConfig", configPath);
        
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log("Created default configuration assets in " + configPath);
    }
    
    private static void CreateConfigAsset<T>(string name, string path) where T : ScriptableObject
    {
        string assetPath = $"{path}/{name}.asset";
        if (!System.IO.File.Exists(assetPath))
        {
            T asset = ScriptableObject.CreateInstance<T>();
            UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
        }
    }
    #endif
}