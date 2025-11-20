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
        movement.playerTarget = playerTarget;

        health.OnDie += OnEnemyDeath;
        movement.OnStuck += OnStuck;
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
        if (currentState == State.Dead) return;

        switch (currentState)
        {
            case State.Patrol:
            
                if (!hasTreasure)
                {
                    LookForTreasure();
                }

                if (currentState == State.Patrol && movement.HasReachedDestination())
                {
                    movement.SetRandomPatrolDestination();
                }
                break;
            
            case State.SeekingTreasure:
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
                else if (treasureTarget == null || !treasureTarget.gameObject.activeInHierarchy)
                {
                    Debug.Log($"Enemy '{gameObject.name}' lost its treasure target, returning to Patrol state.");
                    currentState = State.Patrol;
                    movement.SetRandomPatrolDestination();
                }
                break;

            case State.Fleeing:
                break;
        }
    }

    private void LookForTreasure()
    {
        LayerMask mask = ~LayerMask.GetMask("Enemy");

        float angleStep = viewAngle / rayCount;
        float startingAngle = -viewAngle / 2;

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = startingAngle + angleStep * i;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector2 direction = rotation * transform.up;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange, mask);

            Debug.DrawRay(transform.position, direction * detectionRange, Color.yellow);

            if (hit.collider != null && hit.collider.CompareTag("Treasure"))
            {
                Debug.Log($"Enemy '{gameObject.name}' SAW treasure. Switching to SeekingTreasure state.");
                treasureTarget = hit.transform;
                currentState = State.SeekingTreasure;
                movement.SetDestination(treasureTarget.position);
                return;
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

    void OnStuck()
    {
        if (currentState == State.Patrol)
        {
            Debug.Log($"Enemy '{gameObject.name}' is stuck while patrolling. Finding new patrol point.");
            movement.SetRandomPatrolDestination();
        }
        else
        {
            movement.agent.SetDestination(movement.agent.destination);
        }
    }
}