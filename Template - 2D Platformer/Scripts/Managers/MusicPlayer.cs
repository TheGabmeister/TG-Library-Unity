using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip _mainMenuMusic;
    [SerializeField] AudioClip _gameplayMusic;

    private AudioSource _audioSource;
    

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                _audioSource.clip = _mainMenuMusic;
                break;
            case "Gameplay":
                _audioSource.clip = _gameplayMusic;
                break;
                // add more cases for other scenes as needed
        }
        _audioSource.Play();
    }
}