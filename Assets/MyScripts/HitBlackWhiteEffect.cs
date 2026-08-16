using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class HitBlackWhiteEffect : MonoBehaviour
{
    public Volume hitVolume;

    public float holdTime = 0.12f;
    public float fadeOutTime = 0.4f;

    private Coroutine routine;

    public void PlayEffect()
    {
        if (hitVolume == null)
            return;

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(EffectRoutine());
    }

    private IEnumerator EffectRoutine()
    {
        hitVolume.weight = 1f;

        yield return new WaitForSeconds(holdTime);

        float time = 0f;

        while (time < fadeOutTime)
        {
            time += Time.deltaTime;

            hitVolume.weight =
                Mathf.Lerp(
                    1f,
                    0f,
                    time / fadeOutTime
                );

            yield return null;
        }

        hitVolume.weight = 0f;
        routine = null;
    }
}