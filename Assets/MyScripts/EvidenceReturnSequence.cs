using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class EvidenceReturnSequence : MonoBehaviour
{
    public GameObject reporter;
    public GameObject referee;

    public Animator reporterAnimator;
    public Animator refereeAnimator;

    public NavMeshAgent reporterAgent;
    public NavMeshAgent refereeAgent;

    public Transform player;

    public Transform reporterSpawnPoint;
    public Transform refereeSpawnPoint;

    public float disappearTime = 3f;
    public float reporterAppearDelay = 1f;

    public float reporterMoveSpeed = 1.5f;
    public float refereeMoveSpeed = 1.5f;

    public float arriveDistance = 0.2f;
    public float sampleDistance = 1f;

    public float refereeFrontDistance = 2f;
    public float refereeSideOffset = 1.2f;

    public float reporterBackDistanceFromReferee = 0.8f;
    public float reporterSideOffsetFromReferee = -1.2f;

    public float facePlayerSpeed = 8f;

    public string walkStateName = "walk";
    public string standStateName = "stand";

    public UnityEvent onBothArrived;

    private bool sequenceStarted;
    private bool returning;

    private void Update()
    {
        if (!sequenceStarted)
            return;

        if (reporter != null && reporter.activeInHierarchy)
            FacePlayer(reporter.transform);

        if (referee != null && referee.activeInHierarchy)
            FacePlayer(referee.transform);
    }

    public void StartSequence()
    {
        if (returning)
            return;

        StartCoroutine(ReturnSequence());
    }

    IEnumerator ReturnSequence()
    {
        returning = true;
        sequenceStarted = true;

        reporter.SetActive(false);
        referee.SetActive(false);

        yield return new WaitForSeconds(disappearTime);

        referee.SetActive(true);

        refereeAgent.Warp(refereeSpawnPoint.position);
        refereeAgent.speed = refereeMoveSpeed;
        refereeAgent.stoppingDistance = arriveDistance;
        refereeAgent.updateRotation = false;
        refereeAgent.isStopped = false;

        refereeAnimator.enabled = true;
        refereeAnimator.Play(walkStateName, 0, 0f);

        while (true)
        {
            Vector3 desiredTarget =
                player.position
                + player.forward * refereeFrontDistance
                + player.right * refereeSideOffset;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                desiredTarget,
                out hit,
                sampleDistance,
                NavMesh.AllAreas
            ))
            {
                refereeAgent.SetDestination(hit.position);
            }

            if (HasArrived(refereeAgent))
                break;

            yield return null;
        }

        refereeAgent.isStopped = true;
        refereeAgent.ResetPath();

        refereeAnimator.Play(standStateName, 0, 0f);

        FacePlayer(referee.transform);

        Vector3 fixedReporterTarget =
            referee.transform.position
            - referee.transform.forward * reporterBackDistanceFromReferee
            + referee.transform.right * reporterSideOffsetFromReferee;

        NavMeshHit reporterTargetHit;

        if (NavMesh.SamplePosition(
            fixedReporterTarget,
            out reporterTargetHit,
            sampleDistance,
            NavMesh.AllAreas
        ))
        {
            fixedReporterTarget = reporterTargetHit.position;
        }

        yield return new WaitForSeconds(reporterAppearDelay);

        reporter.SetActive(true);

        reporterAgent.Warp(reporterSpawnPoint.position);
        reporterAgent.speed = reporterMoveSpeed;
        reporterAgent.stoppingDistance = arriveDistance;
        reporterAgent.updateRotation = false;
        reporterAgent.isStopped = false;

        reporterAnimator.enabled = true;
        reporterAnimator.Play(walkStateName, 0, 0f);

        reporterAgent.SetDestination(fixedReporterTarget);

        while (true)
        {
            if (HasArrived(reporterAgent))
                break;

            yield return null;
        }

        reporterAgent.isStopped = true;
        reporterAgent.ResetPath();

        reporterAnimator.Play(standStateName, 0, 0f);

        returning = false;

        onBothArrived?.Invoke();
    }

    bool HasArrived(NavMeshAgent agent)
    {
        if (agent.pathPending)
            return false;

        if (agent.remainingDistance > agent.stoppingDistance + 0.05f)
            return false;

        if (agent.hasPath && agent.velocity.sqrMagnitude > 0.01f)
            return false;

        return true;
    }

    void FacePlayer(Transform character)
    {
        Vector3 direction =
            player.position - character.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        character.rotation =
            Quaternion.Slerp(
                character.rotation,
                targetRotation,
                facePlayerSpeed * Time.deltaTime
            );
    }
}