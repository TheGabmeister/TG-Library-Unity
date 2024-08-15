using ScriptableObjectArchitecture;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] IntReference _currentScore;
    [SerializeField] IntReference _coins;
    [SerializeField] IntReference _lives;
    [SerializeField] GameObject _loadingScreen;
    [SerializeField] GameObject _gameOver;
    [SerializeField] TMP_Text _scoreText;
    [SerializeField] TMP_Text _coinsText;
    [SerializeField] TMP_Text _livesText;
    [SerializeField] GameObject _pauseMenu;

    [Header("Listen to these events...")]
    [SerializeField] GameEvent _onShowLoadingScreen;
    [SerializeField] GameEvent _onHideLoadingScreen;
    [SerializeField] GameEvent _onPauseGame;
    [SerializeField] GameEvent _onUnpauseGame;

    void Start()
    {
        ResetUI();
    }

    private void OnEnable()
    {
        _currentScore.AddListener(UpdateScoreText);
        _coins.AddListener(UpdateCoinsText);
        _lives.AddListener(UpdateLivesText);
        _onShowLoadingScreen.AddListener(ShowLoadingScreen);
        _onHideLoadingScreen.AddListener(HideLoadingScreen);
        _onPauseGame.AddListener(ShowPauseMenu);
        _onUnpauseGame.AddListener(HidePauseMenu);
    }

    private void OnDisable()
    {
        _currentScore.RemoveListener(UpdateScoreText);
        _coins.RemoveListener(UpdateCoinsText);
        _lives.RemoveListener(UpdateLivesText);
        _onShowLoadingScreen.RemoveListener(ShowLoadingScreen);
        _onHideLoadingScreen.RemoveListener(HideLoadingScreen);
        _onPauseGame.RemoveListener(ShowPauseMenu);
        _onUnpauseGame.RemoveListener(HidePauseMenu);
    }

    public void UpdateScoreText()
    {
         _scoreText.SetText("Score: " + _currentScore.Value);
    }

    public void UpdateCoinsText()
    {
        _coinsText.SetText("Coins: " + _coins.Value);
    }

    public void UpdateLivesText()
    {
        _livesText.SetText("Lives: " + _lives.Value);
    }

    void ShowLoadingScreen()
    {
        _loadingScreen.SetActive(true);
    }

    void HideLoadingScreen()
    {
        _loadingScreen.SetActive(false);
    }

    public void ShowGameOverUI()
    {
        _gameOver.SetActive(true);
    }

    void ShowPauseMenu()
    {
        _pauseMenu.SetActive(true);
    }
    void HidePauseMenu()
    {
        _pauseMenu.SetActive(false);
    }

    void ResetUI()
    {
        _scoreText.SetText("Score: 0");
        _coinsText.SetText("Coins: 0");
        _livesText.SetText("Lives: 3");
        _gameOver.SetActive(false);
    }
}
