using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolOnDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;
    public Inventory myBag;
    private int currentToolID; //當前物品ID 

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        currentToolID = originalParent.GetComponent<Slot>().slotID;
        transform.SetParent(transform.parent.parent);
        transform.position = eventData.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            if (eventData.pointerCurrentRaycast.gameObject.name == "item image")
            {
                transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform.parent.parent);
                transform.position = eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.position;

                var temp = myBag.toolList[currentToolID];
                myBag.toolList[currentToolID] = myBag.toolList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>().slotID];
                myBag.toolList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>().slotID] = temp;

                eventData.pointerCurrentRaycast.gameObject.transform.parent.position = originalParent.position;
                eventData.pointerCurrentRaycast.gameObject.transform.parent.SetParent(originalParent);
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                return;
            }

            if (eventData.pointerCurrentRaycast.gameObject.name == "slot(Clone)")
            {
                transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform);
                transform.position = eventData.pointerCurrentRaycast.gameObject.transform.position;

                myBag.toolList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>().slotID] = myBag.toolList[currentToolID];

                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Slot>().slotID != currentToolID)
                    myBag.toolList[currentToolID] = null;

                GetComponent<CanvasGroup>().blocksRaycasts = true;
                return;
            }
        }
        // 其他任何位置都歸位
        // 
        transform.SetParent(originalParent);
        transform.position = originalParent.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

    }

    public void OnMouseDown()
    {
        Destroy(this.gameObject);
        Debug.Log(this.gameObject.name);
    }


}
