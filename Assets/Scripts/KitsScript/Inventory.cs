using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventary", menuName = "Inventory/New Inventary")]
public class Inventory : ScriptableObject
{
    public List<Tool> toolList = new List<Tool>();
}
