using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    AudioSource _audioSource;

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void ToggleMusic(bool value)
    {
        if (value)
            _audioSource.Play();
        else
            _audioSource.Stop();
    }
}