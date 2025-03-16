using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager instance;

    [System.Serializable]
    public class Subtitle
    {
        [TextArea(3, 5)]
        public string text;
        public bool isPriority = false;
    }

    public List<Subtitle> subtitles = new List<Subtitle>();
    public TextMeshProUGUI subtitleText;

    public float minimumDuration = 3f;
    public float durationPerCharacter = 0.1f;
    public float durationPerWord = 0.5f;
    public bool useWordCount = false;
    public float additionalDelay = 1f;
    public float testDelay = 3f;

    private Queue<Command> subtitleQueue = new Queue<Command>();
    private Coroutine subtitleCoroutine;
    private bool isDisplaying = false;

    [Header("Queue Checking")]
    public float queueCheckInterval = 0.5f; // How often to check the queue (in seconds)
    private Coroutine queueCheckerCoroutine;

    private void Awake()
    {
        // Singleton pattern implementation
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (subtitleText != null)
        {
            subtitleText.text = "";
            subtitleText.gameObject.SetActive(false);
        }

        StartCoroutine(TestShowSubtitles());

        // Start the periodic queue checker
        StartQueueChecker();
    }

    // Start the coroutine that periodically checks the queue
    private void StartQueueChecker()
    {
        if (queueCheckerCoroutine != null)
            StopCoroutine(queueCheckerCoroutine);

        queueCheckerCoroutine = StartCoroutine(CheckQueuePeriodically());
    }

    // Coroutine that periodically checks if there are items in the queue
    private IEnumerator CheckQueuePeriodically()
    {
        while (true) // Run indefinitely
        {
            // Check if we're not displaying anything and have items in queue
            if (!isDisplaying && subtitleQueue.Count > 0)
            {
                ProcessQueue();
            }

            // Wait before checking again
            yield return new WaitForSeconds(queueCheckInterval);
        }
    }

    // Existing ProcessQueue method
    private void ProcessQueue()
    {
        if (!isDisplaying && subtitleQueue.Count > 0)
        {
            Command nextSubtitle = subtitleQueue.Dequeue();
            ShowSubtitle(nextSubtitle);
        }
    }

    private IEnumerator TestShowSubtitles()
    {
        yield return new WaitForSeconds(testDelay);
        // ShuffleAndEnqueueSubtitles();
        ProcessQueue();
    }

    public void putInsideSubtitleQueue(Command subtitle, bool force = false)
    {
        if (!force)
        {
            subtitleQueue.Enqueue(subtitle);
        }
        else
        {
            subtitleQueue.Clear();
            subtitleQueue.Enqueue(subtitle);
        }
    }

    public void ForceSubtitle(Command subtitle)
    {
        if (subtitleCoroutine != null)
            StopCoroutine(subtitleCoroutine);

        subtitleQueue.Clear();
        // Subtitle forcedSubtitle = new Subtitle { text = text, isPriority = true };
        ShowSubtitle(subtitle);
        // ShuffleAndEnqueueSubtitles(); 
    }

    private void ShowSubtitle(Command subtitle)
    {
        if (subtitleCoroutine != null)
            StopCoroutine(subtitleCoroutine);

        // float duration = Math.Clamp(CalculateSubtitleDuration(subtitle.commandText), minimumDuration, 1000f);
        subtitleCoroutine = StartCoroutine(DisplaySubtitle(subtitle.commandText, subtitle.timetoread));
        CommandManager.instance.currentPlayingAudioSource = SoundManager.instance.PlayLoopingClipForDuration(subtitle.seletedClip, subtitle.timetoread);
    }

    // private float CalculateSubtitleDuration(string text)
    // {
    //     if (useWordCount)
    //     {
    //         int wordCount = text.Split(' ').Length;
    //         return wordCount * durationPerWord;
    //     }
    //     else
    //     {
    //         int charCount = text.Length;
    //         return charCount * durationPerCharacter;
    //     }
    // }

    private IEnumerator DisplaySubtitle(string text, float duration)
    {
        isDisplaying = true;
        subtitleText.text = text;
        subtitleText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        subtitleText.gameObject.SetActive(false);
        yield return new WaitForSeconds(additionalDelay);
        isDisplaying = false;
        ProcessQueue();
    }
}
