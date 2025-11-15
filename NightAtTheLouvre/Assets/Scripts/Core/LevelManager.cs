using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    protected override bool PersistAcrossScenes => false;
    
    public PlayerController Player {get; private set;}

    protected override void OnSingletonAwake()
    {
        var playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            Debug.LogError("LevelManager could not find a GameObject with tag 'Player'.");
            return;
        }

        Player = playerObject.GetComponent<PlayerController>();
        if (Player == null)
        {
            Debug.LogError("LevelManager found the Player object but it lacks a PlayerController component.");
        }
    }
}
