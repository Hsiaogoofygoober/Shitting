using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ToolOnWorld : MonoBehaviour
{
    public Tool thisTool;
    
    private void OnTriggerEnter(Collider other)
    { 
            if (other.gameObject.CompareTag("Player"))
            {
                AddNewItem();
                Destroy(gameObject);
            } 
    }

    public void AddNewItem() 
    {

        for (int i = 0; i < InventorySystem.instance.toolList.Length; i++) 
        {
            
            if (InventorySystem.instance.toolList[i] == null) 
            {
                InventorySystem.instance.toolList[i] = thisTool;
                break;
            }
        }
        InventoryManager.RefreshTool();
    }
}