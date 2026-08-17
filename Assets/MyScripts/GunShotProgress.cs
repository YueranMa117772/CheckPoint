using UnityEngine;
using UnityEngine.Events;

public class GunShotProgress : MonoBehaviour
{
    public int shotCount = 0;

    [Min(1)]
    public int event1Shot = 1;
    public UnityEvent onEvent1;

    [Min(1)]
    public int event2Shot = 2;
    public UnityEvent onEvent2;

    [Min(1)]
    public int event3Shot = 3;
    public UnityEvent onEvent3;

    private bool event1Triggered;
    private bool event2Triggered;
    private bool event3Triggered;

    public void RegisterShot()
    {
        shotCount++;

        if (!event1Triggered && shotCount >= event1Shot)
        {
            event1Triggered = true;
            onEvent1?.Invoke();
        }

        if (!event2Triggered && shotCount >= event2Shot)
        {
            event2Triggered = true;
            onEvent2?.Invoke();
        }

        if (!event3Triggered && shotCount >= event3Shot)
        {
            event3Triggered = true;
            onEvent3?.Invoke();
        }
    }
}