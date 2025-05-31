using UnityEngine;
using System.Collections.Generic;

public class CoinPool : MonoBehaviour
{
    public static CoinPool Instance { get; private set; }
    
    [Header("Pool Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private int maxPoolSize = 20;
    [SerializeField] private Transform poolParent;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    private Queue<GameObject> coinPool = new Queue<GameObject>();
    private List<GameObject> activeCoinsList = new List<GameObject>();
    private int totalCoinsCreated = 0;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    void Start()
    {
        InitializePool();
    }
    
    private void InitializePool()
    {
        if (coinPrefab == null)
        {
            Debug.LogError("CoinPool: coinPrefab is not assigned!");
            return;
        }
        
        // Create pool parent if not assigned
        if (poolParent == null)
        {
            GameObject poolParentObj = new GameObject("CoinPool");
            poolParent = poolParentObj.transform;
            poolParent.SetParent(transform);
        }
        
        // Pre-instantiate coins
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewCoin();
        }
        
        Debug.Log($"CoinPool initialized with {poolSize} coins");
    }
    
    private GameObject CreateNewCoin()
    {
        GameObject newCoin = Instantiate(coinPrefab, poolParent);
        newCoin.SetActive(false);
        
        // Setup coin for pooling
        PooledCoin pooledCoin = newCoin.GetComponent<PooledCoin>();
        if (pooledCoin == null)
        {
            pooledCoin = newCoin.AddComponent<PooledCoin>();
        }
        pooledCoin.SetPool(this);
        
        coinPool.Enqueue(newCoin);
        totalCoinsCreated++;
        
        if (showDebugInfo)
        {
            Debug.Log($"Created new coin. Total created: {totalCoinsCreated}");
        }
        
        return newCoin;
    }
    
    public GameObject GetCoin()
    {
        GameObject coin;
        
        // Try to get coin from pool
        if (coinPool.Count > 0)
        {
            coin = coinPool.Dequeue();
        }
        else if (totalCoinsCreated < maxPoolSize)
        {
            // Create new coin if under max limit
            coin = CreateNewCoin();
            coinPool.Dequeue(); // Remove from queue since we're using it
        }
        else
        {
            // Pool exhausted, reuse oldest active coin
            if (activeCoinsList.Count > 0)
            {
                coin = activeCoinsList[0];
                ReturnCoin(coin); // Return it first
                coin = coinPool.Dequeue(); // Then get it back
                
                if (showDebugInfo)
                {
                    Debug.LogWarning("Pool exhausted! Recycling oldest coin.");
                }
            }
            else
            {
                Debug.LogError("CoinPool: No coins available and cannot create more!");
                return null;
            }
        }
        
        // Activate and track coin
        coin.SetActive(true);
        activeCoinsList.Add(coin);
        
        if (showDebugInfo)
        {
            Debug.Log($"Got coin from pool. Active: {activeCoinsList.Count}, Pool: {coinPool.Count}");
        }
        
        return coin;
    }
    
    public void ReturnCoin(GameObject coin)
    {
        if (coin == null) return;
        
        // Remove from active list
        activeCoinsList.Remove(coin);
        
        // Reset coin state
        coin.SetActive(false);
        coin.transform.SetParent(poolParent);
        coin.transform.localPosition = Vector3.zero;
        coin.transform.localRotation = Quaternion.identity;
        coin.transform.localScale = Vector3.one;
        
        // Reset coin component
        PooledCoin pooledCoin = coin.GetComponent<PooledCoin>();
        if (pooledCoin != null)
        {
            pooledCoin.ResetCoin();
        }
        
        // Return to pool
        coinPool.Enqueue(coin);
        
        if (showDebugInfo)
        {
            Debug.Log($"Returned coin to pool. Active: {activeCoinsList.Count}, Pool: {coinPool.Count}");
        }
    }
    
    public void ReturnAllCoins()
    {
        // Return all active coins to pool
        for (int i = activeCoinsList.Count - 1; i >= 0; i--)
        {
            ReturnCoin(activeCoinsList[i]);
        }
        
        Debug.Log("Returned all coins to pool");
    }
    
    public void ClearPool()
    {
        // Return all active coins
        ReturnAllCoins();
        
        // Destroy all pooled coins
        while (coinPool.Count > 0)
        {
            GameObject coin = coinPool.Dequeue();
            if (coin != null)
            {
                Destroy(coin);
            }
        }
        
        totalCoinsCreated = 0;
        Debug.Log("Pool cleared and reset");
    }
    
    // Debug information
    public int ActiveCoinsCount => activeCoinsList.Count;
    public int PooledCoinsCount => coinPool.Count;
    public int TotalCoinsCreated => totalCoinsCreated;
    
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 200, 100));
        GUILayout.Label($"Active Coins: {ActiveCoinsCount}");
        GUILayout.Label($"Pooled Coins: {PooledCoinsCount}");
        GUILayout.Label($"Total Created: {TotalCoinsCreated}");
        GUILayout.EndArea();
    }
}