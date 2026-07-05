using System.Collections;
using UnityEngine;

public class PointLightFlicker : MonoBehaviour
{
    public Light pointLight;

    public float minIntensity = 0f;
    public float maxIntensity = 8f;
    public float flickerInterval = 0.05f;

    private Coroutine flickerRoutine;
    private float originalIntensity;
    private bool originalEnabled;

    void Awake()
    {
        if (pointLight == null)
        {
            pointLight = GetComponent<Light>();
        }

        if (pointLight != null)
        {
            originalIntensity = pointLight.intensity;
            originalEnabled = pointLight.enabled;
        }
    }

    public void StartFlicker()
    {
        if (pointLight == null)
        {
            return;
        }

        if (flickerRoutine != null)
        {
            StopCoroutine(flickerRoutine);
        }

        originalIntensity = pointLight.intensity;
        originalEnabled = pointLight.enabled;

        pointLight.enabled = true;
        flickerRoutine = StartCoroutine(FlickerRoutine());
    }

    public void StopFlicker()
    {
        if (flickerRoutine != null)
        {
            StopCoroutine(flickerRoutine);
            flickerRoutine = null;
        }

        if (pointLight != null)
        {
            pointLight.enabled = originalEnabled;
            pointLight.intensity = originalIntensity;
        }
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            pointLight.intensity = Random.Range(minIntensity, maxIntensity);
            yield return new WaitForSeconds(flickerInterval);
        }
    }
}