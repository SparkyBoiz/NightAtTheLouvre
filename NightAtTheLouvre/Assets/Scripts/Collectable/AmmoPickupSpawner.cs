using UnityEngine;
using System.Collections;

public class AmmoPickupSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("The ammo pickup prefab to spawn.")]
    public GameObject ammoPrefab;

    [Tooltip("An array of points where ammo can be spawned.")]
    public Transform[] spawnPoints;

    [Tooltip("The time in seconds between each spawn attempt.")]
    public float spawnInterval = 15f;

    [Tooltip("The maximum number of ammo pickups allowed to be active at one time.")]
    public int maxPickups = 5;

    private int currentPickupCount = 0;

    void Start()
    {
        if (ammoPrefab == null)
        {
            Debug.LogError("Ammo prefab is not assigned in the AmmoPickupSpawner.");
            return;
        }

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned in the AmmoPickupSpawner.");
            return;
        }

        StartCoroutine(SpawnPickups());
    }

    private IEnumerator SpawnPickups()
    {
        while (true)
        {
            if (currentPickupCount < maxPickups)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                GameObject pickup = Instantiate(ammoPrefab, spawnPoint.position, spawnPoint.rotation);
                currentPickupCount++;

                // We need a way to know when the pickup is destroyed.
                // A simple way is to add a component that notifies us.
                pickup.AddComponent<PickupDestructionNotifier>().spawner = this;
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void OnPickupDestroyed()
    {
        currentPickupCount--;
    }
}

/// <summary>
/// A helper component to notify the spawner when a pickup is destroyed.
/// </summary>
public class PickupDestructionNotifier : MonoBehaviour
{
    public AmmoPickupSpawner spawner;

    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnPickupDestroyed();
        }
    }
}
