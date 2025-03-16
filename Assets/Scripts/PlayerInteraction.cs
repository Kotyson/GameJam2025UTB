using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    private Camera MainCamera;
    private RaycastHit hitInfo;
    public float rayDistance = 100f;
    public LayerMask interactableLayers;
    private Transform raycastOrigin;
    public bool showDebugRay;

    public Image interactionIcon;
    public Sprite defaultIcon;
    public Sprite handIcon1;
    public Sprite handIcon2;
    public Sprite leverIcon;
    public Sprite gearIcon;

    // Reference to current interactable object
    private IInteractable currentInteractable;

    private Dictionary<string, Sprite> tagIcons = new Dictionary<string, Sprite>();

    void Start()
    {
        if (raycastOrigin == null)
        {
            MainCamera = Camera.main;
            raycastOrigin = MainCamera.transform;
        }


        tagIcons.Add("Hand", handIcon1);
        tagIcons.Add("Lever", leverIcon);
        tagIcons.Add("Gear", gearIcon);


        interactionIcon.enabled = false;
    }

    void Update()
    {
        CastRay();

        if (Input.GetKey(KeyCode.Mouse0))  // Pokud držíš E, změní se ikona na druhou podobu ruky
        {
            interactionIcon.sprite = handIcon2;
        }
        else if (interactionIcon.enabled && currentInteractable != null)  // Pokud nemáš zmáčknuté E, použije se první ikona
        {
            interactionIcon.sprite = handIcon1;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("lef mouse key was pressed");
            Interact();
        }
    }

    void CastRay()
    {
        if (raycastOrigin == null) return;

        Ray ray = new Ray(raycastOrigin.position, raycastOrigin.forward);
        bool hit = Physics.Raycast(ray, out hitInfo, rayDistance, interactableLayers);

        if (showDebugRay)
        {
            Color debugColor = hit ? Color.green : Color.red;
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, debugColor);
        }

        if (hit)
        {
            //Debug.Log("Hit: " + hitInfo.collider.name);
            // Store reference to current interactable
            currentInteractable = hitInfo.collider.GetComponent<IInteractable>();
            ShowInteractionIcon(hitInfo.collider.tag);
        }
        else
        {
            currentInteractable = null;
            interactionIcon.enabled = false;
        }
    }

    void Interact()
    {
        if (hitInfo.collider != null && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    void ShowInteractionIcon(string tag)
    {
        if (tagIcons.ContainsKey(tag))
        {
            interactionIcon.sprite = tagIcons[tag]; // Nastaví odpovídající ikonku podle tagu
            interactionIcon.enabled = true;
        }
        else
        {
            interactionIcon.sprite = defaultIcon; // Pokud není nalezena ikonka, použije se výchozí
            interactionIcon.enabled = true;
        }
    }
}
