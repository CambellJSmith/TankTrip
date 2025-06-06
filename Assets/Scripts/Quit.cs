using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For Legacy Text

public class Quit : MonoBehaviour
{
    public Text countdownText; // Assign in Inspector
    private float countdown = 5f;
    private bool inputDetected = false;
    private bool hasSceneChanged = false;
    private bool quitting = false;

    void Start()
    {
        if (countdownText != null)
            countdownText.text = countdown.ToString("0");
    }

    void Update()
    {
        // Detect any key, button, or axis
        if (!inputDetected && AnyInputDetected())
        {
            inputDetected = true;
            CancelInvoke(); // Cancel any pending quits just in case
            hasSceneChanged = true;
            SceneManager.LoadScene("Menu");
            return; // Stop processing further
        }

        // Countdown logic only if no input happened
        if (!inputDetected && !quitting)
        {
            countdown -= Time.deltaTime;
            if (countdownText != null)
                countdownText.text = Mathf.Ceil(countdown).ToString("0");

            if (countdown <= 0)
            {
                quitting = true;
                Invoke("QuitApplication", 1f);
            }
        }
    }

    bool AnyInputDetected()
    {
        if (Input.anyKey) return true;

        string[] axes = { "Horizontal", "Vertical", "Mouse X", "Mouse Y", "Fire1", "Fire2", "Fire3", "Jump" };
        foreach (string axis in axes)
        {
            if (Mathf.Abs(Input.GetAxis(axis)) > 0.01f) return true;
        }

        return false;
    }

    void QuitApplication()
    {
        if (hasSceneChanged) return; // Safety check — don’t quit if we already changed scenes
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
