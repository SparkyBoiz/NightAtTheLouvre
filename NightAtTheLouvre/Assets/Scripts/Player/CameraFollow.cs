using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("The target GameObject the camera should follow (your Player).")]
    public Transform target;
    
    [Tooltip("How smoothly the camera follows the target (higher is smoother).")]
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogError("Camera target is not set!");
            return;
        }

        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        transform.position = smoothedPosition;
    }
}