using UnityEngine;

public class PropellorSpin : MonoBehaviour
{
    [Header("Spin Settings")]
    public float rotationSpeed = 3000f; // Degrees per second

    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (HasMoved())
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        }

        lastPosition = transform.position;
    }

    private bool HasMoved()
    {
        return transform.position != lastPosition;
    }
}
