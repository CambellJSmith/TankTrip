using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundCamScale : MonoBehaviour
{
    [Tooltip("Sorting layer to assign to the background and its children.")]
    public string sortingLayerName = "Background";

    [Tooltip("Sorting order for the background.")]
    public int backgroundSortingOrder = -1000;

    void Start()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        // Get the top right and bottom left world positions of the camera's view
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector2 camSize = new Vector2(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr.sprite == null)
        {
            Debug.LogError("SpriteRenderer has no sprite assigned!");
            return;
        }

        // Set sorting layer and order for background
        sr.sortingLayerName = sortingLayerName;
        sr.sortingOrder = backgroundSortingOrder;

        // Get the size of the sprite in world units
        Vector2 spriteSize = sr.sprite.bounds.size;

        // Calculate the scale needed to fill the camera bounds
        float scaleFactor = Mathf.Max(camSize.x / spriteSize.x, camSize.y / spriteSize.y);

        // Apply the scale uniformly
        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);

        // Recalculate the top right of the scaled sprite
        Vector3 scaledSpriteSize = spriteSize * scaleFactor;
        Vector3 spriteTopRight = transform.position + new Vector3(scaledSpriteSize.x / 2f, scaledSpriteSize.y / 2f, 0f);

        // Calculate the difference between current top right and camera's top right
        Vector3 offset = topRight - spriteTopRight;

        // Move the sprite so its top right aligns with the camera's top right
        transform.position += offset;

        // Move background far back in Z (optional, more important in perspective camera)
        transform.position = new Vector3(transform.position.x, transform.position.y, 10f);

        // Make sure children render above background
        ApplyChildSorting(transform);
    }

    private void ApplyChildSorting(Transform parent)
    {
        foreach (Transform child in parent)
        {
            SpriteRenderer childSR = child.GetComponent<SpriteRenderer>();
            if (childSR != null)
            {
                childSR.sortingLayerName = sortingLayerName;
                childSR.sortingOrder = backgroundSortingOrder + 1; // ensures children render above
            }
        }
    }
}
