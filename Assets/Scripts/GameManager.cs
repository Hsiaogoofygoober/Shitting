using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Tooltip("Prefab- 玩家的角色")]
    public GameObject playerPrefab;

    void Start()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("playerPrefab 遺失, 請在 Game Manager 重新設定",
                this);
        }
        else
        {
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("我們從 {0} 動態生成玩家角色",
                    SceneManagerHelper.ActiveSceneName);

                PhotonNetwork.Instantiate(this.playerPrefab.name,
                    new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.LogFormat("忽略場景載入 for {0}",
                    SceneManagerHelper.ActiveSceneName);
            }
        }
    }
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
        PhotonNetwork.LoadLevel("GameScene");
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
