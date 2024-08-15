using UnityEngine;
using UnityEngine.Events;

public class TriggerEventOnCollision : MonoBehaviour
{
    [SerializeField] string _colliderTag = "Player";
    [SerializeField] UnityEvent _unityEvent;
    [SerializeField] bool _destroyAfterEvent = false;
    [SerializeField] float _destroyDelay = 0.0f;

    void OnCollisionEnter(Collision collision)
    {
        if (!string.IsNullOrEmpty(_colliderTag) && collision.gameObject.CompareTag(_colliderTag))
        {
            TriggerEvent();
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (!string.IsNullOrEmpty(_colliderTag) && collision.gameObject.CompareTag(_colliderTag))
        {
            TriggerEvent();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!string.IsNullOrEmpty(_colliderTag) && collision.gameObject.CompareTag(_colliderTag))
        {
            TriggerEvent();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!string.IsNullOrEmpty(_colliderTag) && collision.gameObject.CompareTag(_colliderTag))
        {
            TriggerEvent();
        }
    }

    void TriggerEvent()
    {
        _unityEvent?.Invoke();
        if(_destroyAfterEvent)
        {
            Destroy(gameObject, _destroyDelay);
        }
    }
}
