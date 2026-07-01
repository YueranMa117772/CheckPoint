using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountdownClockUI : MonoBehaviour
{
    public TMP_Text timerText;
    public float totalTime = 10f;
    public bool autoStart = true;

    public UnityEvent OnFinished;

    private float timeLeft;
    private bool running;
    private bool finished;

    private void Start()
    {
        timeLeft = totalTime;
        UpdateText();

        if (autoStart)
        {
            StartCountdown();
        }
    }

    private void Update()
    {
        if (!running || finished)
        {
            return;
        }

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            finished = true;
            running = false;

            UpdateText();
            OnFinished.Invoke();

            return;
        }

        UpdateText();
    }

    public void StartCountdown()
    {
        timeLeft = totalTime;
        running = true;
        finished = false;

        UpdateText();
    }

    private void UpdateText()
    {
        int seconds = Mathf.CeilToInt(timeLeft);

        int minutes = seconds / 60;
        int remainSeconds = seconds % 60;

        timerText.text = minutes.ToString("00") + ":" + remainSeconds.ToString("00");
    }
}