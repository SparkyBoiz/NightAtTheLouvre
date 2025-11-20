using UnityEngine;

public class PlayerSoundMaker : MonoBehaviour
{
    SoundWordSource soundWordSource;
    PlayerMovement playerMovement;
    [SerializeField] float soundInterval = 0.5f;
    float timeSinceLastSound;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        soundWordSource = GetComponentInChildren<SoundWordSource>();
    }

    void Update()
    {
        if (playerMovement == null || soundWordSource == null)
        {
            return;
        }

        timeSinceLastSound += Time.deltaTime;

        if (playerMovement.IsMoving && timeSinceLastSound >= soundInterval)
        {
            soundWordSource.PlayRandomSound();
            timeSinceLastSound = 0f;
        }
    }
}
