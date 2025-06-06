using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    [Tooltip("Add your AudioClips here. They will be played in a shuffled loop.")]
    public AudioClip[] musicTracks;

    private AudioSource audioSource;
    private int currentTrackIndex = 0;
    private const float fadeDuration = 5f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;

        if (musicTracks.Length == 0)
        {
            Debug.LogWarning("No music tracks assigned to BackgroundMusic.");
            return;
        }

        ShuffleTracks();
        StartCoroutine(PlayMusicLoop());
    }

    private void ShuffleTracks()
    {
        for (int i = 0; i < musicTracks.Length; i++)
        {
            int rand = Random.Range(i, musicTracks.Length);
            AudioClip temp = musicTracks[i];
            musicTracks[i] = musicTracks[rand];
            musicTracks[rand] = temp;
        }
    }

    private IEnumerator PlayMusicLoop()
    {
        while (true)
        {
            AudioClip currentClip = musicTracks[currentTrackIndex];
            audioSource.clip = currentClip;
            audioSource.volume = 0f;
            audioSource.Play();

            // Start fade in
            yield return StartCoroutine(FadeVolume(0f, 1f, fadeDuration));

            // Wait for clip length minus fade-out duration
            yield return new WaitForSeconds(currentClip.length - fadeDuration);

            // Start fade out
            yield return StartCoroutine(FadeVolume(1f, 0f, fadeDuration));

            currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Length;
        }
    }

    private IEnumerator FadeVolume(float start, float end, float duration)
{
    float elapsed = 0f;
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        audioSource.volume = Mathf.Lerp(start, end, elapsed / duration);
        yield return null;
    }
    audioSource.volume = end;
}
}
