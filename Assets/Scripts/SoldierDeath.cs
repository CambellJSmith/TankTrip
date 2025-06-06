using UnityEngine;

public class SoldierDeath : MonoBehaviour
{
    [Header("Death Settings")]
    public bool isDead = false; // Can be set from other scripts
    public GameObject deathPrefab; // Assign this in the inspector
    public GameObject chest; // Assign the chest prefab in the inspector

    private bool hasDied = false;
    private Vector3 frozenPosition;

    // Reference to GameManager
    private GameManager gameManager;

    // For fading the sprite
    private float deathTimer = 0f; // Timer for 10 seconds
    private bool isFading = false;
    private float fadeDuration = 5f; // Duration of the fade
    private float fadeTimer = 0f; // Timer for the fade
    private SpriteRenderer[] spriteRenderers;

    void Start()
    {
        // Get reference to the GameManager instance
        gameManager = GameManager.Instance;
        
        // Get all SpriteRenderers (including this object and its children)
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead && !hasDied)
        {
            hasDied = true;

            // Save position to lock it permanently
            frozenPosition = transform.position;

            // Destroy PlayerShootable component
            Destroy(GetComponent<PlayerShootable>());

            // Instantiate death prefab with random Z rotation
            float randomZ = Random.Range(0f, 360f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, randomZ);
            GameObject spawnedPrefab = Instantiate(deathPrefab, transform.position, rotation);

            // Set the prefab's sorting order to be behind this object (assuming a SpriteRenderer)
            SpriteRenderer prefabRenderer = spawnedPrefab.GetComponent<SpriteRenderer>();
            if (prefabRenderer != null)
            {
                SpriteRenderer currentRenderer = GetComponent<SpriteRenderer>();
                if (currentRenderer != null)
                {
                    prefabRenderer.sortingLayerID = currentRenderer.sortingLayerID;
                    prefabRenderer.sortingOrder = currentRenderer.sortingOrder - 1;
                }
            }

            // Instantiate the chest just on top of this object with a 1 in 100 chance
if (chest != null && Random.Range(0, 100) == 0)
{
    Vector3 chestSpawnPos = transform.position + Vector3.up * 0.5f;
    Instantiate(chest, chestSpawnPos, Quaternion.identity);
}


            // Increase the player's score by 5
            if (gameManager != null)
            {
                gameManager.ModifyScore(5);
            }

            // Remove one from the enemy count in GameManager
            if (gameManager != null)
            {
                GameManager.UnregisterEnemy();
            }
        }

        if (hasDied)
        {
            // Lock position
            transform.position = frozenPosition;

            // Destroy if out of camera view
            if (!IsVisibleFrom(Camera.main))
            {
                Destroy(gameObject);
            }

            // Start the 10-second countdown for fading
            deathTimer += Time.deltaTime;
            if (deathTimer >= 10f && !isFading)
            {
                isFading = true;
                fadeTimer = 0f;
            }

            // Handle the fading effect
            if (isFading)
            {
                fadeTimer += Time.deltaTime;
                float fadeAmount = Mathf.Clamp01(fadeTimer / fadeDuration);

                // Fade out the alpha of all SpriteRenderers
                foreach (var renderer in spriteRenderers)
                {
                    Color color = renderer.color;
                    color.a = 1f - fadeAmount;
                    renderer.color = color;
                }

                // Once the fade is complete, destroy the object
                if (fadeAmount >= 1f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private bool IsVisibleFrom(Camera cam)
    {
        if (!cam) return false;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, GetComponent<Renderer>().bounds);
    }
}
