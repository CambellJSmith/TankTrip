using UnityEngine;

public class Explosion : MonoBehaviour
{
    public Sprite[] explosionFrames; // Assign 5 sprites in the Inspector
    public float frameRate = 0.1f;   // Time between frames

    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;
    private float timer = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (explosionFrames == null || explosionFrames.Length == 0)
        {
            Debug.LogWarning("No explosion frames assigned!");
            Destroy(gameObject);
            return;
        }

        spriteRenderer.sprite = explosionFrames[0];
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= frameRate)
        {
            timer -= frameRate;
            currentFrame++;

            if (currentFrame >= explosionFrames.Length)
            {
                Destroy(gameObject);
                return;
            }

            spriteRenderer.sprite = explosionFrames[currentFrame];
        }
    }
}
