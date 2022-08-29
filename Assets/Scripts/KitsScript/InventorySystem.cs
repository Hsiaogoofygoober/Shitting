using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem instance;

    public Tool[] toolList = new Tool[18];

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
}
