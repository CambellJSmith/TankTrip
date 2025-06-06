using UnityEngine;

public class SoldierWalk : MonoBehaviour
{
    private Vector3 lastPosition;
    private float lerpTime;
    private float lerpSpeed = 1f;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        float movementSpeed = (currentPosition - lastPosition).magnitude / Time.deltaTime;

        if (movementSpeed > 0.001f)
        {
            // Map movement speed to lerp speed
            lerpSpeed = Mathf.Clamp(movementSpeed * 0.1f, 1f, 20f); // Adjust multiplier/clamp to your liking

            // Progress lerp over time
            lerpTime += Time.deltaTime * lerpSpeed;

            // Use PingPong to oscillate between 0 and 1
            float lerpValue = Mathf.PingPong(lerpTime, 1f);

            float angle = Mathf.Lerp(-30f, 30f, lerpValue);
            Vector3 currentRotation = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(angle, currentRotation.y, currentRotation.z);
        }

        lastPosition = currentPosition;
    }
}
