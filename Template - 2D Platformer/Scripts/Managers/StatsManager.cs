using ScriptableObjectArchitecture;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    [SerializeField] IntVariable _currentScore;
    [SerializeField] IntVariable _highScore;
    [SerializeField] IntVariable _coins;
    [SerializeField] IntVariable _lives;
    
    [Header("Call these events...")]
    [SerializeField] GameEvent _onZeroLivesLeft;

    void Start()
    {
        _currentScore.Value = 0;
        _highScore.Value = 0;
        _coins.Value = 0;
        _lives.Value = 3;
    }

    public void UpdateCurrentScore(int incomingScore)
    {
        _currentScore.Value += incomingScore;
        if (_currentScore.Value > _highScore.Value)
        {
            _highScore.Value = _currentScore.Value;
        }
    }

    public void UpdateCoins(int incomingCoins)
    {
        _coins.Value += incomingCoins;
        if (_coins.Value >= 100)
        {
            UpdateLives(1);
            _coins.Value -= 100;
        }
    }

    public void UpdateLives(int incomingLives)
    {
        _lives.Value += incomingLives;
        if (_lives.Value <= 0)
        {
            _onZeroLivesLeft?.Raise();
        }
    }
}
