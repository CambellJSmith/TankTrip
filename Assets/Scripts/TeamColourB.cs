using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class TeamColourB : MonoBehaviour
{
    [SerializeField]
    private Color teamBColour;

    private void Start()
    {
        StartCoroutine(ApplyColorAfterDelay());
    }

    private IEnumerator ApplyColorAfterDelay()
    {
        yield return new WaitForSeconds(0.001f); // 1 millisecond delay

        teamBColour = GameManager.Instance.teamBColor;
        GetComponent<SpriteRenderer>().color = teamBColour;
    }
}
