using System.Collections;
using UnityEngine;

public class ApproachTargetDodgePoint : MonoBehaviour
{
    public float DodgeAngle = 45f;
    public float HoldTime = 0.5f;

    private Vector3 originalLocalPosition;
    private Coroutine routine;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
    }

    public void DodgeLeft()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }

        routine = StartCoroutine(DodgeRoutine());
    }

    private IEnumerator DodgeRoutine()
    {
        Quaternion leftRotation = Quaternion.Euler(0f, -DodgeAngle, 0f);
        Vector3 dodgeLocalPosition = leftRotation * originalLocalPosition;

        transform.localPosition = dodgeLocalPosition;

        yield return new WaitForSeconds(HoldTime);

        transform.localPosition = originalLocalPosition;

        routine = null;
    }
}