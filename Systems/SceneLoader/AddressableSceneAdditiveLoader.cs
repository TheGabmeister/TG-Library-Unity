using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class AddressableSceneAdditiveLoader : MonoBehaviour
{
    public AssetReference scene;
    private AsyncOperationHandle<SceneInstance> handle;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadAddressableScene()
    {
        scene.LoadSceneAsync(LoadSceneMode.Additive).Completed += SceneLoadComplete;
    }

    public void UnloadAddressableScene()
    {
        Addressables.UnloadSceneAsync(handle, true).Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
                Debug.Log("Successfully unloaded scene.");
        };
    }

    private void SceneLoadComplete(AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log(obj.Result.Scene.name + " successfully loaded.");
            handle = obj;
        }
    }
}
