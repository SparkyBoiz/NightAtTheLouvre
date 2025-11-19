using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    public enum State { Patrol, Dead }
    [Header("State")]
    [Tooltip("The current state of the enemy AI.")]
    public State currentState = State.Patrol;

    [Header("Component References")]
    public Transform playerTarget;
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
        Debug.Log($"Enemy '{gameObject.name}' current state: {currentState}");
        if (currentState == State.Dead) return;

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