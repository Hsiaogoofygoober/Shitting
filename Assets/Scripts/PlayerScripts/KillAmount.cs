using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAmount : MonoBehaviour
{
    public static KillAmount instance;

	public int amount = 0;

	public int Pid;

	public PhotonView PV;
	
	void Awake()
	{
		PV = GetComponent<PhotonView>();
		//if (instance)
		//{
		//	Destroy(gameObject);
		//	return;
		//}
		//DontDestroyOnLoad(gameObject);
		//instance = this;
	}
}
