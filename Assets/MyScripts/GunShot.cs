using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GunShot : MonoBehaviour
{
    public Transform gun;

    public Vector3 recoilOffset;

    public float recoilTime = 0.08f;
    public float returnTime = 0.12f;

    public UnityEvent onShotReached;

    private Vector3 originalLocalPosition;
    private Coroutine shotRoutine;

    public void PlayShot()
    {
        if (gun == null)
            return;

        if (shotRoutine != null)
            return;

        originalLocalPosition = gun.localPosition;

        shotRoutine = StartCoroutine(ShotRoutine());
    }

    IEnumerator ShotRoutine()
    {
        Vector3 recoilPosition =
            originalLocalPosition + recoilOffset;

        float time = 0f;

        while (time < recoilTime)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(
                time / recoilTime
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            gun.localPosition =
                Vector3.Lerp(
                    originalLocalPosition,
                    recoilPosition,
                    t
                );

            yield return null;
        }

        gun.localPosition = recoilPosition;

        onShotReached?.Invoke();

        time = 0f;

        while (time < returnTime)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(
                time / returnTime
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            gun.localPosition =
                Vector3.Lerp(
                    recoilPosition,
                    originalLocalPosition,
                    t
                );

            yield return null;
        }

        gun.localPosition = originalLocalPosition;

        shotRoutine = null;
    }
}