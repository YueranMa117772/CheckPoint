using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WhiteFlashEffect : MonoBehaviour
{
    public CanvasGroup whiteGroup;

    public float enterFadeInTime = 0.08f;
    public float enterHoldTime = 0.05f;
    public float enterFadeOutTime = 0.20f;

    public float exitFadeInTime = 0.04f;
    public float exitHoldTime = 0.12f;
    public float exitFadeOutTime = 1.0f;

    public UnityEvent onEnterFullWhite;
    public UnityEvent onEnterFinished;

    public UnityEvent onExitFullWhite;
    public UnityEvent onExitFinished;

    private Coroutine routine;

    void Start()
    {
        if (whiteGroup == null)
            return;

        whiteGroup.alpha = 0f;
        whiteGroup.interactable = false;
        whiteGroup.blocksRaycasts = false;
    }

    public void PlayEnterFlash()
    {
        if (whiteGroup == null)
            return;

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(EnterRoutine());
    }

    public void PlayExitFlash()
    {
        if (whiteGroup == null)
            return;

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ExitRoutine());
    }

    IEnumerator EnterRoutine()
    {
        yield return Fade(
            whiteGroup.alpha,
            1f,
            enterFadeInTime
        );

        onEnterFullWhite?.Invoke();

        yield return new WaitForSeconds(enterHoldTime);

        yield return Fade(
            1f,
            0f,
            enterFadeOutTime
        );

        onEnterFinished?.Invoke();

        routine = null;
    }

    IEnumerator ExitRoutine()
    {
        yield return Fade(
            whiteGroup.alpha,
            1f,
            exitFadeInTime
        );

        onExitFullWhite?.Invoke();

        yield return new WaitForSeconds(exitHoldTime);

        yield return Fade(
            1f,
            0f,
            exitFadeOutTime
        );

        onExitFinished?.Invoke();

        routine = null;
    }

    IEnumerator Fade(float from, float to, float time)
    {
        if (time <= 0f)
        {
            whiteGroup.alpha = to;
            yield break;
        }

        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;

            float p = Mathf.Clamp01(t / time);

            whiteGroup.alpha =
                Mathf.Lerp(from, to, p);

            yield return null;
        }

        whiteGroup.alpha = to;
    }
}