using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
		Vector3 spawnPoint = new Vector3(Random.Range(-200f, 260f), 150f,  Random.Range(-130f, 340f));
		controller =  PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPoint, Quaternion.identity , 0, new object[] { PV.ViewID });
	}

	public void Die()
	{
		Debug.Log("Leave Room");
		DisconnectPlayer();
		//Destroy(RoomManager.instance.gameObject);
		//PhotonNetwork.LeaveRoom();
		//PhotonNetwork.Disconnect();
		//SceneManager.LoadScene(2);
		//PhotonNetwork.LoadLevel(2);
	}

	public void Win()
	{
		DisconnectPlayer();
		//Destroy(RoomManager.instance.gameObject);
		//PhotonNetwork.LeaveRoom();
		//PhotonNetwork.Disconnect();	
		//SceneManager.LoadScene(2);
		//PhotonNetwork.LoadLevel(2);
	}

	public void DisconnectPlayer()
	{
		Destroy(RoomManager.instance.gameObject);
		StartCoroutine(DisconnectAndLoad());
	}

	IEnumerator DisconnectAndLoad()
	{
		if (PhotonNetwork.InRoom)
		{
			SceneManager.LoadScene("Finish");

			PhotonNetwork.AutomaticallySyncScene = false;
		}
		else
			yield return null;

		PhotonNetwork.Disconnect();

	}

	//public void LeaveRoom()
	//{
	//	PhotonNetwork.LeaveRoom();
	//}

	//public override void OnLeftRoom()
	//{
	//	SceneManager.LoadScene(2);

	//	base.OnLeftRoom();
	//}
}
