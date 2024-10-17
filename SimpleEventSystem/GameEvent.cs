using System;

// A simple event system based on https://playable.design/a-simple-event-system-for-unity

namespace SimpleEventSystem
{
    public class GameEvent
    {
        private event Action action = delegate { };
        public void Publish() => action?.Invoke();
        public void Add(Action subscriber) => action += subscriber;
        public void Remove(Action subscriber) => action -= subscriber;
    }

    public class GameEvent<T>
    {
        private event Action<T> action;
        public void Publish(T param) => action?.Invoke(param);
        public void Add(Action<T> subscriber) => action += subscriber;
        public void Remove(Action<T> subscriber) => action -= subscriber;
    }
}


/* Example Usage
 
using UnityEngine;
using SimpleEventSystem;
public class Player : MonoBehaviour
{
    private void OnEnable()
    {
        Events.Player.onJump.Add(OnPickup);
        Events.Player.onDamage.Add(OnDamage);
    }

    private void OnDisable()
    {
        Events.Player.onJump.Remove(OnPickup);
        Events.Player.onDamage.Remove(OnDamage);
    }

    private void OnPickup(GameObject item)
    {
        Debug.Log($"Picked up item: {item.name}");
    }

    private void OnDamage(float amount)
    {
        Debug.Log($"Damaged: {amount}");
    }
}


using UnityEngine;
using SimpleEventSystem;
public class Pickup : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.CompareTag("player"))
        {
            Events.Item.onPickup.Publish(gameObject);
        }
    }
}

*/