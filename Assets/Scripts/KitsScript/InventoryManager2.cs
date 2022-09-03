using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager2 : MonoBehaviour
{

    public InventorySystem inventorySystem;

    [SerializeField]
    public GameObject slotGrid;

    [SerializeField]
    //public Slot slotPrefab;
    public GameObject emptySlot;

    [SerializeField]
    public TMP_Text itemInfo;

    public List<GameObject> slots = new List<GameObject>();

    public PhotonView PV;

    public Tool[] toolList;

    private void Awake()
    {
        if (PV == null)
        {
            PV = this.gameObject.GetComponentInParent<PhotonView>();
            Debug.Log("PV 為 : " + PV.ViewID);
            //Debug.Log("slot ID ��: " + GetComponentInChildren<Slot>().slotID);
        }
        toolList = inventorySystem.toolList;

        GameObject grid = GameObject.Find("grid");
        Debug.Log("grid 個數為: " + grid.transform.childCount);
        //foreach (Transform slot in grid.GetChildCount)
        //{
        //    Debug.Log(slot.GetChild(0).GetComponent<Slot>().slotID);
        //}
    }

    private void OnEnable()
    {
        if (PV.IsMine)
            RefreshTool();
        //Debug.Log("1");
        itemInfo.text = "";
    }

    private void Update()
    {
        if (PlayerPrefs.HasKey("SlotID"))
        {
            inventorySystem.toolList[PlayerPrefs.GetInt("SlotID")] = null;
            PlayerPrefs.DeleteAll();
        }
    }

    public void UpdateToolInfo(string itemDescription)
    {
        itemInfo.text = itemDescription;
    }

    public void RefreshTool()
    {
        if (PV.IsMine) 
        {
            for (int i = 0; i < slotGrid.transform.childCount; i++) 
            {
                slotGrid.transform.GetChild(i).GetComponent<Slot>().SetupSlot(toolList[i]);
            }    
        }
    }
}
