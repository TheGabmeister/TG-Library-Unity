using ScriptableObjectArchitecture;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] SceneData[] _levels;
    SceneDataReference _currentLevel;
    SceneDataReference _nextLevel;
    int _currentLevelIndex = 0;
    [SerializeField] IntReference _lives;

    [Header("Listen to these events...")]
    [SerializeField] GameEvent _onStartGame;
    [SerializeField] GameEvent _onRestartStartGame;
    [SerializeField] GameEvent _onPlayerDied;
    [SerializeField] GameEvent _onZeroLives;
    [SerializeField] GameEvent _onReachedFinishLine;

    [Header("Call these events...")]
    [SerializeField] StringGameEvent _onLoadLevel;
    [SerializeField] GameEvent _onRestartLevel;
    [SerializeField] AudioClipGameEvent _onPlayMusic;
    [SerializeField] GameEvent _onStopMusic;
    [SerializeField] GameEvent _onShowLoadingScreen;
    [SerializeField] GameEvent _onHideLoadingScreen;
    [SerializeField] IntGameEvent _onUpdateLives;

    private void OnEnable()
    {
        _onStartGame.AddListener(StartLevel);
        _onRestartStartGame.AddListener(RestartGame);
        _onPlayerDied.AddListener(HandlePlayerDeath);
        _onZeroLives.AddListener(StartGameOverSequence);
        _onReachedFinishLine.AddListener(StartLevelEndSequence);
    }

    private void OnDisable()
    {
        _onStartGame.RemoveListener(StartLevel);
        _onRestartStartGame.RemoveListener(RestartGame);
        _onPlayerDied.RemoveListener(HandlePlayerDeath);
        _onZeroLives.RemoveListener(StartGameOverSequence);
        _onReachedFinishLine.RemoveListener(StartLevelEndSequence);
    }


    void HandlePlayerDeath()
    {
        StartCoroutine(HandlePlayerDeathCoroutine());
    }

    IEnumerator HandlePlayerDeathCoroutine()
    {
        _onStopMusic.Raise();  // Or play death music
        yield return new WaitForSeconds(2);

        _onUpdateLives.Raise(-1);
        if (_lives.Value <= 0)
        {
            StartGameOverSequence();
        }
        else
        {
            StartLevel();
        }
    }

    void RestartGame()
    {
        _currentLevelIndex = 0;
        _onLoadLevel.Raise("MainMenu");
        _onHideLoadingScreen.Raise();
    }

    void StartLevel()
    {
        StartCoroutine(StartLevelCoroutine());
    }

    IEnumerator StartLevelCoroutine()
    {
        _onShowLoadingScreen.Raise();
        _onLoadLevel.Raise(_levels[_currentLevelIndex].sceneName);
        yield return new WaitForSeconds(2);
        _onHideLoadingScreen.Raise();
        _onPlayMusic.Raise(_levels[_currentLevelIndex].musicName);
    }


    void StartNextLevel()
    {

    }

    void StartLevelEndSequence()
    {
        StartCoroutine(StartLevelEndSequenceCoroutine());
    }

    IEnumerator StartLevelEndSequenceCoroutine()
    {
        _onStopMusic.Raise();
        yield return new WaitForSeconds(2);
        // Play success music
        _currentLevelIndex++;

        if (_levels[_currentLevelIndex] != null)
        {
            StartLevel();
        }
        else
        {
            StartGameFinishSequence();
        }
    }

    void StartGameOverSequence()
    {
        // Show Game Over UI
    }

    void StartGameFinishSequence()
    {
        Debug.Log("Game Finished!");
    }
}
