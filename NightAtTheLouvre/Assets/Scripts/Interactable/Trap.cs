using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{
    [Header("Trap Settings")]
    [Tooltip("How long the enemy will be stopped by the trap in seconds.")]
    public float trapDuration = 2.0f;
    [Tooltip("How long the trap remains inactive before resetting.")]
    public float reactivationDelay = 5.0f;

    [Header("Visuals")]
    [Tooltip("Color of the trap when it is armed and ready.")]
    public Color armedColor = Color.white;
    [Tooltip("Color of the trap when it is on cooldown.")]
    public Color disarmedColor = Color.gray;

    private SpriteRenderer spriteRenderer;
    private bool isArmed = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = armedColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isArmed && other.CompareTag("Enemy"))
        {
            EnemyController enemyController = other.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                StartCoroutine(TrapAndReactivate(enemyController));
            }
        }
        else if (isArmed && other.CompareTag("Player"))
        {
            other.GetComponent<TrapPlacer>()?.RegisterPickupableTrap(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isArmed && other.CompareTag("Player"))
        {
            other.GetComponent<TrapPlacer>()?.UnregisterPickupableTrap(this);
        }
    }

    private IEnumerator TrapAndReactivate(EnemyController enemyController)
    {
        isArmed = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = disarmedColor;
        }

        if (enemyController != null)
        {
            // Stop the enemy
            enemyController.canMove = false;
        }

        yield return new WaitForSeconds(trapDuration);

        if (enemyController != null) enemyController.canMove = true;

        yield return new WaitForSeconds(reactivationDelay);
        isArmed = true;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = armedColor;
        }
    }
}
