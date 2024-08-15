using UnityEngine;

public class GameInstance : MonoBehaviour
{
    private static GameInstance instance;
    [SerializeField] GameObject[] gameManagers;
    [SerializeField] GameObject userInterface;

    public static GameInstance Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<GameInstance>();
                
            }

            if (instance == null)
            {
                GameObject obj = new GameObject("GameInstance");
                instance = obj.AddComponent<GameInstance>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if(gameManagers != null && gameManagers.Length != 0)
        {
            foreach (GameObject manager in gameManagers)
            {
                Instantiate(manager, gameObject.transform);
            }
        }

        if (userInterface != null)
        {
            Instantiate(userInterface);
        }
        
    }
}