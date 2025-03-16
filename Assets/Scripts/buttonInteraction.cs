using UnityEngine;
using UnityEngine.Events;

public class buttonInteraction : MonoBehaviour, IInteractable
{
    public bool isPressed = false;
    public bool colored = true;
    [SerializeField] private Color unpressedColor = Color.red;
    [SerializeField] private Color pressedColor = Color.green;
    private Renderer _renderer;

    public float distance = 0.01f;

    public UnityEvent onButtonPressed;
    public UnityEvent onButtonON;
    public UnityEvent onButtonOFF;

    public bool getState()
    {
        return isPressed;
    }

    void Awake()
    {
        isPressed = false;
        _renderer = GetComponent<Renderer>();
        if (_renderer != null && colored)
        {
            _renderer.material.color = unpressedColor;
        }
        else
        {
            Debug.LogWarning("No Renderer component found on button");
        }
    }

    public void Interact()
    {
        SoundManager.instance.PlaySoundAtPosition("buttonPressed", transform.position);
        // Change state first
        changeState();

        // Then invoke event (only once)
        onButtonPressed.Invoke();
    }

    public void changeState()
    {
        isPressed = !isPressed;
        if (isPressed)
        {
            Debug.Log("Button is now pressed");
            onButtonON.Invoke();
        }
        else
        {
            Debug.Log("Button is now released");
            onButtonOFF.Invoke();
        }
        UpdateButtonVisual();
    }

    // This method can be called externally to change state without invoking events
    public void SetStateWithoutEvent(bool newState)
    {
        if (isPressed != newState)
        {
            Debug.Log("Setting button state to: " + newState + " from " + isPressed);
            isPressed = newState;
            Debug.Log("Button is now " + (isPressed ? "pressed" : "released &&&&&&&&&&&&&&&"));
            UpdateButtonVisual();
        }
    }

    private void UpdateButtonVisual()
    {
        if (_renderer != null)
        {
            // Change material color based on state
            if (colored)
            {
                _renderer.material.color = isPressed ? pressedColor : unpressedColor;
            }
            Vector3 newPosition = transform.localPosition;
            newPosition.z += isPressed ? -distance : distance;
            transform.localPosition = newPosition;
        }
        else
        {
            Debug.LogWarning("No Renderer component found on button");
        }
    }
}
