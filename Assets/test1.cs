using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class test1 : MonoBehaviour
{
   
    private StarterAssetsInputs starterAssetsInputs;
   
    private void Awake()
    {
       
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        Debug.Log(starterAssetsInputs.aim);

        
    }
}
