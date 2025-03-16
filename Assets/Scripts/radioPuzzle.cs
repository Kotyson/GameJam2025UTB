using UnityEngine;

public class radioPuzzle : MonoBehaviour
{
    public bool fail = true;
    public float minX = 0.1339f;
    public float maxX = -0.1316f;

    public float frequency = 0.0f;
    public float[] idealFrequencies = { };
    public Command[] speakersFromRadio = { };

    public float frequencyTolerance = 0.02f;
    public Transform sliderTransform;
    public GameObject lightObject;
    private int currentIdealFrequencyIndex = -1;
    public float xPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateSliderPosition();
        lightObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetValue(float freq)
    {
        //Debug.Log("Frequency radio: " + freq);
        frequency = freq;
        UpdateSliderPosition();
        CheckIdealPosition();
    }

    private void UpdateSliderPosition()
    {
        if (sliderTransform != null)
        {
            // Convert frequency (0-360) to normalized value (0-1)
            float normalizedValue = frequency / 360f;

            // Map normalized value to position between minX and maxX
            xPosition = Mathf.Lerp(minX, maxX, normalizedValue);

            // Set the X position while keeping Y and Z the same
            Vector3 currentPosition = sliderTransform.localPosition;
            currentPosition.x = xPosition;
            sliderTransform.localPosition = currentPosition;
        }
    }

    private void CheckIdealPosition()
    {
        int closestIndex = -1;
        float closestDistance = frequencyTolerance; // Initialize with tolerance to only find positions within range

        // Check each ideal position value
        for (int i = 0; i < idealFrequencies.Length; i++)
        {
            float distance = Mathf.Abs(xPosition - idealFrequencies[i]);

            // If this position is the closest one so far and within tolerance
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }


        if (closestIndex != -1 && closestIndex != currentIdealFrequencyIndex)
        {


            currentIdealFrequencyIndex = closestIndex;
            Debug.Log("Tuned to ideal position: " + idealFrequencies[closestIndex]);

            // Turn on the light indicator
            if (lightObject != null)
            {
                lightObject.SetActive(true);
            }



            if (speakersFromRadio != null &&
                closestIndex < speakersFromRadio.Length &&
                speakersFromRadio[closestIndex] != null)
            {
                fail = false;
                Debug.Log("Playing radio message: " + speakersFromRadio[closestIndex].commandText);
                // Play the radio command/message here
                CommandManager.instance.sendForceCommand(speakersFromRadio[closestIndex]);
            }
            else
            {
                fail = true;
            }
        }
        else if (closestIndex == -1 && currentIdealFrequencyIndex != -1)
        {

            currentIdealFrequencyIndex = -1;
            Debug.Log("No longer tuned to any ideal position");

            // Turn off the light indicator
            if (lightObject != null)
            {
                lightObject.SetActive(false);
            }

        }
    }
}
