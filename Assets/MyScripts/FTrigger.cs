using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FTrigger : MonoBehaviour
{
    public UnityEvent onPressF;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            onPressF.Invoke();
        }
    }
}