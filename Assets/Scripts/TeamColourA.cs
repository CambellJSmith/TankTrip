using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class TeamColourA : MonoBehaviour
{
    [SerializeField]
    private Color teamAColour;

    private void Start()
    {
        StartCoroutine(ApplyColorAfterDelay());
    }

    private IEnumerator ApplyColorAfterDelay()
    {
        yield return new WaitForSeconds(0.001f); // 1 millisecond delay

        teamAColour = GameManager.Instance.teamAColor;
        GetComponent<SpriteRenderer>().color = teamAColour;
    }
}
