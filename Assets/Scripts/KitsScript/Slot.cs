using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public int slotID; //空格ID 等於 物品ID

    public Image slotImage;

    public string slotInfo;

    public string slotName;

    public GameObject toolOnSlot;

    public void ItemOnClicked() 
    {   
        Debug.Log(slotName + " 遊戲 + 結束 " + slotID + " + 可憐 + " + slotInfo);
        slotImage.sprite = null;
        slotInfo = null;
        slotName = null;
        toolOnSlot.SetActive(false);
        //InventoryManager.UpdateToolInfo(slotInfo);

        PlayerPrefs.SetInt("SlotID", slotID);
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
