using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

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

    public PhotonView PV;

    private void Awake()
    {
        if (instance != null) 
            Destroy(this);

        if (PV == null)
        {
            PV = GetComponentInParent<PhotonView>();
            instance = PV.GetComponentInChildren<InventoryManager>();
            Debug.Log(instance +" " + PV.ViewID);
        }
        

        Debug.Log(instance);
       
        
    }

    private void OnEnable()
    {
        
        if (PV.IsMine)
        {
            Debug.Log("123123123");
            RefreshTool();
            Debug.Log("1");
            instance.itemInfo.text = "";
        }

        
        
        
    }



    public static void UpdateToolInfo(string itemDescription) 
    {
        instance.itemInfo.text = itemDescription;
    }

    public static void RefreshTool()
    {
        Debug.Log(instance.slotGrid.transform.childCount);
        for (int i = 0; i < instance.slotGrid.transform.childCount; i++)
        {
            if (instance.slotGrid.transform.childCount == 0)
                break;
            Destroy(instance.slotGrid.transform.GetChild(i).gameObject);
            instance.slots.Clear();
        }

        for (int i = 0; i < instance.myBag.toolList.Count; i++) 
        {
            instance.slots.Add(Instantiate(instance.emptySlot)); // 生成18個空白的格子
            instance.slots[i].transform.SetParent(instance.slotGrid.transform);
            instance.slots[i].GetComponent<Slot>().slotID = i;
            instance.slots[i].GetComponent<Slot>().SetupSlot(instance.myBag.toolList[i]);
            Debug.Log(instance.slots[i].GetComponent<Slot>().slotID);
        }
    }
}
