/*using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolOnDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;
    private int currentToolID; //當前物品ID 

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        currentToolID = originalParent.GetComponent<Slot>().slotID;
        //Debug.Log(currentToolID);
        transform.SetParent(transform.parent.parent);
        transform.position = eventData.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        for (int i = 0; i < 18; i++)
        {
            if (GetComponentInParent<FirstPersonController>().toolList[i] == null)
                Debug.Log(i + " 媽的哩 " + GetComponentInParent<FirstPersonController>().toolList.Count);
            GetComponentInParent<FirstPersonController>().toolList[i] = null;
        }

    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }


    // 把myBag.toolList 改成 FirstPersonController.instance.toolList
    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            Debug.Log("幹你娘");
            if (eventData.pointerCurrentRaycast.gameObject.name == "item image")
            {
                Debug.Log("機掰");
                transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform.parent.parent);
                transform.position = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.position;

                var temp = GetComponentInParent<FirstPersonController>().toolList[currentToolID];
                GetComponentInParent<FirstPersonController>().toolList[currentToolID] = GetComponentInParent<FirstPersonController>().toolList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>().slotID];
                GetComponentInParent<FirstPersonController>().toolList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>().slotID] = temp;
                Debug.Log(currentToolID);
                Debug.Log(eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>().slotID);

                eventData.pointerCurrentRaycast.gameObject.transform.parent.position = originalParent.position;
                eventData.pointerCurrentRaycast.gameObject.transform.parent.SetParent(originalParent);
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                return;
            }

            if (eventData.pointerCurrentRaycast.gameObject.name == "slot(Clone)")
            {
                Debug.Log("死老人");
                transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform);
                transform.position = eventData.pointerCurrentRaycast.gameObject.transform.position;

                //for (int i = 0; i < 18; i++) 
                //{
                //    GetComponentInParent<FirstPersonController>().toolList[i] = null;
                //}
                

                //if (7 != currentToolID)
                //    GetComponentInParent<FirstPersonController>().toolList[currentToolID] = null;

                GetComponent<CanvasGroup>().blocksRaycasts = true;
                return;
            }
        }
        // 其他任何位置都歸位
        if (GetComponentInParent<FirstPersonController>().toolList == null) 
        {
            Debug.Log("幹你娘機掰");
        }

        transform.SetParent(originalParent);
        transform.position = originalParent.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

    }

}
*/