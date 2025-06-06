using UnityEngine;

public class ReticlePulse : MonoBehaviour
{
    public float rotationSpeed = 250f; // Speed of rotation
    public float pulseSpeed = 1f; // Speed of pulsing
    public float pulseMaxScale = 1.5f; // Max scale for pulsing
    public float pulseMinScale = 1f; // Min scale for pulsing
    private SpriteRenderer spriteRenderer; // Reference to the sprite renderer

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the sprite renderer component
    }

    private void Update()
    {
        // Rotation effect
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        // Pulsing effect (scale)
        float scale = Mathf.Lerp(pulseMinScale, pulseMaxScale, Mathf.PingPong(Time.time * pulseSpeed, 1f));
        transform.localScale = new Vector3(scale, scale, 1);

        // Alpha fading effect (only reduces during scaling up phase)
        float alpha = Mathf.PingPong(Time.time * pulseSpeed, 0.5f) + 0.5f; // Alpha goes between 0.5 and 1
        Color color = spriteRenderer.color;

        // Apply alpha while the scale is growing
        color.a = alpha * Mathf.InverseLerp(pulseMinScale, pulseMaxScale, scale); // Combine alpha with scale growth
        spriteRenderer.color = color;
    }
}
