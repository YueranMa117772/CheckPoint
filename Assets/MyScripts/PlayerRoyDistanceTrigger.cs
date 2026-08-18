using UnityEngine;
using UnityEngine.Events;

public class PlayerRoyDistanceTrigger : MonoBehaviour
{
    public Transform player;
    public Transform roy;

    public float triggerDistance = 3f;

    public UnityEvent onDistanceReached;

    private bool triggered;

    private void Update()
    {
        if (triggered)
            return;

        if (player == null || roy == null)
            return;

        if (Vector3.Distance(player.position, roy.position) <= triggerDistance)
        {
            triggered = true;
            onDistanceReached?.Invoke();
        }
    }
}