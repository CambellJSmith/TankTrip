using UnityEngine;

public class SkinColour : MonoBehaviour
{
    [Header("Skin Color Options (From GameManager)")]
    public Color skinColorLight; // Exposed to see in Inspector
    public Color skinColorDark;  // Exposed to see in Inspector

    [Header("Gold Color Option")]
    public Color goldColor = new Color(1f, 0.84f, 0f); // Bright Gold (RGB)

    [Header("Current Skin Color Info")]
    public Color chosenColor; // The color selected for this object
    public bool isGold = false; // Whether the gold color is being used

    private void Start()
    {
        // Fetch skin colors from GameManager and expose them
        skinColorLight = GameManager.Instance.skinColorLight;
        skinColorDark = GameManager.Instance.skinColorDark;

        // 1 in 1000 chance for bright gold
        if (Random.Range(0, 1000) == 0)
        {
            chosenColor = goldColor;
            isGold = true;
        }
        else
        {
            // Pick randomly between light and dark skin colors from GameManager
            bool useLightSkin = Random.value < 0.5f;
            chosenColor = useLightSkin ? skinColorLight : skinColorDark;
            isGold = false;
        }

        // Apply to SpriteRenderer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = chosenColor;
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on " + gameObject.name);
        }
    }
}
