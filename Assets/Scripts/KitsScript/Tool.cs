using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "New Tool", menuName = "Inventory/New Tool")]
public class Tool : MonoBehaviour
{
    public string toolName;
    public Sprite toolImage;
    public int toolValue;

    [TextArea]
    public string toolInfo;

}
