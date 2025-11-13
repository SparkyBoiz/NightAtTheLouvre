using UnityEngine;
using System.Collections; // Required for Coroutines

public class EnemyController : MonoBehaviour
{
    // --- ENUMERATIONS ---
    public enum State { Patrol, Flee, Dead }
    [Header("State")]
    public State currentState = State.Patrol;

    // --- ASSIGNMENTS ---
    [Header("Component References")]
    public Transform playerTarget; // Must be assigned in the Inspector
    private EnemyHealth health;
    private EnemyMovement movement;

    // --- VISION & FLEEING ---
    [Header("Vision & Fleeing")]
    [Tooltip("Distance at which the enemy detects the player.")]
    public float detectionRadius = 10f;
    [Tooltip("Angle (in degrees) of the enemy's forward view cone.")]
    [Range(0, 360)] public float fieldOfViewAngle = 120f;
    [Tooltip("Distance the enemy runs away to before checking for player again.")]
    public float fleeRange = 15f;


    void Awake()
    {
        health = GetComponent<EnemyHealth>();
        movement = GetComponent<EnemyMovement>();
        
        health.OnDie += OnEnemyDeath;
    }

    void Start()
    {
        // FIX: Use a coroutine to ensure the NavMeshAgent is initialized before the first movement call.
        StartCoroutine(StartAILogic());
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		agent.updateRotation = false;
		agent.updateUpAxis = false;
    }

    private IEnumerator StartAILogic()
    {
        // Wait one frame until the NavMeshAgent is guaranteed to be on the NavMesh
        yield return null; 

        if (currentState == State.Patrol)
        {
            movement.SetRandomPatrolDestination();
        }
    }

    void Update()
    {
        if (currentState == State.Dead) return;

        // --- TRANSITION LOGIC ---
        bool playerSeen = CheckForPlayer();

        if (playerSeen)
        {
            currentState = State.Flee;
        }
        else if (currentState == State.Flee && !playerSeen)
        {
            if (movement.HasReachedDestination())
            {
                currentState = State.Patrol;
                movement.SetRandomPatrolDestination();
            }
        }
        
        // --- BEHAVIOR LOGIC ---
        switch (currentState)
        {
            case State.Patrol:
                if (movement.HasReachedDestination())
                {
                    movement.SetRandomPatrolDestination();
                }
                break;

            case State.Flee:
                movement.FleeFrom(playerTarget, fleeRange);
                break;
        }
    }

    // --- VISION LOGIC ---
    bool CheckForPlayer()
    {
        if (playerTarget == null) return false;

        Vector3 targetDirection = (playerTarget.position - transform.position);
        float distanceToTarget = targetDirection.magnitude;

        // 1. Distance Check
        if (distanceToTarget < detectionRadius)
        {
            // 2. Angle Check (using transform.up as forward for 2D objects)
            float angleToTarget = Vector3.Angle(transform.up, targetDirection); 
            
            if (angleToTarget < fieldOfViewAngle / 2f)
            {
                // 3. Line of Sight Check (Raycast)
                // Use a LayerMask to ignore the enemy's own collider if needed
                RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection, distanceToTarget);
                
                // If the ray hit something AND that something is the player
                if (hit.collider != null && hit.collider.transform == playerTarget)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void OnEnemyDeath()
    {
        currentState = State.Dead;
        movement.StopMoving();
    }
    
    // --- GIZMOS FOR VISUAL DEBUGGING ---
    private void OnDrawGizmosSelected()
    {
        // Draw the Detection Radius (Yellow circle)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        // Draw the Flee Range (Red circle)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fleeRange);

        // Draw the Field of View (FOV) cone (Green lines)
        Vector3 forward = transform.up; 
        float halfAngle = fieldOfViewAngle / 2f;
        
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.forward);
        Quaternion rightRayRotation = Quaternion.AngleAxis(halfAngle, Vector3.forward);

        Vector3 leftDirection = leftRayRotation * forward;
        Vector3 rightDirection = rightRayRotation * forward;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, leftDirection * detectionRadius);
        Gizmos.DrawRay(transform.position, rightDirection * detectionRadius);
    }
}