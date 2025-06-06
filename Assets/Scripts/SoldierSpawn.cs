using UnityEngine;
using System.Collections;

public class SoldierSpawn : MonoBehaviour
{
    [SerializeField] private GameObject soldierPrefab; // Assign this in the Unity Editor

    void Start()
    {
        StartCoroutine(SpawnSoldiersAfterDelay());
    }

    private IEnumerator SpawnSoldiersAfterDelay()
    {
        // Wait 5 seconds before spawning
        yield return new WaitForSeconds(5f);
        SpawnSoldierCluster();

        // Destroy this GameObject
        Destroy(gameObject);
    }

    private void SpawnSoldierCluster()
    {
        for (int i = 0; i < 5; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * 1.25f;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
            Instantiate(soldierPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
