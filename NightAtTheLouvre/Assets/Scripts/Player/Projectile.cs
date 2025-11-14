using UnityEngine;
using System.Linq;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Light2D))]
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [Tooltip("How long the projectile will exist before being destroyed.")]
    public float lifetime = 3f;

    [Header("Illumination Settings")]
    [Tooltip("The intensity of the projectile's light.")]
    public float lightIntensity = 2f;
    [Tooltip("The outer radius of the projectile's light.")]
    public float lightRadius = 4f;

    private Rigidbody2D rb;
    private Light2D projectileLight;
    private float speed;
    private Vector2 direction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projectileLight = GetComponent<Light2D>();

        // Ensure gravity is off for top-down projectiles
        if (rb != null)
        {
            rb.gravityScale = 0f;
        }

        // Configure the light component's properties from the inspector values
        projectileLight.intensity = lightIntensity;
        projectileLight.pointLightOuterRadius = lightRadius;

        // --- FIX ---
        // Ensure the light affects all sorting layers. This is a common reason for lights not appearing.
        // It gets all sorting layer IDs and assigns them to the light's target.
        var layerIds = SortingLayer.layers.Select(l => l.id).ToArray();
        projectileLight.targetSortingLayers = layerIds;
        Debug.Log($"Projectile '{gameObject.name}' light configured to target all sorting layers.");
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