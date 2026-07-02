using System.Collections;
using UnityEngine;

public class ApproachTargetDodgePoint : MonoBehaviour
{
    public float DodgeDistance = 0.5f;
    public float HoldTime = 0.5f;

    private Vector3 originalLocalPosition;
    private Coroutine routine;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
    }

    public void DodgeLeft()
    {
        MoveToSide(-DodgeDistance);
    }

    public void DodgeRight()
    {
        MoveToSide(DodgeDistance);
    }

    private void MoveToSide(float xOffset)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }

        routine = StartCoroutine(DodgeRoutine(xOffset));
    }

    private IEnumerator DodgeRoutine(float xOffset)
    {
        transform.localPosition = originalLocalPosition + new Vector3(xOffset, 0f, 0f);

        yield return new WaitForSeconds(HoldTime);

        transform.localPosition = originalLocalPosition;

        routine = null;
    }
}