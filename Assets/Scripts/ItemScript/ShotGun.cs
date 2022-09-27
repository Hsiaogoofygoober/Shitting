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
    public int ShotgunBulletPerTap;
    public int ammoPerBox;
    public int ammoInBag = 0;
    public int preAmmoInBag = 0;

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
    public float Sensitivity = 1f;

    private void Awake()
    {

        PV = GetComponent<PhotonView>();
        //make sure magazine is full
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    public override void Use()
    { 
        if (fpsCam == null)
        {
            fpsCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        }
        if (aimVirtualCamera == null)
        {
            aimVirtualCamera = GameObject.FindWithTag("Aim");
        }
        if (starterAssetsInputs == null)
        {
            starterAssetsInputs = GetComponentInParent<StarterAssetsInputs>();
        }

        Aimming();
        MyInput();

        //Set ammo display, if it exists :D
        if(ammunitionDisplay == null)
        {
            ammunitionDisplay = GameObject.FindWithTag("weaponMessage").GetComponent<TextMeshProUGUI>();
        }
        else
        {
            ammunitionDisplay.SetText("shotgun ammo: \n" + bulletsLeft / bulletsPerTap + " / " + (GetComponentInParent<FirstPersonController>().shotgunAmmo) / bulletsPerTap);
        }
            

    }
    public void SetSensitivity(float newSensitivity)
    {
        GetComponentInParent<FirstPersonController>().Sensitivity = newSensitivity;

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


        //Reloading 
        if (starterAssetsInputs.reload && bulletsLeft < magazineSize && !reloading && GetComponentInParent<FirstPersonController>().shotgunAmmo > 0) Reload();
        //Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0 && GetComponentInParent<FirstPersonController>().shotgunAmmo > 0) Reload();

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
        Vector3[] directionWithSpread = new Vector3[ShotgunBulletPerTap];
        for(int i = 0; i < ShotgunBulletPerTap; i++) 
        {
            float x = Random.Range(-2*spread, 2*spread);
            float y = Random.Range(-2*spread, 2*spread);
            directionWithSpread[i] = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction
        }

        Vector3[] directionWithlittleSpread = new Vector3[ShotgunBulletPerTap];
        for (int i = 0; i < ShotgunBulletPerTap; i++)
        {
            float x = Random.Range(-spread/2, spread/2);
            float y = Random.Range(-spread/2, spread/2);
            directionWithlittleSpread[i] = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction
        }


        int[] bulletID = new int[ShotgunBulletPerTap];
        //Add forces to bullet
        if (starterAssetsInputs.aim)
        {
           ShootRPC(directionWithlittleSpread, directionWithoutSpread);           
        }
        else
        {
            ShootRPC(directionWithSpread, directionWithoutSpread);
        }

        bulletsLeft--;
        bulletsShot++;

        //Invoke resetShot function (if not already invoked), with your timeBetweenShooting
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

        }

        //if more than one bulletsPerTap make sure to repeat shoot function
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }
    private void CheckAmmoInBag()
    {
        if (GetComponentInParent<FirstPersonController>().shotgunAmmo % ammoPerBox != 0)
        {
            ammoInBag = GetComponentInParent<FirstPersonController>().shotgunAmmo / ammoPerBox + 1;

        }
        else
        {
            ammoInBag = GetComponentInParent<FirstPersonController>().shotgunAmmo / ammoPerBox;
        }
        Debug.Log("ammo In Bag : " + ammoInBag);
        Debug.Log("ammo In pre Bag : " + preAmmoInBag);
        if (ammoInBag < preAmmoInBag)
        {
            Debug.Log("ammo In two Bag : " + ammoInBag + " , " + preAmmoInBag);
            PlayerPrefs.SetInt("RefreshBag", 3);
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
        if (GetComponentInParent<FirstPersonController>().shotgunAmmo >= magazineSize)
        {
            GetComponentInParent<FirstPersonController>().shotgunAmmo -= magazineSize;
            bulletsLeft = magazineSize;
        }
        else
        {
            bulletsLeft = GetComponentInParent<FirstPersonController>().shotgunAmmo;
            GetComponentInParent<FirstPersonController>().shotgunAmmo = 0;
        }
        CheckAmmoInBag();
        reloading = false; ;
    }
    private void ShootRPC(Vector3[] directionWithSpread, Vector3 directionWithoutSpread)
    {
        PV.RPC("RPC_Shoot", RpcTarget.All,directionWithSpread, directionWithoutSpread);
    }
 

    [PunRPC]
    void RPC_Shoot(Vector3[] directionWithSpread, Vector3 directionWithoutSpread)
    {
        Instantiate(muzzleFlash, attackPoint.position, Quaternion.LookRotation(directionWithoutSpread));
        for (int i = 0; i < ShotgunBulletPerTap; i++)
        {
            GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
            currentBullet.transform.forward = directionWithSpread[i].normalized;
            currentBullet.GetComponent<BulletProjectile>().owner = PV.Owner.NickName;
            Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
            rb.AddRelativeForce(directionWithSpread[i].normalized * shootForce, ForceMode.Impulse);
        }
    }
}
