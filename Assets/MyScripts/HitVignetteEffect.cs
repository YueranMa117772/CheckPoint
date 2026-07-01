using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HitVignetteEffect : MonoBehaviour
{
    public Volume volume;

    public int hitsToMax = 11;

    public float startIntensity = 0f;
    public float maxIntensity = 0.85f;

    public float startSmoothness = 0.5f;
    public float maxSmoothness = 0.9f;

    private Vignette vignette;

    void Start()
    {
        if (volume == null) return;

        volume.profile.TryGet(out vignette);

        if (vignette == null) return;

        vignette.color.overrideState = true;
        vignette.intensity.overrideState = true;
        vignette.smoothness.overrideState = true;

        vignette.color.value = Color.black;
        vignette.intensity.value = startIntensity;
        vignette.smoothness.value = startSmoothness;
    }

    public void AddHitEffect()
    {
        if (vignette == null) return;
        if (hitsToMax <= 0) return;

        float intensityStep = (maxIntensity - startIntensity) / hitsToMax;
        float smoothnessStep = (maxSmoothness - startSmoothness) / hitsToMax;

        vignette.intensity.value = Mathf.Clamp(
            vignette.intensity.value + intensityStep,
            startIntensity,
            maxIntensity
        );

        vignette.smoothness.value = Mathf.Clamp(
            vignette.smoothness.value + smoothnessStep,
            startSmoothness,
            maxSmoothness
        );
    }

    public void ResetHitEffect()
    {
        if (vignette == null) return;

        vignette.intensity.value = startIntensity;
        vignette.smoothness.value = startSmoothness;
    }
}