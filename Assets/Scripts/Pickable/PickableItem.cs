using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PickableItem : MonoBehaviourPun
{
    public string interactText = "Pick up ";
    public BaseItemSO item;
    public Outline outline;

    /// <summary>
    /// Picks up the item and adds it to the player's inventory. If pick up is successful return true and the item is destroyed.
    /// </summary>
    public void BaseInteract(GameObject player)
    {
        if (Interact(player)) DestroyItem();
    }

    protected virtual bool Interact(GameObject player)
    {
        return item.Pickup(player);
    }

    public void DestroyItem()
    {
        photonView.RPC("RequestDestroyItem", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RequestDestroyItem()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}