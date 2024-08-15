using UnityEngine;

public class MoveOverTime2D : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 10f;
    [SerializeField] Vector2 _localMoveDirection = new Vector2(1.0f, 0.0f);

    void Update()
    {
        transform.Translate(_localMoveDirection * _moveSpeed * Time.deltaTime);
    }
}
