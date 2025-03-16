using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class knobInteraction : MonoBehaviour, IInteractable
{
    [Header("Knob Settings")]
    public float maxFrequency = 360.0f;
    public float minFrequency = 0.0f;
    public float currentFrequency = 0.0f;
    public float rotationSpeed = 2.0f;  // Sensitivity for mouse movement

    [Header("Rotation Settings")]
    public Vector3 rotationAxis = new Vector3(0, 1, 0);  // Default to Y-axis rotation
    public float minRotationAngle = 0f;
    public float maxRotationAngle = 360f;
    public bool preserveInitialRotation = true;

    [Header("Visual Feedback")]
    public Transform knobTransform;  // The part of the knob that rotates
    public Color normalColor = Color.white;
    public Color activeColor = Color.yellow;

    private bool isBeingInteracted = false;
    private float lastMouseY;
    private Renderer knobRenderer;
    private Quaternion initialPlayerRotation;

    private FirstPersonLook firstPersonLook;
    private Vector2 originalMousePosition;
    private bool cursorWasLocked;

    public UnityEvent<float> onKnobTurned;

    Quaternion parentRotation;
    void Start()
    {
        firstPersonLook = Camera.main?.GetComponent<FirstPersonLook>();

        // If no knob transform is set, use this object's transform
        if (knobTransform == null)
            knobTransform = transform;

        // Store initial rotation so we can preserve it
        initialPlayerRotation = knobTransform.localRotation;

        // Get renderer for visual feedback
        knobRenderer = GetComponent<Renderer>();
        if (knobRenderer != null)
        {
            knobRenderer.material.color = normalColor;
        }

        // Initialize knob rotation based on current frequency
        UpdateKnobRotation();
    }

    void Update()
    {
        // Only process input if currently being interacted with
        if (isBeingInteracted)
        {
            // Check if interaction key is still being held
            if (Input.GetKey(KeyCode.Mouse0))
            {
                HandleKnobRotation();

                Camera.main.transform.parent.rotation = parentRotation;
            }
            else
            {
                // Key was released, end interaction
                EndInteraction();
            }
        }
    }



    private void HandleKnobRotation()
    {
        onKnobTurned.Invoke(currentFrequency);

        float deltaMouseY = Input.GetAxisRaw("Mouse Y") * rotationSpeed;

        // Adjust current frequency based on mouse movement (inverted for more intuitive control)
        currentFrequency -= deltaMouseY * 10f;

        // Clamp frequency to min/max range
        currentFrequency = Mathf.Clamp(currentFrequency, minFrequency, maxFrequency);


        UpdateKnobRotation();

        // Reset mouse position to center to prevent player camera rotation
        if (firstPersonLook != null && !firstPersonLook.canLook)
        {
            Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2, Screen.height / 2));
        }

    }

    private void UpdateKnobRotation()
    {
        // Calculate the rotation angle based on current frequency
        float normalizedValue = (currentFrequency - minFrequency) / (maxFrequency - minFrequency);
        float rotationAngle = Mathf.Lerp(minRotationAngle, maxRotationAngle, normalizedValue);

        // Create rotation around specified axis
        Quaternion newRotation = Quaternion.AngleAxis(rotationAngle, rotationAxis);

        // If preserving initial rotation, combine with initial rotation
        if (preserveInitialRotation)
        {
            knobTransform.localRotation = initialPlayerRotation * newRotation;
        }
        else
        {

            knobTransform.localRotation = newRotation;
        }
    }

    // Called when player interacts with the knob
    public void Interact()
    {
        // Get the rotation of the parent of the main camera
        parentRotation = Camera.main.transform.parent.rotation;
        Debug.Log($"Parent rotation: {parentRotation}");
        // Start the interaction
        isBeingInteracted = true;

        // Store original mouse position and cursor state
        originalMousePosition = Mouse.current.position.ReadValue();
        cursorWasLocked = Cursor.lockState == CursorLockMode.Locked;

        // Visual feedback
        if (knobRenderer != null)
        {
            knobRenderer.material.color = activeColor;
        }

        // Disable camera look
        if (firstPersonLook != null)
        {
            firstPersonLook.canLook = false;
        }

        // Hide cursor and lock it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Knob interaction started - hold E and move mouse up/down to adjust");
    }

    // End the interaction
    private void EndInteraction()
    {
        isBeingInteracted = false;

        // Reset visual feedback
        if (knobRenderer != null)
        {
            knobRenderer.material.color = normalColor;
        }

        // Re-enable camera controller
        if (firstPersonLook != null)
        {
            firstPersonLook.canLook = true;
        }

        // Restore cursor state
        if (!cursorWasLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Debug.Log("Knob interaction ended");
    }

    public string GetInteractPrompt()
    {
        return "Hold E and move mouse to turn knob";
    }

    // Called when script is disabled/destroyed
    void OnDisable()
    {
        if (isBeingInteracted)
        {
            if (firstPersonLook != null)
            {
                firstPersonLook.canLook = true;
            }

            // Only unlock cursor if it wasn't locked before
            if (!cursorWasLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

}
