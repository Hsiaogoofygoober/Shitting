using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class Slot : MonoBehaviour
{
    public int slotID; //空格ID 等於 物品ID

    public Image slotImage;

    public string slotInfo;
    public int slotValue;
    public string slotName;
    [SerializeField] private float decreasingRate = 2;
    private FirstPersonController player;
    public GameObject toolOnSlot;
    //audio
    [SerializeField] private AudioSource eatingSound;
    [SerializeField] private AudioSource burpSound;

    public void ItemOnClicked() 
    {   
        Debug.Log(slotName + " 遊戲 + 結束 " + slotID + " + 可憐 + " + slotInfo);
        player = GetComponentInParent<FirstPersonController>();

        if (player.canUse)
        {
            switch (slotName)
            {
                case "Burger":
                    
                    if (player.currentHealth < 100)
                    {
                        eatingSound.Play();
                        UsingBurger();
                        Invoke("AfterUsingBurger", 4);
                        slotImage.sprite = null;
                        slotInfo = null;
                        slotName = null;
                        toolOnSlot.SetActive(false);
                        PlayerPrefs.SetInt("SlotID", slotID);
                    }
                    break;
                case "PisolAmmo":
                case "RifleAmmo":
                case "ShotgunAmmo":
                    break;
                default:
                    Debug.Log("error");
                    break;
            }
        }
        
       
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
        if (slotImage.sprite != null)
            Debug.Log("真的是很可悲 " + slotID);
        slotInfo = tool.toolInfo;
        slotName = tool.toolName;
        slotValue = tool.toolValue;
        if (slotName == "Burger")
        {
            slotImage.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }
        else
        {
            slotImage.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }
    public void UsingBurger()
    {
        player.MoveSpeed /= decreasingRate;
        player.SprintSpeed /= decreasingRate;
        player.canUse = false;
       
    }
    public void AfterUsingBurger()
    {
        if(player.currentHealth > 100 - slotValue)
        {
            player.currentHealth = 100;
        }
        else
        {
            player.currentHealth += slotValue;
        }
        burpSound.Play();
        player.MoveSpeed *= decreasingRate;
        player.SprintSpeed *= decreasingRate;
        player.canUse = true;
    }
}
