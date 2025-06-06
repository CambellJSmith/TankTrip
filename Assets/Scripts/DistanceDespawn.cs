using UnityEngine;

public class DistanceDespawn : MonoBehaviour
{
    public float despawnDistance = 100f; // Distance threshold for despawning

    private Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the player by tag
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player is assigned
        if (player != null)
        {
            // Calculate the distance between this object and the player
            float distance = Vector3.Distance(transform.position, player.position);

            // If the distance exceeds the threshold, destroy this object
            if (distance > despawnDistance)
            {
                // Call the GameManager's UnregisterEnemy method before destroying the object
                GameManager.UnregisterEnemy();

                // Destroy the object
                Destroy(gameObject);
            }
        }
    }
}
