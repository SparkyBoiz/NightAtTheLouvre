using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerLook : MonoBehaviour
{
    private Camera mainCamera;

    void Awake()
    {
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

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        Vector3 lookDirection = mouseWorldPosition - transform.position;

        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

        float correctedAngle = angle - 90f;
        
        transform.rotation = Quaternion.Euler(0f, 0f, correctedAngle);
    }
}