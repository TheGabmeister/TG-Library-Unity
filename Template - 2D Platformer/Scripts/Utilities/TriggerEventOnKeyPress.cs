using ScriptableObjectArchitecture;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;

public class TriggerEventOnKeyPress : MonoBehaviour
{
    [SerializeField] KeyCode _key;
    [SerializeField] UnityEvent _unityEvent;

    void Update()
    {
        if (Input.GetKeyDown(_key))
        {
            _unityEvent.Invoke(); 
        }
    }
}
