using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerLook : MonoBehaviour
{
    private Camera mainCamera;

    void Awake()
    {
        // Cache the main camera reference once for efficiency
        mainCamera = Camera.main;
        
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Ensure a camera is tagged as 'MainCamera'.");
        }
    }

    void Update()
    {
        if (mainCamera == null)
        {
            return;
        }

        // 1. Get Mouse Position in Screen Space
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        // 2. Convert Screen Position to World Position
        // Z parameter of 0 is fine for 2D games
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        // 3. Calculate Direction Vector
        Vector3 lookDirection = mouseWorldPosition - transform.position;

        // 4. Calculate Rotation Angle (in degrees)
        // Mathf.Atan2 returns the angle in radians between the positive X-axis and the point (Y, X).
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

        // 5. Apply Rotation with Offset Correction
        
        // --- CHOOSE ONE OPTION BELOW BASED ON YOUR SPRITE'S DEFAULT FACING ---

        // Option 1: If your sprite is drawn to face **RIGHT** by default (standard Unity orientation)
        // transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Option 2 (Most Common Fix): If your sprite is drawn to face **UP** by default (common in 2D top-down assets)
        // You need to subtract 90 degrees to make the top of the sprite face the calculated 'angle'.
        float correctedAngle = angle - 90f;
        
        transform.rotation = Quaternion.Euler(0f, 0f, correctedAngle);
    }
}