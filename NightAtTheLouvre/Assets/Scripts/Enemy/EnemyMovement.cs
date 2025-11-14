using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 3f;
    [Tooltip("The speed at which the agent moves when idling or returning to its anchor.")]
    public float idleSpeed = 2f;
    [Tooltip("The minimum speed for the agent to be considered 'moving'.")]
    public float minMoveSpeed = 0.1f;
    [Tooltip("How close the agent must be to a point to consider it reached.")]
    public float stoppingDistance = 0.1f; // Make sure this is low

    [Header("Patrol Data (Dynamic)")]
    [Tooltip("The radius around the enemy's anchor point to patrol.")]
    public float patrolRadius = 15f; 

    private NavMeshAgent agent;
    private Coroutine stuckCheckCoroutine;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Snap to NavMesh on start
        NavMeshHit hit;
        if (agent != null && NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            transform.position = hit.position; 
        }

        // Setup Agent
        agent.updateRotation = false; 
        agent.updatePosition = true;
        agent.stoppingDistance = stoppingDistance;
        agent.autoBraking = true; // Recommended

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }
    
    // --- MOVEMENT METHODS ---

    public void SetRandomPatrolDestination()
    {
        if (!agent.isActiveAndEnabled) return;
        
        agent.speed = patrolSpeed;

        Vector3 randomPoint = transform.position + Random.insideUnitSphere * patrolRadius;
        NavMeshHit hit;

        // Find a random point on the NavMesh within the patrol radius
        if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            agent.isStopped = false;
            StartStuckCheck();
            return;
        }

        // Fallback if no point is found, which is unlikely if the anchor is on a NavMesh.
        // This can happen if patrolRadius is very small or the NavMesh is fragmented around the agent.
        Debug.LogWarning("AI Error: Could not find a valid patrol point on the NavMesh. Agent will stop.");
        agent.isStopped = true;
        StopStuckCheck();
    }

    public void StopMoving()
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
            StopStuckCheck();
        }
    }
    
    public bool HasReachedDestination()
    {
        if (!agent.isActiveAndEnabled) return true;

        // If the agent doesn't have a path, it has "reached" it's destination.
        if (!agent.hasPath || agent.pathPending) return false;

        // Check if the agent is close to the destination
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            StopStuckCheck();
            return true;
        }

        return false;
    }

    private void StartStuckCheck()
    {
        StopStuckCheck();
        stuckCheckCoroutine = StartCoroutine(CheckIfStuck());
    }

    private void StopStuckCheck()
    {
        if (stuckCheckCoroutine != null)
        {
            StopCoroutine(stuckCheckCoroutine);
            stuckCheckCoroutine = null;
        }
    }

    private IEnumerator CheckIfStuck()
    {
        yield return new WaitForSeconds(2.0f); // Initial delay

        while (agent.hasPath && !agent.pathPending)
        {
            if (agent.velocity.sqrMagnitude < minMoveSpeed * minMoveSpeed && agent.remainingDistance > agent.stoppingDistance)
            {
                Debug.LogWarning("Agent appears to be stuck. Forcing new destination.");
                SetRandomPatrolDestination();
                yield break; // Exit coroutine
            }
            yield return new WaitForSeconds(1.0f); // Check every second
        }
    }

    public bool IsPathStillValid()
    {
        if (agent.pathPending) return false;
        return agent.pathStatus != NavMeshPathStatus.PathInvalid;
    }

    // --- GIZMOS ---
    private void OnDrawGizmosSelected()
    {
        // Draw the Patrol Radius (Blue circle) around the agent's current position
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
        
        if (agent != null && agent.hasPath && !agent.isStopped)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, agent.destination);
            Gizmos.DrawSphere(agent.destination, 0.25f);
        }
    }
}