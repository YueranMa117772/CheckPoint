using UnityEngine;
using UnityEngine.Events;

public class BatHitProgress : MonoBehaviour
{
    [Header("Current Hit Count")]
    public int hitCount = 0;

    [Header("Event 1")]
    [Min(1)]
    public int event1Hit = 1;
    public UnityEvent onEvent1;

    [Header("Event 2")]
    [Min(1)]
    public int event2Hit = 4;
    public UnityEvent onEvent2;

    [Header("Event 3")]
    [Min(1)]
    public int event3Hit = 8;
    public UnityEvent onEvent3;

    private bool event1Triggered = false;
    private bool event2Triggered = false;
    private bool event3Triggered = false;

    public void RegisterHit()
    {
        hitCount++;

        Debug.Log("[Bat Route] Hit Count = " + hitCount);

        if (!event1Triggered && hitCount >= event1Hit)
        {
            event1Triggered = true;
            onEvent1?.Invoke();
        }

        if (!event2Triggered && hitCount >= event2Hit)
        {
            event2Triggered = true;
            onEvent2?.Invoke();
        }

        if (!event3Triggered && hitCount >= event3Hit)
        {
            event3Triggered = true;
            onEvent3?.Invoke();
        }
    }
}