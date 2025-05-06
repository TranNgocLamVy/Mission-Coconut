using Photon.Pun;
using UnityEngine;

public class Interactable : MonoBehaviourPun
{
    public string interactText;

    public void BaseInteract()
    {
        Interact();
    }

    protected virtual void Interact() { }
}
