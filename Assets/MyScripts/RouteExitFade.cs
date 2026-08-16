using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RouteExitFade : MonoBehaviour
{
    public CanvasGroup blackGroup;

    public float teleportBlackHoldTime = 0.2f;

    public UnityEvent onFadeFinished;

    private Coroutine routine;

    public void PlayTeleportBlack(
        UnityAction onFullBlack,
        float totalTime
    )
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(
            TeleportBlackRoutine(
                onFullBlack,
                totalTime
            )
        );
    }

    private IEnumerator TeleportBlackRoutine(
        UnityAction onFullBlack,
        float totalTime
    )
    {
        if (blackGroup == null)
        {
            onFullBlack?.Invoke();
            onFadeFinished?.Invoke();
            yield break;
        }

        float holdTime = Mathf.Clamp(
            teleportBlackHoldTime,
            0f,
            totalTime
        );

        float fadeTime =
            (totalTime - holdTime) * 0.5f;

        SetBlack(0f, true);

        yield return FadeAlpha(
            0f,
            1f,
            fadeTime
        );

        onFullBlack?.Invoke();

        yield return new WaitForSeconds(
            holdTime
        );

        yield return FadeAlpha(
            1f,
            0f,
            fadeTime
        );

        SetBlack(0f, false);

        Debug.Log(
            "[RouteFade] Fade completely finished"
        );

        onFadeFinished?.Invoke();

        routine = null;
    }

    private IEnumerator FadeAlpha(
        float from,
        float to,
        float duration
    )
    {
        if (duration <= 0f)
        {
            blackGroup.alpha = to;
            yield break;
        }

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            blackGroup.alpha =
                Mathf.Lerp(
                    from,
                    to,
                    time / duration
                );

            yield return null;
        }

        blackGroup.alpha = to;
    }

    private void SetBlack(
        float alpha,
        bool block
    )
    {
        if (blackGroup == null)
            return;

        blackGroup.alpha = alpha;
        blackGroup.blocksRaycasts = block;
    }
}