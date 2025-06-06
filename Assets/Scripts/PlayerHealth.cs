using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int playerHealth = 100;
    public int maxHealth = 100;
    public int maxLives = 3;
    public GameObject deathEffectPrefab;

    private int currentLives;
    private float maxScaleX = 6f;
    private float targetScaleX = 6f;

    private Vector3 initialScale;
    private Vector3 initialPosition;
    private float width;
    private SpriteRenderer sr;

    private Color currentTargetColor;
    private float flashTimer;
    private bool flashWhite;

    private bool isInvincible = false;
    private float invincibilityTimer = 0f;
    private const float invincibilityDuration = 6f;

    private InputSystem_Actions inputSystemActions;

    void OnEnable()
    {
        inputSystemActions = new InputSystem_Actions();
        inputSystemActions.Player.Enable();

        inputSystemActions.Player.PlaneBomb.performed += ctx => TriggerInvincibility();
        inputSystemActions.Player.PlaneSoldier.performed += ctx => TriggerInvincibility();
    }

    void OnDisable()
    {
        inputSystemActions.Player.Disable();
    }

    void Start()
    {
        currentLives = maxLives;

        transform.localScale = new Vector3(maxScaleX, transform.localScale.y, transform.localScale.z);
        initialScale = transform.localScale;
        initialPosition = transform.position;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            width = sr.bounds.size.x;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer not found!");
        }
    }

    void Update()
    {
        // Handle invincibility countdown
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
            }
        }

        // Clamp health and calculate scale
        playerHealth = Mathf.Clamp(playerHealth, 0, maxHealth);
        targetScaleX = ((float)playerHealth / maxHealth) * maxScaleX;
        transform.localScale = new Vector3(targetScaleX, initialScale.y, initialScale.z);

        // Get the child sprite renderer (assuming the child has one)
        SpriteRenderer childSR = transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (childSR != null)
        {
            float normalizedScale = targetScaleX / maxScaleX;
            Color green = Color.green;
            Color yellow = new Color(1f, 0.85f, 0f); // vibrant sunflower yellow
            Color red = Color.red;

            if (normalizedScale >= 0.5f)
            {
                float t = Mathf.InverseLerp(1f, 0.5f, normalizedScale);
                currentTargetColor = Color.Lerp(green, yellow, t);
            }
            else
            {
                float t = Mathf.InverseLerp(0.5f, 0f, normalizedScale);
                currentTargetColor = Color.Lerp(yellow, red, t);
            }

            if (normalizedScale < 0.1f)
            {
                flashTimer += Time.deltaTime;
                if (flashTimer >= 0.1f)
                {
                    flashTimer = 0f;
                    flashWhite = !flashWhite;
                }

                childSR.color = flashWhite ? Color.white : currentTargetColor;
            }
            else
            {
                childSR.color = currentTargetColor;
                flashTimer = 0f;
                flashWhite = false;
            }
        }
        else
        {
            Debug.LogWarning("SpriteRenderer not found on child object!");
        }

        // Handle death
        if (playerHealth <= 0)
        {
            if (currentLives > 0)
            {
                currentLives--;
                playerHealth = maxHealth;

                if (deathEffectPrefab != null)
                {
                    Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
                }
            }
            else
            {
                SceneManager.LoadScene("Menu");
            }
        }
    }

    public void ChangeHealth(int amount)
    {
        if (isInvincible) return;

        playerHealth += amount;
        playerHealth = Mathf.Clamp(playerHealth, 0, maxHealth);
    }

    public int GetHealth()
    {
        return playerHealth;
    }

    private void TriggerInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
    }
}
