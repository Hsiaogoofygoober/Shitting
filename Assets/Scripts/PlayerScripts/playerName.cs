using UnityEngine;
using Photon.Pun;
using TMPro;

public class playerName : MonoBehaviourPun
{
    [SerializeField] private TextMeshProUGUI nameText;

    public string name;
    void Start()
    {
        if (photonView.IsMine) { return; }

        SetName();
    }

    private void SetName()
    {
        nameText.text = photonView.Owner.NickName;
        name = photonView.Owner.NickName;
    }

}
