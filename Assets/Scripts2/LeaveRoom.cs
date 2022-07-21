using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveRoom : MonoBehaviour
{

    public void GoBackLobby() 
    {     
        PhotonNetwork.LoadLevel(0);
    }
}
