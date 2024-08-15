using ScriptableObjectArchitecture;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] IntReference _remainingTime;
    [SerializeField] SceneDataReference _currentLevel;

    [Header("Listen to these events...")]
    [SerializeField] GameEvent _onInitializeTimer;
    [SerializeField] GameEvent _onPauseTimer;
    [SerializeField] GameEvent _onStartTimer;

    [Header("Call these events...")]
    [SerializeField] GameEvent _onHunderedSecondsLeft;
    [SerializeField] GameEvent _onFInishedTime;

    private void OnEnable()
    {
        _onInitializeTimer.AddListener(InitializeTimer);
        _onStartTimer.AddListener(StartTimer);
        _onPauseTimer.AddListener(PauseTimer);
    }

    private void OnDisable()
    {
        _onInitializeTimer.RemoveListener(InitializeTimer);
        _onStartTimer.RemoveListener(StartTimer);
        _onPauseTimer.RemoveListener(PauseTimer);
    }

    void StartTimer()
    {
        InvokeRepeating("UpdateTime", 0.0f, 1.0f);
    }

    void InitializeTimer()
    {
        _remainingTime.Value = _currentLevel.Value.time;
    }

    void PauseTimer()
    {

    }

    void UpdateTime()
    {
        _remainingTime.Value -= _remainingTime.Value;
        if (_remainingTime.Value == 100)
        {
            _onHunderedSecondsLeft?.Raise();
        }

        if (_remainingTime.Value < 0)
        {
            _onFInishedTime?.Raise();
        }
    }
}
