using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent

// Ensures a NavMeshAgent is present on the GameObject
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 3f;
    public float fleeSpeed = 6f;
    [Tooltip("How close the agent must be to a point to consider it reached.")]
    public float stoppingDistance = 0.5f;

    [Header("Patrol Data (Dynamic)")]
    [Tooltip("The radius around the enemy to search for a random patrol destination.")]
    public float patrolRadius = 15f; 

    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // NavMeshAgent Setup for 2D (Crucial Fixes)
        agent.updateRotation = false; // Prevents the sprite from rotating on its X/Y axes
        agent.updatePosition = true;
        agent.stoppingDistance = stoppingDistance;

        // Ensure rotation is locked on the Rigidbody if one is present
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }
    
    // --- PUBLIC MOVEMENT METHODS ---

    /// <summary>
    /// Sets a random destination within the patrolRadius around the enemy's current position, 
    /// using a wide search distance to ensure a NavMesh point is found.
    /// </summary>
    public void SetRandomPatrolDestination()
    {
        agent.isStopped = false;
        agent.speed = patrolSpeed;

        // 1. Get a random point within a sphere around the current position
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        
        // Use a large constant (100f) as the maxDistance argument to robustly search 
        // for a valid point on the NavMesh, fixing the "Could not find valid patrol point" error.
        const float maxSearchDistance = 100f; 

        // 2. Sample the NavMesh to find the nearest valid point to the random position
        if (NavMesh.SamplePosition(randomDirection, out hit, maxSearchDistance, NavMesh.AllAreas))
        {
            // Destination found!
            agent.SetDestination(hit.position);
        }
        else
        {
            // If still no point is found, stop moving and log the failure
            agent.isStopped = true;
            Debug.LogWarning("AI Error: Could not find a valid patrol point on the NavMesh, even with a large search distance.");
        }
    }

    /// <summary>
    /// Calculates a position away from the target and sets the destination on the NavMesh.
    /// </summary>
    public void FleeFrom(Transform target, float fleeDistance)
    {
        if (target == null) return;
        
        agent.isStopped = false;
        agent.speed = fleeSpeed;
        
        // 1. Calculate the direction AWAY from the player
        Vector3 directionToTarget = transform.position - target.position;
        
        // 2. Calculate a position far away in that direction
        Vector3 fleePosition = transform.position + directionToTarget.normalized * fleeDistance;

        NavMeshHit hit;
        // 3. Find the nearest valid point on the NavMesh to the calculated flee position
        if (NavMesh.SamplePosition(fleePosition, out hit, fleeDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    /// <summary>
    /// Stops the NavMeshAgent immediately.
    /// </summary>
    public void StopMoving()
    {
        agent.isStopped = true;
    }
    
    /// <summary>
    /// Checks if the agent has reached its current destination.
    /// </summary>
    public bool HasReachedDestination()
    {
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    // --- GIZMOS FOR VISUAL DEBUGGING ---
    private void OnDrawGizmosSelected()
    {
        // Draw the Patrol Radius (Blue circle)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
        
        // Optionally, draw a line to the current NavMesh destination
        if (agent != null && agent.hasPath && !agent.isStopped)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, agent.destination);
            Gizmos.DrawSphere(agent.destination, 0.25f);
        }
    }
}