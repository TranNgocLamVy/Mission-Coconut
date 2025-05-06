using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class GameManager : MonoBehaviourPunCallbacks
{

    [Header("Player Prefab")]
    [SerializeField] private GameObject player;

    [Space]

    [Header("Player")]
    [SerializeField] public List<GameObject> playerList = new List<GameObject>();

    [Space]

    [Header("Spawn")]
    [SerializeField] public Transform spawnPoint;
    [SerializeField] private bool spawnPlayerOnStart = true;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion("asia");
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.JoinOrCreateRoom("TestRoom", null, null);
    }

    public override void OnJoinedRoom()
    {
        if (!spawnPlayerOnStart) return;
        base.OnJoinedRoom();
        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        PlayerSetup _playerSetup = _player.GetComponentInChildren<PlayerSetup>();
        if (_playerSetup)
        {
            _playerSetup.SetupLocalPlayer();
        }
        else
        {
            Debug.LogError("PlayerSetup not found on player prefab");
        }
        playerList.Add(_player);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        foreach (var go in playerList)
        {
            if (go.GetComponent<PhotonView>().Owner == otherPlayer)
            {
                playerList.Remove(go);
                Destroy(go);
            }
        }
    }
}
