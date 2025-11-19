using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Tooltip("The amount of ammo this pickup provides.")]
    public int ammoAmount = 20;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerShoot playerShoot = other.GetComponent<PlayerShoot>();
            if (playerShoot != null)
            {
                if (!collected)
                {
                    // Set the flag to true immediately to prevent this block from running again.
                    collected = true;
                    playerShoot.AddAmmo(ammoAmount);

                    // Now destroy the object.
                    Destroy(gameObject);
                }
            }
        }
    }
}
