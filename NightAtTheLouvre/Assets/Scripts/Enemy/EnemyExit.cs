using UnityEngine;

public class EnemyExit : MonoBehaviour
{
    public System.Action OnExit;

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
                    TreasureManager.Instance.IncrementStolenTreasureCount();
                    OnExit?.Invoke();
                    Destroy(other.gameObject);
                }
            }
        }
    }
}
