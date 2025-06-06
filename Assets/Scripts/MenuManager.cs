using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;


public class MenuManager : MonoBehaviour
{
    [Header("Ordered Menu Items (Highest Y to Lowest Y)")]
    public MenuItem[] orderedMenuItems = new MenuItem[4];

    [Header("Currently Highlighted Item Index")]
    public int highlightedIndex = -1; // No item highlighted by default

    public bool option0Confirmed = false; //  Flag for external access

    private Vector3 lastMousePosition;
    private float mouseMoveThreshold = 0.01f;
    private bool isMouseMoving = false;

    private SpriteRenderer[] spriteRenderers;
    private GameObject mouseLimit;

    private InputSystem_Actions inputActions;
    private float inputCooldown = 0.25f;
    private float nextInputTime = 0f;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
    }

    void Start()
    {
        GameObject[] foundItems = GameObject.FindGameObjectsWithTag("MenuItem");

        if (foundItems.Length != 4)
        {
            Debug.LogWarning($"Expected 4 menu items with tag 'MenuItem', but found {foundItems.Length}.");
            return;
        }

        // Sort by Y position descending
        System.Array.Sort(foundItems, (a, b) =>
            b.transform.position.y.CompareTo(a.transform.position.y)
        );

        spriteRenderers = new SpriteRenderer[orderedMenuItems.Length];

        for (int i = 0; i < orderedMenuItems.Length; i++)
        {
            orderedMenuItems[i].menuItemObject = foundItems[i];
            spriteRenderers[i] = orderedMenuItems[i].menuItemObject.GetComponent<SpriteRenderer>();

            if (spriteRenderers[i] == null)
            {
                Debug.LogWarning($"Menu item {orderedMenuItems[i].menuItemObject.name} does not have a SpriteRenderer.");
            }
        }

        mouseLimit = GameObject.Find("MouseLimit");
        if (mouseLimit == null)
        {
            Debug.LogError("MouseLimit object not found in the scene!");
        }

        UpdateVisualHighlight();
    }

    void Update()
    {
        Vector3 currentMousePosition = Input.mousePosition;
        isMouseMoving = Vector3.Distance(currentMousePosition, lastMousePosition) > mouseMoveThreshold;

        if (isMouseMoving && mouseLimit != null)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(currentMousePosition);
            mouseWorldPos.z = 0f;

            if (mouseWorldPos.x < mouseLimit.transform.position.x)
            {
                UpdateHighlightToClosestToMouse(mouseWorldPos);
            }
            else
            {
                highlightedIndex = -1;
            }
        }
        else
        {
            HandleStickNavigation();
        }

        lastMousePosition = currentMousePosition;

        if (inputActions.Player.Confirm.triggered)
        {
            HandleConfirmAction();
        }

        UpdateVisualHighlight();
    }

    void UpdateHighlightToClosestToMouse(Vector3 mouseWorldPos)
    {
        float closestDistance = float.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < orderedMenuItems.Length; i++)
        {
            float yDistance = Mathf.Abs(mouseWorldPos.y - orderedMenuItems[i].menuItemObject.transform.position.y);
            if (yDistance <= 0.75f)
            {
                float distance = Vector3.Distance(mouseWorldPos, orderedMenuItems[i].menuItemObject.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }
        }

        highlightedIndex = closestIndex;
    }

    void HandleStickNavigation()
    {
        if (Time.time < nextInputTime) return;

        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        if (moveInput.y > 0.5f)
        {
            if (highlightedIndex == -1) highlightedIndex = 0;

            highlightedIndex = (highlightedIndex - 1 + orderedMenuItems.Length) % orderedMenuItems.Length;
            nextInputTime = Time.time + inputCooldown;
        }
        else if (moveInput.y < -0.5f)
        {
            if (highlightedIndex == -1) highlightedIndex = 0;

            highlightedIndex = (highlightedIndex + 1) % orderedMenuItems.Length;
            nextInputTime = Time.time + inputCooldown;
        }
    }

    void HandleConfirmAction()
    {
        if (highlightedIndex == -1) return;

        string sceneToLoad = orderedMenuItems[highlightedIndex].sceneName;

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            if (highlightedIndex == 0)
            {
                option0Confirmed = true; // ✅ Set flag when option 0 is confirmed
                StartCoroutine(LoadSceneWithDelay(sceneToLoad, 4f)); // ✅ Delay transition
            }
            else
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }

    IEnumerator LoadSceneWithDelay(string sceneName, float delay)
{
    yield return new WaitForSeconds(delay);

    // Force cleanup
    Resources.UnloadUnusedAssets();
    System.GC.Collect();

    // Now load scene cleanly
    SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
}

    void UpdateVisualHighlight()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (spriteRenderers[i] == null) continue;

            spriteRenderers[i].color = (i == highlightedIndex)
                ? new Color(1f, 0.5f, 0.5f)
                : Color.white;
        }
    }

    void OnDestroy()
    {
        inputActions.Disable();
    }
}

[System.Serializable]
public class MenuItem
{
    public GameObject menuItemObject;
    public string sceneName;
}
