using UnityEngine;
using UnityEngine.Events;

public class BoxerHitTrigger : MonoBehaviour
{
    public string FistTag = "OpponentFist";

    public int RequiredHitCount = 3;

    public UnityEvent OnHit;
    public UnityEvent OnHitCountReached;

    private int hitCount = 0;
    private bool countEventTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(FistTag)) return;

        hitCount++;

        OnHit.Invoke();

        if (!countEventTriggered && hitCount >= RequiredHitCount)
        {
            countEventTriggered = true;
            OnHitCountReached.Invoke();
        }
    }

    public void ResetHitCount()
    {
        hitCount = 0;
        countEventTriggered = false;
    }
}