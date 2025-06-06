using UnityEngine;
using System.Collections;

public class SoldierSounds : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] public string deathLinesPath = "Assets/AudioClips/DeathLines";  // Path to the DeathLines folder
    [SerializeField] public string panicLinesPath = "Assets/AudioClips/PanicLines";  // Path to the PanicLines folder
    public AudioClip[] DeathLines;  // Array of death-related lines
    public AudioClip[] PanicLines;  // Array of panic-related lines

    private AudioSource audioSource;  // AudioSource to play the clips

    private SoldierDeath soldierDeath;  // Reference to SoldierDeath script
    private EnemySoldierMove enemyMove;  // Reference to EnemySoldierMove script

    private bool hasPlayedDeathLine = false;  // Flag to track if the death line has been played
    private bool hasPlayedPanicLine = false;  // Flag to track if the panic line has been played

    // Static flag to ensure only one panic line is playing at a time across all SoldierSounds objects
    private static bool isPanicLinePlaying = false;

    private Transform playerTransform;  // Reference to the player for distance-based volume

    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing from " + gameObject.name);
        }

        // Get references to the other components
        soldierDeath = GetComponent<SoldierDeath>();
        enemyMove = GetComponent<EnemySoldierMove>();

        // Find the player in the scene by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Player object not found. Make sure it's tagged as 'Player'.");
        }
    }

    void Update()
    {
        // If the soldier is dead and hasn't played a death line yet
        if (soldierDeath != null && soldierDeath.isDead && !hasPlayedDeathLine)
        {
            // Immediately stop any playing audio (including panic lines)
            audioSource.Stop();

            // Play a random death line
            PlayRandomClip(DeathLines);
            hasPlayedDeathLine = true;
        }

        // If the panic condition is met and nothing else is playing
        if (enemyMove != null && enemyMove.hasMetDeadThreshold && !audioSource.isPlaying && !hasPlayedPanicLine && !isPanicLinePlaying)
        {
            if (Random.Range(0, 30) == 0)  // 1/30 chance
            {
                PlayRandomClip(PanicLines);
                hasPlayedPanicLine = true;
                isPanicLinePlaying = true;
            }
        }
    }

    private void PlayRandomClip(AudioClip[] clipArray)
    {
        if (clipArray == null || clipArray.Length == 0 || playerTransform == null) return;

        int randomIndex = Random.Range(0, clipArray.Length);
        AudioClip randomClip = clipArray[randomIndex];

        // Calculate volume based on distance to player
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        float volume = Mathf.Clamp01(1f - (distance / 10f)); // Full volume at 0, 0 volume at 10+

        // Set a random pitch
        audioSource.pitch = Random.Range(0.85f, 1.15f);

        // Play the clip with volume attenuation
        audioSource.PlayOneShot(randomClip, volume);

        // If this is a panic line, reset the global flag afterward
        StartCoroutine(ResetPanicLineFlag(randomClip.length));
    }

    private IEnumerator ResetPanicLineFlag(float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        isPanicLinePlaying = false;
    }

    // Call this to reset flags (e.g., on respawn or reset)
    public void ResetAudioFlags()
    {
        hasPlayedDeathLine = false;
        hasPlayedPanicLine = false;
    }
}
