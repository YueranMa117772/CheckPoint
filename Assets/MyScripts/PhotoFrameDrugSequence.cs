using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PhotoFrameDrugSequence : MonoBehaviour
{
    enum FrameState
    {
        Waiting,
        CanFlip,
        BackAudio,
        CanRemoveTape,
        DrugRevealed,
        Flashback,
        FinalAudio,
        Finished
    }

    public LockerRoomObjectSequence lockerSequence;
    public int lockerStepIndex;

    public Transform heldFrame;
    public Vector3 flipRotation = new Vector3(0, 180, 0);

    public GameObject pressFUI;

    public GameObject heldTape;
    public GameObject sceneTape;

    public AudioSource backAudio;
    public AudioSource finalAudio;

    public float flashbackDelay = 1.5f;
    public UnityEvent onDrugRevealed;
    public UnityEvent onFlashbackStart;

    FrameState state = FrameState.Waiting;
    Quaternion originalRotation;

    void Start()
    {
        if (heldFrame != null)
            originalRotation = heldFrame.localRotation;

        if (pressFUI != null)
            pressFUI.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.fKey.wasPressedThisFrame)
            return;

        if (state == FrameState.CanFlip)
            FlipFrame();
        else if (state == FrameState.CanRemoveTape)
            RemoveTape();
    }

    public void EnableFlip()
    {
        if (state != FrameState.Waiting)
            return;

        state = FrameState.CanFlip;

        if (pressFUI != null)
            pressFUI.SetActive(true);
    }

    void FlipFrame()
    {
        state = FrameState.BackAudio;

        if (pressFUI != null)
            pressFUI.SetActive(false);

        if (heldFrame != null)
            heldFrame.localRotation = originalRotation * Quaternion.Euler(flipRotation);

        if (backAudio != null)
        {
            backAudio.Stop();
            backAudio.Play();
        }
    }

    public void EnableRemoveTape()
    {
        if (state != FrameState.BackAudio)
            return;

        state = FrameState.CanRemoveTape;

        if (pressFUI != null)
            pressFUI.SetActive(true);
    }

    void RemoveTape()
    {
        state = FrameState.DrugRevealed;

        if (pressFUI != null)
            pressFUI.SetActive(false);

        if (heldTape != null)
            heldTape.SetActive(false);

        if (onDrugRevealed != null)
            onDrugRevealed.Invoke();

        StartCoroutine(StartFlashbackAfterDelay());
    }

    IEnumerator StartFlashbackAfterDelay()
    {
        yield return new WaitForSeconds(flashbackDelay);

        state = FrameState.Flashback;

        if (onFlashbackStart != null)
            onFlashbackStart.Invoke();
    }

    public void FlashbackFinished()
    {
        if (state != FrameState.Flashback)
            return;

        state = FrameState.FinalAudio;

        if (finalAudio != null)
        {
            finalAudio.Stop();
            finalAudio.Play();
        }
    }

    public void FinishSequence()
    {
        if (state != FrameState.FinalAudio)
            return;

        state = FrameState.Finished;

        if (pressFUI != null)
            pressFUI.SetActive(false);

        if (sceneTape != null)
            sceneTape.SetActive(false);

        if (lockerSequence != null)
            lockerSequence.CompleteHeldInspection(lockerStepIndex);
    }
}