using UnityEngine;
using UnityEngine.Events;

public class lightOutGameManager : MonoBehaviour
{
    public bool vertical = false;
    public int gridHeight = 3;
    public int gridWidth = 3;

    public GameObject btnPrefab;
    public GameObject[,] btnsGrid;

    public float spacingX = 1.0f;
    public float spacingY = 1.0f;

    [Header("Puzzle Settings")]
    public int targetOnButtonCount = 9;
    public bool puzzleSolved = false;
    public bool puzzleDestroyed = false;

    // Flag to prevent recursive calls
    private bool isProcessingMove = false;

    public bool[] startingState;

    public UnityEvent onPuzzleSolved;
    public UnityEvent onPuzzleDestroyed;

    void Awake()
    {
        btnsGrid = new GameObject[gridWidth, gridHeight];
        InitializeGrid();
        if (vertical)
        {
            this.transform.Rotate(90, 0, 0);
        }
    }

    void Start()
    {
        if (startingState.Length > 0)
        {
            SetStartingState();
        }
    }

    public void Reset()
    {
        ResetAllButtons();
    }

    public void ResetAllButtons()
    {
        SetStartingState();
    }

    void SetStartingState()
    {
        int index = 0;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                buttonInteraction btnComp = btnsGrid[x, y].GetComponentInChildren<buttonInteraction>();
                if (btnComp != null)
                {
                    Debug.Log("david nema kamarady, jen Michala " + startingState[index]);
                    btnComp.SetStateWithoutEvent(startingState[index]);
                }
                index++;
            }
        }
    }

    void InitializeGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                CreateLight(x, y);
            }
        }
    }

    void CreateLight(int x, int y)
    {
        Vector3 position = new Vector3(
            this.transform.position.x + spacingX * x,
            this.transform.position.y,
            this.transform.position.z + spacingY * y
        );

        GameObject btn = Instantiate(btnPrefab, position, Quaternion.Euler(-90, 0, 0), this.transform);
        btn.name = $"Btn_{x}_{y}";

        btnsGrid[x, y] = btn;

        // Store final x,y to avoid closure issues  
        int finalX = x;
        int finalY = y;

        // Add listener to button press
        buttonInteraction btnComponent = btn.GetComponentInChildren<buttonInteraction>();
        btnComponent.onButtonPressed.AddListener(() =>
        {
            // Prevent recursive processing to avoid infinite loops
            if (!isProcessingMove)
            {
                HandleButtonPress(finalX, finalY);
            }
        });
    }

    void HandleButtonPress(int x, int y)
    {
        // Set flag to prevent recursive calls
        isProcessingMove = true;

        Debug.Log($"Processing move at {x},{y}");

        // Toggle the neighbors (but don't toggle the original button again)
        ToggleNeighbors(x, y);

        // Check if puzzle is solved
        checkSolution();

        // Reset flag
        isProcessingMove = false;
    }

    void ToggleNeighbors(int x, int y)
    {
        // Toggle neighboring buttons
        if (x > 0) ToggleButton(x - 1, y);                // Left
        if (x < gridWidth - 1) ToggleButton(x + 1, y);    // Right
        if (y > 0) ToggleButton(x, y - 1);                // Down
        if (y < gridHeight - 1) ToggleButton(x, y + 1);   // Up
    }

    void ToggleButton(int x, int y)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            buttonInteraction btnComp = btnsGrid[x, y].GetComponentInChildren<buttonInteraction>();
            if (btnComp != null)
            {
                // Use changeState which doesn't invoke events
                btnComp.changeState();
            }
        }
    }

    void checkSolution()
    {
        int countOn = 0;

        // Count how many buttons are ON
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                bool buttonState = btnsGrid[x, y].GetComponentInChildren<buttonInteraction>().getState();
                if (buttonState)
                {
                    countOn++;
                }
            }
        }

        if (countOn == targetOnButtonCount)
        {
            if (!puzzleSolved) // Only trigger victory once
            {
                puzzleSolved = true;
                Debug.Log($"Puzzle solved! {countOn}/{targetOnButtonCount} buttons are ON");
                OnPuzzleSolved();
            }
        }
        else if (countOn == 0)
        {
            Debug.Log("Puzzle destroyed!");
            puzzleDestroyed = true;
        }
        else
        {
            puzzleSolved = false;
            puzzleDestroyed = false;
            Debug.Log($"Progress: {countOn}/{targetOnButtonCount} buttons are ON");
        }

    }

    void OnPuzzleSolved()
    {
        onPuzzleSolved.Invoke();
        Debug.Log("Puzzle solved!");
        if (!puzzleSolved)
        {
            CommandManager.instance.changeMeter(-1);
            puzzleSolved = true;
        }
        // for (int x = 0; x < gridWidth; x++)
        // {
        //     for (int y = 0; y < gridHeight; y++)
        //     {
        //         btnsGrid[x, y].GetComponentInChildren<Collider>().enabled = false;
        //     }
        // }
    }

    void OnPuzzelDestroyed()
    {
        Debug.Log("Puzzle destroyed!");
        onPuzzleDestroyed.Invoke();
    }
}
