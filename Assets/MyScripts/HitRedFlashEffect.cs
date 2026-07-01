using System.Collections;
using UnityEngine;

public class HitRedFlashEffect : MonoBehaviour
{
    public CanvasGroup redGroup;

    public float maxAlpha = 0.32f;
    public float fadeInTime = 0.03f;
    public float holdTime = 0.04f;
    public float fadeOutTime = 0.16f;

    private Coroutine flashCoroutine;

    private void Start()
    {
        if (redGroup == null) return;

        redGroup.alpha = 0f;
        redGroup.interactable = false;
        redGroup.blocksRaycasts = false;
    }

    public void PlayRedFlash()
    {
        if (redGroup == null) return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        yield return Fade(redGroup.alpha, maxAlpha, fadeInTime);

        yield return new WaitForSeconds(holdTime);

        yield return Fade(maxAlpha, 0f, fadeOutTime);

        flashCoroutine = null;
    }

    private IEnumerator Fade(float from, float to, float time)
    {
        if (time <= 0f)
        {
            redGroup.alpha = to;
            yield break;
        }

        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / time);
            redGroup.alpha = Mathf.Lerp(from, to, p);
            yield return null;
        }

        redGroup.alpha = to;
    }
}