using UnityEngine;

public class TreadMove : MonoBehaviour
{
    private Vector3 lastPosition;
    private float lastZRotation;
    private bool flip = false;

    private float flipInterval = 0.05f; // How often to flip (in seconds)
    private float flipTimer = 0f; // Timer to track time between flips

    void Start()
    {
        lastPosition = transform.position;
        lastZRotation = transform.eulerAngles.z;
    }

    void Update()
    {
        // Increment the flip timer based on time passed
        flipTimer += Time.deltaTime;

        // Check if the object has moved or rotated and if enough time has passed for a flip
        if ((HasMoved() || HasRotatedOnZ()) && flipTimer >= flipInterval)
        {
            FlipScaleY();

            // Reset the timer after performing the flip
            flipTimer = 0f;
        }

        lastPosition = transform.position;
        lastZRotation = transform.eulerAngles.z;
    }

    private bool HasMoved()
    {
        return transform.position != lastPosition;
    }

    private bool HasRotatedOnZ()
    {
        return transform.eulerAngles.z != lastZRotation;
    }

    private void FlipScaleY()
    {
        Vector3 scale = transform.localScale;
        scale.y = flip ? 1f : -1f;
        transform.localScale = scale;

        flip = !flip;
    }
}
