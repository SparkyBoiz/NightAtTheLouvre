using UnityEngine;

public class EnemySoundMakder : MonoBehaviour
{
    SoundWordSource soundWordSource;
    EnemyMovement enemyMovement;
    [SerializeField] float soundInterval = 0.5f;
    float timeSinceLastSound;

    void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        soundWordSource = GetComponentInChildren<SoundWordSource>();
    }

    void Update()
    {
        if (enemyMovement == null || soundWordSource == null)
        {
            return;
        }

        timeSinceLastSound += Time.deltaTime;

        if (enemyMovement.IsMoving && timeSinceLastSound >= soundInterval)
        {
            soundWordSource.PlayRandomSound();
            timeSinceLastSound = 0f;
        }
    }
}
