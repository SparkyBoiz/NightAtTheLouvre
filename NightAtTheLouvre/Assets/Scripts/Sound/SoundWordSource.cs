using System.Collections.Generic;
using UnityEngine;

public class SoundWordSource : MonoBehaviour
{
    [SerializeField] Texture2D[] textures;
    [SerializeField] float intensity = 1f;

    public void PlaySound(int index = 0)
    {
        if (textures == null || textures.Length == 0) return;
        
        SoundWordSpawner.Instance.SpawnSoundWord(textures[index], transform.position, intensity);
    }

    public void PlayRandomSound()
    {
        if (textures == null || textures.Length == 0) return;

        int randomIndex = Random.Range(0, textures.Length);
        PlaySound(randomIndex);
    }
}