using System.Collections;
using UnityEngine;

public class RoyBatReaction : MonoBehaviour
{
    public Animator animator;
    public RoyFacePlayer facePlayer;

    public Transform royRoot;
    public Transform fallFacingTarget;

    public Transform trembleTarget;

    public float trembleAngle = 5f;
    public float trembleTime = 0.12f;

    private bool downed = false;
    private Coroutine trembleRoutine;

    public void Hit()
    {
        if (!downed)
        {
            if (facePlayer != null)
                facePlayer.StopFacing();

            if (royRoot != null &&
                fallFacingTarget != null)
            {
                Vector3 direction =
                    fallFacingTarget.position -
                    royRoot.position;

                direction.y = 0f;

                if (direction.sqrMagnitude > 0.001f)
                {
                    royRoot.rotation =
                        Quaternion.LookRotation(direction);
                }
            }

            if (animator != null)
                animator.SetTrigger("Hit");

            downed = true;

            Debug.Log("[RoyBat] Downed");

            return;
        }

        Tremble();
    }

    private void Tremble()
    {
        if (trembleTarget == null)
            return;

        if (trembleRoutine != null)
            StopCoroutine(trembleRoutine);

        trembleRoutine =
            StartCoroutine(TrembleRoutine());
    }

    private IEnumerator TrembleRoutine()
    {
        Quaternion originalRotation =
            trembleTarget.localRotation;

        Quaternion hitRotation =
            originalRotation *
            Quaternion.Euler(
                trembleAngle,
                0f,
                trembleAngle
            );

        float halfTime =
            trembleTime * 0.5f;

        float time = 0f;

        while (time < halfTime)
        {
            time += Time.deltaTime;

            trembleTarget.localRotation =
                Quaternion.Slerp(
                    originalRotation,
                    hitRotation,
                    time / halfTime
                );

            yield return null;
        }

        time = 0f;

        while (time < halfTime)
        {
            time += Time.deltaTime;

            trembleTarget.localRotation =
                Quaternion.Slerp(
                    hitRotation,
                    originalRotation,
                    time / halfTime
                );

            yield return null;
        }

        trembleTarget.localRotation =
            originalRotation;

        trembleRoutine = null;
    }
}