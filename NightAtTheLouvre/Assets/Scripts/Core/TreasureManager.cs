using UnityEngine;
using UnityEngine.SceneManagement;

public class TreasureManager : MonoBehaviour
{
    public static TreasureManager Instance { get; private set; }

    [Tooltip("The name of the scene to load when the treasure steal goal is met.")]
    public string gameOverSceneName = "GameOverScene";

    [Tooltip("The number of treasures that need to be stolen to trigger the game over.")]
    [Min(1)]
    public int treasuresForGameOver = 3;

    private int stolenTreasureCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void IncrementStolenTreasureCount()
    {
        stolenTreasureCount++;
        if (stolenTreasureCount >= treasuresForGameOver)
        {
            SceneManager.LoadScene(gameOverSceneName);
        }
    }

    public void ResetStolenTreasureCount() => stolenTreasureCount = 0;

    public int GetStolenTreasureCount() => stolenTreasureCount;
}