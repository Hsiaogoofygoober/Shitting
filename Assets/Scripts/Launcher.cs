using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;

namespace Com.FPSGaming
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        public static Launcher instance;

        [Tooltip("遊戲室玩家人數上限. 當遊戲室玩家人數已滿額, 新玩家只能新開遊戲室來進行遊戲.")]
        [SerializeField]
        private byte maxPlayersPerRoom = 5;

        [Tooltip("顯示/隱藏 遊戲玩家名稱與 Play 按鈕")]
        [SerializeField]
        private GameObject controlPanel;

        [SerializeField]
        private GameObject waitingQueue;

        [Tooltip("顯示/隱藏 線上的人數")]
        [SerializeField]
        private Text playersOnMaster;

        // 遊戲版本的編碼, 可讓 Photon Server 做同款遊戲不同版本的區隔.
        string gameVersion = "1";
        bool isConnecting = false;
        static int playerNumber = 0;


        void Awake()
        {
            instance = this;
            // 確保所有連線的玩家均載入相同的遊戲場景
        }

        void Start()
        {
            PhotonNetwork.ConnectUsingSettings();

            waitingQueue.SetActive(false);
            controlPanel.SetActive(true);
        }

        async public void PayforTicket()
        {

            // set chain
            string chain = "polygon";
            // set network
            string network = "mainnet";
            // set chainID, here we use the networkID for goerli
            string chainId = "137";
            // abi in json format
            string abi = "[{\"inputs\":[],\"stateMutability\":\"payable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"text\",\"type\":\"string\"}],\"name\":\"error\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"addPlayer\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"ownerWithdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes32\",\"name\":\"_ethSignedMassageHash\",\"type\":\"bytes32\"},{\"internalType\":\"bytes\",\"name\":\"_sig\",\"type\":\"bytes\"}],\"name\":\"recover\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"pure\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"money\",\"type\":\"uint256\"}],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"stateMutability\":\"payable\",\"type\":\"receive\"}]";
            // address of contract
            string contract = "0x5c99D774519Dd8d601F438ceF2B541E5B6793Fb2";
            // value in wei 6*10^14 = 30 TWD
            string value = "1000000000000000000";
            // method you want to write to
            string method = "addPlayer";
            // amount you want to change, in this case we are adding 1 to "addTotal"
            string amount = "0xa8c38c36aab9ab846a2a42899f07bd7809211ccc4746ed4677678acd99c174015c56f5f3d6533b19d1298e50ff886827f6992c7b810cb4ca8a6a5a7d7d7c309d1c";
            // array of arguments for contract you can also add a nonce here as optional parameter
            string[] obj = { amount };
            string args = JsonConvert.SerializeObject(obj);
            // create data for contract interaction
            string data = await EVM.CreateContractData(abi, method, args);
            print(data);
            // send transaction
            try
            {
#if UNITY_WEBGL
                string response = await Web3GL.SendContract(method, abi, contract, args, value, "", "");
                print("sent contract " + response);
#endif
                Connect();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                print("error, doesn't send contract");
            }
        }
        // 與 Photon Cloud 連線
        public void Connect()
        {
            //playersOnMaster.SetActive(true);
            waitingQueue.SetActive(true);
            controlPanel.SetActive(false);
            // 檢查是否與 Photon Cloud 連線
            if (PhotonNetwork.IsConnected)
            {
                  PhotonNetwork.JoinRandomRoom();         
                
            }
            else
            {
                // 未連線, 嘗試與 Photon Cloud 連線
                PhotonNetwork.GameVersion = gameVersion;
                isConnecting = PhotonNetwork.ConnectUsingSettings();
            }
        }


        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
            Debug.Log("PUN 呼叫 OnConnectedToMaster(), 已連上 Photon Cloud.");
            if (isConnecting)
            {
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom(null, maxPlayersPerRoom, MatchmakingMode.FillRoom, TypedLobby.Default, null);
                isConnecting = false;
            }
            // 確認已連上 Photon Cloud
            // 隨機加入一個遊戲室
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            //playersOnMaster.SetActive(true);
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
                MaxPlayers = 5,
                CleanupCacheOnLeave = false
            }); 
            Debug.Log("PUN , 創建遊戲室成功.");
        }


        public override void OnJoinedRoom()
        {
            waitingQueue.SetActive(true);
            Debug.Log("PUN 呼叫 OnJoinedRoom(), 已成功進入遊戲室中." + " " + PhotonNetwork.CurrentRoom.PlayerCount);
            /*if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayersPerRoom)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;    
                PhotonNetwork.LoadLevel(1);    
            }*/
        }
    }
}