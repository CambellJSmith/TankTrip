using UnityEngine;
using System.Collections.Generic;

public class PlaneFly : MonoBehaviour
{
    public float speed = 3f;
    private Transform player;
    private Vector3 moveDirection;
    private float lifetime = 7f;
    private float timer;

    // Static set to track followed planes
    private static HashSet<PlaneFly> followedPlanes = new HashSet<PlaneFly>();

    public bool HasBeenFollowed => followedPlanes.Contains(this);

    public void MarkAsFollowed()
    {
        followedPlanes.Add(this);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player object not found!");
            return;
        }

        Vector2 randomDir2D = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = player.position + new Vector3(randomDir2D.x, randomDir2D.y, 0f) * 15f;

        transform.position = spawnPosition;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
        moveDirection = directionToPlayer;
        timer = 0f;
    }

    void Update()
    {
        if (player == null) return;

        transform.position += moveDirection * speed * Time.deltaTime;
        timer += Time.deltaTime;

        if (timer >= lifetime && !IsVisibleToCamera(Camera.main))
        {
            Destroy(gameObject);
        }
    }

    bool IsVisibleToCamera(Camera cam)
    {
        if (cam == null) return false;
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        return GeometryUtility.TestPlanesAABB(planes, GetComponent<Renderer>().bounds);
    }
}
