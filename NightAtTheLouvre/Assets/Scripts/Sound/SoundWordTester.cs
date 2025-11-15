using UnityEngine;

public class SoundWordTester : MonoBehaviour
{
    SoundWordSource soundWordSource;
    float timer;
    [SerializeField] float interval = 2f;

    void Start()
    {
        soundWordSource = GetComponent<SoundWordSource>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            MakeSound();
            timer -= interval;
        }
    }

    void MakeSound()
    {
        soundWordSource.PlayRandomSound();
    }
}
