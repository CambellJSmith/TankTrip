using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;
    private bool hasHitPlayer = false;  // Flag to avoid repeated health damage

    void Start()
    {
        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;

            // Calculate direction from bullet to player
            Vector2 direction = (target.position - transform.position).normalized;

            // Calculate angle for Z rotation
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Add randomness between -45 and 45 degrees
            angle += Random.Range(-45f, 45f);

            // Apply rotation to Z axis
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Move towards the player
            transform.position += transform.right * speed * Time.deltaTime;

            // Check if the bullet is outside the camera bounds
            if (IsOutsideCameraBounds())
            {
                Destroy(gameObject);  // Destroy the bullet if it's out of bounds
            }

            // Check the distance between the bullet and the player
            float distance = Vector3.Distance(transform.position, target.position);

            // Debugging: Log the distance to the player
            Debug.Log("Distance to player: " + distance);

            // Check if the bullet is within 0.5 units from the player
            if (distance < 0.5f && !hasHitPlayer)
            {
                // If the bullet is close enough and hasn't hit the player yet, damage the player
                GameObject healthOrigin = GameObject.Find("HealthOrigin");  // Find the HealthOrigin GameObject
                if (healthOrigin != null)
                {
                    PlayerHealth playerHealth = healthOrigin.GetComponent<PlayerHealth>();  // Access the PlayerHealth component from HealthOrigin
                    if (playerHealth != null)
                    {
                        // Debugging: Log player's health before and after hit
                        Debug.Log("PlayerHealth before: " + playerHealth.playerHealth);
                        playerHealth.playerHealth -= 1;  // Reduce health by 1 points
                        Debug.Log("PlayerHealth after: " + playerHealth.playerHealth);
                    }
                    else
                    {
                        Debug.LogWarning("PlayerHealth component not found on HealthOrigin.");
                    }
                }
                else
                {
                    Debug.LogWarning("HealthOrigin GameObject not found.");
                }

                // Mark that the bullet has hit the player
                hasHitPlayer = true;

                // Destroy the bullet after reducing the player's health
                Destroy(gameObject);
            }
        }
    }

    // Function to check if the bullet is outside the camera bounds
    private bool IsOutsideCameraBounds()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;
    }
}
