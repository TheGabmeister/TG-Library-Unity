using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This component marks temporary objects in the scene that can be destroyed when
/// loading into another scene.
///
/// This is useful when loading gameplay scenes additively. Attach this to any GameObject
/// only needed for testing (e.g. cameras) but not necessary when loading into another scene.
/// </summary>
public class DestroyOnLoad : MonoBehaviour
{
    [Tooltip("Don't destroy the GameObject when this is the active scene")]
    [SerializeField] private string _activeWithinScene;
    [Tooltip("Defaults to current GameObject unless specified")]
    [SerializeField] private GameObject _objectToDestroy;
    [Tooltip("Shows debug messages.")]
    [SerializeField] private bool _debug;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != _activeWithinScene)
        {
            if (_objectToDestroy == null)
                _objectToDestroy = gameObject;

            Destroy(_objectToDestroy);

            if (_debug)
            {
                Debug.Log("Active scene: " + SceneManager.GetActiveScene().name);
                Debug.Log("Do not destroy in scene: " + _activeWithinScene);
                Debug.Log("Destroy on load: " + _objectToDestroy);
            }
        }
    }
}