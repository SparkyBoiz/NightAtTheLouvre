using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("The target GameObject the camera should follow (your Player).")]
    public Transform target;
    
    [Tooltip("How smoothly the camera follows the target (higher is smoother).")]
    public float smoothSpeed = 0.125f;

    // Use LateUpdate to ensure the camera moves AFTER the player has moved in Update/FixedUpdate
    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogError("Camera target is not set!");
            return;
        }

        // Define the target position, maintaining the camera's Z-depth for 2D.
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        // Smoothly interpolate between the camera's current position and the desired position.
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        // Apply the new position
        transform.position = smoothedPosition;
    }
}