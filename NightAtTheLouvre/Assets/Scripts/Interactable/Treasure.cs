using UnityEngine;

public class Treasure : MonoBehaviour
{
    private bool isPickedUp = false;

    [Tooltip("The local position offset when attached to the enemy.")]
    public Vector3 pickupOffset = Vector3.zero;

    /// <summary>
    /// Called when another collider enters this object's trigger collider.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Treasure '{gameObject.name}' 2D trigger collided with '{other.name}' (Tag: {other.tag})");
        // Check if the treasure has already been picked up and if the colliding object is an enemy.
        if (!isPickedUp && other.CompareTag("Enemy"))
        {
            EnemyController controller = other.GetComponentInParent<EnemyController>();
            if (controller != null && !controller.hasTreasure)
            {
                Debug.Log($"Treasure '{gameObject.name}' is being picked up by '{other.name}'.");
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
}
