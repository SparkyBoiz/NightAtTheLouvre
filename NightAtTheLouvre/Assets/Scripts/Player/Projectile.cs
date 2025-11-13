using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Tooltip("How long the projectile will exist before being destroyed.")]
    public float lifetime = 3f;
    
    private Rigidbody2D rb;
    private float speed;
    private Vector2 direction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Ensure gravity is off for top-down projectiles
        if (rb != null)
        {
            rb.gravityScale = 0f;
        }
    }

    // Called by PlayerShoot.cs to set the initial velocity
    public void Launch(Vector2 launchDirection, float launchSpeed)
    {
        direction = launchDirection.normalized;
        speed = launchSpeed;
        
        // Start the self-destruct timer
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            // Apply constant velocity in the specified direction
            rb.linearVelocity = direction * speed;
        }
    }

    // Handle collision with other objects
    void OnTriggerEnter2D(Collider2D other)
    {
        // Optionally add logic here for what the projectile hits (e.g., ApplyDamage)

        // For now, just destroy the projectile on collision
        Destroy(gameObject);
    }
}