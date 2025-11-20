using UnityEngine;

public class Treasure : MonoBehaviour
{
    private bool isPickedUp = false;

    [Tooltip("The local position offset when attached to the enemy.")]
    public Vector3 pickupOffset = Vector3.zero;

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
}
