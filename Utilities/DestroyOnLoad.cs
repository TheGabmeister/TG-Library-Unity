using UnityEngine;

/// <summary>
/// Use this to destroy temporary game objects such as a camera in a UI scene.
/// </summary>
public class DestroyOnLoad : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject);
    }
}