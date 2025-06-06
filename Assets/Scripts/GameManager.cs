using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Team Colors")]
    public Color teamAColor = Color.white;
    public Color teamBColor = Color.white;

    [Header("Skin Colors")]
    public Color skinColorLight = new Color(1.0f, 0.85f, 0.75f); // Pale Peach
    public Color skinColorDark = new Color(0.36f, 0.20f, 0.09f);  // Chocolate Brown

    [Header("Score")]
    public float score = 200f;  // Initialize the score at 200

    [Header("Enemies")]
    public static int EnemyCount { get; private set; } = 0;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional
    }

    private void Start()
    {
        GenerateTeamColorsIfUnset();
        DisplayScore(); // Display the initial score
    }

    private void GenerateTeamColorsIfUnset()
    {
        if (teamAColor == Color.white || teamBColor == Color.white)
        {
            Color colorA = RandomBrightColor();
            Color colorB;

            do
            {
                colorB = RandomBrightColor();
            }
            while (AreColorsTooSimilar(colorA, colorB));

            teamAColor = colorA;
            teamBColor = colorB;
        }

        Debug.Log($"Team A Color: {ColorUtility.ToHtmlStringRGB(teamAColor)}");
        Debug.Log($"Team B Color: {ColorUtility.ToHtmlStringRGB(teamBColor)}");
    }

    private Color RandomBrightColor()
    {
        float hue = Random.Range(0f, 1f);
        float saturation = 1f;
        float value = 1f;
        return Color.HSVToRGB(hue, saturation, value);
    }

    private bool AreColorsTooSimilar(Color a, Color b)
    {
        float threshold = 0.4f;
        return Vector3.Distance(new Vector3(a.r, a.g, a.b), new Vector3(b.r, b.g, b.b)) < threshold;
    }

    // Method to modify the score
    public void ModifyScore(float amount)
    {
        score += amount;
        DisplayScore();
    }

    // Method to display the score (for example, in the console or UI)
    private void DisplayScore()
    {
        Debug.Log($"Score: {score}");
    }

    // Enemy count management
    public static void RegisterEnemy()
    {
        EnemyCount++;
        Debug.Log($"Enemy spawned. Total enemies: {EnemyCount}");
    }

    public static void UnregisterEnemy()
    {
        EnemyCount = Mathf.Max(0, EnemyCount - 1);
        Debug.Log($"Enemy destroyed. Remaining enemies: {EnemyCount}");
    }
}
