using UnityEngine;

public class CamZoomMenu : MonoBehaviour
{
    // Reference to MenuManager
    public MenuManager menuManager; // Make this public to assign it in the Inspector

    // Reference to the camera component
    private Camera mainCamera;

    // Reference to the target object (CamTarget)
    private GameObject camTarget;

    // Variables for the zoom effect
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float startSize;
    private float targetSize;
    private float journeyLength;
    private float startTime;
    private bool isZooming = false;

    // Time for the transition
    private const float transitionDuration = 4f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (menuManager == null)
        {
            Debug.LogError("MenuManager reference is not assigned in the Inspector!");
            return;
        }

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found in the scene!");
        }

        // Find CamTarget using tag
        camTarget = GameObject.FindGameObjectWithTag("CamTarget");
        if (camTarget == null)
        {
            Debug.LogError("CamTarget object not found in the scene using tag!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (menuManager != null && camTarget != null)
        {
            // Check if option0Confirmed is true and start zooming
            if (menuManager.option0Confirmed && !isZooming)
            {
                // Start the zooming process
                StartZoom();
            }

            // Perform the zooming if the process is active
            if (isZooming)
            {
                PerformZoom();
            }
        }
    }

    void StartZoom()
    {
        // Initialize variables for the zooming transition
        startPosition = mainCamera.transform.position;
        targetPosition = camTarget.transform.position;
        
        // Calculate the target camera size based on the CamTarget's radius
        float targetRadius = camTarget.GetComponent<SpriteRenderer>().bounds.extents.x;
        targetSize = targetRadius * 2f * mainCamera.aspect;

        // Add more dramatic size reduction at the end
        targetSize *= 0.1f;  // Reduce the final size by a factor for more zoom-in effect.

        startSize = mainCamera.orthographicSize;

        // Calculate the journey length and start time
        journeyLength = Vector3.Distance(startPosition, targetPosition);
        startTime = Time.time;

        // Set the zooming flag to true
        isZooming = true;
    }

    void PerformZoom()
    {
        // Calculate how much time has passed since the start of the zoom
        float distanceCovered = (Time.time - startTime) * (journeyLength / transitionDuration);
        float fractionOfJourney = distanceCovered / journeyLength;

        // Apply an ease-in-out curve for speed ramping (using a cubic ease function)
        float easedFraction = Mathf.Pow(fractionOfJourney, 3) / (Mathf.Pow(fractionOfJourney, 3) + Mathf.Pow(1 - fractionOfJourney, 3));

        // Move the camera towards the target position with eased speed
        mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, easedFraction);

        // Adjust the camera size with easing
        mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, easedFraction);

        // If the zoom is complete, stop the transition
        if (easedFraction >= 1f)
        {
            isZooming = false;
        }
    }
}
