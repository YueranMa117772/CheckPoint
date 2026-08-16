using UnityEngine;

public class BatDropSwap : MonoBehaviour
{
    public GameObject playerBat;
    public GameObject droppedBat;

    private bool dropped;

    public void DropBat()
    {
        if (dropped)
            return;

        dropped = true;

        if (playerBat != null)
            playerBat.SetActive(false);

        if (droppedBat != null)
        {
            droppedBat.transform.SetParent(null, true);
            droppedBat.SetActive(true);
        }
    }
}