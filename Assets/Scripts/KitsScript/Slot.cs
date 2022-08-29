using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public int slotID; //空格ID 等於 物品ID

    //[SerializeField]
    //public Tool slotTool;

    [SerializeField]
    public Image slotImage;

    public string slotInfo;

    public string slotName;

    [SerializeField]
    public GameObject toolOnSlot;

    //public Inventory myBag;

    public void ItemOnClicked() 
    {
        InventoryManager.UpdateToolInfo(slotInfo);
        Debug.Log(slotName + " " + slotID);
        InventorySystem.instance.toolList[slotID] = null;
        InventoryManager.RefreshTool();
    }

    public void SetupSlot(Tool tool) 
    {
        toolOnSlot.SetActive(true);
        if (tool == null) 
        {
            toolOnSlot.SetActive(false);
            return;
        }
        
        slotImage.sprite = tool.toolImage;
        if(slotImage.sprite != null)
            Debug.Log("真的是很可悲 " + slotID);
        slotInfo = tool.toolInfo;
        slotName = tool.toolName;
    }
}
