using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CountdownClockUI : MonoBehaviour
{
    public TMP_Text timerText;
    public float totalTime = 10f;
    public bool autoStart = true;

    public AudioSource tickAudio;

    public UnityEvent OnFinished;

    private float timeLeft;
    private bool running;
    private bool finished;
    private int lastSecond;

    private void Start()
    {
        timeLeft = totalTime;
        lastSecond = Mathf.CeilToInt(timeLeft);

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

        int currentSecond = Mathf.CeilToInt(timeLeft);

        if (currentSecond != lastSecond)
        {
            lastSecond = currentSecond;

            if (currentSecond > 0 && tickAudio != null)
            {
                tickAudio.Play();
            }
        }

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
        lastSecond = Mathf.CeilToInt(timeLeft);

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