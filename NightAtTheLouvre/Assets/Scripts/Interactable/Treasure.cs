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

    private void OnTriggerEnter2D(Collider2D other)
    {
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

    private void PickUp(Transform enemy)
    {
        isPickedUp = true;

        transform.SetParent(enemy);

        transform.localPosition = pickupOffset;

        GetComponent<Collider2D>().enabled = false;
    }

    public void Drop()
    {
        transform.SetParent(null);

        GetComponent<Collider2D>().enabled = true;

        isPickedUp = false;
    }

    public static void ResetStolenTreasureCount()
    {
        stolenTreasureCount = 0;
    }

    public static void IncrementStolenTreasureCount()
    {
        stolenTreasureCount++;
    }

    public static int GetStolenTreasureCount() => stolenTreasureCount;
}
