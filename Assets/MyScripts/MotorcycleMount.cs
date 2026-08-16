using UnityEngine;
using UnityEngine.Events;

public class MotorcycleMount : MonoBehaviour
{
    public Transform player;
    public Transform seatPoint;

    public UnityEvent onMounted;

    private bool mounted;

    public void Mount()
    {
        if (mounted)
            return;

        if (player == null || seatPoint == null)
            return;

        mounted = true;

        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        player.position = seatPoint.position;

        if (controller != null)
            controller.enabled = true;

        onMounted?.Invoke();
    }

    private void LateUpdate()
    {
        if (!mounted)
            return;

        if (player == null || seatPoint == null)
            return;

        player.position = seatPoint.position;
    }

    public void Release()
    {
        mounted = false;
    }
}