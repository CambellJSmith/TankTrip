using UnityEngine;

public class KillCause : MonoBehaviour
{
    // Reference to the SoldierDeath script attached to this object or its parent
    private SoldierDeath soldierDeath;

    // Start is called once before the first execution of Update
    void Start()
    {
        // Attempt to find the SoldierDeath script on this object or its parent
        soldierDeath = GetComponentInParent<SoldierDeath>();
    }

    // Update is called once per frame
    void Update()
    {
        if (soldierDeath != null)
        {
            // Find all objects with the "Killer" tag in the scene
            GameObject[] killers = GameObject.FindGameObjectsWithTag("Killer");

            foreach (GameObject killer in killers)
            {
                // Calculate the distance to each "Killer" object
                float distance = Vector2.Distance(transform.position, killer.transform.position);

                // If the distance is less than or equal to 0.5 units, trigger the death. 0.3 was attempted but was too fine.
                if (distance <= 0.5f)
                {
                    soldierDeath.isDead = true; // Set the death condition to true
                    return; // Exit once we detect the "Killer" object
                }
            }
        }
    }
}
