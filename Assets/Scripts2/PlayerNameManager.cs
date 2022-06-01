using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInput;

    public void OnUserNameInputValueChanged() 
    { 
        PhotonNetwork.NickName = usernameInput.text;
    }
}
