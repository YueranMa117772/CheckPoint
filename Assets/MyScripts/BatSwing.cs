using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BatSwing : MonoBehaviour
{
    public Transform bat;
    public Transform pivot;

    public Vector3 windupRotation;
    public Vector3 swingRotation;

    public float windupTime = 0.15f;
    public float swingTime = 0.2f;
    public float returnTime = 0.3f;

    public UnityEvent onSwingReached;

    private Vector3 originalRelativePosition;
    private Quaternion originalRelativeRotation;

    private Coroutine swingRoutine;

    public void PlaySwing()
    {
        if (bat == null || pivot == null)
            return;

        if (swingRoutine != null)
            return;

        originalRelativePosition =
            pivot.InverseTransformPoint(bat.position);

        originalRelativeRotation =
            Quaternion.Inverse(pivot.rotation) *
            bat.rotation;

        swingRoutine = StartCoroutine(SwingRoutine());
    }

    IEnumerator SwingRoutine()
    {
        Quaternion windup =
            Quaternion.Euler(windupRotation);

        Quaternion swing =
            Quaternion.Euler(swingRotation);

        float time = 0f;

        while (time < windupTime)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(
                time / windupTime
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            Quaternion rotation =
                Quaternion.Slerp(
                    Quaternion.identity,
                    windup,
                    t
                );

            ApplyRotation(rotation);

            yield return null;
        }

        ApplyRotation(windup);

        time = 0f;

        while (time < swingTime)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(
                time / swingTime
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            Quaternion rotation =
                Quaternion.Slerp(
                    windup,
                    swing,
                    t
                );

            ApplyRotation(rotation);

            yield return null;
        }

        ApplyRotation(swing);

        onSwingReached?.Invoke();

        time = 0f;

        while (time < returnTime)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(
                time / returnTime
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            Quaternion rotation =
                Quaternion.Slerp(
                    swing,
                    Quaternion.identity,
                    t
                );

            ApplyRotation(rotation);

            yield return null;
        }

        ApplyRotation(Quaternion.identity);

        swingRoutine = null;
    }

    void ApplyRotation(Quaternion rotation)
    {
        Vector3 relativePosition =
            rotation *
            originalRelativePosition;

        bat.position =
            pivot.TransformPoint(relativePosition);

        bat.rotation =
            pivot.rotation *
            rotation *
            originalRelativeRotation;
    }
}