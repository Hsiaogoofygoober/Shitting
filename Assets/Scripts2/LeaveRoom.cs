using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveRoom : MonoBehaviour
{
    // public static RoomManager Instance;
    // Start is called before the first frame update
    private void Awake()
    {
         Destroy(RoomManager.instance);
    }

    public void GoBackLobby() 
    {     
        PhotonNetwork.LoadLevel(0);
    }
}
