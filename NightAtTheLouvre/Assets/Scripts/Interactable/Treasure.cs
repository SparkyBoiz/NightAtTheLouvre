using UnityEngine;
using UnityEngine.SceneManagement;

public class Treasure : MonoBehaviour
{
    private static int stolenTreasureCount = 0;
    private bool isPickedUp = false;

    [Tooltip("The local position offset when attached to the enemy.")]
    public Vector3 pickupOffset = Vector3.zero;

    [Tooltip("The name of the scene to load when 3 treasures are stolen.")]
    public string gameOverSceneName = "GameOverScene";

    [Tooltip("The number of treasures that need to be stolen to trigger the game over.")]
    [Min(1)]
    public int treasuresForGameOver = 3;

    /// <summary>
    /// Called when another collider enters this object's trigger collider.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the treasure has already been picked up and if the colliding object is an enemy.
        if (!isPickedUp && other.CompareTag("Enemy"))
        {
            EnemyController controller = other.GetComponentInParent<EnemyController>();
            if (controller != null && !controller.hasTreasure)
            {
                PickUp(other.transform);
                controller.hasTreasure = true;
            }
        }
    }

    /// <summary>
    /// Handles the logic for when the treasure is picked up by an enemy.
    /// </summary>
    /// <param name="enemy">The transform of the enemy that picked up the treasure.</param>
    private void PickUp(Transform enemy)
    {
        isPickedUp = true;

        // Attach the treasure to the enemy.
        transform.SetParent(enemy);

        // Reset its position relative to the enemy to place it at the enemy's origin.
        transform.localPosition = pickupOffset;

        // Disable the collider to prevent further pickup triggers.
        GetComponent<Collider2D>().enabled = false;
    }

    /// <summary>
    /// Handles the logic for when the treasure is dropped by an enemy.
    /// </summary>
    public void Drop()
    {
        // Un-parent the treasure so it's no longer attached to the dead enemy.
        transform.SetParent(null);

        // Re-enable the collider so it can be picked up again.
        GetComponent<Collider2D>().enabled = true;

        isPickedUp = false;
    }

    /// <summary>
    /// Resets the stolen treasure count, useful for when starting a new game.
    /// </summary>
    public static void ResetStolenTreasureCount()
    {
        stolenTreasureCount = 0;
    }

    /// <summary>
    /// Increments the stolen treasure count.
    /// </summary>
    public static void IncrementStolenTreasureCount()
    {
        stolenTreasureCount++;
    }

    /// <summary>
    /// Gets the current stolen treasure count.
    /// </summary>
    /// <returns>The number of treasures currently considered stolen.</returns>
    public static int GetStolenTreasureCount() => stolenTreasureCount;
}
