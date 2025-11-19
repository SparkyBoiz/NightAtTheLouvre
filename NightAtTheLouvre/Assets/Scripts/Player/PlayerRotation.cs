using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("The speed at which the player rotates to face the movement direction.")]
    public float rotationSpeed = 10f;
 
    private Vector3 lastPosition;

    void Start()
    {
        // Initialize lastPosition to the starting position
        lastPosition = transform.position;
    }

    void LateUpdate()
    {
        // Calculate velocity based on the change in position since the last frame
        Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;

        // Update lastPosition for the next frame
        lastPosition = transform.position;

        // Rotate the player to face the movement direction
        if (velocity.sqrMagnitude > 0.01f) // Use a small threshold to avoid jitter when standing still
        {
            // We subtract 90 degrees because the sprite might be facing upwards by default
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}