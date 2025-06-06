using UnityEngine;
using System.Collections;

public class BombSpawn : MonoBehaviour
{
    [SerializeField] private GameObject bombPrefab; // Assign in the Unity Editor
    [SerializeField] private int numberOfWaves = 20;
    [SerializeField] private float delayBeforeStart = 5f;
    [SerializeField] private float delayBetweenWaves = 0.2f;
    [SerializeField] private int bombsPerCluster = 20;
    [SerializeField] private float clusterRadius = 4f;

    void Start()
    {
        StartCoroutine(SpawnSequence());
    }

    private IEnumerator SpawnSequence()
    {
        // Initial delay before first wave
        yield return new WaitForSeconds(delayBeforeStart);

        for (int i = 0; i < numberOfWaves; i++)
        {
            SpawnBombCluster();
            yield return new WaitForSeconds(delayBetweenWaves);
        }

        Destroy(gameObject);
    }

    private void SpawnBombCluster()
    {
        for (int i = 0; i < bombsPerCluster; i++)
        {
            Vector3 randomOffset = Random.insideUnitCircle * clusterRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
            Instantiate(bombPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
