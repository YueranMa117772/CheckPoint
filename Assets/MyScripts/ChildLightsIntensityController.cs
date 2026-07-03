using UnityEngine;

public class ChildLightsIntensityController : MonoBehaviour
{
    public bool includeInactive = true;

    private Light[] lights;

    void Awake()
    {
        lights = GetComponentsInChildren<Light>(includeInactive);
    }

    public void SetIntensity(float intensity)
    {
        if (lights == null || lights.Length == 0)
        {
            lights = GetComponentsInChildren<Light>(includeInactive);
        }

        foreach (Light light in lights)
        {
            if (light != null)
            {
                light.intensity = intensity;
            }
        }
    }

    public void TurnOff()
    {
        SetIntensity(0f);
    }

    public void TurnOn()
    {
        SetIntensity(3f);
    }
}