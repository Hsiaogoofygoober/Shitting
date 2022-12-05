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

        [Tooltip("�C���Ǫ��a�H�ƤW��. ��C���Ǫ��a�H�Ƥw���B, �s���a�u��s�}�C���ǨӶi��C��.")]
        [SerializeField]
        private byte maxPlayersPerRoom = 5;

        [Tooltip("���/���� �C�����a�W�ٻP Play ���s")]
        [SerializeField]
        private GameObject controlPanel;

        [SerializeField]
        private GameObject waitingQueue;

        [Tooltip("���/���� �u�W���H��")]
        [SerializeField]
        private Text playersOnMaster;

        // �C���������s�X, �i�� Photon Server ���P�ڹC�����P�������Ϲj.
        string gameVersion = "1";
        bool isConnecting = false;
        static int playerNumber = 0;


        void Awake()
        {
            instance = this;
            // �T�O�Ҧ��s�u�����a�����J�ۦP���C������
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
        // �P Photon Cloud �s�u
        public void Connect()
        {
            //playersOnMaster.SetActive(true);
            waitingQueue.SetActive(true);
            controlPanel.SetActive(false);
            // �ˬd�O�_�P Photon Cloud �s�u
            if (PhotonNetwork.IsConnected)
            {
                  PhotonNetwork.JoinRandomRoom();         
                
            }
            else
            {
                // ���s�u, ���ջP Photon Cloud �s�u
                PhotonNetwork.GameVersion = gameVersion;
                isConnecting = PhotonNetwork.ConnectUsingSettings();
            }
        }


        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
            Debug.Log("PUN �I�s OnConnectedToMaster(), �w�s�W Photon Cloud.");
            if (isConnecting)
            {
            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom(null, maxPlayersPerRoom, MatchmakingMode.FillRoom, TypedLobby.Default, null);
                isConnecting = false;
            }
            // �T�{�w�s�W Photon Cloud
            // �H���[�J�@�ӹC����
        }
        public override void OnDisconnected(DisconnectCause cause)
        {
            //playersOnMaster.SetActive(true);
            controlPanel.SetActive(true);
            Debug.LogWarningFormat("PUN �I�s OnDisconnected() {0}.", cause);
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN �I�s OnJoinRandomFailed(), �H���[�J�C���ǥ���.");
            Debug.Log("PUN , �H���[�J�C���ǥ���.");
            // �H���[�J�C���ǥ���. �i���]�O 1. �S���C���� �� 2. ���������F.    
            // �n�a, �ڭ̦ۤv�}�@�ӹC����.
            PhotonNetwork.CreateRoom(null, new RoomOptions
            {
                MaxPlayers = 5,
                CleanupCacheOnLeave = false
            }); 
            Debug.Log("PUN , �ЫعC���Ǧ��\.");
        }


        public override void OnJoinedRoom()
        {
            waitingQueue.SetActive(true);
            Debug.Log("PUN �I�s OnJoinedRoom(), �w���\�i�J�C���Ǥ�." + " " + PhotonNetwork.CurrentRoom.PlayerCount);
            /*if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayersPerRoom)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;    
                PhotonNetwork.LoadLevel(1);    
            }*/
        }
    }
}