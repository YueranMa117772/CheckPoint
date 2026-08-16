using UnityEngine;
using UnityEngine.Events;

public class MotorcycleDismount : MonoBehaviour
{
    public Transform player;
    public Transform dismountPoint;
    public MotorcycleMount motorcycleMount;

    public UnityEvent onDismounted;

    public void Dismount()
    {
        if (player == null || dismountPoint == null)
            return;

        if (motorcycleMount != null)
            motorcycleMount.Release();

        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        player.position = dismountPoint.position;

        if (controller != null)
            controller.enabled = true;

        onDismounted?.Invoke();
    }
}