using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BlackScreenFade : MonoBehaviour
{
    public CanvasGroup blackGroup;

    public float introHoldTime = 3f;
    public float introFadeOutTime = 1f;

    public float knockdownHoldTime = 1f;
    public float knockdownFadeOutTime = 1f;

    public Transform playerToLock;

    public UnityEvent onIntroFadeOutStart;
    public UnityEvent onKnockdownFadeOutStart;

    private Coroutine routine;

    private bool locked = false;
    private bool lockPosition = false;

    private Quaternion lockedRotation;
    private Vector3 lockedPosition;

    private void Start()
    {
        routine = StartCoroutine(IntroRoutine());
    }

    private void LateUpdate()
    {
        if (!locked)
            return;

        if (playerToLock != null)
        {
            playerToLock.rotation = lockedRotation;

            if (lockPosition)
                playerToLock.position = lockedPosition;
        }
    }

    public void PlayKnockdownBlack()
    {
        if (routine != null)
            StopCoroutine(routine);

        locked = false;
        lockPosition = false;

        routine = StartCoroutine(KnockdownRoutine());
    }

    private IEnumerator IntroRoutine()
    {
        SetBlack(1f, true);

        LockPlayer(false);

        yield return new WaitForSeconds(introHoldTime);

        onIntroFadeOutStart?.Invoke();

        UnlockPlayer();

        yield return FadeOut(introFadeOutTime);
    }

    private IEnumerator KnockdownRoutine()
    {
        SetBlack(1f, true);

        LockPlayer(true);

        yield return new WaitForSeconds(knockdownHoldTime);

        onKnockdownFadeOutStart?.Invoke();

        UnlockPlayer();

        yield return FadeOut(knockdownFadeOutTime);
    }

    private IEnumerator FadeOut(float fadeTime)
    {
        if (fadeTime <= 0f)
        {
            SetBlack(0f, false);
            yield break;
        }

        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            blackGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }

        SetBlack(0f, false);
    }

    private void SetBlack(float alpha, bool block)
    {
        blackGroup.alpha = alpha;
        blackGroup.blocksRaycasts = block;
    }

    private void LockPlayer(bool shouldLockPosition)
    {
        if (playerToLock == null)
            return;

        lockedRotation = playerToLock.rotation;
        lockedPosition = playerToLock.position;

        lockPosition = shouldLockPosition;
        locked = true;
    }

    private void UnlockPlayer()
    {
        locked = false;
        lockPosition = false;
    }
}