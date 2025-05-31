using UnityEngine;

[CreateAssetMenu(fileName = "CoinConfig", menuName = "Game/Coin Configuration")]
public class CoinConfig : ScriptableObject
{
    [Header("Coin Variants")]
    public CoinVariant[] coinVariants = new CoinVariant[]
    {
        new CoinVariant { name = "Bronze", value = 1, color = new Color(0.8f, 0.5f, 0.2f), spawnWeight = 60 },
        new CoinVariant { name = "Silver", value = 2, color = new Color(0.75f, 0.75f, 0.75f), spawnWeight = 30 },
        new CoinVariant { name = "Gold", value = 5, color = new Color(1f, 0.84f, 0f), spawnWeight = 10 }
    };
    
    [Header("Special Coins")]
    [Tooltip("Enable bonus coins with special effects?")]
    public bool enableBonusCoins = false;
    
    [Tooltip("Chance for bonus coin spawn (0-1)")]
    [Range(0f, 0.2f)]
    public float bonusCoinChance = 0.05f;
    
    [Tooltip("Bonus coin multiplier")]
    [Range(2, 20)]
    public int bonusCoinMultiplier = 10;
    
    [Header("Visual Effects")]
    [Tooltip("Should coins have glow effect?")]
    public bool enableCoinGlow = true;
    
    [Tooltip("Glow intensity")]
    [Range(0f, 2f)]
    public float glowIntensity = 1.2f;
    
    [Tooltip("Should coins have trail effect?")]
    public bool enableCoinTrail = false;
}

[System.Serializable]
public class CoinVariant
{
    public string name;
    public int value;
    public Color color;
    [Range(0, 100)]
    public int spawnWeight;
}