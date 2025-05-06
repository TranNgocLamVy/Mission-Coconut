using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerInteract : MonoBehaviourPun
{
    [Header("Main")]
    [SerializeField] private PlayerSO playerStats;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private AudioClip pickUpClip;
    private AudioSource audioSource;
    private PickableItem lastOutlinedItem;

    void Start()
    {
        if (!playerCamera) Debug.LogError("Player camera not assigned in PlayerInteract script.");

        audioSource = GetComponentInParent<AudioSource>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        interactText.text = string.Empty;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * playerStats.interactDistance, Color.red);
        RaycastHit hitInfo;

        // Default: disable previous outline if nothing is hit
        bool hitSomething = Physics.Raycast(ray, out hitInfo, playerStats.interactDistance, playerStats.interactableLayer);
        PickableItem currentPickable = null;

        if (hitSomething)
        {
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactText.text = "F: " + interactable.interactText;
            }

            currentPickable = hitInfo.collider.GetComponent<PickableItem>();
            if (currentPickable != null)
            {
                interactText.text = "F: " + currentPickable.interactText + currentPickable.item.itemName;
            }
        }

        // Update outline logic
        if (currentPickable != lastOutlinedItem)
        {
            if (lastOutlinedItem != null && lastOutlinedItem.outline != null)
            {
                lastOutlinedItem.outline.enabled = false;
            }

            if (currentPickable != null && currentPickable.outline != null)
            {
                currentPickable.outline.enabled = true;
            }

            lastOutlinedItem = currentPickable;
        }
    }


    public void Interact()
    {
        interactText.text = string.Empty;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * playerStats.interactDistance, Color.red);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, playerStats.interactDistance, playerStats.interactableLayer))
        {
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.BaseInteract();
            }

            PickableItem pickableItem = hitInfo.collider.GetComponent<PickableItem>();
            if (pickableItem != null)
            {
                pickableItem.BaseInteract(gameObject);
                audioSource.PlayOneShot(pickUpClip, 0.5f);
            }
        }
    }
}
