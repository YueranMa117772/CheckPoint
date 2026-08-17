using System.Collections;
using UnityEngine;

public class AudioFadeIn : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeDuration = 3f;
    public float targetVolume = 1f;

    private Coroutine fadeRoutine;

    public void FadeIn()
    {
        if (audioSource == null)
            return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        float time = 0f;

        audioSource.volume = 0f;

        if (!audioSource.isPlaying)
            audioSource.Play();

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(
                time / fadeDuration
            );

            audioSource.volume =
                Mathf.Lerp(
                    0f,
                    targetVolume,
                    t
                );

            yield return null;
        }

        audioSource.volume = targetVolume;
        fadeRoutine = null;
    }
}
