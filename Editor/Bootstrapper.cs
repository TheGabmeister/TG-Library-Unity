using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Load a prefab "Systems" in the Resources folder. Works together with BootstrapperMenu.cs
// https://low-scope.com/unity-tips-1-dont-use-your-first-scene-for-global-script-initialization/
// https://www.youtube.com/watch?v=zJOxWmVveXU

public static class Bootstrapper
{
    private const string EditorPrefKey = "BootstrapperEnabled";
    
    public static bool Enabled
    {
        get
        {
#if UNITY_EDITOR
            return EditorPrefs.GetBool(EditorPrefKey, true);
#else
            return true;
#endif
        }
        set
        {
#if UNITY_EDITOR
            EditorPrefs.SetBool(EditorPrefKey, value);
#endif
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        if (Enabled)
        {
            Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Systems")));
        }
    }
}