using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public override void OnLeftRoom()
    {
        // 玩家離開遊戲室時, 把他帶回到遊戲場入口
        SceneManager.LoadScene(0);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("我不是 Master Client, 不做載入場景的動作");
        }
        Debug.LogFormat("載入{0}人的場景",
            PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("GameScene" +
            PhotonNetwork.CurrentRoom.PlayerCount);
    }
    // Update is called once per frame

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("我是 Master Client 嗎? {0}",
                PhotonNetwork.IsMasterClient);
            LoadArena();
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.LogFormat("{0} 離開遊戲室", otherPlayer.NickName);
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("我是 Master Client 嗎? {0}",
                PhotonNetwork.IsMasterClient);
            LoadArena();
        }
    }
}
