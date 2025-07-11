using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using PrimeTween;

public class SceneLoader : MonoBehaviour
{
    string _currentScene;
    string _nextScene;
    bool _isLoading = false;
    
    private void Start()
    {
        _currentScene = SceneManager.GetActiveScene().name;
    }

    // Loads the new scene. Afterwards, unloads the previous scene.
    public void SwitchScene(string sceneName)
    {
        if (_isLoading) 
        {
            Debug.Log("Not yet finished loading a scene!");
            return;
        }

        _isLoading = true;
        _nextScene = sceneName;
        SceneManager.sceneLoaded += BeginUnloadingScene;
        SceneManager.LoadScene(_nextScene, LoadSceneMode.Additive);
    }

    void BeginUnloadingScene(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= BeginUnloadingScene;
        SceneManager.UnloadSceneAsync(_currentScene);
        _currentScene = _nextScene;
        _nextScene = null;
        _isLoading = false;
    }
    
    public void LoadSceneByIndex(int index, System.Action onComplete = null)
    {
        if (index >= 0 && index < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LoadSceneAsyncByIndex(index, onComplete));
        }
        else
        {
            Debug.LogError($"Invalid scene build index: {index}");
        }
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        
        if (IsSceneInBuild(sceneName))
        {
            StartCoroutine(LoadSceneAsyncByName(sceneName));
            _currentScene = sceneName;
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' is not included in the build settings.");
        }
    }
    
    IEnumerator LoadSceneAsyncByName(string name)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
        yield return null;
    }
    
    IEnumerator LoadSceneAsyncByIndex(int index, System.Action onComplete = null)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);
            
        // You can monitor the loading progress
        while (!asyncLoad.isDone)
        {
            //float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            //Debug.Log($"Loading progress: {progress * 100}%");
                
            // Yield control back to Unity until next frame
            yield return null;
        }
        onComplete?.Invoke();
    }
    
    private bool IsSceneInBuild(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        
            if (string.Equals(sceneNameFromPath, sceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
    
        return false;
    }
}
