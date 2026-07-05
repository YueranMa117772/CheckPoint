using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HeartbeatVignettePulse : MonoBehaviour
{
    public Volume globalVolume;
    public AudioSource heartbeatAudioSource;

    public float baseIntensity = 0.25f;
    public float peakIntensity = 0.6f;
    public float riseTime = 0.04f;
    public float fallTime = 0.25f;

    private Vignette vignette;
    private Coroutine pulseRoutine;
    private int lastSample;
    private bool wasPlaying;

    void Awake()
    {
        if (globalVolume != null)
        {
            globalVolume.profile.TryGet(out vignette);
        }

        if (vignette != null)
        {
            vignette.intensity.Override(baseIntensity);
        }
    }

    void Update()
    {
        if (heartbeatAudioSource == null || heartbeatAudioSource.clip == null)
        {
            return;
        }

        if (!heartbeatAudioSource.isPlaying)
        {
            wasPlaying = false;
            lastSample = 0;
            return;
        }

        int currentSample = heartbeatAudioSource.timeSamples;

        if (!wasPlaying)
        {
            PulseOnly();
            wasPlaying = true;
        }

        if (currentSample < lastSample)
        {
            PulseOnly();
        }

        lastSample = currentSample;
    }

    void PulseOnly()
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
        }

        pulseRoutine = StartCoroutine(PulseSequence());
    }

    IEnumerator PulseSequence()
    {
        if (vignette == null)
        {
            yield break;
        }

        float t = 0f;

        while (t < riseTime)
        {
            t += Time.deltaTime;
            vignette.intensity.Override(Mathf.Lerp(baseIntensity, peakIntensity, Mathf.Clamp01(t / riseTime)));
            yield return null;
        }

        t = 0f;

        while (t < fallTime)
        {
            t += Time.deltaTime;
            vignette.intensity.Override(Mathf.Lerp(peakIntensity, baseIntensity, Mathf.Clamp01(t / fallTime)));
            yield return null;
        }

        vignette.intensity.Override(baseIntensity);
        pulseRoutine = null;
    }
}