using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FETrigger : MonoBehaviour
{
    public UnityEvent onPressF;
    public UnityEvent onPressE;

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            onPressF?.Invoke();
        }
        else if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            onPressE?.Invoke();
        }
    }
}