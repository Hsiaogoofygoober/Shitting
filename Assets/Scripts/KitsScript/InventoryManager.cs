//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;
//using Photon.Pun;
//using StarterAssets;

//public class InventoryManager : MonoBehaviour
//{
//    public static InventoryManager instance;

//    [SerializeField]
//    public InventorySystem inventorySystem;

//    [SerializeField]
//    public GameObject slotGrid;

//    [SerializeField]
//    //public Slot slotPrefab;
//    public GameObject emptySlot;

//    [SerializeField]
//    public TMP_Text itemInfo;

//    public List<GameObject> slots = new List<GameObject>();

//    public PhotonView PV;

//    public Tool[] toolList;

//    private void Awake()
//    {
//        //if (instance != null) 
//        //    Destroy(this);
//        //instance = this;
//        if (PV == null)
//        {
//            PV = this.gameObject.GetComponentInParent<PhotonView>();
//            instance = PV.GetComponentInChildren<InventoryManager>();
//            Debug.Log("幹你娘真的很可悲 ＋ " + PV.ViewID);
//        }
//        toolList = inventorySystem.toolList;
//    }

//    private void OnEnable()
//    {
//        if (instance.PV.IsMine)
//            RefreshTool();
//        //Debug.Log("1");
//        instance.itemInfo.text = "";
//    }

//    private void Start()
//    {

//    }

//    public static void UpdateToolInfo(string itemDescription)
//    {
//        instance.itemInfo.text = itemDescription;
//    }

//    public static void RefreshTool()
//    {
//        if (instance.PV.IsMine)
//        {
//            Debug.Log("PV ID 為: " + instance.PV.ViewID);
//            //Debug.Log(instance.slotGrid.transform.childCount);
//            for (int i = 0; i < instance.slotGrid.transform.childCount; i++)
//            {
//                Debug.Log("Fuck my life " + instance.slotGrid.transform.childCount);
//                if (instance.slotGrid.transform.childCount == 0)
//                    break;
//                Destroy(instance.slotGrid.transform.GetChild(i).gameObject);

//            }
//            instance.slots.Clear();

//            for (int i = 0; i < instance.toolList.Length; i++)
//            {
//                instance.slots.Add(Instantiate(instance.emptySlot)); // 生成18個空白的格子
//                instance.slots[i].transform.SetParent(instance.slotGrid.transform);
//                instance.slots[i].GetComponent<Slot>().slotID = i;
//                if (instance.toolList[i] != null)
//                    Debug.Log("幹 " + i);
//                instance.slots[i].GetComponent<Slot>().SetupSlot(instance.toolList[i]);
//                if (instance.slots[i].GetComponent<Slot>().slotImage.sprite != null)
//                    Debug.Log("Apex小達人 " + i);
//                    Debug.Log("PV ID 為: " + instance.PV.ViewID);
//            }
//        }
//        else {
//            Debug.Log("PV ID 為: " + instance.PV.ViewID);
//        }
//    }
//}
