using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Credits : MonoBehaviour
{
    // Define a structure to store Role and a list of Names
    [System.Serializable]
    public struct CreditEntry
    {
        public string role;
        public string[] names;  // Array to store an unlimited number of names
    }

    // Store all role-name pairs
    public CreditEntry[] credits;

    // Reference to the Text component
    private Text creditText;
    private int currentCreditIndex = 0;

    void Start()
    {
        // Get the Text component attached to this GameObject
        creditText = GetComponent<Text>();

        // Ensure there's a Text component attached to the object
        if (creditText == null)
        {
            Debug.LogError("No Text component attached to this GameObject.");
            return;
        }

        // Enable rich text on the Text component for font size control
        creditText.supportRichText = true;

        // Start with black text
        creditText.color = Color.black;

        // Start the credit display process
        StartCoroutine(DisplayCredits());
    }

    IEnumerator DisplayCredits()
    {
        while (true)  // Infinite loop to display credits forever
        {
            // Get the current CreditEntry to display
            CreditEntry currentEntry = credits[currentCreditIndex];
            string formattedText = "<size=3>" + currentEntry.role + "</size>\n";  // Larger font for the role
            
            // Add separator line
            formattedText += "- - -\n";

            // Add the names under the role
            foreach (var name in currentEntry.names)
            {
                formattedText += name + "\n";
            }

            // Fade out the current text (to black) before switching
            yield return StartCoroutine(FadeTextToColor(Color.black));

            // Update the text with the next credit entry
            creditText.text = formattedText;

            // Fade back to white
            yield return StartCoroutine(FadeTextToColor(Color.white));

            // Wait for 2.5 seconds before showing the next credit entry
            yield return new WaitForSeconds(2.5f);

            // Move to the next credit entry, looping back to the start if needed
            currentCreditIndex = (currentCreditIndex + 1) % credits.Length;
        }
    }

    IEnumerator FadeTextToColor(Color targetColor)
    {
        float duration = 0.5f;  // Fade duration
        Color startColor = creditText.color;  // Get the current color of the text

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            // Lerp between the current color and the target color
            creditText.color = Color.Lerp(startColor, targetColor, t / duration);
            yield return null;
        }

        // Ensure we end at the exact target color
        creditText.color = targetColor;
    }
}
