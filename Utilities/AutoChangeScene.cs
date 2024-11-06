using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

// From this thread: https://discussions.unity.com/t/executing-first-scene-in-build-settings-when-pressing-play-button-in-editor/489673
// Also check out this repo: https://github.com/STARasGAMES/unity-main-scene-auto-loading

[InitializeOnLoad]
public class AutoChangeScene
{
    static AutoChangeScene()
    {
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneUtility.GetScenePathByBuildIndex(0));
    }
}