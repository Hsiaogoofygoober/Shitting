using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Tool", menuName = "Inventory/New Tool")]
public class Tool : ScriptableObject
{
    public string toolName;
    public Sprite toolImage;

    [TextArea]
    public string toolInfo;

}
