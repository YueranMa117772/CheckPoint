using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BoxerOpponent : MonoBehaviour
{
    public Transform ApproachTargetPoint;
    public Transform LookTarget;
    public Animator AvatarAnimator;

    public float FaceRotationSpeed = 8f;

    public string PunchTriggerString = "punch";

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
        {
            return;
        }

        if (ApproachTargetPoint == null)
        {
            return;
        }

        if (!agent.isOnNavMesh)
        {
            return;
        }

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
        {
            AvatarAnimator.SetTrigger(PunchTriggerString);
        }
    }

    private void FacePlayer()
    {
        Transform target = LookTarget != null ? LookTarget : ApproachTargetPoint;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            FaceRotationSpeed * Time.deltaTime
        );
    }
}