using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public enum LeverState
{
    Off,
    On,
    Neutral
}
public class leverInteraction : MonoBehaviour, IInteractable
{

    public int leverStateCount = 2;
    public int currentLeverState = 0;
    public int badLeverState = 0;

    public int correctLeverState = 1;

    public float angleRange = 100;
    private float angleStep = 0;
    public bool reverseMovement = false;

    public Transform pivotRotation;

    [Header("Animation")]
    public float transitionSpeed = 5.0f;  // Adjust this to control animation speed
    private float currentAngle;
    private float targetAngle;
    private bool isAnimating = false;

    public UnityEvent onLeverCorrect;
    public UnityEvent onLeverIncorrect;
    public UnityEvent onLeverSwitched;

    public LeverState state = LeverState.Neutral;


    void evaluateState()
    {
        if (currentLeverState == correctLeverState)
        {
            state = LeverState.On;
        }
        else if (currentLeverState == badLeverState)
        {
            state = LeverState.Off;
        }
        else
        {
            state = LeverState.Neutral;
        }
    }

    void Start()
    {
        angleStep = angleRange / (leverStateCount - 1);

        // Initialize both current and target angle
        float startAngle = -angleRange / 2;
        currentAngle = startAngle + (currentLeverState * angleStep);
        targetAngle = currentAngle;

        // Set initial position
        pivotRotation.localRotation = Quaternion.Euler(currentAngle, 0, 0);
        evaluateState();
    }

    void Update()
    {
        // If we need to animate to a new position
        if (isAnimating)
        {
            // Lerp to target angle
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * transitionSpeed);

            // Apply rotation
            pivotRotation.localRotation = Quaternion.Euler(currentAngle, 0, 0);


            if (Mathf.Abs(currentAngle - targetAngle) < 0.1f)
            {
                currentAngle = targetAngle; // Snap to exact position
                pivotRotation.localRotation = Quaternion.Euler(currentAngle, 0, 0);
                isAnimating = false;
            }
        }
    }

    public void Interact()
    {
        Debug.Log("Lever is now switched");
        updateLeverRoation();
        if (state == LeverState.On)
        {
            onLeverCorrect.Invoke();
        }
        else if (state == LeverState.Off)
        {
            onLeverIncorrect.Invoke();
        }
        evaluateState();
        onLeverSwitched.Invoke();
    }

    public void updateLeverRoation()
    {
        if (reverseMovement)
        {
            currentLeverState--;
        }
        else
        {
            currentLeverState++;
        }

        if (currentLeverState >= leverStateCount - 1 || currentLeverState == 0)
        {
            reverseMovement = !reverseMovement;
        }

        // Instead of immediately updating rotation, set target and start animating
        UpdateLeverTargetAngle();

        Debug.Log("Current lever state: " + currentLeverState);
        Debug.Log("Reverse movement: " + reverseMovement);
    }

    private void UpdateLeverTargetAngle()
    {
        float startAngle = -angleRange / 2; // Lowest angle position
        targetAngle = startAngle + (currentLeverState * angleStep);

        // Start animating
        isAnimating = true;
    }

}
