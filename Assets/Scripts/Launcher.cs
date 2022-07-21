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
        public static Launcher instance;

        [Tooltip("�C���Ǫ��a�H�ƤW��. ��C���Ǫ��a�H�Ƥw���B, �s���a�u��s�}�C���ǨӶi��C��.")]
        [SerializeField]
        private byte maxPlayersPerRoom = 2;

        [Tooltip("���/���� �C�����a�W�ٻP Play ���s")]
        [SerializeField]
        private GameObject controlPanel;

        [Tooltip("���/���� �s�u�� �r��")]
        [SerializeField]
        private GameObject progressLabel;

        [Tooltip("���/���� ���ݤ� �r��")]
        [SerializeField]
        private GameObject waitingLabel;

        [Tooltip("���/���� �u�W���H��")]
        [SerializeField]
        private Text playersOnMaster;

        private Text playerUI;
        // �C���������s�X, �i�� Photon Server ���P�ڹC�����P�������Ϲj.
        string gameVersion = "1";
        bool isConnecting;
        static int playerNumber = 0;

        void Awake()
        {
            instance = this;
            // �T�O�Ҧ��s�u�����a�����J�ۦP���C������
        }

        void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
            progressLabel.SetActive(false);
            waitingLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        // �P Photon Cloud �s�u
        public void Connect()
        {
            //playersOnMaster.SetActive(true);
            progressLabel.SetActive(true);
            waitingLabel.SetActive(false);
            controlPanel.SetActive(false);
            // �ˬd�O�_�P Photon Cloud �s�u
            if (PhotonNetwork.IsConnected)
            {
                // �w�s�u, �|���H���[�J�@�ӹC����
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
            progressLabel.SetActive(false);
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
                MaxPlayers = maxPlayersPerRoom
            });
            Debug.Log("PUN , �ЫعC���Ǧ��\.");
        }


        public override void OnJoinedRoom()
        {
            waitingLabel.SetActive(true);
            progressLabel.SetActive(false);
            Debug.Log("PUN �I�s OnJoinedRoom(), �w���\�i�J�C���Ǥ�." + " " + PhotonNetwork.CurrentRoom.PlayerCount);
            if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayersPerRoom)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;    
                PhotonNetwork.LoadLevel(1);    
            }
        }
    }
}