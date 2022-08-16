using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ToolOnWorld : MonoBehaviour
{
    public Tool thisTool;
    public Inventory playerInventary;
    
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

        for (int i = 0; i < playerInventary.toolList.Count; i++) 
        {
            if (playerInventary.toolList[i] == null) 
            {
                playerInventary.toolList[i] = thisTool;
                break;
            }
        }
        InventoryManager.RefreshTool();
    }
}