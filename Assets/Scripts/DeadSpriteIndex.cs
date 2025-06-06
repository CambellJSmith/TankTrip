using UnityEngine;

public class DeadSpriteIndex : MonoBehaviour
{
    // Declare a public array of sprites with 2 entries
    public Sprite[] sprites = new Sprite[2];
    
    // Reference to the SoldierDeath script on the parent object
    private SoldierDeath soldierDeath;

    // Reference to the SpriteRenderer on this object
    private SpriteRenderer spriteRenderer;

    // Variable to track sprite index
    private int spriteIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the SoldierDeath component from the parent object
        soldierDeath = transform.parent.GetComponent<SoldierDeath>();

        // Initialize the SpriteRenderer on this object
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Check if the SoldierDeath script is attached to the parent object
        if (soldierDeath == null)
        {
            Debug.LogError("SoldierDeath script not found on the parent object.");
        }

        // Check if we have the sprites array correctly set
        if (sprites.Length != 2)
        {
            Debug.LogError("Sprites array should have exactly 2 entries.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Ensure the parent object has the SoldierDeath script
        if (soldierDeath != null && soldierDeath.isDead && spriteIndex == 0)
        {
            spriteIndex = 1; // Increment sprite index when the character dies

            // Update the sprite on the SpriteRenderer
            if (spriteRenderer != null && sprites.Length > spriteIndex)
            {
                spriteRenderer.sprite = sprites[spriteIndex];
            }
        }
    }
}
