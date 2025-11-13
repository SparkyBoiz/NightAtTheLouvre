using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    
    // --- FIX: This is the missing declaration ---
    private int currentHealth; 
    // ---------------------------------------------

    // Public property to check death status from other scripts
    public bool IsDead { get; private set; } = false;

    // Public event to notify the controller when health changes
    public System.Action OnDie; 

    void Awake()
    {
        currentHealth = maxHealth;
    }

    // Public method to be called by projectiles
    public void TakeDamage(int damageAmount)
    {
        if (IsDead) return;
        
        // This is where the error occurred because currentHealth wasn't declared above
        currentHealth -= damageAmount;
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Remaining health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        IsDead = true;
        OnDie?.Invoke(); // Notify subscribed scripts (like the controller)

        // Simple visual/physics death handling
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        
        // Ensure the Rigidbody doesn't interfere if it exists
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.isKinematic = true; 

        Debug.Log(gameObject.name + " has died.");
        
        // Destroy the enemy object after a delay
        Destroy(gameObject, 3f);
    }
    
    // Handles collision with a Projectile using OnCollisionEnter2D (as requested)
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision object has the Projectile component
        Projectile projectile = collision.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            // Assume projectile does 1 damage
            TakeDamage(1); 
            
            // Destroy the projectile after it hits
            Destroy(collision.gameObject);
        }
    }
}