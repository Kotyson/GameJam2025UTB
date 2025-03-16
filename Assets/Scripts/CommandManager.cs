using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommandManager : MonoBehaviour
{
    public static CommandManager instance;
    public int angerMeter = 0;
    public int maxAnger = 2;
    public int minAnger = 0;

    public int timeMin = 5; // seconds
    public int timeMax = 20; // seconds
    public float timeLeft = 0;

    public bool generateRandomCommands = true;

    public Command initialCommand;

    public List<Command> tier1RandomCommands = new List<Command>();
    public List<Command> tier2RandomCommands = new List<Command>();
    public List<Command> tier3RandomCommands = new List<Command>();

    public List<Command> redToGreenCommand = new List<Command>();
    public List<Command> greenToRedCommand = new List<Command>();

    public List<AudioClip> tier1AudioVariants = new List<AudioClip>();
    public List<AudioClip> tier2AudioVariants = new List<AudioClip>();
    public List<AudioClip> tier3AudioVariants = new List<AudioClip>();

    public List<AudioClip> redToGreenAudio = new List<AudioClip>();
    public List<AudioClip> greenToRedAudio = new List<AudioClip>();

    public List<Command>[] commandTiers = new List<Command>[3];
    public List<AudioClip>[] audioTiers = new List<AudioClip>[3];

    public AudioSource currentPlayingAudioSource;
    public float timerLeft = 420;
    public bool timerActive = true;


    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        timeLeft = timeMax;
        commandTiers[0] = tier1RandomCommands;
        commandTiers[1] = tier2RandomCommands;
        commandTiers[2] = tier3RandomCommands;
        audioTiers[0] = tier1AudioVariants;
        audioTiers[1] = tier2AudioVariants;
        audioTiers[2] = tier3AudioVariants;

        sendForceCommand(initialCommand);
    }

    private void Update()
    {
        if (angerMeter == 0)
        {
            SceneManager.LoadScene("Main_scene_open");
        }

        if (generateRandomCommands)
        {
            countToNextCommand();
        }
    }

    void countToNextCommand()
    {
        if (timeLeft <= 0)
        {
            timeLeft = UnityEngine.Random.Range(timeMin, timeMax);
            Command cmd = GetRandomCommand();
            cmd.seletedClip = GetRandomAudio();
            Debug.Log(cmd.commandText);
            SubtitleManager.instance.putInsideSubtitleQueue(cmd);
            timeLeft += cmd.timetoread;
        }
        else
        {
            timeLeft -= Time.deltaTime;
        }
    }

    public void setRedToGreenCommand()
    {
        Command cmd = redToGreenCommand[UnityEngine.Random.Range(0, redToGreenCommand.Count)];
        cmd.seletedClip = redToGreenAudio[UnityEngine.Random.Range(0, redToGreenAudio.Count)];
        sendForceCommand(cmd);
    }

    public void setGreenToRedCommand()
    {
        Command cmd = greenToRedCommand[UnityEngine.Random.Range(0, greenToRedCommand.Count)];
        cmd.seletedClip = greenToRedAudio[UnityEngine.Random.Range(0, greenToRedAudio.Count)];
        sendForceCommand(cmd);
    }

    public void sendForceCommand(Command cm)
    {
        SubtitleManager.instance.putInsideSubtitleQueue(cm, true);
        timeLeft = timeMax + cm.timetoread;
        if (currentPlayingAudioSource != null)
            currentPlayingAudioSource.Stop();
    }

    public void changeMeter(int amount)
    {
        angerMeter = Math.Clamp(angerMeter + amount, minAnger, maxAnger);
    }

    Command GetRandomCommand()
    {
        if (angerMeter <= 2)
        {
            return commandTiers[0][UnityEngine.Random.Range(0, commandTiers[0].Count)];
        }
        else if (angerMeter >= 5)
        {
            return commandTiers[2][UnityEngine.Random.Range(0, commandTiers[2].Count)];
        }
        else
        {
            return commandTiers[1][UnityEngine.Random.Range(0, commandTiers[1].Count)];
        }
    }

    AudioClip GetRandomAudio()
    {
        if (angerMeter <= 2)
        {
            return audioTiers[0][UnityEngine.Random.Range(0, audioTiers[0].Count)];
        }
        else if (angerMeter >= 5)
        {
            return audioTiers[2][UnityEngine.Random.Range(0, audioTiers[2].Count)];
        }
        else
        {
            return audioTiers[1][UnityEngine.Random.Range(0, audioTiers[1].Count)];
        }
    }
}



[System.Serializable]
public class Command
{
    [TextArea(3, 10)]
    public string commandText;
    public float timetoread = 5f;
    public AudioClip seletedClip;
    public bool unskipable = false;

}


