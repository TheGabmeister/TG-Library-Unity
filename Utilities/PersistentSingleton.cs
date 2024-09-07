using UnityEngine;


/// <summary>
/// A generic and persistent implementation of the Singleton design pattern. Use this for
/// singletons that need to survive scene loading/unloading. 
/// </summary>
/// <typeparam name="T">The type of the MonoBehaviour that should be a Singleton.</typeparam>
public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            // If the singleton instance does not exist, try to find it in the scene
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();

                // Create a new GameObject with the Type T if it does not exist
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<T>();
                        
                    // Name the singleton instance for the Type
                    singletonObject.name = typeof(T).ToString();

                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
