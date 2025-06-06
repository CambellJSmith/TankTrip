using UnityEngine;

public class ScreenBlack : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float stayOpaqueTime = 0.5f; // Stay fully opaque for this long
    private float fadeTime = 2f;         // Time to fade to transparent
    private float timer = 0f;

    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set the sprite to be completely black and fully opaque
        spriteRenderer.color = Color.black;

        // Set the object to the camera bounds
        SetToCameraBounds();

        // Ensure the object is rendered in front of everything else
        SetRenderOrder();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer <= stayOpaqueTime)
        {
            // Stay fully opaque
            spriteRenderer.color = Color.black;
        }
        else if (timer <= stayOpaqueTime + fadeTime)
        {
            // Start fading
            float fadeProgress = (timer - stayOpaqueTime) / fadeTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeProgress);
            spriteRenderer.color = new Color(0f, 0f, 0f, alpha);
        }
        else
        {
            // Destroy the object once it has fully faded
            Destroy(gameObject);
        }
    }

    // Set the object's position and scale to the camera bounds
    void SetToCameraBounds()
    {
        Camera mainCamera = Camera.main;
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        transform.position = mainCamera.transform.position + new Vector3(0, 0, 1); // Place in front of the camera
        transform.localScale = new Vector3(cameraWidth, cameraHeight, 1f);
    }

    // Set the object to always render in front of everything else
    void SetRenderOrder()
    {
        spriteRenderer.sortingOrder = 9999;
        // Optionally use a sorting layer
        // spriteRenderer.sortingLayerName = "UI"; 
    }
}
