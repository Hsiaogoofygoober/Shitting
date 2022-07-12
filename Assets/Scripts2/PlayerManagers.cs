using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerManagers : MonoBehaviour
{
	PhotonView PV;

	GameObject controller;

	public float minx;
	public float maxx;
	public float miny;
	public float maxy;

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
		//Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
		//Vector3 randomPosition = new Vector3(Random.Range(minx, maxx), 1, Random.Range(miny, maxy));
		controller =  PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), Vector3.zero, Quaternion.identity, 0, new object[] { PV.ViewID });
	}

	public void Die()
	{
		PhotonNetwork.LeaveRoom();
		Debug.Log("Leave Room");
		PhotonNetwork.LoadLevel(2);
	}
}
