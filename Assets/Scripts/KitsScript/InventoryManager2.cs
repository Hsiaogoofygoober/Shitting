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
        //if (instance != null) 
        //    Destroy(this);
        //instance = this;
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
        GameObject grid = GameObject.Find("grid");

        if (PV.IsMine) 
        {
            for (int i = 0; i < grid.transform.childCount; i++) 
            {
                grid.transform.GetChild(i).GetComponent<Slot>().SetupSlot(toolList[i]);
            }    
        }

        //if (PV.IsMine)
        //{
        //    Debug.Log("slots�ӼƬ�: " + GetComponentsInChildren<Slot>().Length);
        //    ////Debug.Log(instance.slotGrid.transform.childCount);
        //    //for (int i = 0; i < slotGrid.transform.childCount; i++)
        //    //{
        //    //    Debug.Log("Fuck my life " + slotGrid.transform.childCount);
        //    //    if (slotGrid.transform.childCount == 0)
        //    //        break;
        //    //    //slotGrid.transform.GetChild(i).gameObject = null;
        //    //    slots.Clear();
        //    //}
            

        //    for (int i = 0; i < toolList.Length; i++)
        //    {
        //        slots.Add(Instantiate(emptySlot)); // �ͦ�18�Ӫťժ���l
        //        slots[i].transform.SetParent(slotGrid.transform);
        //        slots[i].GetComponent<Slot>().slotID = i;
        //        if (toolList[i] != null)
        //            Debug.Log("�F " + i);
        //        slots[i].GetComponent<Slot>().SetupSlot(toolList[i]);
        //        if (slots[i].GetComponent<Slot>().slotImage.sprite != null)
        //            Debug.Log("Apex�p�F�H " + i);
        //        Debug.Log("PV ID ��: " + PV.ViewID);
        //    }
        //}
        //else
        //{
        //    Debug.Log("PV ID ��: " + PV.ViewID);
        //}
    }
}
