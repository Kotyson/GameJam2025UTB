using UnityEngine;
using UnityEngine.Events;

public class tlakomerPuzzle : MonoBehaviour
{
    public UnityEvent puzzleSolved;
    private bool invoked = false;
    private int buttonPressCount = 0;
    public int buttonPressThreshold = 6;

    public int minPosition = 0;
    public int maxPosition = 3;
    public int positionValue = 0;
    public int targetValue = 2;

    public int firstBtnEffectValue = 0;
    public int secondBtnEffectValue = 0;
    public int thirdBtnEffectValue = 0;

    public int effectValue = 0;

    public Transform carka;

    public float[] angles;
    public float frequency = 0;
    public float maxFrequency = 100;
    public float minFrequency = 0;
    public float idealFrequency = -0.6f;
    public Transform frequencyGauge;

    // Rotation
    public float rotationSpeed = 5.0f; // Higher is faster
    private float currentAngle;
    private float targetAngle;
    private bool isRotating = false;
    private bool idealPosition = false;
    private bool idealFreq = false;

    void Start()
    {
        frequencyGauge.localPosition = new Vector3(frequencyGauge.localPosition.x, minFrequency, frequencyGauge.localPosition.z);
        positionValue = minPosition;
        targetAngle = angles[positionValue];
        currentAngle = targetAngle;
        carka.localEulerAngles = new Vector3(0, 0, currentAngle);
    }

    void Update()
    {
        if (isRotating)
        {
            // smooth rotation
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);

            // Apply
            carka.localEulerAngles = new Vector3(0, 0, currentAngle);

            // Check if close to the target angle
            if (Mathf.Abs(currentAngle - targetAngle) < 0.1f)
            {
                // Snap to final angle to prevent floating-point imprecision
                currentAngle = targetAngle;
                carka.localEulerAngles = new Vector3(0, 0, currentAngle);
                isRotating = false;
            }
        }
        if (idealFreq && idealPosition && !invoked)
        {
            puzzleSolved.Invoke();
            invoked = true;
            Debug.Log("Puzzle solved!");

            CommandManager.instance.changeMeter(-1);
            Debug.Log("Puzzle vy�e�eno! Na�tv�n� sn�eno.");
        }

    }

    void changeValue()
    {
        effectValue = firstBtnEffectValue + secondBtnEffectValue + thirdBtnEffectValue;
        positionValue = Mathf.Clamp(effectValue, minPosition, maxPosition);

        // start animation with target angle
        targetAngle = angles[positionValue];
        isRotating = true;

        Debug.Log("Position value: " + positionValue + ", Target angle: " + targetAngle);

        if (positionValue == targetValue)
        {
            idealPosition = true;
        }
        else
        {
            idealPosition = false;
        }
    }

    public void getFrequency(float inputFreq)
    {
        // Normalize input frequency (0-360) to a value between 0 and 1
        float normalizedValue = Mathf.Clamp01(inputFreq / 360f);

        // Convert normalized value to the target range (minFrequency to maxFrequency)
        frequency = Mathf.Lerp(minFrequency, maxFrequency, normalizedValue);

        // Debug.Log($"Input frequency: {inputFreq}, Normalized: {normalizedValue}, Mapped: {frequency}");

        // Optional: Map the frequency to a position and update the gauge
        // MapFrequencyToPosition();
        Vector3 tempPosition = frequencyGauge.localPosition;
        tempPosition.y = frequency;
        frequencyGauge.localPosition = tempPosition;

        // Define the tolerance - how close is "close enough"
        float tolerance = 0.01f; // Adjust this value as needed

        // Check if the current frequency is close to the ideal frequency
        if (Mathf.Abs(frequency - idealFrequency) <= tolerance)
        {
            Debug.Log("Frequency is close to ideal!");
            // The frequency is close enough to the ideal
            // You can trigger events or change visuals here
            idealFreq = true;
        }
        else
        {
            Debug.Log($"Frequency {frequency} is not close enough to ideal {idealFrequency}");
            idealFreq = false;
        }

    }

    public void pressedBtn()
    {
        changeValue();
        buttonPressCount++;
        Debug.Log("Tlacitko stisknuto: " + buttonPressCount);

        // Pokud hr�� zm��kl tla��tko dostate�n�kr�t, zv���me na�tv�n�
        if (buttonPressCount >= buttonPressThreshold)
        {
            Debug.Log("dostal jsem se sem");

            CommandManager.instance.changeMeter(1);
            Debug.Log("KOKOT");
            Debug.Log("P��li� mnoho stisknut�! Na�tv�n� se zv��ilo.");


        }

    }


    public void firstBtnPressed()
    {
        firstBtnEffectValue = 1;
    }

    public void firstBtnReleased()
    {
        firstBtnEffectValue = 0;
    }

    public void secondBtnPressed()
    {
        secondBtnEffectValue = 3;
    }

    public void secondBtnReleased()
    {
        secondBtnEffectValue = 0;
    }

    public void thirdBtnPressed()
    {
        thirdBtnEffectValue = -1;
    }

    public void thirdBtnReleased()
    {
        thirdBtnEffectValue = 0;
    }



}
