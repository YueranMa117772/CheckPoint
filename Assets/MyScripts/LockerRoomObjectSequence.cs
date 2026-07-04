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

        public UnityEvent onPicked;

        public bool keepHeldAtEnd;
    }

    public Step[] steps;

    int currentStep;
    bool busy;

    void Start()
    {
        ResetSequence();
    }

    public void PickCurrent()
    {
        if (busy)
            return;

        if (steps == null || steps.Length == 0)
            return;

        if (currentStep < 0 || currentStep >= steps.Length)
            return;

        StartCoroutine(RunStep(steps[currentStep]));
    }

    IEnumerator RunStep(Step step)
    {
        busy = true;

        SetAllSceneColliders(false);
        SetAllPickupUI(false);

        if (step.sceneObject != null)
            step.sceneObject.SetActive(false);

        if (step.heldObject != null)
            step.heldObject.SetActive(true);

        if (step.onPicked != null)
            step.onPicked.Invoke();

        if (step.monologueAudio != null)
        {
            step.monologueAudio.Stop();
            step.monologueAudio.Play();

            while (step.monologueAudio.isPlaying)
                yield return null;
        }

        if (step.keepHeldAtEnd)
        {
            SetAllSceneColliders(false);
            SetAllPickupUI(false);
            busy = false;
            yield break;
        }

        if (step.heldObject != null)
            step.heldObject.SetActive(false);

        if (step.sceneObject != null)
            step.sceneObject.SetActive(true);

        currentStep++;

        SetAllSceneColliders(false);
        SetAllPickupUI(false);

        if (currentStep < steps.Length)
        {
            SetSceneColliders(steps[currentStep].sceneObject, true);
            SetPickupUI(steps[currentStep], true);
        }

        busy = false;
    }

    public void ResetSequence()
    {
        StopAllCoroutines();
        StopAllAudio();

        currentStep = 0;
        busy = false;

        if (steps == null)
            return;

        for (int i = 0; i < steps.Length; i++)
        {
            if (steps[i].sceneObject != null)
                steps[i].sceneObject.SetActive(true);

            if (steps[i].heldObject != null)
                steps[i].heldObject.SetActive(false);

            SetSceneColliders(steps[i].sceneObject, false);
            SetPickupUI(steps[i], false);
        }

        if (steps.Length > 0)
        {
            SetSceneColliders(steps[0].sceneObject, true);
            SetPickupUI(steps[0], true);
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

    void StopAllAudio()
    {
        if (steps == null)
            return;

        for (int i = 0; i < steps.Length; i++)
        {
            if (steps[i].monologueAudio != null)
                steps[i].monologueAudio.Stop();
        }
    }
}