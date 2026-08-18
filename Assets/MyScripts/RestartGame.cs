using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public CanvasGroup blackScreen;

    public float fadeDuration = 0.25f;
    public float audioFadeDuration = 1.5f;
    public float blackHoldDuration = 1.5f;

    private bool restarting;

    public void Restart()
    {
        if (restarting)
            return;

        restarting = true;

        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        Time.timeScale = 1f;

        float startVolume = AudioListener.volume;

        StartCoroutine(
            FadeOutAudio(
                startVolume
            )
        );

        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            blackScreen.blocksRaycasts = true;

            float startAlpha = blackScreen.alpha;
            float time = 0f;

            while (time < fadeDuration)
            {
                time += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(
                    time / fadeDuration
                );

                blackScreen.alpha =
                    Mathf.Lerp(
                        startAlpha,
                        1f,
                        t
                    );

                yield return null;
            }

            blackScreen.alpha = 1f;
        }

        float minimumHold =
            Mathf.Max(
                0f,
                audioFadeDuration -
                fadeDuration
            );

        yield return new WaitForSecondsRealtime(
            Mathf.Max(
                blackHoldDuration,
                minimumHold
            )
        );

        Scene currentScene =
            SceneManager.GetActiveScene();

        SceneManager.LoadScene(
            currentScene.buildIndex
        );

        AudioListener.volume =
            startVolume;
    }

    private IEnumerator FadeOutAudio(
        float startVolume
    )
    {
        if (audioFadeDuration <= 0f)
        {
            AudioListener.volume = 0f;
            yield break;
        }

        float time = 0f;

        while (time < audioFadeDuration)
        {
            time += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                time / audioFadeDuration
            );

            AudioListener.volume =
                Mathf.Lerp(
                    startVolume,
                    0f,
                    t
                );

            yield return null;
        }

        AudioListener.volume = 0f;
    }
}