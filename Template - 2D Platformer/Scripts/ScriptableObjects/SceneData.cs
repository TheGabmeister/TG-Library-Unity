using UnityEngine;

[CreateAssetMenu(fileName = "SceneData", menuName = "Scene Data", order = 0)]
public class SceneData : ScriptableObject
{
    public string sceneName;
    public string displayName;
    public AudioClip musicName;
    public int time;
}