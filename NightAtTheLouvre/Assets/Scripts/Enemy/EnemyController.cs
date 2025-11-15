using UnityEngine;
using System.Collections; // Required for Coroutines

public class EnemyController : MonoBehaviour
{
    // --- ENUMERATIONS ---
    public enum State { Patrol, Dead }
    [Header("State")]
    [Tooltip("The current state of the enemy AI.")]
    public State currentState = State.Patrol;

    // --- ASSIGNMENTS ---
    [Header("Component References")]
    public Transform playerTarget; // Must be assigned in the Inspector
    private EnemyHealth health;
    private EnemyMovement movement;


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
        Debug.Log($"Enemy '{gameObject.name}' current state: {currentState}");
        if (currentState == State.Dead) return;

        // --- BEHAVIOR LOGIC ---
        switch (currentState)
        {
            case State.Patrol:
                if (movement.HasReachedDestination())
                {
                    Debug.Log($"Enemy '{gameObject.name}' reached patrol point. Finding new patrol destination.");
                    movement.SetRandomPatrolDestination();
                }
                break;
        }
    }

    void OnEnemyDeath()
    {
        Debug.Log($"Enemy '{gameObject.name}' has died. Transitioning from {currentState} to Dead.");
        currentState = State.Dead;
        movement.StopMoving();
    }
}