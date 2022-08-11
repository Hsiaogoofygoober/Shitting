using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField]
    public Tool slotTool;

    [SerializeField]
    public Image slotImage;

    public string slotInfo;

    [SerializeField]
    public GameObject toolOnSlot;

    public void ItemOnClicked() 
    {
        InventoryManager.UpdateToolInfo(slotInfo);
    }

    public void SetupSlot(Tool tool) 
    {
        if (tool == null) 
        {
            toolOnSlot.SetActive(false);
            return;
        }
        slotImage.sprite = tool.toolImage;
        slotInfo = tool.toolInfo;
    }
}
