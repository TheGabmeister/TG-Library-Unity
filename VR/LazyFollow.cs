using UnityEngine;

/// <summary>
/// Makes the GameObject this component is attached to follow a target with a delay
/// </summary>

public class LazyFollow : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] float followSpeed = 6f;

    void Update()
    {
        if (_target == null)
            return;

        transform.position = Vector3.Lerp(transform.position, _target.position, Time.deltaTime * followSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, _target.rotation, Time.deltaTime * followSpeed);
    }
}