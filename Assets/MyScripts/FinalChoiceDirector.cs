using UnityEngine;
using UnityEngine.Events;

public class FinalChoiceDirector : MonoBehaviour
{
    public FinalRouteDirector routeDirector;

    public UnityEvent onBatGun;
    public UnityEvent onBatKey;

    public UnityEvent onEvidenceGun;
    public UnityEvent onEvidenceKey;

    public UnityEvent onLeaveGun;
    public UnityEvent onLeaveKey;

    private bool choiceMade;

    public void ChooseGun()
    {
        if (choiceMade)
            return;

        if (routeDirector == null)
            return;

        choiceMade = true;

        if (routeDirector.currentRoute == FinalRouteDirector.Route.Bat)
        {
            onBatGun?.Invoke();
        }
        else if (routeDirector.currentRoute == FinalRouteDirector.Route.Evidence)
        {
            onEvidenceGun?.Invoke();
        }
        else if (routeDirector.currentRoute == FinalRouteDirector.Route.Leave)
        {
            onLeaveGun?.Invoke();
        }
    }

    public void ChooseKey()
    {
        if (choiceMade)
            return;

        if (routeDirector == null)
            return;

        choiceMade = true;

        if (routeDirector.currentRoute == FinalRouteDirector.Route.Bat)
        {
            onBatKey?.Invoke();
        }
        else if (routeDirector.currentRoute == FinalRouteDirector.Route.Evidence)
        {
            onEvidenceKey?.Invoke();
        }
        else if (routeDirector.currentRoute == FinalRouteDirector.Route.Leave)
        {
            onLeaveKey?.Invoke();
        }
    }
}