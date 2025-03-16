using UnityEngine;
using UnityEngine.Events;

public class teleforPuzzle : MonoBehaviour
{
    public UnityEvent puzzleSolved;
    public string idealNumber = "012";
    public string alternative = "268";
    public string currentNumber = "";
    public AudioClip successSound;
    public AudioClip failSound;

    public Command[] commands;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddNumber(string number)
    {
        currentNumber += number;
        if (currentNumber.Length == idealNumber.Length)
        {
            CheckNumber();
        }
    }

    public void CheckNumber()
    {
        if (currentNumber == idealNumber)
        {
            Debug.Log("Puzzle solved!");
            puzzleSolved.Invoke();
            CommandManager.instance.sendForceCommand(commands[0]);
            currentNumber = "";
        }
        else if (currentNumber == alternative)
        {
            Debug.Log("Puzzle solved!");
            puzzleSolved.Invoke();
            CommandManager.instance.sendForceCommand(commands[1]);
            currentNumber = "";
        }
        else
        {
            Debug.Log("Puzzle failed!");
            currentNumber = "";
        }
    }
}
