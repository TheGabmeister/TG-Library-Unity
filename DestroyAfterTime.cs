using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] bool startCountdownImmediately = false;
    [SerializeField] float destroyAfterSeconds = 0f;

    void Start()
    {
        if (startCountdownImmediately)
            StartDestroyCountdown();
    }

    public void StartDestroyCountdown()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }
}
