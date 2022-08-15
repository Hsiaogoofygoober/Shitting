using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private void Awake()
    {
        if (instance != null) 
            Destroy(this);
        instance = this;
    }

    private void OnEnable()
    {
        RefreshTool();
        instance.itemInfo.text  = "";
    }

    public static void UpdateToolInfo(string itemDescription) 
    {
        instance.itemInfo.text = itemDescription;
    }

    public static void RefreshTool() 
    {
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
        }
    }
}
