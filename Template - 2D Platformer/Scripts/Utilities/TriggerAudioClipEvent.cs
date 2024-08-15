using ScriptableObjectArchitecture;
using UnityEngine;

public class TriggerAudioClipEvent : MonoBehaviour
{
    [SerializeField] AudioClip _audioClip;
    [SerializeField] AudioClipGameEvent _audioClipEvent;

    public void TriggerAudioClip()
    {
        _audioClipEvent?.Raise(_audioClip);
    }
}