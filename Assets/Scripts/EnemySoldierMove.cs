using UnityEngine;
using System.Linq;
using System.Collections;

public class EnemySoldierMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float checkRadius = 10f;
    private GameObject player;

    // States
    private bool isRunningAway = false;
    private bool isFrozen = false;

    // Panic coroutine handle
    private Coroutine panicCoroutine;

    // Reference to the SoldierDeath component on this object
    private SoldierDeath soldierDeath;

    // The random stop distance the enemy will use
    private float randomStopDistance;

    // Distance at which soldiers should not be closer than (anti‑bunching distance)
    private float minDistanceFromOtherEnemy = 1f; 

    // Smoothing factor for the avoidance movement
    public float avoidanceForce = 5f;
    private Vector3 avoidanceDirection = Vector3.zero;

    // Random number threshold for dead enemies to trigger behavior
    private int randomDeadThreshold;

    // Private variable to track if the enemy has met the dead threshold (panic state)
    public bool hasMetDeadThreshold = false;

    void Start()
    {
        // Find the player object in the scene tagged "Player"
        player = GameObject.FindGameObjectWithTag("Player");

        // Get the SoldierDeath component of this object
        soldierDeath = GetComponent<SoldierDeath>();

        // Pick a random stop distance between 2 and 5 units
        randomStopDistance = Random.Range(2f, 5f);

        // Set a random threshold for the number of dead enemies to trigger behavior (between 2 and 7)
        randomDeadThreshold = Random.Range(2, 8);
    }

    void Update()
    {
        // If no player or no death component, skip
        if (player == null || soldierDeath == null)
            return;

        // If dead, stop all behavior
        if (soldierDeath.isDead)
        {
            isRunningAway = false;
            isFrozen = false;
            if (panicCoroutine != null) { StopCoroutine(panicCoroutine); panicCoroutine = null; }
            return;
        }

        // If frozen, do nothing
        if (isFrozen)
            return;

        // If in panic-run, the coroutine is already moving us—skip the normal move checks
        if (panicCoroutine != null)
            return;

        // If running away, move directly away
        if (isRunningAway)
        {
            MoveAwayFromPlayer();
        }
        else
        {
            // Otherwise, approach and check whether to switch state
            MoveTowardsPlayer();
            CheckNearbyEnemies();
        }

        // Always apply anti‑bunching unless frozen or panicking
        AvoidNearbyEnemies();
    }

    void MoveTowardsPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > randomStopDistance)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
        // else stay put until state changes
    }

    void CheckNearbyEnemies()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("EnemySoldier");
        var closestEnemies = allEnemies.Select(enemy => new 
        {
            Enemy = enemy,
            Distance = Vector3.Distance(transform.position, enemy.transform.position),
            DeathComp = enemy.GetComponent<SoldierDeath>()
        })
        .OrderBy(x => x.Distance)
        .Take(10)
        .ToList();

        int deadCount = closestEnemies.Count(x => x.DeathComp != null && x.DeathComp.isDead);

        if (deadCount >= randomDeadThreshold && !isRunningAway && panicCoroutine == null)
        {
            float chance = Random.value; // 0.0 to 1.0

            hasMetDeadThreshold = true;  // Update the state when the threshold is met

            if (chance <= 0.75f)
            {
                // 75% chance: run away
                isRunningAway = true;
                MoveAwayFromPlayer();
            }
            else if (chance <= 0.85f)
            {
                // Next 10%: freeze
                isFrozen = true;
            }
            else
            {
                // Remaining 15%: panic-run loop
                panicCoroutine = StartCoroutine(PanicRunLoop());
            }
        }
    }

    void MoveAwayFromPlayer()
    {
        Vector3 direction = (transform.position - player.transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    void AvoidNearbyEnemies()
    {
        Vector3 avoidance = Vector3.zero;
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("EnemySoldier");

        foreach (var enemy in allEnemies)
        {
            if (enemy == this.gameObject) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDistanceFromOtherEnemy)
            {
                Vector3 dir = (transform.position - enemy.transform.position).normalized;
                avoidance += dir * (minDistanceFromOtherEnemy - dist) * avoidanceForce;
            }
        }

        if (avoidance != Vector3.zero)
        {
            avoidanceDirection = Vector3.Lerp(avoidanceDirection, avoidance, 0.1f);
            transform.position += avoidanceDirection * Time.deltaTime;
        }
    }

    IEnumerator PanicRunLoop()
{
    float panicSpeedMin   = moveSpeed * 1.5f;
    float panicSpeedMax   = moveSpeed * 2f;
    float jitterIntensity = 0.7f;

    while (true)
    {
        // 1) Pick a random 2D offset within a circle (X & Y)
        float panicRadius = Random.Range(3f, 8f);
        Vector2 offset2D  = Random.insideUnitCircle * panicRadius;
        Vector3 target    = new Vector3(
            transform.position.x + offset2D.x,
            transform.position.y + offset2D.y,
            transform.position.z    // keep Z where it is
        );

        // 2) Choose a random panic speed
        float panicSpeed = Random.Range(panicSpeedMin, panicSpeedMax);

        // 3) Dash toward the 2D target with frame‑by‑frame jitter
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            Vector3 toTarget = (target - transform.position).normalized;

            // jitter in X/Y
            Vector2 jig2D = Random.insideUnitCircle.normalized * jitterIntensity;
            Vector3 jitter = new Vector3(jig2D.x, jig2D.y, 0f);

            Vector3 dir = (toTarget + jitter).normalized;
            transform.position += dir * panicSpeed * Time.deltaTime;
            yield return null;
        }

        // 4) Tiny pause before the next frantic dash
        yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
    }
}
}
