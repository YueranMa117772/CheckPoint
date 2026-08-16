using UnityEngine;

public class RoyFacePlayer : MonoBehaviour
{
    public Transform player;
    public float rotationSpeed = 8f;

    private bool facing = false;

    private void Update()
    {
        if (!facing)
            return;

        if (player == null)
            return;

        Vector3 direction =
            player.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    public void StartFacing()
    {
        facing = true;

        Debug.Log("[RoyBat] Start facing player");
    }

    public void StopFacing()
    {
        facing = false;

        Debug.Log("[RoyBat] Stop facing player");
    }
}