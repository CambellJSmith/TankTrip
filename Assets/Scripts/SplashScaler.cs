using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SplashScaler : MonoBehaviour
{
    public float scaleDuration = 0.5f; // Time for each phase
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Smooth easing curve for transitions
    public float movementSpeed = 2f; // Speed at which the object moves toward Y = 0

    private enum ScaleState { ScaleYUp, ScaleXUp, ScaleYDown, Done }
    private ScaleState currentState = ScaleState.ScaleYUp;

    private Camera mainCam;
    private Renderer rend;

    private float phaseStartTime;
    private Vector3 startScale;
    private Vector3 targetScale;
    private float startY;

    void Start()
    {
        mainCam = Camera.main;
        rend = GetComponent<Renderer>();

        // Initial scale setup
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        startY = transform.position.y; // Remember the initial Y position

        SetupNextPhase();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (currentState == ScaleState.Done) return;

        // Scale the object based on the current state
        float t = (Time.time - phaseStartTime) / scaleDuration;
        float evalT = scaleCurve.Evaluate(Mathf.Clamp01(t));
        transform.localScale = Vector3.Lerp(startScale, targetScale, evalT);

        // Move the object toward Y = 0 as it scales
        transform.position = new Vector3(transform.position.x, Mathf.Lerp(startY, 0f, evalT), transform.position.z);

        if (t >= 1f)
        {
            AdvancePhase();
        }
    }

    void AdvancePhase()
    {
        switch (currentState)
        {
            case ScaleState.ScaleYUp:
                currentState = ScaleState.ScaleXUp;
                break;
            case ScaleState.ScaleXUp:
                currentState = ScaleState.ScaleYDown;
                break;
            case ScaleState.ScaleYDown:
                currentState = ScaleState.Done;
                return;
        }

        SetupNextPhase();
    }

    void SetupNextPhase()
    {
        startScale = transform.localScale;
        targetScale = startScale;

        Bounds bounds = rend.bounds;
        float camHeight = 2f * mainCam.orthographicSize;
        float camWidth = camHeight * mainCam.aspect;

        switch (currentState)
        {
            case ScaleState.ScaleYUp:
                // Smoothly scale up based on Y camera bounds
                float yScaleFactor = camHeight / bounds.size.y;
                targetScale.y *= yScaleFactor;
                break;

            case ScaleState.ScaleXUp:
                // Smoothly scale up based on X camera bounds
                float xScaleFactor = camWidth / bounds.size.x;
                targetScale.x *= xScaleFactor;
                break;

            case ScaleState.ScaleYDown:
                // Smoothly scale down to 25% of the Y camera bounds
                float smallY = camHeight * 0.25f;
                float downScaleFactor = smallY / bounds.size.y;
                targetScale.y *= downScaleFactor;
                break;
        }

        phaseStartTime = Time.time;
    }
}
