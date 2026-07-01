using UnityEngine;

public class PlayerGlovePunch : MonoBehaviour
{
    public Vector3 PunchOffset = new Vector3(0f, 0f, 0.45f);
    public float PunchSpeed = 4f;
    public float ReturnSpeed = 6f;

    private Vector3 startLocalPos;
    private bool isPunching;

    private void Awake()
    {
        startLocalPos = transform.localPosition;
    }

    private void Update()
    {
        Vector3 targetPos = isPunching
            ? startLocalPos + PunchOffset
            : startLocalPos;

        float speed = isPunching
            ? PunchSpeed
            : ReturnSpeed;

        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            targetPos,
            speed * Time.deltaTime
        );

        if (isPunching && transform.localPosition == targetPos)
        {
            isPunching = false;
        }
    }

    public void Punch()
    {
        isPunching = true;
    }
}