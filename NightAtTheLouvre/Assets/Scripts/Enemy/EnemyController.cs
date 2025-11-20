using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public enum State { Patrol, SeekingTreasure, Fleeing, Dead }
    [Header("State")]
    [Tooltip("The current state of the enemy AI.")]
    public State currentState = State.Patrol;
    public bool hasTreasure = false;

    [Header("Component References")]
    public Transform playerTarget;
    private Transform treasureTarget;
    [Tooltip("The destination the enemy will flee to after picking up treasure.")]
    public Transform fleeDestination;
    private EnemyHealth health;
    private EnemyMovement movement;

    [Header("Detection")]
    [Tooltip("How far the enemy can 'see' treasure.")]
    public float detectionRange = 15f;
    [Tooltip("The angle of the cone of vision for detection.")]
    [Range(0, 360)]
    public float viewAngle = 90f;
    public int rayCount = 10;


    void Awake()
    {
        health = GetComponent<EnemyHealth>();
        movement = GetComponent<EnemyMovement>();

        health.OnDie += OnEnemyDeath;
    }

    void Start()
    {
        StartCoroutine(StartAILogic());
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		agent.updateRotation = false;
		agent.updateUpAxis = false;
    }

    private IEnumerator StartAILogic()
    {
        yield return null; 

        if (currentState == State.Patrol)
        {
            movement.SetRandomPatrolDestination();
        }
    }

    void Update()
    {
        // This log can be very noisy. Uncomment it if you need to see the state every frame.
        // Debug.Log($"Enemy '{gameObject.name}' current state: {currentState}");
        if (currentState == State.Dead) return;

        switch (currentState)
        {
            case State.Patrol:
                if (!hasTreasure)
                {
                    LookForTreasure();
                }

                // Only find a new patrol point if we are still in the Patrol state and have reached the destination.
                if (currentState == State.Patrol && movement.HasReachedDestination())
                {
                    movement.SetRandomPatrolDestination();
                }
                break;
            
            case State.SeekingTreasure:
                // If we have picked up the treasure, go back to patrolling.
                if (hasTreasure)
                {
                    if (fleeDestination != null)
                    {
                        Debug.Log($"Enemy '{gameObject.name}' has treasure, switching to Fleeing state.");
                        currentState = State.Fleeing;
                        movement.SetDestination(fleeDestination.position, movement.fleeSpeed);
                    }
                    else
                    {
                        Debug.LogWarning($"Enemy '{gameObject.name}' has no flee destination set. Returning to Patrol state.");
                        currentState = State.Patrol;
                        movement.SetRandomPatrolDestination();
                    }
                }
                // If the treasure is lost (e.g., picked up by another) before we get there, go back to patrolling.
                else if (treasureTarget == null || !treasureTarget.gameObject.activeInHierarchy)
                {
                    Debug.Log($"Enemy '{gameObject.name}' lost its treasure target, returning to Patrol state.");
                    currentState = State.Patrol;
                    movement.SetRandomPatrolDestination();
                }
                break;

            case State.Fleeing:
                // The EnemyExit script now handles what happens when the enemy reaches the destination.
                break;
        }
    }

    private void LookForTreasure()
    {
        // Create a layer mask that includes everything EXCEPT the "Enemy" layer.
        // This prevents the enemy's raycast from hitting its own collider.
        LayerMask mask = ~LayerMask.GetMask("Enemy");

        float angleStep = viewAngle / rayCount;
        float startingAngle = -viewAngle / 2;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = startingAngle + angleStep * i;
            // Use the enemy's forward vector (transform.up) as the center of our vision cone.
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector2 direction = rotation * transform.up;

            // Cast a ray to look for treasure.
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange, mask);

            // Draw a line in the editor to visualize the raycast for debugging.
            Debug.DrawRay(transform.position, direction * detectionRange, Color.yellow);

            if (hit.collider != null && hit.collider.CompareTag("Treasure"))
            {
                Debug.Log($"Enemy '{gameObject.name}' SAW treasure. Switching to SeekingTreasure state.");
                treasureTarget = hit.transform;
                currentState = State.SeekingTreasure;
                movement.SetDestination(treasureTarget.position);
                return; // Found treasure, no need to cast more rays.
            }
        }
    }


    void OnEnemyDeath()
    {
        currentState = State.Dead;
        movement.StopMoving();

        if (hasTreasure)
        {
            Treasure treasure = GetComponentInChildren<Treasure>();
            if (treasure != null)
            {
                Debug.Log($"Enemy '{gameObject.name}' dropped treasure upon death.");
                treasure.Drop();
            }
        }
    }
}