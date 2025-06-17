using UnityEngine;

// Load a prefab "Systems" in the Resources folder 
// https://low-scope.com/unity-tips-1-dont-use-your-first-scene-for-global-script-initialization/
// https://www.youtube.com/watch?v=zJOxWmVveXU

public static class Bootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute() => Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Systems")));
}
