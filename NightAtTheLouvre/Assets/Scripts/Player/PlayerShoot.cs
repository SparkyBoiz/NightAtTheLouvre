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

    [Header("Ammo")]
    [Tooltip("Current amount of ammunition.")]
    public int currentAmmo = 20;
    [Tooltip("Maximum amount of ammunition.")]
    public int maxAmmo = 100;

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
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }

        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("Projectile Prefab or Fire Point is not assigned in the Inspector!");
            return;
        }

        currentAmmo--;
        Debug.Log($"Fired! Ammo remaining: {currentAmmo}");

        GameObject projectileGO = Instantiate(
            projectilePrefab, 
            firePoint.position, 
            firePoint.rotation
        );

        Projectile projectileComponent = projectileGO.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            Vector2 launchDirection = firePoint.up; 
            
            projectileComponent.Launch(launchDirection, projectileSpeed);
        }
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        Debug.Log($"Picked up {amount} ammo. Current ammo: {currentAmmo}");
    }
}