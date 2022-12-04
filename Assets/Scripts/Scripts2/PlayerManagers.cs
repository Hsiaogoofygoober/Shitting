using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManagers : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    GameObject controller;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Debug.Log("Instantiated Player Controller");
        Vector3 spawnPoint = new Vector3(Random.Range(-180f, 140f), 150f, Random.Range(-120f, 300f));
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPoint, Quaternion.identity, 0, new object[] { PV.ViewID });
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        Debug.Log("Player Die and Leave Room");
        DisconnectPlayer();
    }

    public void Win()
    {
        Invoke("wait", 5);
        
        //Destroy(RoomManager.instance.gameObject);
        DisconnectPlayer();
    }

    public void wait() 
    {
        PhotonNetwork.Destroy(controller);;
    }

    public void DisconnectPlayer()
    {
        PhotonNetwork.LeaveRoom(false);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("��������");
        Destroy(RoomManager.instance.gameObject);
        SceneManager.LoadScene("Finish");
    }

    public static PlayerManagers Find(Player player)
    {
        return FindObjectsOfType<PlayerManagers>().SingleOrDefault(x => x.PV.Owner == player);
    }
}
