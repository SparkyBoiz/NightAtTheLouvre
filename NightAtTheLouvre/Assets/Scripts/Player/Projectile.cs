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

        if (rb != null)
        {
            rb.gravityScale = 0f;
        }

        projectileLight.intensity = lightIntensity;
        projectileLight.pointLightOuterRadius = lightRadius;

        var layerIds = SortingLayer.layers.Select(l => l.id).ToArray();
        projectileLight.targetSortingLayers = layerIds;
        Debug.Log($"Projectile '{gameObject.name}' light configured to target all sorting layers.");
    }

    public void Launch(Vector2 launchDirection, float launchSpeed)
    {
        direction = launchDirection.normalized;
        speed = launchSpeed;
        
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            return; // Ignore collision with the player
        }

        Destroy(gameObject);
    }
}