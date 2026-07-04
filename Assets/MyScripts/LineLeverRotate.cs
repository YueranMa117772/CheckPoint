using UnityEngine;
using System.Collections;

/// Rotates the line lever.
public class LineLeverRotate : MonoBehaviour
{
    public Transform Pivot;
    public float RotateAngle = 25f;
    public float RotateSpeed = 200f;

    private bool isMoving;

    public void PullLever()
    {
        if (isMoving)
        {
            return;
        }

        if (Pivot == null)
        {
            return;
        }

        StartCoroutine(RotateLever());
    }

    private IEnumerator RotateLever()
    {
        isMoving = true;

        float movedAngle = 0f;
        float targetAngle = Mathf.Abs(RotateAngle);
        float direction = Mathf.Sign(RotateAngle);

        while (movedAngle < targetAngle)
        {
            float step = RotateSpeed * Time.deltaTime;

            if (movedAngle + step > targetAngle)
            {
                step = targetAngle - movedAngle;
            }

            transform.RotateAround(Pivot.position, Pivot.up, step * direction);

            movedAngle += step;
            yield return null;
        }

        isMoving = false;
    }
}