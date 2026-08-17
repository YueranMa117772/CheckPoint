using UnityEngine;
using UnityEngine.Events;

public class RoyGunRouteTrigger : MonoBehaviour
{
    public UnityEvent onBegin;

    public void Begin()
    {
        onBegin?.Invoke();
    }
}