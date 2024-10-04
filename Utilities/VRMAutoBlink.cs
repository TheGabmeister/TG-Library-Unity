using UnityEngine;
using System.Collections;

public class VRMAutoBlink : MonoBehaviour
{
    // The skinned mesh that has the eye blink blendshape. For VRM, it's the Face mesh.
    [SerializeField] SkinnedMeshRenderer _eyesMesh;

    // The blendshape index for the eyes. For VRM, it's 13.
    [SerializeField] int _blendShapeIndex = 13;

	[SerializeField] float _minInterval = 2.0f;
    [SerializeField] float _maxInterval = 5.0f;
	[SerializeField] float _eyeSpeed = 0.1f;

	void Start ()
	{
		StartCoroutine ("RandomBlinking");
	}

    IEnumerator Blink()
	{
        Debug.Log("Blink");
        float timer = 0f;
        float value = 0f;

		// close eyes
        while (timer < _eyeSpeed)
        {
            timer += Time.deltaTime;
            value = Mathf.Lerp(0.0f, 100.0f, timer / _eyeSpeed);
            _eyesMesh.SetBlendShapeWeight(_blendShapeIndex, value);
            yield return null;
        }

        timer = 0.0f;

		// open eyes
        while (timer < _eyeSpeed)
        {
            timer += Time.deltaTime;
            value = Mathf.Lerp(100f, 0.0f, timer / _eyeSpeed);
            _eyesMesh.SetBlendShapeWeight(_blendShapeIndex, value);
            yield return null; 
        }
    }

    IEnumerator RandomBlinking()
    {
		while (true)
		{
            StartCoroutine("Blink");
            yield return new WaitForSeconds(Random.Range(_minInterval, _maxInterval));
		}
    }
}