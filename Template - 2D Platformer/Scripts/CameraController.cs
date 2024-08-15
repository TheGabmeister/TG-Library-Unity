using ScriptableObjectArchitecture;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Vector2Reference _playerPosition;

    void LateUpdate()
    {
        if (_playerPosition.Value.x >= transform.position.x - 1)
        {
            Vector3 newCamPos = new Vector3(_playerPosition.Value.x + 1, transform.position.y, -10.0f);
            transform.position = newCamPos;
        }
    }
}