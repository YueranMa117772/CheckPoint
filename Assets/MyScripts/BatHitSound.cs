using UnityEngine;

public class BatHitSound : MonoBehaviour
{
    public AudioSource audioSource;

    public void PlayHit()
    {
        if (audioSource == null || audioSource.clip == null)
            return;

        audioSource.PlayOneShot(audioSource.clip);

        Debug.Log("[BatHit] Sound played");
    }
}