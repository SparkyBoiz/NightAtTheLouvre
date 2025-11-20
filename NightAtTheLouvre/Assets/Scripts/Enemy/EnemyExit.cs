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
                    if (Treasure.GetStolenTreasureCount() >= treasure.treasuresForGameOver - 1)
                    {
                        SceneManager.LoadScene(treasure.gameOverSceneName);
                    }

                    Treasure.IncrementStolenTreasureCount();
                }

                Destroy(other.gameObject);
            }
        }
    }
}
