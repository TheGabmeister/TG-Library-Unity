using ScriptableObjectArchitecture;
using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;

public class OnKeyPressEvent : MonoBehaviour
{
    [SerializeField] KeyCode key;

    [SerializeField] bool useGameEvent = false;
    [SerializeField] List<GameEvent> gameEvents;

    [SerializeField] bool useUnityEvent = true;
    [SerializeField] UnityEvent unityEvent;

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            if (useGameEvent)
            {
                foreach (GameEvent gameEvent in gameEvents)
                {
                    gameEvent.Raise();
                }
            }
            if (useUnityEvent)
                unityEvent.Invoke(); 
        }
    }
}
