using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class RoyAttackSequence : MonoBehaviour
{
    public GameObject roy;

    public Animator royAnimator;
    public NavMeshAgent royAgent;

    public Transform approachTargetPoint;
    public Transform lookTarget;

    public float moveSpeed = 1.8f;
    public float arriveDistance = 0.2f;
    public float faceRotationSpeed = 8f;
    public float punchRotationOffset = 0f;

    public string runStateName = "run";
    public string standStateName = "stand";
    public string punchStateName = "punch";

    public UnityEvent onEvidenceArrived;
    public UnityEvent onLeaveArrived;

    private bool sequenceStarted;
    private bool arrived;
    private bool punching;

    private bool evidenceRoute;

    private void Update()
    {
        if (!sequenceStarted)
            return;

        if (punching)
        {
            UpdatePunchChase();
            return;
        }

        if (arrived)
            FaceLookTarget(0f);
    }

    public void StartEvidenceApproach()
    {
        evidenceRoute = true;
        StartApproach();
    }

    public void StartLeaveApproach()
    {
        evidenceRoute = false;
        StartApproach();
    }

    private void StartApproach()
    {
        if (sequenceStarted)
            return;

        if (
            roy == null ||
            royAnimator == null ||
            royAgent == null ||
            approachTargetPoint == null
        )
        {
            return;
        }

        sequenceStarted = true;

        StartCoroutine(ApproachSequence());
    }

    private IEnumerator ApproachSequence()
    {
        roy.SetActive(true);

        yield return null;

        royAgent.speed = moveSpeed;
        royAgent.stoppingDistance = arriveDistance;
        royAgent.updateRotation = false;
        royAgent.isStopped = false;

        royAnimator.enabled = true;
        royAnimator.Play(runStateName, 0, 0f);

        while (true)
        {
            if (!royAgent.isOnNavMesh)
            {
                yield return null;
                continue;
            }

            royAgent.isStopped = false;
            royAgent.SetDestination(
                approachTargetPoint.position
            );

            FaceTarget(
                approachTargetPoint.position,
                0f
            );

            if (HasArrived())
                break;

            yield return null;
        }

        royAgent.isStopped = true;
        royAgent.ResetPath();

        royAnimator.Play(
            standStateName,
            0,
            0f
        );

        arrived = true;

        FaceLookTarget(0f);

        if (evidenceRoute)
            onEvidenceArrived?.Invoke();
        else
            onLeaveArrived?.Invoke();
    }

    public void StartPunching()
    {
        if (!arrived || punching)
            return;

        punching = true;

        if (
            royAgent != null &&
            royAgent.isOnNavMesh
        )
        {
            royAgent.isStopped = false;

            royAgent.SetDestination(
                approachTargetPoint.position
            );
        }

        FaceLookTarget(
            punchRotationOffset
        );

        royAnimator.Play(
            punchStateName,
            0,
            0f
        );
    }

    private void UpdatePunchChase()
    {
        if (
            royAgent == null ||
            !royAgent.isOnNavMesh ||
            approachTargetPoint == null
        )
        {
            return;
        }

        royAgent.isStopped = false;

        royAgent.SetDestination(
            approachTargetPoint.position
        );

        FaceLookTarget(
            punchRotationOffset
        );
    }

    private bool HasArrived()
    {
        if (
            royAgent == null ||
            !royAgent.isOnNavMesh
        )
        {
            return false;
        }

        if (royAgent.pathPending)
            return false;

        if (
            royAgent.remainingDistance >
            royAgent.stoppingDistance + 0.05f
        )
        {
            return false;
        }

        if (
            royAgent.hasPath &&
            royAgent.velocity.sqrMagnitude > 0.01f
        )
        {
            return false;
        }

        return true;
    }

    private void FaceLookTarget(
        float yawOffset
    )
    {
        Transform target =
            lookTarget != null
                ? lookTarget
                : approachTargetPoint;

        if (target == null)
            return;

        FaceTarget(
            target.position,
            yawOffset
        );
    }

    private void FaceTarget(
        Vector3 targetPosition,
        float yawOffset
    )
    {
        Vector3 direction =
            targetPosition -
            roy.transform.position;

        direction.y = 0f;

        if (
            direction.sqrMagnitude <
            0.001f
        )
        {
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction
            ) *
            Quaternion.Euler(
                0f,
                yawOffset,
                0f
            );

        roy.transform.rotation =
            Quaternion.Slerp(
                roy.transform.rotation,
                targetRotation,
                faceRotationSpeed *
                Time.deltaTime
            );
    }
}