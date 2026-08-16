using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTeleporter : MonoBehaviour, ISubjectRelay
{
    public Transform subject;
    public Transform destination;
    public bool syncOrientation = false;

    public float activaitonDelay = 0.25f;
    public float cooldown = 0.25f;
    public float cooldownClock;

    public BlackScreenFade blackScreenFade;
    public float teleportFadeTime = 1.75f;

    private bool teleportingWithFade = false;

    private void FixedUpdate()
    {
        if (cooldownClock > 0)
            cooldownClock -= Time.fixedDeltaTime;
        else
            cooldownClock = 0;
    }

    public void Teleport()
    {
        if (cooldownClock > 0)
            return;

        if (teleportingWithFade)
            return;

        Debug.Log("Teleporting! " + name);

        cooldownClock = cooldown;

        if (blackScreenFade != null &&
            teleportFadeTime > 0f)
        {
            StartCoroutine(
                TeleportWithFadeRoutine()
            );
        }
        else
        {
            ForceTeleport();
        }
    }

    private IEnumerator TeleportWithFadeRoutine()
    {
        teleportingWithFade = true;

        blackScreenFade.PlayTeleportBlack(
            ForceTeleport,
            teleportFadeTime
        );

        yield return new WaitForSeconds(
            teleportFadeTime
        );

        teleportingWithFade = false;
    }

    public void ForceTeleport()
    {
        Debug.Log("FORCE TELEPORT");

        if (subject == null ||
            destination == null)
        {
            Debug.LogWarning(
                "SimpleTeleporter missing subject or destination."
            );

            return;
        }

        subject.position =
            destination.position;

        if (syncOrientation)
        {
            subject.rotation =
                destination.rotation;
        }

        Physics.SyncTransforms();
    }

    void ISubjectRelay.SyncSubject(
        GameObject newSubject
    )
    {
        subject = newSubject.transform;
    }
}