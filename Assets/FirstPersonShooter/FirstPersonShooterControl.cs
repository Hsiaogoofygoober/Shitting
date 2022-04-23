using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class FirstPersonShooterControl : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    
    private StarterAssetsInputs starterAssetsInputs;
    private FirstPersonController firstPersonController;
    private void Awake()
    {
        firstPersonController = GetComponent<FirstPersonController>();
        
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
       
        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            firstPersonController.SetSensitivity(aimSensitivity);   
        }
        else {
            aimVirtualCamera.gameObject.SetActive(false);
            firstPersonController.SetSensitivity(normalSensitivity);       
        }
    }
}
