using UnityEngine;

public class KnockdownSequence : MonoBehaviour
{
    public SimpleTeleporter teleporter;
    public BlackScreenFade blackScreenFade;

    public void Play()
    {
        if (teleporter != null)
            teleporter.ForceTeleport();

        if (blackScreenFade != null)
            blackScreenFade.PlayKnockdownBlack();
    }
}