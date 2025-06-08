using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] bool _startCountdownImmediately = true;
    [SerializeField] float _destroyAfterSeconds = 5f;

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
