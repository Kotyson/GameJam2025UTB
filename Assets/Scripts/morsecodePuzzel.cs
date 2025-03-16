using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class morsecodePuzzel : MonoBehaviour, IInteractable
{
    public UnityEvent onSolved;
    public Light blikLight;
    public Light[] solutionLights;
    public Color goodColor = Color.green;
    public Color badColor = Color.red;
    public Color letterTakenColor = Color.yellow;

    public AudioClip successSound;
    public AudioClip failSound;
    public AudioClip goodSound;
    public AudioClip bib; // Sound played while holding
    public AudioSource bibSource; // Audio source for continuous sound
    private bool isSoundPlaying = false;

    public Dictionary<string, string> morseabc = new Dictionary<string, string> {
        {".-", "A"},
        {"-...", "B"},
        {"-.-.", "C"},
        {"-..", "D"},
        {".", "E"},
        {"..-.", "F"},
        {"--.", "G"},
        {"....", "H"},
        {"..", "I"},
        {".---", "J"},
        {"-.-", "K"},
        {".-..", "L"},
        {"--", "M"},
        {"-.", "N"},
        {"---", "O"},
        {".--.", "P"},
        {"--.-", "Q"},
        {".-.", "R"},
        {"...", "S"},
        {"-", "T"},
        {"..-", "U"},
        {"...-", "V"},
        {".--", "W"},
        {"-..-", "X"},
        {"-.--", "Y"},
        {"--..", "Z"}
    };

    public string solution = "ST";
    public string currentMorseInput = ""; // Current morse code being input
    public string currentLetterInput = ""; // Current letter being formed
    public string currentWordInput = ""; // Current word being formed
    public GameObject handle;

    private bool isHolding = false;
    private float holdDuration = 0;
    private float holdStartTime = 0;
    private float lastReleaseTime = 0;
    public float letterPauseThreshold = 1.2f;
    private bool waitingForLetterCheck = false;
    public float dotThreshold = 0.8f; // Press duration under this is a dot

    bool waiting = false;
    private int failCount = 0; // Po�et ne�sp�n�ch pokus�
    public int failsBeforeAnger = 2; // Kolik pokus� ignorovat

    public void Interact()
    {
        Debug.Log("Interacting with Morse Code Puzzle");

        // Player has started interacting with the puzzle
        Debug.Log("Morse code input activated - Hold E for dots/dashes");

        // Reset input state
        StartHolding();
    }

    void Start()
    {
        for (int i = 0; i < solutionLights.Length; i++)
        {
            solutionLights[i].enabled = false;
        }
    }

    public void ResetInput()
    {
        // Start the coroutine for delayed reset
        StartCoroutine(DelayedResetInput());
    }

    public void LetterTaken()
    {
        StartCoroutine(DelayedLetterTaken());
    }

    private IEnumerator DelayedLetterTaken()
    {

        for (int i = 0; i < solutionLights.Length; i++)
        {
            solutionLights[i].enabled = true;
            solutionLights[i].color = letterTakenColor;
        }
        // Wait for 2 seconds
        yield return new WaitForSeconds(1.0f);


        Debug.Log("Input reset complete");

        // Optional - turn off error lights after the delay
        for (int i = 0; i < solutionLights.Length; i++)
        {
            solutionLights[i].enabled = false;
        }
        waiting = false;
    }


    // Coroutine to wait 2 seconds before resetting input
    private IEnumerator DelayedResetInput()
    {
        Debug.Log("Resetting input in 2 seconds...");
        waiting = true;
        // Wait for 2 seconds
        yield return new WaitForSeconds(2.0f);

        // Reset all inputs after waiting
        currentMorseInput = "";
        currentLetterInput = "";
        currentWordInput = "";

        Debug.Log("Input reset complete");

        // Optional - turn off error lights after the delay
        for (int i = 0; i < solutionLights.Length; i++)
        {
            solutionLights[i].enabled = false;
        }

        waiting = false;
    }

    void Update()
    {
        if (isHolding && Input.GetKeyUp(KeyCode.Mouse0) && !waiting)
        {
            Debug.Log("Key released - ending hold");
            EndHolding();
        }

        // If currently holding, update the duration
        if (isHolding && !waiting)
        {
            holdDuration = Time.time - holdStartTime;

            // Optional: Rotate or animate handle based on holding duration
            if (handle != null)
            {
                handle.transform.localRotation = Quaternion.Euler(0, 10f, 0);
            }

            // Auto-release if held too long (optional safety measure)
            if (holdDuration > 3.0f)
            {
                EndHolding();
            }
        }

        // If we're waiting for letter completion check
        if (waitingForLetterCheck && !isHolding)
        {
            float timeSinceRelease = Time.time - lastReleaseTime;

            // After a short pause, check if we've completed a letter
            if (timeSinceRelease >= letterPauseThreshold)
            {
                LetterTaken();
                waiting = true;
                CheckMorseCode();
                waitingForLetterCheck = false;
            }
        }
    }

    private void StartHolding()
    {
        SoundManager.instance.PlaySoundAtPosition("morseShort", transform.position);
        blikLight.enabled = true;
        isHolding = true;
        holdStartTime = Time.time;
        holdDuration = 0f;

        // Cancel any pending letter check
        waitingForLetterCheck = false;
    }

    private void EndHolding()
    {
        blikLight.enabled = false;
        isHolding = false;
        lastReleaseTime = Time.time;


        // Reset handle position
        if (handle != null)
        {
            handle.transform.localRotation = Quaternion.identity;
        }

        // Determine if dot or dash based on hold duration
        if (holdDuration < dotThreshold)
        {
            // It's a dot
            currentMorseInput += ".";
            Debug.Log("Input: dot (.)");
        }
        else
        {
            // It's a dash
            currentMorseInput += "-";
            Debug.Log("Input: dash (-)");
        }

        Debug.Log("Current morse: " + currentMorseInput);

        // After release, wait for a short time to see if we've completed a letter
        waitingForLetterCheck = true;
    }

    private void CheckMorseCode()
    {
        if (string.IsNullOrEmpty(currentMorseInput)) return;

        if (morseabc.TryGetValue(currentMorseInput, out string letter))
        {
            currentLetterInput = letter;
            currentWordInput += letter;

            Debug.Log($"Letter completed: {letter}, Current word: {currentWordInput}");
            Debug.Log($"Current word: {currentWordInput}" + " Solution: " + solution);
            // Check if we've completed the solution
            if (currentWordInput.ToUpper() == solution.ToUpper())
            {
                Debug.Log("Correct solution entered!");
                for (int i = 0; i < solutionLights.Length; i++)
                {
                    solutionLights[i].enabled = true;
                    solutionLights[i].color = goodColor;
                }
                onSolved.Invoke();
                CommandManager.instance.changeMeter(-1);
                SoundManager.instance.PlaySoundAtPosition("successSoundMorse", transform.position);
                // You could trigger success effects here
            }
            else if (solution.StartsWith(currentWordInput.ToUpper()))
            {
                Debug.Log("Current input matches the beginning of the solution.");
            }
            else
            {
                Debug.Log("Current input does not match the solution.");
                for (int i = 0; i < solutionLights.Length; i++)
                {
                    solutionLights[i].enabled = true;
                    solutionLights[i].color = badColor;
                }

                failCount++;
                Debug.Log("FAILY: " + failCount);


                if (failCount > failsBeforeAnger)
                {
                    Debug.Log("Angry");
                    // CommandManager.instance.changeMeter(1);

                }
                SoundManager.instance.PlaySoundAtPosition("failSoundMorse", transform.position);
                ResetInput();
            }

        }
        else
        {
            Debug.Log($"Unknown morse code: {currentMorseInput}");
        }

        // Reset current morse input for the next letter
        currentMorseInput = "";
    }
}
