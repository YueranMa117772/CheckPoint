using System.Collections;
using UnityEngine;

public class ReporterCameraFlash : MonoBehaviour
{
    public Light cameraLight;

    public float flashIntensity = 8f;
    public float flashDuration = 0.08f;

    public float minInterval = 0.8f;
    public float maxInterval = 1.8f;

    public AudioSource shutterAudio;

    private float originalIntensity;
    private Coroutine flashRoutine;
    private bool flashing;

    void Awake()
    {
        originalIntensity = cameraLight.intensity;
    }

    public void StartFlashing()
    {
        if (flashing)
            return;

        flashing = true;
        flashRoutine = StartCoroutine(FlashLoop());
    }

    public void StopFlashing()
    {
        flashing = false;

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        cameraLight.intensity = originalIntensity;
    }

    IEnumerator FlashLoop()
    {
        while (flashing)
        {
            yield return new WaitForSeconds(
                Random.Range(minInterval, maxInterval)
            );

            if (!flashing)
                break;

            cameraLight.intensity = flashIntensity;

            if (shutterAudio != null)
                shutterAudio.PlayOneShot(shutterAudio.clip);

            yield return new WaitForSeconds(flashDuration);

            cameraLight.intensity = originalIntensity;
        }

        flashRoutine = null;
    }
}