using UnityEngine;
using UnityEngine.Events;

public class AudioEndEvent : MonoBehaviour
{
    public AudioSource audioSource;
    public UnityEvent onAudioFinished;

    private bool hasPlayed;
    private bool hasTriggered;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (audioSource == null)
            return;

        if (hasTriggered)
            return;

        if (audioSource.isPlaying)
        {
            hasPlayed = true;
            return;
        }

        if (hasPlayed)
        {
            hasTriggered = true;
            onAudioFinished.Invoke();
        }
    }
}