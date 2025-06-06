using UnityEngine;
using UnityEngine.InputSystem;

public class CamComply : MonoBehaviour
{
    private Camera mainCam;
    private Bounds combinedBounds;
    private const int TopSortingOrder = 32767; // Max value to ensure top render layer
    private bool isSliding = false;  // Track if the object is sliding

    private Vector3 initialPosition;
    private Vector3 initialScale;
    private Quaternion initialRotation;

    private InputSystem_Actions inputActions; // Assuming you have this from the Unity Input System

    // Add a reference to the MenuManager (or whatever component tracks highlightedIndex)
    public MenuManager menuManager; // Reference to MenuManager script

    private float currentSlideSpeed = 0f; // Current speed of the slide
    private const float maxSlideSpeed = 10f; // Maximum slide speed
    private const float slideRampUpSpeed = 3f; // Speed at which the slide speed ramps up
    private const float slideMovementSpeed = 5f; // Base movement speed for sliding

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Confirm.performed += OnConfirmTriggered; // Listen for the "confirm" action
    }

    void OnDisable()
    {
        inputActions.Disable();
        inputActions.Player.Confirm.performed -= OnConfirmTriggered;
    }

    void Start()
    {
        mainCam = Camera.main;
        if (mainCam == null) return;

        CalculateCombinedBounds();

        float camHeight = 2f * mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;

        float objectHeight = combinedBounds.size.y;
        if (objectHeight <= 0f) return;

        float scaleFactor = (camHeight * 0.75f) / objectHeight;

        transform.localScale = Vector3.one * scaleFactor;

        CalculateCombinedBounds();

        float cameraLeftEdge = mainCam.transform.position.x - camWidth / 2f;
        float xOffset = combinedBounds.min.x - transform.position.x;
        float newX = cameraLeftEdge - xOffset;

        float cameraYCenter = mainCam.transform.position.y;
        float objectYCenterOffset = combinedBounds.center.y - transform.position.y;
        float newY = cameraYCenter - objectYCenterOffset;

        transform.position = new Vector3(newX, newY, transform.position.z);

        SetTopRenderLayers();
    }

    void CalculateCombinedBounds()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            combinedBounds = new Bounds(transform.position, Vector3.zero);
            return;
        }

        combinedBounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            combinedBounds.Encapsulate(rend.bounds);
        }
    }

    void SetTopRenderLayers()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "Canvas")
            {
                Canvas canvas = child.GetComponent<Canvas>();
                if (canvas != null)
                {
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = TopSortingOrder;
                }
            }
            else if (child.name == "Text (Legacy)")
            {
                Renderer rend = child.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.sortingOrder = TopSortingOrder;
                }
            }
        }
    }

    // Triggered when the confirm action is pressed
    void OnConfirmTriggered(InputAction.CallbackContext context)
    {
        // Check if there is a highlighted item (highlightedIndex should not be -1)
        if (menuManager.highlightedIndex == -1)
            return; // If nothing is highlighted, do nothing

        if (isSliding)
            return; // If already sliding, don't trigger again

        StartSliding(); // Start the sliding behavior
    }

    void StartSliding()
    {
        isSliding = true;

        // Optionally, you could also store the initial position, scale, and rotation if you need them for later use.
        initialPosition = transform.position;
        initialScale = transform.localScale;
        initialRotation = transform.rotation;

        // Disable movement, scaling, and rotation temporarily
        foreach (Transform child in transform)
        {
            // Disable any possible script that controls movement
            var movementScript = child.GetComponent<MonoBehaviour>(); 
            if (movementScript != null)
            {
                movementScript.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isSliding)
        {
            // Ramp up the speed gradually
            currentSlideSpeed = Mathf.Min(currentSlideSpeed + slideRampUpSpeed * Time.deltaTime, maxSlideSpeed);

            // Move the object to the left based on the current speed
            transform.position += Vector3.left * currentSlideSpeed * Time.deltaTime; 
        }
    }

    // Optionally, you can add an unfreeze method if needed.
    public void UnfreezeObject()
    {
        isSliding = false;

        // Re-enable movement, scaling, and rotation by re-enabling the relevant scripts
        foreach (Transform child in transform)
        {
            var movementScript = child.GetComponent<MonoBehaviour>();
            if (movementScript != null)
            {
                movementScript.enabled = true;
            }
        }
    }
}
