using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController controller = other.GetComponent<EnemyController>();
            if (controller != null && controller.currentState == EnemyController.State.Fleeing && controller.hasTreasure)
            {

                Treasure treasure = other.GetComponentInChildren<Treasure>();
                if (treasure != null)
                {
                    // Check if the game over condition is met.
                    // We check if the current count is one less than the game over threshold.
                    // After this enemy escapes, the count will be incremented, meeting the threshold.
                    if (Treasure.GetStolenTreasureCount() >= treasure.treasuresForGameOver - 1)
                    {
                        SceneManager.LoadScene(treasure.gameOverSceneName);
                    }
                    Treasure.IncrementStolenTreasureCount();
                }

                // Destroy the enemy GameObject. Since the treasure is a child, it will also be destroyed.
                Destroy(other.gameObject);
            }
        }
    }
}
