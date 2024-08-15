using UnityEngine;

public class MoveOverTime : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 10f;
    [SerializeField] Vector3 _localMoveDirection = new Vector3(1.0f, 0.0f, 0.0f);

    void Update()
    {
        transform.Translate(_localMoveDirection * _moveSpeed * Time.deltaTime);
    }
}
