using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class ReporterWalkToTarget : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;
    public Transform target;

    public float moveSpeed = 1.5f;
    public float arriveDistance = 0.2f;
    public float sampleDistance = 1f;

    public UnityEvent onArrived;

    private bool walking = false;

    private void Update()
    {
        if (!walking)
            return;

        if (agent == null || !agent.isOnNavMesh)
            return;

        if (agent.pathPending)
            return;

        if (
            agent.remainingDistance <= agent.stoppingDistance &&
            (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f)
        )
        {
            Arrive();
        }
    }

    public void StartWalk()
    {
        if (target == null || agent == null)
            return;

        if (!agent.isOnNavMesh)
        {
            Debug.Log("[Reporter] Agent is not on NavMesh");
            return;
        }

        NavMeshHit hit;

        if (!NavMesh.SamplePosition(
            target.position,
            out hit,
            sampleDistance,
            NavMesh.AllAreas
        ))
        {
            Debug.Log("[Reporter] Leave target is not near NavMesh");
            return;
        }

        walking = true;

        agent.speed = moveSpeed;
        agent.stoppingDistance = arriveDistance;
        agent.isStopped = false;

        agent.SetDestination(hit.position);

        if (animator != null)
        {
            animator.enabled = true;
            animator.Play("walk", 0, 0f);
        }

        Debug.Log("[Reporter] Start walking");
    }

    private void Arrive()
    {
        walking = false;

        agent.isStopped = true;
        agent.ResetPath();

        if (animator != null)
            animator.enabled = false;

        Debug.Log("[Reporter] Arrived at leave target");

        onArrived?.Invoke();
    }
}