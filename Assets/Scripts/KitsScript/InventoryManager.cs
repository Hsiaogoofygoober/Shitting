using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using StarterAssets;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    [SerializeField]
    public Inventory myBag;

    [SerializeField]
    public GameObject slotGrid;

    [SerializeField]
    //public Slot slotPrefab;
    public GameObject emptySlot;  

    [SerializeField]
    public TMP_Text itemInfo;

    public List<GameObject> slots = new List<GameObject>();

    public Tool[] toolList;

    //public PhotonView PV;

    private void Awake()
    {
        toolList = InventorySystem.instance.toolList;
        if (instance != null) 
            Destroy(this);
        instance = this;

        //if (PV == null)
        //{
        //    PV = GetComponentInParent<PhotonView>();
        //    instance = PV.GetComponentInChildren<InventoryManager>();
        //    Debug.Log(instance +" " + PV.ViewID);
        //}
        //Debug.Log(instance);
    }

    private void OnEnable()
    {  
        RefreshTool();
        //Debug.Log("1");
        instance.itemInfo.text = "";
        //if (PV.IsMine)
        //{
            //Debug.Log("123123123");
            //RefreshTool();
            //Debug.Log("1");
        //    instance.itemInfo.text = "";
        //}
    }

    private void Start()
    {
    
    }


    public static void UpdateToolInfo(string itemDescription) 
    {
        instance.itemInfo.text = itemDescription;
    }

    public static void RefreshTool()
    {
        //Debug.Log(instance.slotGrid.transform.childCount);
        for (int i = 0; i < instance.slotGrid.transform.childCount; i++)
        {
            if (instance.slotGrid.transform.childCount == 0)
                break;
            Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
            instance.slots.Clear();
        }

        for (int i = 0; i < InventorySystem.instance.toolList.Length; i++) 
        {
            instance.slots.Add(Instantiate(instance.emptySlot)); // 生成18個空白的格子
            instance.slots[i].transform.SetParent(instance.slotGrid.transform);
            instance.slots[i].GetComponent<Slot>().slotID = i;
            if (InventorySystem.instance.toolList[i] != null)
                Debug.Log("幹 " + i);
            instance.slots[i].GetComponent<Slot>().SetupSlot(InventorySystem.instance.toolList[i]);
            if (instance.slots[i].GetComponent<Slot>().slotImage != null)
                Debug.Log("Apex小達人 " + i);
           
        }
    }
}
