using UnityEngine;
using UnityEngine.UI;

public class CoinSpawner : MonoBehaviour
{
    public static CoinSpawner Instance { get; private set; }
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform coinsContainer;
    [SerializeField] private RectTransform spawnArea;
    
    [Header("Spawn Bounds (Screen Percentage)")]
    [SerializeField] private float leftBound = 0.1f;   // 10% from left
    [SerializeField] private float rightBound = 0.9f;  // 90% from left  
    [SerializeField] private float topBound = 0.8f;    // 80% from bottom
    [SerializeField] private float bottomBound = 0.2f; // 20% from bottom
    
    [Header("Coin Variants")]
    [SerializeField] private CoinVariant[] coinVariants;
    
    [System.Serializable]
    public class CoinVariant
    {
        public string name;
        public int value;
        public Color color;
        [Range(0f, 1f)]
        public float spawnChance;
    }
    
    void Awake()
    {
        Instance = this;
        
        // Auto-find containers if not assigned
        if (coinsContainer == null)
        {
            GameObject container = GameObject.Find("CoinsContainer");
            if (container != null)
                coinsContainer = container.transform;
        }
        
        if (spawnArea == null)
        {
            spawnArea = GameObject.Find("Canvas")?.GetComponent<RectTransform>();
        }
    }
    
    public void SpawnCoin()
    {
        if (CoinPool.Instance == null)
        {
            Debug.LogError("CoinSpawner: CoinPool.Instance is null!");
            return;
        }
        
        // Get coin from pool
        GameObject coin = CoinPool.Instance.GetCoin();
        if (coin == null)
        {
            Debug.LogWarning("CoinSpawner: Failed to get coin from pool!");
            return;
        }
        
        // Position coin
        Vector2 spawnPosition = GetRandomSpawnPosition();
        RectTransform coinRect = coin.GetComponent<RectTransform>();
        
        if (coinRect != null)
        {
            coin.transform.SetParent(coinsContainer);
            coinRect.anchoredPosition = spawnPosition;
        }
        
        // Set coin variant (if using variants)
        SetCoinVariant(coin);
        
        Debug.Log($"Spawned coin at position: {spawnPosition}");
    }
    
    private void SetCoinVariant(GameObject coin)
    {
        if (coinVariants == null || coinVariants.Length == 0) return;
        
        // Choose random variant based on spawn chances
        float randomValue = Random.Range(0f, 1f);
        float cumulativeChance = 0f;
        
        foreach (var variant in coinVariants)
        {
            cumulativeChance += variant.spawnChance;
            if (randomValue <= cumulativeChance)
            {
                PooledCoin pooledCoin = coin.GetComponent<PooledCoin>();
                if (pooledCoin != null)
                {
                    // Apply variant properties
                    Image coinImage = coin.GetComponent<Image>();
                    if (coinImage != null)
                    {
                        coinImage.color = variant.color;
                    }
                }
                break;
            }
        }
    }
    
    private Vector2 GetRandomSpawnPosition()
    {
        if (spawnArea == null) return Vector2.zero;
        
        float width = spawnArea.rect.width;
        float height = spawnArea.rect.height;
        
        float x = Random.Range(width * leftBound - width/2, width * rightBound - width/2);
        float y = Random.Range(height * bottomBound - height/2, height * topBound - height/2);
        
        return new Vector2(x, y);
    }
    
    public void ClearAllCoins()
    {
        if (CoinPool.Instance != null)
        {
            CoinPool.Instance.ReturnAllCoins();
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw spawn area in scene view
        if (spawnArea != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = spawnArea.position;
            Vector3 size = new Vector3(
                spawnArea.rect.width * (rightBound - leftBound),
                spawnArea.rect.height * (topBound - bottomBound),
                0
            );
            Gizmos.DrawWireCube(center, size);
        }
    }
}