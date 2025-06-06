using UnityEngine;
using UnityEngine.InputSystem;

public class TankAttacks : MonoBehaviour
{
    public GameObject planeBombPrefab;      // Reference to the PlaneBomb prefab (to be set in Unity Editor)
    public GameObject planeSoldierPrefab;   // Reference to the PlaneSoldier prefab (to be set in Unity Editor)
    public GameObject additionalPrefab;     // The additional prefab to spawn (to be set in Unity Editor)
    public GameObject tankShellPrefab;      // Reference to the TankShell prefab (to be set in Unity Editor)

    private InputSystem_Actions inputActions;  // Reference to the generated InputSystem_Actions class
    private InputSystem_Actions.PlayerActions playerActions;  // Reference to the Player action map inside InputSystem_Actions

    private void Awake()
    {
        // Initialize the input actions class
        inputActions = new InputSystem_Actions();

        // Get the PlayerActions map from the InputSystem_Actions class
        playerActions = inputActions.Player;

        // Bind actions to handlers
        playerActions.PlaneBomb.performed += ctx => HandlePlaneBomb();
        playerActions.PlaneSoldier.performed += ctx => HandlePlaneSoldier();
        playerActions.TankShell.performed += ctx => HandleTankShell();  // New input binding
    }

    private void OnEnable()
    {
        // Enable input actions
        inputActions.Enable();
    }

    private void OnDisable()
    {
        // Disable input actions to avoid memory leaks
        inputActions.Disable();
    }

    private void HandlePlaneBomb()
    {
        if (GameManager.Instance.score >= 100f)
        {
            Instantiate(planeBombPrefab, transform.position, Quaternion.identity);
            Instantiate(additionalPrefab, transform.position, Quaternion.identity);
            GameManager.Instance.ModifyScore(-100f);
        }
    }

    private void HandlePlaneSoldier()
    {
        if (GameManager.Instance.score >= 100f)
        {
            Instantiate(planeSoldierPrefab, transform.position, Quaternion.identity);
            Instantiate(additionalPrefab, transform.position, Quaternion.identity);
            GameManager.Instance.ModifyScore(-100f);
        }
    }

    private void HandleTankShell()
    {
        if (GameManager.Instance.score >= 100f)
        {
            Instantiate(tankShellPrefab, transform.position, Quaternion.identity);
            GameManager.Instance.ModifyScore(-1f);
        }
    }
}
