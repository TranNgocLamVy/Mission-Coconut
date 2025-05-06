using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    [Header("Camera Setup")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject playerModal;
    [SerializeField] private LayerMask hiddenModelLayer;
    [SerializeField] private bool hidePlayerModal;

    [Header("Scripts")]
    [SerializeField] private PlayerInputManager playerInput;

    public void SetupLocalPlayer()
    {

        //Main Camera Setup
        if (playerCamera)
        {
            playerCamera.SetActive(true);
        }
        else
        {
            Debug.LogError("Player Camera not assigned!");
        }

        // Hide Player Modal from camera
        if (playerModal)
        {
            SetLayerRecursively(playerModal, hiddenModelLayer);
        }
        else
        {
            Debug.LogError("Player Modal not assigned!");
        }

        //Input
        if (playerInput)
        {
            playerInput.enabled = true;
        }
        else
        {
            Debug.LogError("Input Manager not assigned!");
        }
    }

    public void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        int layerIndex = Mathf.RoundToInt(Mathf.Log(newLayer, 2));

        obj.layer = layerIndex;

        foreach (Transform child in obj.transform)
        {
            if (child != null)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }
}
