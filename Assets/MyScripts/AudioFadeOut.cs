using System.Collections;
using UnityEngine;

public class AudioFadeOut : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeDuration = 3f;

    private Coroutine fadeRoutine;

    public void FadeOut()
    {
        if (audioSource == null)
            return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(
                time / fadeDuration
            );

            audioSource.volume =
                Mathf.Lerp(
                    startVolume,
                    0f,
                    t
                );

            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        audioSource.volume = startVolume;

        fadeRoutine = null;
    }
}