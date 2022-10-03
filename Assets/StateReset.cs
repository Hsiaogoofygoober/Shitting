using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateReset : MonoBehaviour
{
    public bool isAimming = true;
    public bool notAimming = true;

    public void ResetState()
    {
        isAimming = true;
        notAimming = true;
    }
}
