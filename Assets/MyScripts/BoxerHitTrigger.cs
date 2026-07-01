using UnityEngine;
using UnityEngine.Events;

public class BoxerHitTrigger : MonoBehaviour
{
    public string FistTag = "OpponentFist";
    public UnityEvent OnHit;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(FistTag)) return;

        OnHit.Invoke();
    }
}