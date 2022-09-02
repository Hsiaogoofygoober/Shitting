using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kits : MonoBehaviour 
{

    public string toolName;

    public Sprite toolImage;

    public string toolInfo; 

    //public string Name
    //{
    //    get
    //    {
    //        return "HealthPortion";
    //    }
    //}

    //public Sprite _Image = null;

    //public Sprite Image
    //{
    //    get
    //    {
    //        return _Image;
    //    }
    //}

    //public void OnUse()
    //{
    //    gameObject.SetActive(false);
    //}

    //public void OnPickup()
    //{
    //    gameObject.SetActive(false);
    //}

    //public void OnDrop() 
    //{
    //    Vector3 vector = new Vector3(0, 0, 0);
    //    RaycastHit hit = new RaycastHit();
    //    Ray ray = Camera.main.ScreenPointToRay(vector);
    //    if (Physics.Raycast(ray, out hit, 1000)) 
    //    {
    //        gameObject.SetActive(true);
    //        gameObject.transform.position = hit.point;
    //    }
    //}
}
