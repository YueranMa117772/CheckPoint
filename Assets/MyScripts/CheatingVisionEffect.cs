using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CheatingVisionEffect : MonoBehaviour
{
    public Volume cheatingVolume;

    public float minWeight = 0.4f;
    public float maxWeight = 0.6f;

    public float minMoveTime = 0.8f;
    public float maxMoveTime = 1.8f;

    public float stopFadeOutTime = 3f;

    public float flashbackFocalLength = 50f;

    private bool effectActive;
    private Coroutine effectRoutine;
    private Coroutine stopRoutine;

    private DepthOfField depthOfField;
    private float originalFocalLength;
    private bool originalFocalLengthOverrideState;
    private bool focalLengthSaved;

    void Start()
    {
        if (cheatingVolume != null)
        {
            cheatingVolume.weight = 0f;

            if (cheatingVolume.profile != null)
                cheatingVolume.profile.TryGet(out depthOfField);
        }
    }

    public void StartEffect()
    {
        if (cheatingVolume == null)
            return;

        if (effectActive)
            return;

        if (stopRoutine != null)
        {
            StopCoroutine(stopRoutine);
            stopRoutine = null;
        }

        effectActive = true;

        cheatingVolume.weight =
            Random.Range(minWeight, maxWeight);

        effectRoutine = StartCoroutine(EffectLoop());
    }

    public void StopEffect()
    {
        effectActive = false;

        if (effectRoutine != null)
        {
            StopCoroutine(effectRoutine);
            effectRoutine = null;
        }

        if (stopRoutine != null)
        {
            StopCoroutine(stopRoutine);
            stopRoutine = null;
        }

        if (cheatingVolume != null)
            stopRoutine = StartCoroutine(StopRoutine());
    }

    public void EnterFlashbackFocalLength()
    {
        if (depthOfField == null)
            return;

        originalFocalLength = depthOfField.focalLength.value;
        originalFocalLengthOverrideState =
            depthOfField.focalLength.overrideState;

        focalLengthSaved = true;

        depthOfField.focalLength.overrideState = true;
        depthOfField.focalLength.value =
            flashbackFocalLength;
    }

    public void ExitFlashbackFocalLength()
    {
        if (depthOfField == null)
            return;

        if (!focalLengthSaved)
            return;

        depthOfField.focalLength.value =
            originalFocalLength;

        depthOfField.focalLength.overrideState =
            originalFocalLengthOverrideState;

        focalLengthSaved = false;
    }

    IEnumerator EffectLoop()
    {
        while (effectActive)
        {
            float targetWeight =
                Random.Range(minWeight, maxWeight);

            float moveTime =
                Random.Range(minMoveTime, maxMoveTime);

            yield return FadeWeight(
                cheatingVolume.weight,
                targetWeight,
                moveTime
            );
        }
    }

    IEnumerator StopRoutine()
    {
        yield return FadeWeight(
            cheatingVolume.weight,
            1f,
            stopFadeOutTime
        );

        cheatingVolume.weight = 0f;

        stopRoutine = null;
    }

    IEnumerator FadeWeight(
        float from,
        float to,
        float duration
    )
    {
        if (duration <= 0f)
        {
            cheatingVolume.weight = to;
            yield break;
        }

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(
                time / duration
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            cheatingVolume.weight =
                Mathf.Lerp(from, to, t);

            yield return null;
        }

        cheatingVolume.weight = to;
    }
}