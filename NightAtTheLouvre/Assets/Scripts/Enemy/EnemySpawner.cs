using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("The enemy prefab to spawn.")]
    public GameObject enemyPrefab;

    [Tooltip("An array of points where enemies can be spawned.")]
    public Transform[] spawnPoints;

    [Tooltip("The time in seconds between each spawn attempt.")]
    public float spawnInterval = 5f;

    [Tooltip("The maximum number of enemies allowed to be active at one time.")]
    public int maxEnemies = 10;

    private int currentEnemyCount = 0;

    void Start()
    {
        if (enemyPrefab == null)
        {
            return;
        }

        if (spawnPoints.Length == 0)
        {
            return;
        }

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (currentEnemyCount < maxEnemies)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                currentEnemyCount++;

                enemy.GetComponent<EnemyHealth>().OnDie += () => currentEnemyCount--;
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
