using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerManagers : MonoBehaviour
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
        //Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
       controller =  PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"),Vector3.zero,Quaternion.identity,0, new object[] { PV.ViewID });
	}

	public void Die()
	{
		PhotonNetwork.Destroy(controller);
		
	}
}
