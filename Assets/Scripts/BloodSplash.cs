using UnityEngine;

public class BloodSplash : MonoBehaviour
{
    private Camera mainCamera;
    private Vector3 targetScale = Vector3.one * 2f;
    private float scaleDuration = 0.5f;
    private SpriteRenderer spriteRenderer;

    private bool isFadingOut = false;

    void Start()
    {
        transform.localScale = Vector3.one * 0.1f;
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Find the closest "Killer" tagged object
        GameObject killer = FindClosestKiller();

        if (killer != null)
        {
            // Stretch the sprite along the line from the killer to this object, away from the killer
            StretchAwayFromKiller(killer);
        }

        StartCoroutine(ScaleOverTime());
    }

    void Update()
    {
        if (!IsVisibleFrom(mainCamera) && !isFadingOut)
        {
            StartCoroutine(FadeOut());
        }
    }

    private System.Collections.IEnumerator ScaleOverTime()
    {
        float timeElapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (timeElapsed < scaleDuration)
        {
            float t = timeElapsed / scaleDuration;
            // Ease out: starts fast, ends slow
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);
            transform.localScale = Vector3.Lerp(startScale, targetScale, smoothT);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }

    private bool IsVisibleFrom(Camera camera)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(transform.position);
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
               viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
               viewportPoint.z > 0;
    }

    private GameObject FindClosestKiller()
    {
        // Find all objects tagged with "Killer"
        GameObject[] killers = GameObject.FindGameObjectsWithTag("Killer");
        GameObject closestKiller = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject killer in killers)
        {
            float distance = Vector3.Distance(transform.position, killer.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestKiller = killer;
            }
        }

        return closestKiller;
    }

    private void StretchAwayFromKiller(GameObject killer)
    {
        // Find the direction vector from the killer to this object
        Vector3 direction = transform.position - killer.transform.position;
        float distance = direction.magnitude;

        // Normalize the direction vector so it only affects the scaling
        direction.Normalize();

        // Stretch the object away from the "Killer" along the direction
        // We stretch along the Y-axis, but you can adjust it for X or Z if needed
        Vector3 scale = new Vector3(1f, distance, 1f); // Stretch along the Y-axis
        transform.localScale = scale;

        // Optionally, rotate the object to align with the line between the killer and this object
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private System.Collections.IEnumerator FadeOut()
    {
        isFadingOut = true;

        float fadeDuration = 1f; // Time it takes to fade out
        float timeElapsed = 0f;
        Color startColor = spriteRenderer.color;

        while (timeElapsed < fadeDuration)
        {
            float t = timeElapsed / fadeDuration;
            // Linear fade out
            Color newColor = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0f, t));
            spriteRenderer.color = newColor;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure fully transparent
        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        // Destroy the object after fading out
        Destroy(gameObject);
    }
}
