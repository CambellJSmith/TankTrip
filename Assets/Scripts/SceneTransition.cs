using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneTransition : MonoBehaviour
{
    private InputSystem_Actions inputActions;  // Reference to the InputSystem_Actions class
    private InputAction declineAction;

    private void Awake()
    {
        // Dynamically create and assign inputActions from the asset
        inputActions = new InputSystem_Actions();  // Create a new instance of the Input Actions
    }

    private void OnEnable()
    {
        // Ensure inputActions is assigned
        if (inputActions != null)
        {
            inputActions.Enable();
            declineAction = inputActions.Player.Decline;  // Assuming "Player" is the action map and "Decline" is the action name

            if (declineAction != null)
            {
                declineAction.performed += OnDeclinePressed;  // Register the event for when the decline action is performed
            }
            else
            {
                Debug.LogError("Decline action is not properly assigned.");
            }
        }
        else
        {
            Debug.LogError("InputActions is not assigned.");
        }
    }

    private void OnDisable()
    {
        // Ensure inputActions and declineAction are properly assigned before trying to disable them
        if (inputActions != null && declineAction != null)
        {
            inputActions.Disable();
            declineAction.performed -= OnDeclinePressed;
        }
    }

    // This method will be called when the "Decline" action is triggered
    private void OnDeclinePressed(InputAction.CallbackContext context)
    {
        // Load the "Menu" scene and reset it by ensuring no other scene stays loaded
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);  // Single mode ensures the old scene is unloaded and the new one starts fresh
    }
}
