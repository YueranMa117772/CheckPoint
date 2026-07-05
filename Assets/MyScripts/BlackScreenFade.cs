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

    public void FullBlackInstant()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }

        locked = false;
        lockPosition = false;

        SetBlack(1f, true);
    }

    public void PlayKnockdownBlack()
    {
        if (routine != null)
            StopCoroutine(routine);

        locked = false;
        lockPosition = false;

        routine = StartCoroutine(KnockdownRoutine());
    }

    public void PlayTeleportBlack(UnityAction onFullBlack, float totalTime)
    {
        if (routine != null)
            StopCoroutine(routine);

        locked = false;
        lockPosition = false;

        routine = StartCoroutine(TeleportBlackRoutine(onFullBlack, totalTime));
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

    private IEnumerator TeleportBlackRoutine(UnityAction onFullBlack, float totalTime)
    {
        if (blackGroup == null)
        {
            onFullBlack?.Invoke();
            yield break;
        }

        float halfTime = totalTime * 0.5f;

        SetBlack(0f, true);

        yield return FadeAlpha(0f, 1f, halfTime);

        onFullBlack?.Invoke();

        yield return FadeAlpha(1f, 0f, halfTime);

        SetBlack(0f, false);
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

    private IEnumerator FadeAlpha(float from, float to, float fadeTime)
    {
        if (fadeTime <= 0f)
        {
            blackGroup.alpha = to;
            yield break;
        }

        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            blackGroup.alpha = Mathf.Lerp(from, to, t / fadeTime);
            yield return null;
        }

        blackGroup.alpha = to;
    }

    private void SetBlack(float alpha, bool block)
    {
        if (blackGroup == null)
            return;

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