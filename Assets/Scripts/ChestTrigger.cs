using UnityEngine;

public class ChestTrigger : MonoBehaviour
{
    public GameObject prefabToInstantiate; // Prefab to instantiate if roll is 1
    public GameObject[] randomPrefabs;     // Array of prefabs to choose from if roll is 2

    private GameObject crushPoint;

    void Start()
    {
        // Dynamically find the "ChrushPoint" GameObject by name
        crushPoint = GameObject.Find("CrushPoint");

        if (crushPoint == null)
        {
            Debug.LogWarning("ChrushPoint not found in the scene.");
        }
    }

    void Update()
    {
        if (crushPoint != null)
        {
            float distance = Vector3.Distance(transform.position, crushPoint.transform.position);
            if (distance <= 1f)
            {
                int roll = Random.Range(1, 3); // Generates 1 or 2

                if (roll == 1)
                {
                    Instantiate(prefabToInstantiate, transform.position, Quaternion.identity);
                }
                else if (roll == 2 && randomPrefabs.Length > 0)
                {
                    int randomIndex = Random.Range(0, randomPrefabs.Length);
                    Instantiate(randomPrefabs[randomIndex], transform.position, Quaternion.identity);
                }

                // Destroy this GameObject after instantiation
                Destroy(gameObject);
            }
        }
    }
}
