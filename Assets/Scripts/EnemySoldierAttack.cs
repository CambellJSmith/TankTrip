using UnityEngine;

public class EnemySoldierAttack : MonoBehaviour
{
    public GameObject bulletPrefab;  // The bullet prefab to be instantiated
    public float shootIntervalMin = 1f;  // Minimum time between shots (1 second)
    public float shootIntervalMax = 1.2f;  // Maximum time between shots (1.2 seconds)
    public float detectionRadius = 5f;  // Detection radius for the player

    private Transform player;  // Reference to the player's transform
    private float shootTimer = 0f;  // Timer to control shooting interval

    void Start()
    {
        // Find the player by tag
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogWarning("Player not found! Make sure there is an object tagged 'Player'.");
        }
    }

    void Update()
    {
        if (player == null) return;

        // If the soldier is dead, do not allow shooting
        if (GetComponent<SoldierDeath>()?.isDead == true)
        {
            return; // Exit without shooting if the soldier is dead
        }

        // Check the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // If the player is within range, check shooting logic
        if (distanceToPlayer <= detectionRadius)
        {
            // Count down the shoot timer
            shootTimer -= Time.deltaTime;

            // If the timer has reached zero, shoot a bullet and reset the timer
            if (shootTimer <= 0f)
            {
                Shoot();
                // Reset the shoot timer with a random interval between 1 and 1.2 seconds
                shootTimer = Random.Range(shootIntervalMin, shootIntervalMax);
            }
        }
    }

    void Shoot()
    {
        // Instantiate the bullet at the enemy's position
        if (bulletPrefab != null)
        {
            Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        }
    }
}
