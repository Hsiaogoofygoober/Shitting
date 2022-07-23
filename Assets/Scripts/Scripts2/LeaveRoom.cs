using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveRoom : MonoBehaviour
{

    [SerializeField]
    private GameObject LeaveRoomButton;

    void Awake()
    {
        LeaveRoomButton.SetActive(true);
        Cursor.visible = true;
        Screen.lockCursor = false;
    }

    public void GoBackLobby()
    {
        SceneManager.LoadScene(0);
    }
}
