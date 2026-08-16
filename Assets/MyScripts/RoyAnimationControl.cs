using UnityEngine;

public class RoyAnimationControl : MonoBehaviour
{
    public Animator animator;

    public void Stand()
    {
        animator.Play("stand", 0, 0f);
    }

    public void Talking()
    {
        animator.Play("talking", 0, 0f);
    }
}