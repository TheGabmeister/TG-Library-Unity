using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class TriggerEventAfterDelay : MonoBehaviour
{
    [SerializeField] UnityEvent _unityEvent;
    
    private IEnumerator _coroutine;

    public void TriggerEvent(float delay)
    {
        _coroutine = TriggerEventDelayed(delay);
        StartCoroutine(_coroutine);
    }

    private IEnumerator TriggerEventDelayed(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _unityEvent?.Invoke();
    }
}
