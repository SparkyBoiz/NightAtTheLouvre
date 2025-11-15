using UnityEngine;

/// <summary>
/// Generic singleton base that derived MonoBehaviours can inherit to gain a shared instance.
/// </summary>
/// <typeparam name="T">Concrete type implementing the singleton.</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    static T instance;
    bool initialized;

    /// <summary>
    /// Singleton instance. Null if no active instance exists.
    /// </summary>
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();
            }
            return instance;
        }
    }

    /// <summary>
    /// Override to prevent persistence across scene loads.
    /// </summary>
    protected virtual bool PersistAcrossScenes => true;

    /// <summary>
    /// Hook for derived classes to run initialization when the singleton is created.
    /// </summary>
    protected virtual void OnSingletonAwake() { }

    /// <summary>
    /// Destroy duplicate instances and initialize the singleton on first Awake.
    /// </summary>
    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = (T)this;

        if (PersistAcrossScenes)
        {
            DontDestroyOnLoad(gameObject);
        }

        if (!initialized)
        {
            initialized = true;
            OnSingletonAwake();
        }
    }

    /// <summary>
    /// Clears the cached instance when the singleton is destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            initialized = false;
        }
    }
}
