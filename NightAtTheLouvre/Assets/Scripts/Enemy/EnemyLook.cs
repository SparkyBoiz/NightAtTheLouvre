using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyLook : MonoBehaviour
{
    [Header("Look Settings")]
    [Tooltip("How fast the enemy turns to face its movement direction.")]
    public float rotationSpeed = 10f;

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        HandleMovementRotation();
    }

    private void HandleMovementRotation()
    {
        // Only rotate if the agent is actually moving.
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            // Get the direction the agent is moving.
            Vector3 direction = agent.velocity.normalized;

            // Calculate the angle for the 2D top-down view (around the Z-axis).
            // We subtract 90 degrees because Atan2(y,x) treats right as 0 degrees,
            // but in Unity 2D top-down, 'up' is usually considered the forward direction (0 degrees rotation).
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            // Create the target rotation and smoothly interpolate towards it.
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
