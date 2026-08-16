using UnityEngine;
using UnityEngine.Events;

public class FinalRouteDirector : MonoBehaviour
{
    public enum Route
    {
        None,
        Bat,
        Evidence,
        Leave
    }

    public Route currentRoute = Route.None;

    public bool decisionOpen = false;

    public UnityEvent onBatExit;
    public UnityEvent onEvidenceExit;
    public UnityEvent onLeaveExit;

    private bool exitTriggered = false;

    public void OpenDecision()
    {
        decisionOpen = true;
        currentRoute = Route.None;

        Debug.Log("[Route] Decision opened");
    }

    public void ChooseBat()
    {
        if (!decisionOpen)
        {
            Debug.Log("[Route] Bat ignored: decision not open");
            return;
        }

        if (currentRoute != Route.None)
        {
            Debug.Log("[Route] Bat ignored: route already selected = " + currentRoute);
            return;
        }

        currentRoute = Route.Bat;

        Debug.Log("[Route] BAT selected");
    }

    public void ChooseEvidence()
    {
        if (!decisionOpen)
        {
            Debug.Log("[Route] Evidence ignored: decision not open");
            return;
        }

        if (currentRoute != Route.None)
        {
            Debug.Log("[Route] Evidence ignored: route already selected = " + currentRoute);
            return;
        }

        currentRoute = Route.Evidence;

        Debug.Log("[Route] EVIDENCE selected");
    }

    public void ExitLockerRoom()
    {
        if (!decisionOpen)
        {
            Debug.Log("[Route] Exit ignored: decision not open");
            return;
        }

        if (exitTriggered)
            return;

        exitTriggered = true;

        if (currentRoute == Route.None)
        {
            currentRoute = Route.Leave;
            Debug.Log("[Route] LEAVE selected");
        }

        Debug.Log("[Route] Exiting locker room with route = " + currentRoute);

        if (currentRoute == Route.Bat)
        {
            Debug.Log("[Route] Starting BAT route");
            onBatExit?.Invoke();
        }

        else if (currentRoute == Route.Evidence)
        {
            Debug.Log("[Route] Starting EVIDENCE route");
            onEvidenceExit?.Invoke();
        }

        else if (currentRoute == Route.Leave)
        {
            Debug.Log("[Route] Starting LEAVE route");
            onLeaveExit?.Invoke();
        }
    }
}