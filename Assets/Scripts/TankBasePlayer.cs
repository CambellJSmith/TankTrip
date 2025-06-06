using UnityEngine;
using UnityEngine.InputSystem;

public class TankBasePlayer : MonoBehaviour
{
    private InputSystem_Actions inputSystemActions;  // Reference to your input actions
    private Vector2 moveInput;

    [Header("Movement Settings")]
    public float moveSpeed = -7.5f;  // Speed of the tank movement (adjustable in the Inspector)
    public float rotationSpeed = 250f;  // Speed of rotation (adjustable in the Inspector)

    private void OnEnable()
    {
        // Initialize the input actions class
        inputSystemActions = new InputSystem_Actions();

        // Enable the "Player" action map
        inputSystemActions.Player.Enable();

        // Bind the "Move" action to our input method
        inputSystemActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputSystemActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        // Disable input actions to avoid memory leaks
        inputSystemActions.Player.Disable();
    }

    void Update()
    {
    if (moveInput.magnitude > 0.1f)
    {
        float targetAngle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg + 90;
        float currentAngle = transform.eulerAngles.z;

        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

        // If angle difference is between 140 and 220 degrees, skip rotation and move backward
        if (Mathf.Abs(angleDifference) >= 140f && Mathf.Abs(angleDifference) <= 220f)
        {
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
            return; // Skip rotation
        }

        // Smoothly rotate towards the target angle
        float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Move forward or backward based on angle difference
        if (Mathf.Abs(angleDifference) < 90f)
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        }
        else if (Mathf.Abs(angleDifference) > 270f)
        {
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
        }
        }
    }


}
