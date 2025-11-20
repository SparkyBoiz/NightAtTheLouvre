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
    public float stoppingDistance = 0.1f;

    [Header("Patrol Data (Dynamic)")]
    [Tooltip("The radius around the enemy's anchor point to patrol.")]
    public float patrolRadius = 15f; 

    [Header("Stuck Handling")]
    public Transform playerTarget;

    public NavMeshAgent agent { get; private set; }
    private Coroutine stuckCheckCoroutine;
    private Coroutine positionStuckCheckCoroutine;

    public System.Action OnStuck;
    
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

        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * patrolRadius;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                agent.isStopped = false;
                StartStuckCheck();
                StartPositionStuckCheck();
                return;
            }
        }
        StopMoving();
    }

    public void SetDestination(Vector3 destination, float speed)
    {
        if (!agent.isActiveAndEnabled) return;

        agent.speed = speed;

        agent.SetDestination(destination);
        agent.isStopped = false;
        StartStuckCheck();
        StartPositionStuckCheck();
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
            StopPositionStuckCheck();
        }
    }
    
    public bool HasReachedDestination()
    {
        if (!agent.isActiveAndEnabled) return true;

        if (!agent.hasPath || agent.pathPending) return false;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            StopStuckCheck();
            StopPositionStuckCheck();
            return true;
        }

        return false;
    }

    private void StartStuckCheck()
    {
        StopStuckCheck();
        stuckCheckCoroutine = StartCoroutine(CheckIfStuck());
        StartPositionStuckCheck();
    }

    private void StopStuckCheck()
    {
        if (stuckCheckCoroutine != null)
        {
            StopCoroutine(stuckCheckCoroutine);
            stuckCheckCoroutine = null;
        }
    }

    private void StartPositionStuckCheck()
    {
        StopPositionStuckCheck();
        positionStuckCheckCoroutine = StartCoroutine(CheckIfStuckByPosition());
    }

    private void StopPositionStuckCheck()
    {
        if (positionStuckCheckCoroutine != null)
        {
            StopCoroutine(positionStuckCheckCoroutine);
            positionStuckCheckCoroutine = null;
        }
    }

    private IEnumerator CheckIfStuck()
    {
        yield return new WaitForSeconds(2.0f);

        while (agent.hasPath && !agent.pathPending)
        {
            if (agent.velocity.sqrMagnitude < minMoveSpeed * minMoveSpeed && agent.remainingDistance > agent.stoppingDistance)
            {
                OnStuck?.Invoke();
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    private IEnumerator CheckIfStuckByPosition()
    {
        while (agent.hasPath && !agent.pathPending)
        {
            Vector3 lastPosition = transform.position;
            yield return new WaitForSeconds(3.0f);

            if (agent.pathPending || !agent.hasPath || agent.isStopped)
                yield break;

            float distanceMoved = Vector3.Distance(lastPosition, transform.position);
            if (distanceMoved < 0.1f && agent.remainingDistance > agent.stoppingDistance)
            {
                if (playerTarget != null)
                {
                    Debug.Log($"Enemy '{gameObject.name}' is stuck, teleporting towards player.");
                    Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;
                    Vector3 teleportPosition = transform.position + directionToPlayer * 1.0f;

                    if (NavMesh.SamplePosition(teleportPosition, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
                    {
                        agent.Warp(hit.position);
                    }
                }
            }
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