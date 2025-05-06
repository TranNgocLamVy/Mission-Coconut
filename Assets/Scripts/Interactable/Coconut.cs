using Photon.Pun;

public class Coconut : Interactable
{
    protected override void Interact()
    {
        // Any client can request scene change
        photonView.RPC("RPC_RequestEndScene", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_RequestEndScene()
    {
        // Only MasterClient will execute the actual scene change
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("EndScene"); // replace with your actual scene name
        }
    }
}
