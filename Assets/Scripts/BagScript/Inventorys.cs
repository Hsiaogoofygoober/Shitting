using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventorys : MonoBehaviour
{
    private const int SLOTS = 7;

    private List<IInventoryItem> mItems = new List<IInventoryItem>();

    public event EventHandler<InventoryEventArgs> ItemAdded;

    public event EventHandler<InventoryEventArgs> ItemRemoved;

    public event EventHandler<InventoryEventArgs> ItemUsed;

    public Inventorys() 
    {
        for (int i = 0; i < SLOTS; i++) 
        {
        }
    }

    public void AddItem(IInventoryItem item)
    {
        if (mItems.Count < SLOTS)
        {
            mItems.Add(item);

            item.OnPickup();

            if (ItemAdded != null)
            {
                ItemAdded(this, new InventoryEventArgs(item));
            }

            //Collider collider = (item as MonoBehaviour).GetComponent<Collider>();
            //if (collider.enabled)
            //{
            //    collider.enabled = false;

            //    mItems.Add(item);

            //    item.OnPickup();

            //    if (ItemAdded != null)
            //    {
            //        ItemAdded(this, new InventoryEventArgs(item));
            //    }
            //}
        }
    }

    internal void UseItem(IInventoryItem item)
    {
        if (ItemUsed != null) 
        {
            ItemUsed(this, new InventoryEventArgs(item));
        }
    }

    public void RemoveItem(IInventoryItem item) 
    {
        if (mItems.Contains(item)) 
        {
            mItems.Remove(item);

            item.OnDrop();

            if (ItemRemoved != null)
            {
                ItemRemoved(this, new InventoryEventArgs(item));
            }
        }
    }
}
