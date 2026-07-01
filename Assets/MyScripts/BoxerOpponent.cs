using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BoxerOpponent : MonoBehaviour
{
    public Transform ApproachTargetPoint;
    public Transform LookTarget;
    public Animator AvatarAnimator;

    public Transform WinPoint;

    public float FaceRotationSpeed = 8f;

    public string PunchTriggerString = "punch";
    public string CheerTriggerString = "cheer";

    private NavMeshAgent agent;
    private bool started;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    private void Start()
    {
        started = false;

        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    private void Update()
    {
        if (!started)
            return;

        if (ApproachTargetPoint == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        agent.isStopped = false;
        agent.SetDestination(ApproachTargetPoint.position);

        FacePlayer();
    }

    public void StartRound()
    {
        started = true;

        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(ApproachTargetPoint.position);
        }

        if (AvatarAnimator != null)
            AvatarAnimator.SetTrigger(PunchTriggerString);
    }

    public void PlayWinCheer()
    {
        started = false;

        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();

            if (WinPoint != null)
                agent.Warp(WinPoint.position);
        }
        else if (WinPoint != null)
        {
            transform.position = WinPoint.position;
        }

        if (WinPoint != null)
            transform.rotation = WinPoint.rotation;

        if (AvatarAnimator != null)
            AvatarAnimator.SetTrigger(CheerTriggerString);
    }

    private void FacePlayer()
    {
        Transform target = LookTarget != null ? LookTarget : ApproachTargetPoint;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            FaceRotationSpeed * Time.deltaTime
        );
    }
}