using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using System.IO;
using StarterAssets;
using UnityEngine;
using TMPro;

public class Pistol : Gun
{
    private StarterAssetsInputs starterAssetsInputs;
    //bullet 
    public GameObject bullet;
    public int ammoPerBox;
    public int ammoInBag = 0;
    public int preAmmoInBag = 0;
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
    public ParticleSystem muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;
    PhotonView PV;

    //bug fixing :D
    public bool allowInvoke = true;

    // aimming
    [SerializeField] private GameObject aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    public GameObject AimPos;
    public GameObject OriginalPos;

    private void Awake()
    {


        PV = GetComponent<PhotonView>();
        
        //make sure magazine is full
        bulletsLeft = magazineSize;
        readyToShoot = true;
        Debug.Log("gun id: " + PV.ViewID);
        

    }

    

    public override void Use()
    {

        
        if (fpsCam == null)
        {
            fpsCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        }
        if(aimVirtualCamera == null)
        {
            aimVirtualCamera = GameObject.FindWithTag("Aim");
        }
        if(starterAssetsInputs == null)
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
            ammunitionDisplay.SetText("pistol ammo: \n" + bulletsLeft / bulletsPerTap + " / " + (GetComponentInParent<FirstPersonController>().pistolAmmo)/bulletsPerTap);
        }

    }

    public void SetSensitivity(float newSensitivity)
    {
        GetComponentInParent<FirstPersonController>().Sensitivity = newSensitivity;

    }
    private void Aimming()
    {
        if (starterAssetsInputs.aim && GetComponent<StateReset>().isAimming)
        {
            GetComponentInParent<FirstPersonController>().constraint.data.target = GetComponentInParent<FirstPersonController>().PistolAimPos.transform;
            GetComponentInParent<FirstPersonController>().rigBuilder.Build();
            aimVirtualCamera.GetComponent<CinemachineVirtualCamera>().Priority = 20;
            SetSensitivity(aimSensitivity);
            GetComponent<StateReset>().isAimming = false;
            GetComponent<StateReset>().notAimming = true;
        }
        else if(!starterAssetsInputs.aim && GetComponent<StateReset>().notAimming)
        {
            transform.localPosition = OriginalPos.transform.localPosition;
            transform.localRotation = OriginalPos.transform.localRotation;
            transform.localScale = OriginalPos.transform.localScale;
            GetComponentInParent<FirstPersonController>().constraint.data.target = GetComponentInParent<FirstPersonController>().PistolInitPos.transform;
            GetComponentInParent<FirstPersonController>().rigBuilder.Build();
            aimVirtualCamera.GetComponent<CinemachineVirtualCamera>().Priority = 5;
            SetSensitivity(normalSensitivity);
            GetComponent<StateReset>().isAimming = true;
            GetComponent<StateReset>().notAimming = false;
        }
    }
    private void MyInput()
    {
        //Check if allowed to hold down button and take corresponding input
        shooting = starterAssetsInputs.shoot;


        //Reloading 
        if (starterAssetsInputs.reload && bulletsLeft < magazineSize && !reloading && GetComponentInParent<FirstPersonController>().pistolAmmo > 0) Reload();
        //Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0 && GetComponentInParent<FirstPersonController>().pistolAmmo>0) Reload();

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
            ShootRPC(directionWithoutSpread);
        }
        else
        {
            
            
            ShootRPC(directionWithSpread);
        }


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

    private void CheckAmmoInBag()
    {
        if (GetComponentInParent<FirstPersonController>().pistolAmmo % ammoPerBox != 0)
        {
            ammoInBag = GetComponentInParent<FirstPersonController>().pistolAmmo / ammoPerBox + 1;
            
        }
        else
        {
            ammoInBag = GetComponentInParent<FirstPersonController>().pistolAmmo / ammoPerBox;
        }
        Debug.Log("ammo In Bag : " + ammoInBag);
        Debug.Log("ammo In pre Bag : " + preAmmoInBag);
        if (ammoInBag < preAmmoInBag)
        {
            Debug.Log("ammo In two Bag : " + ammoInBag + " , " + preAmmoInBag);
            PlayerPrefs.SetInt("RefreshBag", 1);
        }

        preAmmoInBag = ammoInBag;

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
        if (GetComponentInParent<FirstPersonController>().pistolAmmo >= magazineSize)
        {
            GetComponentInParent<FirstPersonController>().pistolAmmo -= magazineSize;
            bulletsLeft = magazineSize;
        }
        else
        {
            bulletsLeft = GetComponentInParent<FirstPersonController>().pistolAmmo;
            GetComponentInParent<FirstPersonController>().pistolAmmo = 0;  
        }
        CheckAmmoInBag();
        reloading = false;
    }
    private void ShootRPC(Vector3 directionWithoutSpread)
    {
        PV.RPC("RPC_Shoot", RpcTarget.All,directionWithoutSpread);
    }


    [PunRPC]

    void RPC_Shoot(Vector3 directionWithSpread) 
    {
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        Instantiate(muzzleFlash, attackPoint.position, Quaternion.LookRotation(directionWithSpread));
        currentBullet.transform.forward = directionWithSpread.normalized;
        currentBullet.GetComponent<BulletProjectile>().owner = PV.Owner.NickName;
        Debug.Log(PV.Owner.NickName);
        Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
        rb.AddRelativeForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        //PhotonView.Find(BulletID).GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
    }

}
