using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagControll : MonoBehaviour
{
    [SerializeField]
    public GameObject mybag;
    bool isOpen;

    void Update()
    {
        OpenMyBag();
    }
    public void OpenMyBag()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            isOpen = !isOpen;
            mybag.SetActive(isOpen);
        }
    }
}
