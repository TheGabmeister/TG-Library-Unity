using UnityEngine;

public class MovePerFrame : MonoBehaviour
{
    public float movementSpeed = 10f;

    void Update()
    {
        transform.Translate(Vector3.right * movementSpeed * Time.deltaTime);
    }
}
