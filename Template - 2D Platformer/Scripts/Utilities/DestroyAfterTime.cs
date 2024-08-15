using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] bool _startCountdownImmediately = false;
    [SerializeField] float _destroyAfterSeconds = 0f;

    void Start()
    {
        if (_startCountdownImmediately)
            StartDestroyCountdown();
    }

    public void StartDestroyCountdown()
    {
        Destroy(gameObject, _destroyAfterSeconds);
    }
}
