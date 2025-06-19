using UnityEngine;
using UnityEngine.Events;

// An example of how to make a gameobject component that listens to events.
// You need to create one for each event.

namespace EventBus
{
    public class EVL_GameStart : MonoBehaviour
    {
        [SerializeField] UnityEvent _unityEvent;

        void OnEnable()
        {
            Bus<EV_GameStart>.Add(TriggerEvent);
        }

        void OnDisable()
        {
            Bus<EV_GameStart>.Remove(TriggerEvent);
        }

        void TriggerEvent()
        {
            _unityEvent?.Invoke();
        }
    }
}