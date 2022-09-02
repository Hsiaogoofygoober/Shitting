//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class HUD : MonoBehaviour
//{
//    public Inventorys Inventorys;

//    private void Start()
//    {
//        Inventorys.ItemAdded += InventoryScript_ItemAdd;
//        Inventorys.ItemRemoved += InventoryScript_ItemRemoved;
//    }

//    private void InventoryScript_ItemAdd(object sender, InventoryEventArgs e)
//    {
//        Transform inventoryPanel = transform.Find("inventoryPanel");
//        foreach (Transform slot in inventoryPanel)
//        {
//            Debug.Log("靠杯 " + inventoryPanel.childCount); // ok
//            Transform imageTransform = slot.GetChild(0).GetChild(0);
//            ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();
//            //Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();

//            if (slot.GetChild(0).GetChild(0).GetComponent<Image>().sprite == null) 
//            {
//                Debug.Log("低能專案");

//                slot.GetChild(0).GetChild(0).GetComponent<Image>().enabled = true;
//                slot.GetChild(0).GetChild(0).GetComponent<Image>().sprite = e.Item.Image;

//                itemDragHandler.Item = e.Item;

//                break;
//            }

//            //if (slot.GetChild(0).GetChild(0).GetComponent<Image>() == null)
//            //{
//            //    Debug.Log("低能老師");
//            //    slot.GetChild(0).GetChild(0).GetComponent<Image>().enabled = true;
//            //    slot.GetChild(0).GetChild(0).GetComponent<Image>().sprite = e.Item.Image;

//            //    break;
//            //}
//        }
//    }

//    private void InventoryScript_ItemRemoved(object sender, InventoryEventArgs e)
//    {
//        Transform inventoryPanel = transform.Find("inventoryPanel");
//        foreach (Transform slot in inventoryPanel)
//        {
//            Debug.Log("靠杯 " + inventoryPanel.childCount); // ok
//            Transform imageTransform = slot.GetChild(0).GetChild(0);
//            ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();
//            //Image image = slot.GetChild(0).GetChild(0).GetComponent<Image>();

//            if (itemDragHandler.Item.Equals(e.Item))
//            {
//                Debug.Log("低能專案");

//                slot.GetChild(0).GetChild(0).GetComponent<Image>().enabled = false;
//                slot.GetChild(0).GetChild(0).GetComponent<Image>().sprite = null;
//                itemDragHandler.Item = null;
//                break;
//            }

//            //if (slot.GetChild(0).GetChild(0).GetComponent<Image>() == null)
//            //{
//            //    Debug.Log("低能老師");
//            //    slot.GetChild(0).GetChild(0).GetComponent<Image>().enabled = true;
//            //    slot.GetChild(0).GetChild(0).GetComponent<Image>().sprite = e.Item.Image;

//            //    break;
//            //}
//        }
//    }
//}
