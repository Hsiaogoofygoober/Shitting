using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace Com.FPSGaming
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [Tooltip("遊戲室玩家人數上限. 當遊戲室玩家人數已滿額, 新玩家只能新開遊戲室來進行遊戲.")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        [Tooltip("顯示/隱藏 遊戲玩家名稱與 Play 按鈕")]
        [SerializeField]
        private GameObject controlPanel;

        [Tooltip("顯示/隱藏 連線中 字串")]
        [SerializeField]
        private GameObject progressLabel;

        [Tooltip("顯示/隱藏 等待中 字串")]
        [SerializeField]
        private GameObject waitingLabel;

        [Tooltip("顯示/隱藏 線上的人數")]
        [SerializeField]
        private Text playersOnMaster;

        private Text playerUI;
        // 遊戲版本的編碼, 可讓 Photon Server 做同款遊戲不同版本的區隔.
        string gameVersion = "1";
        bool isConnecting;
        static int playerNumber = 0;

        void Awake()
        {
            // 確保所有連線的玩家均載入相同的遊戲場景
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        void Start()
        {   
            playerNumber = PhotonNetwork.CountOfPlayers;
            playersOnMaster.GetComponent<Text>().text = "Player On Server : " + playerNumber;
            progressLabel.SetActive(false);
            waitingLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        // 與 Photon Cloud 連線
        public void Connect()
        {
            //playersOnMaster.SetActive(true);
            progressLabel.SetActive(true);
            waitingLabel.SetActive(false);
            controlPanel.SetActive(false);
            // 檢查是否與 Photon Cloud 連線
            if (PhotonNetwork.IsConnected)
            {
                // 已連線, 嚐試隨機加入一個遊戲室
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // 未連線, 嘗試與 Photon Cloud 連線
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN 呼叫 OnConnectedToMaster(), 已連上 Photon Cloud.");

            // 確認已連上 Photon Cloud
            // 隨機加入一個遊戲室
            PhotonNetwork.JoinRandomRoom();
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            //playersOnMaster.SetActive(true);
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarningFormat("PUN 呼叫 OnDisconnected() {0}.", cause);
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN 呼叫 OnJoinRandomFailed(), 隨機加入遊戲室失敗.");
            Debug.Log("PUN , 隨機加入遊戲室失敗.");
            // 隨機加入遊戲室失敗. 可能原因是 1. 沒有遊戲室 或 2. 有的都滿了.    
            // 好吧, 我們自己開一個遊戲室.
            PhotonNetwork.CreateRoom(null, new RoomOptions
            {
                MaxPlayers = maxPlayersPerRoom
            });
        }
        public override void OnJoinedRoom()
        {
            waitingLabel.SetActive(true);
            progressLabel.SetActive(false);
            Debug.Log("PUN 呼叫 OnJoinedRoom(), 已成功進入遊戲室中.");
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                
                Debug.Log("我是第一個進入遊戲室的玩家");
                Debug.Log("我得主動做載入場景 'SampleScene' 的動作");
                PhotonNetwork.LoadLevel("GameScene");
            }
            //else if(PhotonNetwork.CurrentRoom.PlayerCount <= 4)
            //{
            //    PhotonNetwork.LoadLevel("SampleScene");
            //}
        }

    }
}