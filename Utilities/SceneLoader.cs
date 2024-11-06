using UnityEngine;
using UnityEngine.SceneManagement;
using ScriptableObjectArchitecture;

// Other references:
// https://pastebin.com/Vecczt5Q
// https://gist.github.com/kurtdekker/862da3bc22ee13aff61a7606ece6fdd3

public class SceneLoader : MonoBehaviour
{
    [Header("Listen to these events...")]
    [SerializeField] GameEvent _onRestartScene;
    [SerializeField] StringGameEvent _onLoadScene;

    [Header("Set these variables...")]
    [SerializeField] SceneDataReference _currentLevel;
    [SerializeField] SceneDataReference _nextLevel;

    void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        _onLoadScene.AddListener(LoadSceneByName);
        _onRestartScene.AddListener(RestartCurrentScene);
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        _onLoadScene.RemoveListener(LoadSceneByName);
        _onRestartScene.RemoveListener(RestartCurrentScene);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    void Start()
    {
 
    }

    void RestartCurrentScene()
    {
        SceneManager.LoadScene(_currentLevel.Value.sceneName);
    }
}