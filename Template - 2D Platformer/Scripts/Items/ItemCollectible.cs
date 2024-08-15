using ScriptableObjectArchitecture;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemCollectible : MonoBehaviour
{
    [SerializeField] AudioClip _audioClip;
    [SerializeField] AudioClipGameEvent _onPlaySound;

    [SerializeField] int score = 0;
    [SerializeField] IntGameEvent _onUpdateScore;

    [SerializeField] int lives = 0;
    [SerializeField] IntGameEvent _onUpdateLives;

    [SerializeField] List<GameEvent> _gameEvents;
    [SerializeField] UnityEvent _unityEvent;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CollectItem();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CollectItem();
        }
    }

    void CollectItem()
    {
        _onPlaySound?.Raise(_audioClip);
        _onUpdateScore?.Raise(score);
        _onUpdateLives?.Raise(lives);

        if (_gameEvents != null && _gameEvents.Count > 0)
        {
            foreach (GameEvent gameEvent in _gameEvents)
                gameEvent.Raise();
        }
        
        _unityEvent?.Invoke();

        Destroy(gameObject);
    }
}