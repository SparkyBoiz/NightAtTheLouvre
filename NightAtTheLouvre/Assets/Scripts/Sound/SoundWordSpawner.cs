using UnityEngine;
using UnityEngine.VFX;

public class SoundWordSpawner : Singleton<SoundWordSpawner>
{
    // protected override bool PersistAcrossScenes => false;

    [SerializeField] float maxHearDistance = 10f;
    VisualEffect soundVfx;
    Camera mainCamera;

    protected override void OnSingletonAwake()
    {
        soundVfx = GetComponent<VisualEffect>();
        mainCamera = Camera.main;
    }

    /// <summary>
    /// Spawns a sound word visual, aligning it to the world position where the sound originates.
    /// </summary>
    /// <param name="texture">Texture used for the spawned word visual.</param>
    /// <param name="position">World-space position indicating the exact location of the sound.</param>
    /// <param name="intensity">The size of sound word.</param>
    /// <param name="lifetime">How long the spawned visual should remain active.</param>
    public void SpawnSoundWord(Texture2D texture, Vector3 position, float intensity=1f, float lifetime=1f)
    {
        Vector3 playerPosition = LevelManager.Instance.Player.transform.position;
        float distance = Vector3.Distance(playerPosition, position);
        if (distance > maxHearDistance) return;

        Vector3 finalPosition = ClampPositionToScreen(position, playerPosition, mainCamera);
        float distancePercent = 1f - Mathf.Clamp01(distance / maxHearDistance);

        soundVfx.SetTexture("Texture", texture);
        soundVfx.SetVector3("Position", finalPosition);
        soundVfx.SetFloat("Lifetime", lifetime);
        soundVfx.SetFloat("Transparency", distancePercent);
        soundVfx.SetFloat("Size", intensity);
        soundVfx.SendEvent("OnSpawn");
    }

    Vector3 ClampPositionToScreen(Vector3 target, Vector3 reference, Camera camera)
    {
        Vector3 screenPoint = camera.WorldToScreenPoint(target);
        if (IsInsideScreen(screenPoint, camera)) return target;

        Vector3 refScreenPoint = camera.WorldToScreenPoint(reference);
        if (!IsInsideScreen(refScreenPoint, camera))
        {
            return target;
        }

        Vector3 direction = target - reference;
        float low = 0f;
        float high = 1f;
        Vector3 result = reference;

        for (int i = 0; i < 20; i++)
        {
            float mid = (low + high) * 0.5f;
            Vector3 sample = reference + direction * mid;
            Vector3 sampleScreen = camera.WorldToScreenPoint(sample);

            if (IsInsideScreen(sampleScreen, camera))
            {
                result = sample;
                low = mid;
            }
            else
            {
                high = mid;
            }
        }

        return result;
    }

    static bool IsInsideScreen(Vector3 screenPoint, Camera camera)
    {
        return screenPoint.z > 0f &&
               screenPoint.x >= 0f && screenPoint.x <= camera.pixelWidth &&
               screenPoint.y >= 0f && screenPoint.y <= camera.pixelHeight;
    }
}
