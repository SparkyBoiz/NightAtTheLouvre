using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("The Projectile prefab to be instantiated.")]
    public GameObject projectilePrefab;
    
    [Tooltip("The point from which the projectile is fired (e.g., the barrel of the gun).")]
    public Transform firePoint;

    [Header("Firing Properties")]
    [Tooltip("How fast the projectile moves.")]
    public float projectileSpeed = 20f;
    
    [Tooltip("Time between shots.")]
    public float fireRate = 0.25f;

    private float nextFireTime = 0f;

    public void OnFire(InputValue value)
    {
        if (value.isPressed && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("Projectile Prefab or Fire Point is not assigned in the Inspector!");
            return;
        }

        // 1. Instantiate the Projectile
        GameObject projectileGO = Instantiate(
            projectilePrefab, 
            firePoint.position, 
            firePoint.rotation // The rotation is set by PlayerLook.cs
        );

        // 2. Pass data to the Projectile script
        Projectile projectileComponent = projectileGO.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            // --- THE FIX IS HERE ---
            // If your sprite is drawn facing UP (and you used angle - 90f in PlayerLook),
            // the local "UP" vector (Y-axis) of the FirePoint is actually the direction 
            // the player is looking.
            Vector2 launchDirection = firePoint.up; 
            
            // If the projectile is still not right, try firePoint.right instead, 
            // but firePoint.up is the most common fix for top-down shooters.
            
            projectileComponent.Launch(launchDirection, projectileSpeed);
        }
    }
}