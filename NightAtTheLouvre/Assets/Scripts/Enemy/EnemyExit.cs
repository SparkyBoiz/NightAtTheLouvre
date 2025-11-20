using UnityEngine;

public class EnemyExit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController controller = other.GetComponent<EnemyController>();
            if (controller != null && controller.currentState == EnemyController.State.Fleeing && controller.hasTreasure)
            {
                Debug.Log($"Enemy '{other.name}' has escaped with the treasure!");
                
                // Here you can add any game logic for when an enemy escapes, 
                // like incrementing a score or ending the game.
                
                // Destroy the enemy GameObject. Since the treasure is a child, it will also be destroyed.
                Destroy(other.gameObject);
            }
        }
    }
}
