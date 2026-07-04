using System.Collections;
using UnityEngine;

public class LockerRoomObjectSequence : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        public GameObject sceneObject;
        public GameObject heldObject;
        public AudioSource monologueAudio;
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

        if (step.sceneObject != null)
            step.sceneObject.SetActive(false);

        if (step.heldObject != null)
            step.heldObject.SetActive(true);

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
            busy = false;
            yield break;
        }

        if (step.heldObject != null)
            step.heldObject.SetActive(false);

        if (step.sceneObject != null)
            step.sceneObject.SetActive(true);

        currentStep++;

        if (currentStep < steps.Length)
            SetSceneColliders(steps[currentStep].sceneObject, true);

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
        }

        if (steps.Length > 0)
            SetSceneColliders(steps[0].sceneObject, true);
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