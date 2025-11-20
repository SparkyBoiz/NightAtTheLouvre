using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 3f;
    [Tooltip("The speed at which the agent moves when fleeing with treasure.")]
    public float fleeSpeed = 5f;
    [Tooltip("The speed at which the agent moves when idling or returning to its anchor.")]
    public float idleSpeed = 2f;
    [Tooltip("The minimum speed for the agent to be considered 'moving'.")]
    public float minMoveSpeed = 0.1f;
    [Tooltip("How close the agent must be to a point to consider it reached.")]
    public float stoppingDistance = 0.1f; // Make sure this is low

    [Header("Patrol Data (Dynamic)")]
    [Tooltip("The radius around the enemy's anchor point to patrol.")]
    public float patrolRadius = 15f; 

    public NavMeshAgent agent { get; private set; }
    private Coroutine stuckCheckCoroutine;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        NavMeshHit hit;
        if (agent != null && NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            transform.position = hit.position; 
        }

        agent.updateRotation = false; 
        agent.updatePosition = true;
        agent.stoppingDistance = stoppingDistance;
        agent.autoBraking = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }
    

    public void SetRandomPatrolDestination()
    {
        if (!agent.isActiveAndEnabled) return;
        
        agent.speed = patrolSpeed;

        for (int i = 0; i < 10; i++) // Try up to 10 times to find a valid point
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * patrolRadius;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                agent.isStopped = false;
                StartStuckCheck();
                return;
            }
        }
        // If we can't find a valid point after 10 tries, stop moving.
        StopMoving();
    }

    public void SetDestination(Vector3 destination, float speed)
    {
        if (!agent.isActiveAndEnabled) return;

        agent.speed = speed;

        agent.SetDestination(destination);
        agent.isStopped = false;
        StartStuckCheck();
    }

    public void SetDestination(Vector3 destination)
    {
        SetDestination(destination, patrolSpeed);
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

        if (!agent.hasPath || agent.pathPending) return false;

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
        yield return new WaitForSeconds(2.0f);

        while (agent.hasPath && !agent.pathPending)
        {
            if (agent.velocity.sqrMagnitude < minMoveSpeed * minMoveSpeed && agent.remainingDistance > agent.stoppingDistance)
            {
                // Instead of just setting a new patrol destination, try recalculating the path to the current destination first.
                // If that fails, then find a new random destination.
                agent.SetDestination(agent.destination);
                yield return new WaitForSeconds(0.5f); // Give it a moment to recalculate
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    public bool IsPathStillValid()
    {
        if (agent.pathPending) return false;
        return agent.pathStatus != NavMeshPathStatus.PathInvalid;
    }

    private void OnDrawGizmosSelected()
    {
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