using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaveRoom : MonoBehaviour
{

    [SerializeField]
    private GameObject LoserLabel;

    [SerializeField]
    private GameObject WinnerLabel;

    [SerializeField]
    private TMP_Text my_text;

    void Start()
    {
        Cursor.visible = true;

        if (PlayerPrefs.GetInt("Status") == 0)
        {
            my_text.text = "GOT " + PlayerPrefs.GetString("killer") + " KILLED !!!";
            
            //WinnerLabel.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Status") == 1)
        {
           my_text.text = "YOU ARE CHAMPION !!!";
        }
        Cursor.visible = true;
        Screen.lockCursor = false;
        PlayerPrefs.DeleteAll();
    }



    public void GoBackLobby()
    {
        SceneManager.LoadScene(0);
    }

    public void Open() 
    {
        gameObject.SetActive(true);
    }
}
