using System.Collections;
using UnityEngine;

public class BlackScreenFade : MonoBehaviour
{
    public CanvasGroup blackGroup;

    public float introHoldTime = 3f;
    public float introFadeOutTime = 1f;

    public float knockdownHoldTime = 1f;
    public float knockdownFadeOutTime = 1f;

    private Coroutine routine;

    private void Start()
    {
        routine = StartCoroutine(IntroRoutine());
    }

    public void PlayKnockdownBlack()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(KnockdownRoutine());
    }

    private IEnumerator IntroRoutine()
    {
        SetBlack(1f, true);

        yield return new WaitForSeconds(introHoldTime);

        yield return FadeOut(introFadeOutTime);
    }

    private IEnumerator KnockdownRoutine()
    {
        SetBlack(1f, true);

        yield return new WaitForSeconds(knockdownHoldTime);

        yield return FadeOut(knockdownFadeOutTime);
    }

    private IEnumerator FadeOut(float fadeTime)
    {
        if (fadeTime <= 0f)
        {
            SetBlack(0f, false);
            yield break;
        }

        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            blackGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }

        SetBlack(0f, false);
    }

    private void SetBlack(float alpha, bool block)
    {
        blackGroup.alpha = alpha;
        blackGroup.blocksRaycasts = block;
    }
}