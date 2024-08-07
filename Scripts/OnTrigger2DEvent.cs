using ScriptableObjectArchitecture;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]

public class OnTrigger2DEvent : MonoBehaviour
{
    [SerializeField] string colliderTag = "Player";

    [SerializeField] bool useGameEvent = false;
    [SerializeField] List<GameEvent> gameEvents;

    [SerializeField] bool useUnityEvent = true;
    [SerializeField] UnityEvent unityEvent;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(colliderTag))
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