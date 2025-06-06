using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class RandomText : MonoBehaviour
{
    public Text textObject;
    private string targetName = "CAMBELL JAMES SMITH";
    private string currentText = "";
    private bool[] lockedIn;  // Tracks which characters are locked in

    void Start()
    {
        // Bring parent and self to the top of the UI hierarchy
        if (transform.parent != null) transform.parent.SetAsLastSibling();
        transform.SetAsLastSibling();

        lockedIn = new bool[targetName.Length];
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(DisplayRandomText());
    }

    IEnumerator DisplayRandomText()
    {
        for (int i = 0; i < targetName.Length; i++)
        {
            currentText += RandomCharacter();
        }

        textObject.text = currentText;

        int totalCharacters = targetName.Length;
        int lockedCount = 0;

        // Create a list of indices to control even distribution of correction
        List<int> remainingIndices = new List<int>();
        for (int i = 0; i < targetName.Length; i++) remainingIndices.Add(i);

        while (lockedCount < totalCharacters)
        {
            char[] characters = currentText.ToCharArray();

            // Shuffle remaining indices to avoid front-to-back bias
            ShuffleList(remainingIndices);

            foreach (int i in remainingIndices)
            {
                if (!lockedIn[i])
                {
                    // Random chance to lock in the correct character, scaled over time
                    if (Random.value < 0.05f)
                    {
                        characters[i] = targetName[i];
                        lockedIn[i] = true;
                        lockedCount++;
                    }
                    else
                    {
                        characters[i] = RandomCharacter();
                    }
                }
            }

            currentText = new string(characters);
            textObject.text = currentText;

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Menu");
    }

    private char RandomCharacter()
    {
        int rand = Random.Range(0, 27); // 26 uppercase letters + space
        if (rand < 26)
        {
            return (char)('A' + rand);
        }
        else
        {
            return ' ';
        }
    }

    // Fisher–Yates shuffle
    private void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
