using UnityEngine;
using System.Collections;

public class EnemySoldierSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float initialInterval = 3f;
    public float minSpawnInterval = 0.25f;
    public int minSpawnCount = 5;
    public int maxSpawnCount = 10;

    private float spawnInterval;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        spawnInterval = initialInterval;
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            int groupCount = Random.Range(1, 4); // Number of spawn bursts per cycle (1 to 3)
            for (int g = 0; g < groupCount; g++)
            {
                int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1);
                Vector3 spawnPosition = GetRandomPositionOutsideCameraBounds();

                for (int i = 0; i < spawnCount; i++)
                {
                    Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                }

                yield return new WaitForSeconds(0.25f); // Small delay between group bursts
            }

            // Short pause between cycles
            yield return new WaitForSeconds(spawnInterval);

            // Ramp up difficulty
            spawnInterval = Mathf.Max(spawnInterval * 0.95f, minSpawnInterval);
        }
    }

    Vector3 GetRandomPositionOutsideCameraBounds()
    {
        Vector3 cameraBoundsMin = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 cameraBoundsMax = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        bool spawnOutsideX = Random.value > 0.5f;
        bool spawnOutsideY = Random.value > 0.5f;

        float xPos = spawnOutsideX
            ? Random.Range(cameraBoundsMax.x + 1f, cameraBoundsMax.x + 10f)
            : Random.Range(cameraBoundsMin.x - 10f, cameraBoundsMin.x - 1f);

        float yPos = spawnOutsideY
            ? Random.Range(cameraBoundsMax.y + 1f, cameraBoundsMax.y + 10f)
            : Random.Range(cameraBoundsMin.y - 10f, cameraBoundsMin.y - 1f);

        return new Vector3(xPos, yPos, 0);
    }
}
