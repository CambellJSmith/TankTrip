using UnityEngine;
using UnityEngine.InputSystem;

public class TankTurretPlayer : MonoBehaviour
{
    private InputSystem_Actions inputSystemActions;
    private Vector2 lookInput;
    private Vector3 lastMousePosition;
    private Vector3 targetDirection;

    [Header("Turret Settings")]
    public float rotationSpeed = 250f;
    public float mouseMovementThreshold = 5f; // Threshold for significant mouse movement
    public float rotationThreshold = 1f; // Threshold for considering the target reached

    private Camera mainCamera;

    private void Awake()
    {
        // Cache the main camera reference directly in Awake
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("No camera found tagged as 'MainCamera'. Please tag your main camera accordingly.");
        }
    }

    private void OnEnable()
    {
        inputSystemActions = new InputSystem_Actions();
        inputSystemActions.Player.Enable();

        inputSystemActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputSystemActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        // Initialize the last mouse position and target direction
        lastMousePosition = Mouse.current.position.ReadValue();
        targetDirection = transform.up; // Start by looking straight up
    }

    private void OnDisable()
    {
        inputSystemActions.Player.Disable();
    }

    private void Update()
    {
        if (mainCamera == null) return;

        // Track mouse movement and update target direction if there's significant movement
        Vector3 currentMousePosition = Mouse.current.position.ReadValue();
        if (Vector3.Distance(currentMousePosition, lastMousePosition) > mouseMovementThreshold)
        {
            lastMousePosition = currentMousePosition;

            // Only update target direction based on mouse if the "Look" input is not active
            if (lookInput.magnitude <= 0.1f)
            {
                Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(currentMousePosition);
                mouseWorldPos.z = transform.position.z;
                targetDirection = mouseWorldPos - transform.position;
            }
        }

        // Update target direction based on look input (controller or joystick)
        if (lookInput.magnitude > 0.1f)
        {
            targetDirection = new Vector3(lookInput.x, lookInput.y, 0f);
        }

        // Only rotate if target direction has a significant magnitude
        if (targetDirection.sqrMagnitude > 0.01f)
        {
            // Normalize the direction to avoid scale issues
            targetDirection.Normalize();

            // Calculate the angle between current and target direction
            float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg + 90;
            float currentAngle = transform.eulerAngles.z;
            float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

            // Apply the rotation
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Stop rotating if the angle difference is small (within a threshold)
            if (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) <= rotationThreshold)
            {
                targetDirection = Vector3.zero; // Stop rotating once the target is reached
            }
        }
    }
}
