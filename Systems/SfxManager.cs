using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SfxManager : MonoBehaviour
{
    AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }
}