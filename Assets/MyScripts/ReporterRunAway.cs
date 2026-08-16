using UnityEngine;
using UnityEngine.AI;

public class ReporterRunAway : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;
    public Transform target;

    public float runSpeed = 3.5f;
    public float arriveDistance = 0.2f;
    public float sampleDistance = 1f;

    private bool running = false;

    private void Update()
    {
        if (!running)
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
            Disappear();
        }
    }

    public void StartRun()
    {
        if (target == null || agent == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        NavMeshHit hit;

        if (!NavMesh.SamplePosition(
            target.position,
            out hit,
            sampleDistance,
            NavMesh.AllAreas
        ))
            return;

        running = true;

        agent.speed = runSpeed;
        agent.stoppingDistance = arriveDistance;
        agent.isStopped = false;

        agent.SetDestination(hit.position);

        if (animator != null)
        {
            animator.enabled = true;
            animator.Play("run", 0, 0f);
        }
    }

    private void Disappear()
    {
        running = false;

        agent.isStopped = true;
        agent.ResetPath();

        gameObject.SetActive(false);
    }
}