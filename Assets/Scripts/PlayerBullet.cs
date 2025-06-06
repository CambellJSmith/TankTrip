using UnityEngine;
using System.Collections;

public class PlayerBullet : MonoBehaviour
{
    public GameObject explosionPrefab; // Assign in Inspector
    public float speed = 20f;

    private bool detectionActive = false;

    void Start()
    {
        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Match position
            transform.position = player.transform.position;

            // Match Z rotation with 180 degree flip
            float playerZ = player.transform.eulerAngles.z;
            transform.rotation = Quaternion.Euler(0, 0, playerZ + 180f);

            // Move slightly forward along local Z
            transform.position += transform.forward * 0.001f;
        }

        // Start delayed detection activation
        StartCoroutine(EnableDetectionAfterDelay(0.005f));
    }

    void Update()
    {
        // Move forward along local Y axis
        transform.Translate(Vector3.up * speed * Time.deltaTime, Space.Self);

        // Destroy if out of screen bounds
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
            if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
            {
                Destroy(gameObject);
                return;
            }
        }

        if (detectionActive)
        {
            GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject obj in allObjects)
            {
                if (obj == gameObject || obj.CompareTag("Player")) continue;

                if (obj.GetComponent<PlayerShootable>() == null) continue;

                Vector3 screenPoint = mainCamera.WorldToViewportPoint(obj.transform.position);
                if (screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1)
                {
                    float distance = Vector3.Distance(transform.position, obj.transform.position);
                    if (distance <= 0.5f)
                    {
                        if (explosionPrefab != null)
                        {
                            float collidedZ = obj.transform.position.z;
                            Vector3 explosionPosition = new Vector3(transform.position.x, transform.position.y, collidedZ - 0.01f);
                            Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);
                        }
                        Destroy(gameObject);
                        break;
                    }
                }
            }
        }
    }

    IEnumerator EnableDetectionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        detectionActive = true;
    }
}
