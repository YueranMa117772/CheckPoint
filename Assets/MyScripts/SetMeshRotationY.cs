using UnityEngine;

public class SetMeshRotationY : MonoBehaviour
{
    public float Y = 90f;

    public void Apply()
    {
        Vector3 euler = transform.localEulerAngles;
        euler.y = Y;
        transform.localEulerAngles = euler;
    }
}