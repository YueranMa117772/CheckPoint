using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FlashbackVideoPlayer : MonoBehaviour
{
    public Transform player;
    public Transform flashbackPoint;
    public PhotoFrameDrugSequence photoSequence;

    public float flashbackDuration = 5f;

    public UnityEvent onVideoFinished;

    private Vector3 savedPlayerPosition;
    private Quaternion savedPlayerRotation;

    private bool hasSavedPlayerTransform;
    private bool flashbackRunning;

    private Coroutine flashbackRoutine;

    public void PlayFlashback()
    {
        if (flashbackRunning)
            return;

        if (player == null || flashbackPoint == null)
            return;

        savedPlayerPosition = player.position;
        savedPlayerRotation = player.rotation;

        hasSavedPlayerTransform = true;
        flashbackRunning = true;

        if (photoSequence != null &&
            photoSequence.heldFrame != null)
        {
            photoSequence.heldFrame.gameObject.SetActive(false);
        }

        TeleportPlayer(
            flashbackPoint.position,
            flashbackPoint.rotation
        );

        if (flashbackRoutine != null)
            StopCoroutine(flashbackRoutine);

        flashbackRoutine = StartCoroutine(FlashbackTimer());
    }

    private IEnumerator FlashbackTimer()
    {
        yield return new WaitForSeconds(flashbackDuration);

        flashbackRoutine = null;

        onVideoFinished?.Invoke();
    }

    public void ReturnToLocker()
    {
        if (!hasSavedPlayerTransform)
            return;

        TeleportPlayer(
            savedPlayerPosition,
            savedPlayerRotation
        );

        if (photoSequence != null &&
            photoSequence.heldFrame != null)
        {
            photoSequence.heldFrame.gameObject.SetActive(true);
        }

        flashbackRunning = false;
        hasSavedPlayerTransform = false;

        if (photoSequence != null)
            photoSequence.FlashbackFinished();
    }

    public void StopFlashback()
    {
        if (flashbackRoutine != null)
        {
            StopCoroutine(flashbackRoutine);
            flashbackRoutine = null;
        }

        flashbackRunning = false;
    }

    private void TeleportPlayer(
        Vector3 position,
        Quaternion rotation
    )
    {
        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        player.SetPositionAndRotation(
            position,
            rotation
        );

        if (controller != null)
            controller.enabled = true;
    }
}