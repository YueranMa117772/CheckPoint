using UnityEngine;

public class GuideLightMaterialSwitch : MonoBehaviour
{
    public Renderer targetRenderer;

    public Material offMaterial;
    public Material onMaterial;

    public bool startOff = true;

    void Start()
    {
        if (startOff)
        {
            TurnOff();
        }
    }

    public void TurnOn()
    {
        if (targetRenderer != null && onMaterial != null)
        {
            targetRenderer.material = onMaterial;
        }
    }

    public void TurnOff()
    {
        if (targetRenderer != null && offMaterial != null)
        {
            targetRenderer.material = offMaterial;
        }
    }
}