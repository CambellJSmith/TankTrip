using UnityEngine;
using TMPro; // Only needed if you're using TextMeshPro

public class FloatFade : MonoBehaviour
{
    public float duration = 2f;
    public float moveAmount = 1f;
    public float scaleAmount = 1.5f;

    private float timer = 0f;
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 startScale;
    private Vector3 endScale;
    private TextMeshProUGUI tmpText; // For TextMeshPro. If you're using regular UI Text, change accordingly.
    private Color startColor;

    void Start()
    {
        startPos = transform.position;
        endPos = startPos + new Vector3(0f, moveAmount, 0f);

        startScale = transform.localScale;
        endScale = startScale * scaleAmount;

        tmpText = GetComponent<TextMeshProUGUI>();
        if (tmpText != null)
        {
            startColor = tmpText.color;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        float easeOutT = 1f - Mathf.Pow(1f - t, 3); // Cubic ease-out

        // Move and scale
        transform.position = Vector3.Lerp(startPos, endPos, easeOutT);
        transform.localScale = Vector3.Lerp(startScale, endScale, easeOutT);

        // Fade
        if (tmpText != null)
        {
            Color newColor = startColor;
            newColor.a = Mathf.Lerp(startColor.a, 0f, easeOutT);
            tmpText.color = newColor;
        }

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
