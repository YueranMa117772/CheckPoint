using System.Collections;
using UnityEngine;

public class RouteExitTeleporter : MonoBehaviour, ISubjectRelay
{
    public Transform subject;
    public Transform destination;
    public bool syncOrientation = false;

    public float cooldown = 0.25f;
    public float cooldownClock;

    public RouteExitFade routeExitFade;
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

        Debug.Log(
            "[RouteTeleport] Teleport started"
        );

        cooldownClock = cooldown;

        if (routeExitFade != null &&
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

        routeExitFade.PlayTeleportBlack(
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
        Debug.Log(
            "[RouteTeleport] FORCE TELEPORT"
        );

        if (subject == null ||
            destination == null)
        {
            Debug.LogWarning(
                "[RouteTeleport] Missing subject or destination"
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