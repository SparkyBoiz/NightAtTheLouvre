using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Tooltip("The amount of ammo this pickup provides.")]
    public int ammoAmount = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerShoot playerShoot = other.GetComponent<PlayerShoot>();
            if (playerShoot != null)
            {
                playerShoot.AddAmmo(ammoAmount);
                
                Destroy(gameObject);
            }
        }
    }
}
