using UnityEngine;

/// Blinks child material colour and emission.
public class ColourBlinkShift : MonoBehaviour
{
    [Header("Animation Timing Settings")]
    public float BlinkClock;
    public float ClockSpeed = 1f;
    public float MaxClock = 1f;
    public float ChildOffset = 0.12f;
    public AnimationCurve BlinkCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.5f, 1f),
        new Keyframe(1f, 0f)
    );

    [Header("Colour Settings")]
    public Color CurrentColour;
    public Color StartColour = new Color(0.08f, 0.08f, 0.08f, 1f);
    public Color EndColour = Color.white;
    public float EmissionPower = 1.5f;

    [Header("System Settings")]
    public float LerpValue;
    public Material[] MyMaterials;

    void Awake()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        MyMaterials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            MyMaterials[i] = renderers[i].material;
            MyMaterials[i].EnableKeyword("_EMISSION");
        }
    }

    void Update()
    {
        if (BlinkClock > MaxClock)
            BlinkClock = 0f;

        for (int i = 0; i < MyMaterials.Length; i++)
        {
            float localClock = BlinkClock + i * ChildOffset;

            if (localClock > MaxClock)
                localClock -= MaxClock;

            LerpValue = BlinkCurve.Evaluate(localClock);
            CurrentColour = Color.Lerp(StartColour, EndColour, LerpValue);

            MyMaterials[i].color = CurrentColour;
            MyMaterials[i].SetColor("_EmissionColor", CurrentColour * EmissionPower);
        }

        BlinkClock += Time.deltaTime * ClockSpeed;
    }
}