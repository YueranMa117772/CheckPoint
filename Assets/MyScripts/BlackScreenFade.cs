using System.Collections;
using UnityEngine;

public class BlackScreenFade : MonoBehaviour
{
    public CanvasGroup blackGroup;

    public float holdTime = 3f;
    public float fadeTime = 1f;

    private void Start()
    {
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        blackGroup.alpha = 1f;

        yield return new WaitForSeconds(holdTime);

        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            blackGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            yield return null;
        }

        blackGroup.alpha = 0f;

        gameObject.SetActive(false);
    }
}