using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveRoom : MonoBehaviour
{

    [SerializeField]
    private GameObject LoserLabel;

    [SerializeField]
    private GameObject WinnerLabel;

    void Awake()
    {   
        Cursor.visible = true;
        if (PlayerPrefs.GetInt("Status") == 0)
        {
            LoserLabel.SetActive(true);
            WinnerLabel.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Status") == 1) 
        {
            LoserLabel.SetActive(false);
            WinnerLabel.SetActive(true);
        }
        Cursor.visible = true;
        Screen.lockCursor = false;
        PlayerPrefs.DeleteAll();
    }

    public void GoBackLobby()
    {
        SceneManager.LoadScene(0);
    }
}
