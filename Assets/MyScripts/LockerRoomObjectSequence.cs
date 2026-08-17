using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LockerRoomObjectSequence : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        public GameObject sceneObject;
        public GameObject heldObject;
        public GameObject pickupUI;

        public AudioSource monologueAudio;
        public AudioSource cheatingMonologueAudio;

        public UnityEvent onPicked;
        public UnityEvent onDropped;

        public bool requiredForCompletion = true;
        public bool availableAtStart = true;
        public bool keepHeldAtEnd;

        [HideInInspector] public bool inspected;
        [HideInInspector] public bool interactionEnabled;
        [HideInInspector] public bool choiceMode;
    }

    public Step[] steps;

    public Collider[] lockerDoorColliders;

    public bool foundCheating = false;

    public UnityEvent onAllRequiredInspected;

    bool busy;
    bool completionTriggered;
    int selectedChoiceIndex = -1;

    bool[] lockerDoorColliderStates;
    bool lockerDoorCollidersTemporarilyLocked;

    void Start()
    {
        ResetSequence();
    }

    public void SetFoundCheating()
    {
        foundCheating = true;
    }

    public void PickStep(int index)
    {
        if (busy)
            return;

        if (!ValidIndex(index))
            return;

        Step step = steps[index];

        if (!step.interactionEnabled)
            return;

        if (step.choiceMode)
        {
            SelectChoice(index);
            return;
        }

        StartCoroutine(RunInspection(index));
    }

    IEnumerator RunInspection(int index)
    {
        Step step = steps[index];
        busy = true;

        LockLockerDoorColliders();

        SetAllSceneColliders(false);
        SetAllPickupUI(false);

        if (step.sceneObject != null)
            step.sceneObject.SetActive(false);

        if (step.heldObject != null)
            step.heldObject.SetActive(true);

        if (step.onPicked != null)
            step.onPicked.Invoke();

        AudioSource audioToPlay = step.monologueAudio;

        if (foundCheating && step.cheatingMonologueAudio != null)
            audioToPlay = step.cheatingMonologueAudio;

        if (audioToPlay != null)
        {
            audioToPlay.Stop();
            audioToPlay.Play();

            while (audioToPlay.isPlaying)
                yield return null;
        }

        if (step.keepHeldAtEnd)
        {
            step.interactionEnabled = false;
            busy = false;
            yield break;
        }

        if (step.heldObject != null)
            step.heldObject.SetActive(false);

        if (step.sceneObject != null)
            step.sceneObject.SetActive(true);

        if (step.onDropped != null)
            step.onDropped.Invoke();

        step.inspected = true;
        step.interactionEnabled = false;

        CheckAllRequiredInspected();

        busy = false;

        RestoreLockerDoorColliders();

        RefreshInteractions();
    }

    public void CompleteHeldInspection(int index)
    {
        if (!ValidIndex(index))
            return;

        Step step = steps[index];

        if (step.heldObject != null)
            step.heldObject.SetActive(false);

        if (step.sceneObject != null)
            step.sceneObject.SetActive(true);

        if (step.onDropped != null)
            step.onDropped.Invoke();

        step.inspected = true;
        step.interactionEnabled = false;

        CheckAllRequiredInspected();

        busy = false;

        RestoreLockerDoorColliders();

        RefreshInteractions();
    }

    public void EnableStepAsChoice(int index)
    {
        if (!ValidIndex(index))
            return;

        Step step = steps[index];

        step.choiceMode = true;
        step.interactionEnabled = true;

        if (step.sceneObject != null)
            step.sceneObject.SetActive(true);

        if (step.heldObject != null)
            step.heldObject.SetActive(false);

        if (!busy)
            RefreshInteractions();
    }

    public void EnableStep(int index)
    {
        if (!ValidIndex(index))
            return;

        Step step = steps[index];

        step.choiceMode = false;
        step.interactionEnabled = true;

        if (step.sceneObject != null)
            step.sceneObject.SetActive(true);

        if (step.heldObject != null)
            step.heldObject.SetActive(false);

        if (!busy)
            RefreshInteractions();
    }

    public void DisableStep(int index)
    {
        if (!ValidIndex(index))
            return;

        Step step = steps[index];
        step.interactionEnabled = false;

        SetSceneColliders(step.sceneObject, false);
        SetPickupUI(step, false);
    }

    void SelectChoice(int index)
    {
        busy = true;
        SetAllSceneColliders(false);
        SetAllPickupUI(false);

        if (selectedChoiceIndex >= 0 && selectedChoiceIndex != index)
            ReleaseChoice(selectedChoiceIndex);

        Step step = steps[index];

        if (step.sceneObject != null)
            step.sceneObject.SetActive(false);

        if (step.heldObject != null)
            step.heldObject.SetActive(true);

        step.interactionEnabled = false;
        selectedChoiceIndex = index;

        if (step.onPicked != null)
            step.onPicked.Invoke();

        busy = false;
        RefreshInteractions();
    }

    public void ClearChoice()
    {
        if (selectedChoiceIndex < 0)
            return;

        ReleaseChoice(selectedChoiceIndex);
        selectedChoiceIndex = -1;
        RefreshInteractions();
    }

    void ReleaseChoice(int index)
    {
        if (!ValidIndex(index))
            return;

        Step step = steps[index];

        if (step.heldObject != null)
            step.heldObject.SetActive(false);

        if (step.sceneObject != null)
            step.sceneObject.SetActive(true);

        step.interactionEnabled = true;

        if (step.onDropped != null)
            step.onDropped.Invoke();
    }

    void CheckAllRequiredInspected()
    {
        if (completionTriggered || steps == null)
            return;

        bool hasRequiredStep = false;

        for (int i = 0; i < steps.Length; i++)
        {
            if (!steps[i].requiredForCompletion)
                continue;

            hasRequiredStep = true;

            if (!steps[i].inspected)
                return;
        }

        if (!hasRequiredStep)
            return;

        completionTriggered = true;

        if (onAllRequiredInspected != null)
            onAllRequiredInspected.Invoke();
    }

    public void ResetSequence()
    {
        StopAllCoroutines();
        StopAllAudio();

        busy = false;
        completionTriggered = false;
        selectedChoiceIndex = -1;
        foundCheating = false;

        RestoreLockerDoorColliders();

        if (steps == null)
            return;

        for (int i = 0; i < steps.Length; i++)
        {
            Step step = steps[i];

            step.inspected = false;
            step.choiceMode = false;
            step.interactionEnabled = step.availableAtStart;

            if (step.sceneObject != null)
                step.sceneObject.SetActive(true);

            if (step.heldObject != null)
                step.heldObject.SetActive(false);
        }

        RefreshInteractions();
    }

    void RefreshInteractions()
    {
        if (steps == null)
            return;

        for (int i = 0; i < steps.Length; i++)
        {
            bool active = steps[i].interactionEnabled && i != selectedChoiceIndex;
            SetSceneColliders(steps[i].sceneObject, active);
            SetPickupUI(steps[i], active);
        }
    }

    void SetAllSceneColliders(bool enabled)
    {
        if (steps == null)
            return;

        for (int i = 0; i < steps.Length; i++)
            SetSceneColliders(steps[i].sceneObject, enabled);
    }

    void SetSceneColliders(GameObject obj, bool enabled)
    {
        if (obj == null)
            return;

        Collider[] colliders = obj.GetComponentsInChildren<Collider>(true);

        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = enabled;
    }

    void SetAllPickupUI(bool active)
    {
        if (steps == null)
            return;

        for (int i = 0; i < steps.Length; i++)
            SetPickupUI(steps[i], active);
    }

    void SetPickupUI(Step step, bool active)
    {
        if (step == null)
            return;

        if (step.pickupUI != null)
            step.pickupUI.SetActive(active);
    }

    void LockLockerDoorColliders()
    {
        if (lockerDoorColliders == null)
            return;

        lockerDoorColliderStates = new bool[lockerDoorColliders.Length];

        for (int i = 0; i < lockerDoorColliders.Length; i++)
        {
            if (lockerDoorColliders[i] == null)
                continue;

            lockerDoorColliderStates[i] = lockerDoorColliders[i].enabled;
            lockerDoorColliders[i].enabled = false;
        }

        lockerDoorCollidersTemporarilyLocked = true;
    }

    void RestoreLockerDoorColliders()
    {
        if (!lockerDoorCollidersTemporarilyLocked || lockerDoorColliders == null)
            return;

        for (int i = 0; i < lockerDoorColliders.Length; i++)
        {
            if (lockerDoorColliders[i] == null)
                continue;

            if (lockerDoorColliderStates != null && i < lockerDoorColliderStates.Length)
                lockerDoorColliders[i].enabled = lockerDoorColliderStates[i];
        }

        lockerDoorCollidersTemporarilyLocked = false;
    }

    void StopAllAudio()
    {
        if (steps == null)
            return;

        for (int i = 0; i < steps.Length; i++)
        {
            if (steps[i].monologueAudio != null)
                steps[i].monologueAudio.Stop();

            if (steps[i].cheatingMonologueAudio != null)
                steps[i].cheatingMonologueAudio.Stop();
        }
    }

    bool ValidIndex(int index)
    {
        return steps != null && index >= 0 && index < steps.Length;
    }
}