using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerManagers : MonoBehaviour
{
	PhotonView PV;

	GameObject controller;

	CheckWinner CW;

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
		Vector3 spawnPoint = new Vector3(Random.Range(-200f, 260f), 150f,  Random.Range(-130f, 340f));

        controller =  PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPoint, Quaternion.identity , 0, new object[] { PV.ViewID });
	}

	public void Die()
	{
		PlayerPrefs.SetInt("Status", 0);
		PhotonNetwork.LeaveRoom();
		Debug.Log("Leave Room");
		Destroy(RoomManager.instance.gameObject);
		PhotonNetwork.LoadLevel(2);
	}

	public void Win() 
	{
		PlayerPrefs.SetInt("Status", 1);
		PhotonNetwork.LeaveRoom();
		Destroy(RoomManager.instance.gameObject);
		PhotonNetwork.LoadLevel(2);
	}
}
