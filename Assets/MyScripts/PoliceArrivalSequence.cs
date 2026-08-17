using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class PoliceArrivalSequence : MonoBehaviour
{
    public GameObject firstOfficer;
    public GameObject secondOfficer;

    public Animator firstOfficerAnimator;
    public Animator secondOfficerAnimator;

    public NavMeshAgent firstOfficerAgent;
    public NavMeshAgent secondOfficerAgent;

    public Transform player;

    public float firstFrontDistance = 2f;
    public float firstSideOffset = 0f;

    public float secondBackDistanceFromFirst = 0.8f;
    public float secondSideOffsetFromFirst = 1.2f;

    public float firstOfficerMoveSpeed = 1.5f;
    public float secondOfficerMoveSpeed = 1.5f;

    public float arriveDistance = 0.2f;
    public float sampleDistance = 1f;

    public float secondOfficerAppearDelay = 1f;
    public float secondOfficerShootDelay = 1.5f;

    public float faceRotationSpeed = 8f;

    public string runStateName = "run";
    public string standStateName = "stand";
    public string fallStateName = "fall";
    public string shootStateName = "shoot";

    public AudioSource secondOfficerShootAudio;

    public UnityEvent onBothArrivedGun;
    public UnityEvent onBothArrivedNoGun;
    public UnityEvent onSecondOfficerShoot;

    private bool sequenceStarted;
    private bool hasGun;

    private bool firstOfficerStanding;
    private bool secondOfficerStanding;

    private bool firstOfficerDown;
    private bool secondOfficerShot;
    private bool secondOfficerShootScheduled;

    private void Update()
    {
        if (!sequenceStarted || player == null)
            return;

        if (
            firstOfficer != null &&
            firstOfficer.activeInHierarchy &&
            firstOfficerStanding &&
            !firstOfficerDown
        )
        {
            FacePlayer(firstOfficer.transform);
        }

        if (
            secondOfficer != null &&
            secondOfficer.activeInHierarchy &&
            secondOfficerStanding
        )
        {
            FacePlayer(secondOfficer.transform);
        }
    }

    public void StartGunSequence()
    {
        StartSequence(true);
    }

    public void StartNoGunSequence()
    {
        StartSequence(false);
    }

    private void StartSequence(bool gun)
    {
        if (sequenceStarted)
            return;

        if (
            firstOfficer == null ||
            secondOfficer == null ||
            firstOfficerAnimator == null ||
            secondOfficerAnimator == null ||
            firstOfficerAgent == null ||
            secondOfficerAgent == null ||
            player == null
        )
        {
            return;
        }

        hasGun = gun;
        sequenceStarted = true;

        StartCoroutine(ArrivalSequence());
    }

    private IEnumerator ArrivalSequence()
    {
        firstOfficer.SetActive(false);
        secondOfficer.SetActive(false);

        firstOfficerStanding = false;
        secondOfficerStanding = false;

        firstOfficer.SetActive(true);

        firstOfficerAgent.speed = firstOfficerMoveSpeed;
        firstOfficerAgent.stoppingDistance = arriveDistance;
        firstOfficerAgent.updateRotation = false;
        firstOfficerAgent.isStopped = false;

        firstOfficerAnimator.enabled = true;
        firstOfficerAnimator.Play(runStateName, 0, 0f);

        while (true)
        {
            Vector3 desiredTarget =
                player.position +
                player.forward * firstFrontDistance +
                player.right * firstSideOffset;

            NavMeshHit hit;

            if (
                NavMesh.SamplePosition(
                    desiredTarget,
                    out hit,
                    sampleDistance,
                    NavMesh.AllAreas
                )
            )
            {
                firstOfficerAgent.SetDestination(hit.position);
                FaceTarget(firstOfficer.transform, hit.position);
            }

            if (HasArrived(firstOfficerAgent))
                break;

            yield return null;
        }

        firstOfficerAgent.isStopped = true;
        firstOfficerAgent.ResetPath();

        firstOfficerAnimator.Play(standStateName, 0, 0f);

        firstOfficerStanding = true;
        FacePlayer(firstOfficer.transform);

        Vector3 directionToPlayer =
            player.position - firstOfficer.transform.position;

        directionToPlayer.y = 0f;

        if (directionToPlayer.sqrMagnitude > 0.001f)
            directionToPlayer.Normalize();
        else
            directionToPlayer = firstOfficer.transform.forward;

        Vector3 sideDirection =
            Vector3.Cross(Vector3.up, directionToPlayer).normalized;

        Vector3 fixedSecondTarget =
            firstOfficer.transform.position -
            directionToPlayer * secondBackDistanceFromFirst +
            sideDirection * secondSideOffsetFromFirst;

        NavMeshHit secondTargetHit;

        if (
            NavMesh.SamplePosition(
                fixedSecondTarget,
                out secondTargetHit,
                sampleDistance,
                NavMesh.AllAreas
            )
        )
        {
            fixedSecondTarget = secondTargetHit.position;
        }

        yield return new WaitForSeconds(secondOfficerAppearDelay);

        secondOfficer.SetActive(true);

        secondOfficerAgent.speed = secondOfficerMoveSpeed;
        secondOfficerAgent.stoppingDistance = arriveDistance;
        secondOfficerAgent.updateRotation = false;
        secondOfficerAgent.isStopped = false;

        secondOfficerAnimator.enabled = true;
        secondOfficerAnimator.Play(runStateName, 0, 0f);

        secondOfficerAgent.SetDestination(fixedSecondTarget);

        while (true)
        {
            FaceTarget(secondOfficer.transform, fixedSecondTarget);

            if (HasArrived(secondOfficerAgent))
                break;

            yield return null;
        }

        secondOfficerAgent.isStopped = true;
        secondOfficerAgent.ResetPath();

        secondOfficerAnimator.Play(standStateName, 0, 0f);

        secondOfficerStanding = true;
        FacePlayer(secondOfficer.transform);

        if (hasGun)
            onBothArrivedGun?.Invoke();
        else
            onBothArrivedNoGun?.Invoke();
    }

    public void KnockDownFirstOfficer()
    {
        if (
            firstOfficer == null ||
            firstOfficerAnimator == null ||
            !firstOfficer.activeInHierarchy ||
            firstOfficerDown
        )
        {
            return;
        }

        firstOfficerDown = true;
        firstOfficerStanding = false;

        FacePlayer(firstOfficer.transform);

        if (
            firstOfficerAgent != null &&
            firstOfficerAgent.isOnNavMesh
        )
        {
            firstOfficerAgent.isStopped = true;
            firstOfficerAgent.ResetPath();
            firstOfficerAgent.enabled = false;
        }

        firstOfficerAnimator.Play(fallStateName, 0, 0f);

        StartCoroutine(FirstOfficerFallBack());

        TriggerSecondOfficerShoot();
    }

    private IEnumerator FirstOfficerFallBack()
    {
        Vector3 startPosition =
            firstOfficer.transform.position;

        Vector3 backwardDirection =
            -firstOfficer.transform.forward;

        backwardDirection.y = 0f;

        if (backwardDirection.sqrMagnitude > 0.001f)
            backwardDirection.Normalize();

        Vector3 endPosition =
            startPosition +
            backwardDirection * 0.8f;

        float time = 0f;

        while (time < 0.5f)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(
                time / 0.5f
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            firstOfficer.transform.position =
                Vector3.Lerp(
                    startPosition,
                    endPosition,
                    t
                );

            yield return null;
        }

        firstOfficer.transform.position = endPosition;
    }

    public void TriggerSecondOfficerShoot()
    {
        if (
            secondOfficerShootScheduled ||
            secondOfficerShot
        )
        {
            return;
        }

        secondOfficerShootScheduled = true;

        StartCoroutine(SecondOfficerShootAfterDelay());
    }

    private IEnumerator SecondOfficerShootAfterDelay()
    {
        yield return new WaitForSeconds(secondOfficerShootDelay);

        secondOfficerShootScheduled = false;

        if (
            secondOfficer == null ||
            secondOfficerAnimator == null ||
            !secondOfficer.activeInHierarchy ||
            secondOfficerShot
        )
        {
            yield break;
        }

        secondOfficerShot = true;
        secondOfficerStanding = true;

        if (
            secondOfficerAgent != null &&
            secondOfficerAgent.isOnNavMesh
        )
        {
            secondOfficerAgent.isStopped = true;
            secondOfficerAgent.ResetPath();
        }

        FacePlayer(secondOfficer.transform);

        secondOfficerAnimator.Play(shootStateName, 0, 0f);

        if (secondOfficerShootAudio != null)
            secondOfficerShootAudio.Play();

        onSecondOfficerShoot?.Invoke();
    }

    private bool HasArrived(NavMeshAgent agent)
    {
        if (agent == null || !agent.isOnNavMesh)
            return false;

        if (agent.pathPending)
            return false;

        if (
            agent.remainingDistance >
            agent.stoppingDistance + 0.05f
        )
        {
            return false;
        }

        if (
            agent.hasPath &&
            agent.velocity.sqrMagnitude > 0.01f
        )
        {
            return false;
        }

        return true;
    }

    private void FaceTarget(
        Transform character,
        Vector3 targetPosition
    )
    {
        if (character == null)
            return;

        Vector3 direction =
            targetPosition - character.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        character.rotation =
            Quaternion.Slerp(
                character.rotation,
                targetRotation,
                faceRotationSpeed * Time.deltaTime
            );
    }

    private void FacePlayer(Transform character)
    {
        if (character == null || player == null)
            return;

        FaceTarget(character, player.position);
    }
}