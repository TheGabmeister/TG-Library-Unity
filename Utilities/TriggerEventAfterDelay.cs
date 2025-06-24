using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class TriggerEventAfterDelay : MonoBehaviour
{
    [SerializeField] float _delay;
    [SerializeField] UnityEvent _unityEvent;
    
    void Start()
    {
        Invoke("TriggerEvent", _delay);
    }

    public void TriggerEvent()
    {
        _unityEvent?.Invoke();
    }
}
