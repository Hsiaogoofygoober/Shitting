using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using System.IO;
using StarterAssets;
using UnityEngine;
using TMPro;

public class ShotGun : Gun
{
    private const int shotgun = 5;
    private StarterAssetsInputs starterAssetsInputs;
    //bullet 
    public GameObject bullet;

    //public float damage = 10;
    //bullet force
    public float shootForce, upwardForce;

    //Gun stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    int bulletsLeft, bulletsShot;

    //Recoil
    // public Rigidbody playerRb;
    public float recoilForce;

    //bools
    bool shooting, readyToShoot, reloading;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;

    //Graphics
    //public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;


    //bug fixing :D
    public bool allowInvoke = true;

    // aimming
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    public float Sensitivity = 1f;

    private void Awake()
    {
        starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
        Debug.Log("Awake");
        //make sure magazine is full
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    public override void Use()
    {
        Aimming();
        MyInput();

        //Set ammo display, if it exists :D
        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText("ammo left: \n" + bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);

    }
    public void SetSensitivity(float newSensitivity)
    {
        Sensitivity = newSensitivity;

    }
    private void Aimming()
    {
        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            SetSensitivity(normalSensitivity * aimSensitivity);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            SetSensitivity(normalSensitivity);
        }
    }
    private void MyInput()
    {
        //Check if allowed to hold down button and take corresponding input

        shooting = starterAssetsInputs.shoot;


        //Reloading 
        if (starterAssetsInputs.reload && bulletsLeft < magazineSize && !reloading) Reload();
        //Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            //Set bullets shot to 0
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Find the exact hit position using a raycast
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view
        RaycastHit hit;

        //check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75); //Just a point far away from the player

        //Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //Calculate spread
        

        //Calculate new direction with spread
        Vector3[] directionWithSpread = new Vector3[shotgun];
        for(int i = 0; i < shotgun; i++) 
        {
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);
            directionWithSpread[i] = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction
        }

        Vector3[] directionWithlittleSpread = new Vector3[shotgun];
        for (int i = 0; i < shotgun; i++)
        {
            float x = Random.Range(-spread/2, spread/2);
            float y = Random.Range(-spread/2, spread/2);
            directionWithlittleSpread[i] = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction
        }


        //Instantiate bullet/projectile
        GameObject[] currentBullet = new  GameObject[shotgun];
        
        for(int i = 0; i < shotgun; i++) 
        {
            currentBullet[i] = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", bullet.name), attackPoint.position, Quaternion.identity); //store instantiated bullet in currentBullet
            Debug.Log("ammo");                                                                                                                        //Rotate bullet to shoot direction
            currentBullet[i].transform.forward = directionWithSpread[i].normalized;
        }
       

        //Add forces to bullet
        if (starterAssetsInputs.aim)
        {
            for (int i = 0; i < shotgun; i++)
            {
                currentBullet[i].GetComponent<Rigidbody>().AddForce(directionWithlittleSpread[i].normalized * shootForce, ForceMode.Impulse);
            }
            
        }
        else
        {
            for(int i = 0; i < shotgun; i++) 
            {
                currentBullet[i].GetComponent<Rigidbody>().AddForce(directionWithSpread[i].normalized * shootForce, ForceMode.Impulse);
                Debug.Log("shoot");
            }
        }
        for (int i = 0; i < shotgun; i++)
        {
            currentBullet[i].GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);
        }
        

        //Instantiate muzzle flash, if you have one
        //if (muzzleFlash != null)
        //    Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot++;

        //Invoke resetShot function (if not already invoked), with your timeBetweenShooting
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            //Add recoil to player (should only be called once)
            //playerRb.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);
        }

        //if more than one bulletsPerTap make sure to repeat shoot function
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    private void ResetShot()
    {
        //Allow shooting and invoking again
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime); //Invoke ReloadFinished function with your reloadTime as delay
    }
    private void ReloadFinished()
    {
        //Fill magazine
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
