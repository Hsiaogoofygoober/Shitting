using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using StarterAssets;

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

    private Stack<int> pistolAmmoStack = new Stack<int> ();
    private Stack<int> rifleAmmoStack = new Stack<int>();
    private Stack<int> shotgunAmmoStack = new Stack<int>();
    private FirstPersonController player;
    public int rifleAmmoPerBox;
    public int pistolAmmoPerBox;
    public int shotgunAmmoPerBox;
    private int rifleCount;
    private int pistolCount;
    private int shotgunCount;
    private void Awake()
    {
        if (PV == null)
        {
            PV = this.gameObject.GetComponentInParent<PhotonView>();
            Debug.Log("PV 為 : " + PV.ViewID);
            //Debug.Log("slot ID ��: " + GetComponentInChildren<Slot>().slotID);
        }
        toolList = inventorySystem.toolList;
        player = GetComponentInParent<FirstPersonController>();
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
        itemInfo.text = "";
    }

    private void Update()
    {
        if (PlayerPrefs.HasKey("SlotID"))
        {
            if (inventorySystem.toolList[PlayerPrefs.GetInt("SlotID")].toolName == "PistolAmmo")
            {
                pistolAmmoStack.Push(PlayerPrefs.GetInt("SlotID"));
                Debug.Log("push id >> " + PlayerPrefs.GetInt("SlotID"));
                PlayerPrefs.DeleteKey("SlotID");
            }
            else if(inventorySystem.toolList[PlayerPrefs.GetInt("SlotID")].toolName == "RifleAmmo")
            {
                rifleAmmoStack.Push(PlayerPrefs.GetInt("SlotID"));
                Debug.Log("push id >> " + PlayerPrefs.GetInt("SlotID"));
                PlayerPrefs.DeleteKey("SlotID");
            }
            else if (inventorySystem.toolList[PlayerPrefs.GetInt("SlotID")].toolName == "ShotgunAmmo")
            {
                shotgunAmmoStack.Push(PlayerPrefs.GetInt("SlotID"));
                Debug.Log("push id >> " + PlayerPrefs.GetInt("SlotID"));
                PlayerPrefs.DeleteKey("SlotID");
            }
            else
            {
                inventorySystem.toolList[PlayerPrefs.GetInt("SlotID")] = null;
                PlayerPrefs.DeleteKey("SlotID");
            }
            Debug.Log("Slot id >> " + PlayerPrefs.GetInt("SlotID"));

        }

        if (PlayerPrefs.GetInt("RefreshBag") == 1) 
        {
            if(player.pistolAmmo % pistolAmmoPerBox == 0)
            {
                pistolCount = player.pistolAmmo / pistolAmmoPerBox;
            }
            else
            {
                pistolCount = player.pistolAmmo / pistolAmmoPerBox+1;
            }
            if (pistolAmmoStack.Count > pistolCount)
            {
                int popId = pistolAmmoStack.Pop();
                Debug.Log("pop id >> " + popId);
                inventorySystem.toolList[popId] = null;
                RefreshTool();
            }
            
        }
        else if(PlayerPrefs.GetInt("RefreshBag") == 2)
        {
            if (player.rifleAmmo % rifleAmmoPerBox == 0)
            {
                rifleCount = player.rifleAmmo / rifleAmmoPerBox;
            }
            else
            {
                rifleCount = player.rifleAmmo / rifleAmmoPerBox + 1;
            }
            if (rifleAmmoStack.Count > rifleCount)
            {
                //Debug.Log("pop id >> " + PlayerPrefs.GetInt("SlotID"));
                int popId = rifleAmmoStack.Pop();
                Debug.Log("pop id >> " + popId);
                inventorySystem.toolList[popId] = null;
                RefreshTool();
                //PlayerPrefs.DeleteAll();
            }   
        }
        else if (PlayerPrefs.GetInt("RefreshBag") == 3)
        {
            if (player.shotgunAmmo % shotgunAmmoPerBox == 0)
            {
                shotgunCount = player.shotgunAmmo / shotgunAmmoPerBox;
            }
            else
            {
                shotgunCount = player.shotgunAmmo / shotgunAmmoPerBox + 1;
            }
            if (shotgunAmmoStack.Count > shotgunCount)
            {
                //Debug.Log("pop id >> " + PlayerPrefs.GetInt("SlotID"));
                int popId = shotgunAmmoStack.Pop();
                Debug.Log("pop id >> " + popId);
                inventorySystem.toolList[popId] = null;
                RefreshTool();
               // PlayerPrefs.DeleteAll();
            }
            
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

    public bool IsBagFull()
    {
        if (PV.IsMine)
        {
            for (int i = 0; i < toolList.Length; i++)
            {
                if(toolList[i] == null) return false;
            }
            return true;
        }
        return true;
    }

    public void DropKitWhenDie(float DropUpForce)
    {
        if (PV.IsMine)
        {
            for (int i = 0; i < toolList.Length; i++)
            {
                if (toolList[i] != null)
                {
                    GameObject Kit = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Kits", toolList[i].toolName), transform.position, Quaternion.identity);
                    Kit.GetComponent<Rigidbody>().AddForce(Vector3.up * DropUpForce, ForceMode.Impulse);
                    float random = Random.Range(-1f, 1f);
                    Kit.GetComponent<Rigidbody>().AddTorque(new Vector3(random, random, random) * 10);
                }
            }
            
        }
    }
}
