using UnityEngine;
using UnityEngine.Events;

public class DrawerPull : MonoBehaviour
{
    public Transform drawer;

    public int requiredPulls = 3;

    public Vector3 smallLocalMove;
    public Vector3 finalLocalMove;

    public AudioSource pullAudio;

    public UnityEvent onFullyOpened;

    private int pullCount;
    private bool fullyOpened;

    public void Pull()
    {
        if (fullyOpened || drawer == null)
            return;

        pullCount++;

        if (pullAudio != null)
            pullAudio.Play();

        if (pullCount < requiredPulls)
        {
            drawer.localPosition += smallLocalMove;
        }
        else
        {
            drawer.localPosition += finalLocalMove;

            fullyOpened = true;

            onFullyOpened?.Invoke();
        }
    }
}