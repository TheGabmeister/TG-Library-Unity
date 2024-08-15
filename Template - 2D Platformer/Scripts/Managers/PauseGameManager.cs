using ScriptableObjectArchitecture;
using UnityEngine;

public class PauseGameManager : MonoBehaviour
{
    [SerializeField] AudioClip _pauseAudioClip;

    [Header("Listen to these events...")]
    [SerializeField] GameEvent _onTogglePause;

    [Header("Call these events...")]
    [SerializeField] GameEvent _onPauseGame;
    [SerializeField] GameEvent _onUnpauseGame;
    [SerializeField] AudioClipGameEvent _onPlaySound;

    bool gamePaused = false;

    private void OnEnable()
    {
        _onTogglePause.AddListener(TogglePauseGame);
    }

    private void OnDisable()
    {
        _onTogglePause.RemoveListener(TogglePauseGame);
    }

    void TogglePauseGame()
    {
        if (gamePaused)
        {
            gamePaused = false;
            _onUnpauseGame.Raise();
            _onPlaySound.Raise(_pauseAudioClip);
            Time.timeScale = 1;
        }
        else
        {
            gamePaused = true;
            _onPauseGame.Raise();
            _onPlaySound.Raise(_pauseAudioClip);
            Time.timeScale = 0;
        }
    }
}