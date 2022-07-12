using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
	public static RoomManager instance;

	//public static RoomManager Instance{
	//	get
	//	{
	//		return instance ?? (instance = GameObject.FindObjectOfType(typeof(RoomManager)) as RoomManager);
	//	}
	//}

	public static bool isHave = true;
	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else {
			Destroy(gameObject);
		}
		if (isHave == true)
		{
			DontDestroyOnLoad(gameObject);
			isHave = false;	
		}
	}

	public override void OnEnable()
	{
		base.OnEnable();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		if (scene.buildIndex == 1) // We're in the game scene
		{
			PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
		}
		else if (scene.buildIndex == 0) 
		{
			Destroy(instance);	
		}
	}
}
