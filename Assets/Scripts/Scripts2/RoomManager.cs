using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;
using Photon.Realtime;
public class RoomManager : MonoBehaviourPunCallbacks
{
	public static RoomManager instance;

	void Awake()
	{
		if (instance)
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		instance = this;
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
		else if (scene.buildIndex == 0) {
			if(PhotonNetwork.IsConnected)
				PhotonNetwork.Disconnect();
		}
	}

	public override void OnPlayerEnteredRoom(Player other)
	{
		Debug.LogFormat("{0} 進入遊戲室", other.NickName);
		if (PhotonNetwork.IsMasterClient)
		{
			Debug.LogFormat("我是 Master Client 嗎? {0}",
				PhotonNetwork.IsMasterClient);
			LoadArena();
		}
	}

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("我是 Master Client 嗎? {0}",
                PhotonNetwork.IsMasterClient);
            LoadArena();
        }
    }

    void LoadArena()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			Debug.LogError("我不是 Master Client, 不做載入場景的動作");
		}
		Debug.LogFormat("載入{0}人的場景",
			PhotonNetwork.CurrentRoom.PlayerCount);
		if (PhotonNetwork.CurrentRoom.PlayerCount == 3)
        {;
			PhotonNetwork.CurrentRoom.IsOpen = false;
			PhotonNetwork.LoadLevel("GameScene");
        }
	}
}
