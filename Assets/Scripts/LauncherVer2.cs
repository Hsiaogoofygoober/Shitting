using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherVer2 : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings(); // 連接至Photon雲端伺服器
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby(); // 當連接到伺服器時，要去尋找房間(Lobby)
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
