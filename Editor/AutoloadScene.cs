using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// Scene auto loader.
/// </summary>
/// <description>
/// Loads the scene at build index 0 before everything else. Put this in the Assets > Editor folder
/// Used for initializing global scripts such as managers and event systems.
/// In Godot, this is the Autoload feature. In Unreal, this is the GameInstance, GameMode, etc.
/// Based on: http://forum.unity3d.com/threads/157502-Executing-first-scene-in-build-settings-when-pressing-play-button-in-editor
/// </description>

[InitializeOnLoad]
public class AutoloadScene
{
    static AutoloadScene()
    {
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneUtility.GetScenePathByBuildIndex(0));
    }
}