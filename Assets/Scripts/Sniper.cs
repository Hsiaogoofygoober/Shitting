using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using System.IO;
using StarterAssets;
using UnityEngine;
using TMPro;

public class Sniper : Gun
{
    private StarterAssetsInputs starterAssetsInputs;
    //bullet 
    public GameObject bullet;

    //bullet force
    public float shootForce, upwardForce;

    //Gun stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    public int sniperAmmo;
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
    public ParticleSystem muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    PhotonView PV;

    //bug fixing :D
    public bool allowInvoke = true;

    // aimming
    [SerializeField] private GameObject aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    public float Sensitivity = 1;

    private void Awake()
    {


        PV = GetComponent<PhotonView>();

        //make sure magazine is full
        bulletsLeft = magazineSize;
        readyToShoot = true;
        //Debug.Log("gun id: " + PV.ViewID);

    }



    public override void Use()
    {
        
        
        if (fpsCam == null)
        {
            fpsCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        }
        if (aimVirtualCamera == null)
        {
            aimVirtualCamera = GameObject.FindWithTag("SniperAim");
        }
        if (starterAssetsInputs == null)
        {
            starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
        }

        Aimming();
        MyInput();

        //Set ammo display, if it exists :D
        if (ammunitionDisplay == null)
        {
            ammunitionDisplay = GameObject.FindWithTag("weaponMessage").GetComponent<TextMeshProUGUI>();
        }
        else
        {
            ammunitionDisplay.SetText("sniper ammo: \n" + bulletsLeft / bulletsPerTap + " / " + sniperAmmo / bulletsPerTap);
        }

    }

    public void SetSensitivity(float newSensitivity)
    {
        GetComponentInParent<FirstPersonController>().Sensitivity = newSensitivity;
        Sensitivity = newSensitivity;

    }

    private void Aimming()
    {
        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.GetComponent<CinemachineVirtualCamera>().Priority = 20;
            SetSensitivity(aimSensitivity);
        }
        else
        {
            aimVirtualCamera.GetComponent<CinemachineVirtualCamera>().Priority = 5;
            SetSensitivity(normalSensitivity);
        }
    }
    private void MyInput()
    {
        //Check if allowed to hold down button and take corresponding input
        shooting = starterAssetsInputs.shoot;

        if (starterAssetsInputs.shoot)
        {
            Debug.Log("tap");
        }
        //Reloading 
        if (starterAssetsInputs.reload && bulletsLeft < magazineSize && !reloading && sniperAmmo>0) Reload();
        //Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0 && sniperAmmo > 0) Reload();

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
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction


        //Add forces to bullet
        if (starterAssetsInputs.aim)
        {
            ShootWithoutSpread(directionWithoutSpread);
            //currentBullet.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * shootForce, ForceMode.Impulse);
        }
        else
        {
            ShootWithSpread(directionWithSpread);
            //currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        }

        //currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

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
        if (sniperAmmo >= magazineSize)
        {
            sniperAmmo -= magazineSize;
            bulletsLeft = magazineSize;
        }
        else
        {
            bulletsLeft = sniperAmmo;
            sniperAmmo = 0;
        }
        reloading = false;
    }
    private void ShootWithoutSpread(Vector3 directionWithoutSpread)
    {
        PV.RPC("RPC_Shoot", RpcTarget.All,directionWithoutSpread);
    }
    private void ShootWithSpread(Vector3 directionWithSpread)
    {
        PV.RPC("RPC_Shoot", RpcTarget.All,directionWithSpread);
    }

    [PunRPC]

    void RPC_Shoot(Vector3 directionWithSpread)
    {
        GameObject currentBullet = Instantiate(bullet,attackPoint.position, Quaternion.identity);
        Instantiate(muzzleFlash, attackPoint.position, Quaternion.LookRotation(directionWithSpread));
        currentBullet.transform.forward = directionWithSpread.normalized;
        currentBullet.GetComponent<BulletProjectile>().owner = PV.Owner.NickName;
        Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
        rb.AddRelativeForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        //PhotonView.Find(BulletID).GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
    }
}
