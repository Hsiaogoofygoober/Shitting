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
    public GameObject AimPos;
    public GameObject OriginalPos;

    //audio
    [SerializeField] private AudioSource shootSoundEffect;
    [SerializeField] private AudioSource reloadSoundEffect;

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
        if (starterAssetsInputs.aim && GetComponent<StateReset>().isAimming)
        {
            AimRPC();
            GetComponentInParent<FirstPersonController>().GunCrosshair.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            aimVirtualCamera.GetComponent<CinemachineVirtualCamera>().Priority = 20;
            SetSensitivity(aimSensitivity);
            GetComponent<StateReset>().isAimming = false;
            GetComponent<StateReset>().notAimming = true;
        }
        else if (!starterAssetsInputs.aim && GetComponent<StateReset>().notAimming)
        {
            transform.localPosition = OriginalPos.transform.localPosition;
            transform.localRotation = OriginalPos.transform.localRotation;
            transform.localScale = OriginalPos.transform.localScale;
            GetComponentInParent<FirstPersonController>().GunCrosshair.SetActive(true);
            GetComponentInParent<FirstPersonController>().GunCrosshair.transform.localScale = new Vector3(1, 1, 1);
            NotAimRPC();
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
        if (starterAssetsInputs.reload && bulletsLeft < magazineSize && !reloading && GetComponentInParent<FirstPersonController>().shotgunAmmo > 0) Reload();
        //Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0 && GetComponentInParent<FirstPersonController>().shotgunAmmo > 0) Reload();

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0 && GetComponentInParent<FirstPersonController>().canUse && !starterAssetsInputs.openbag)
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
        Vector3 directionWithoutSpread = ray.GetPoint(75) - attackPoint.position;

        //Calculate spread
        

        //Calculate new direction with spread
        Vector3[] directionWithSpread = new Vector3[ShotgunBulletPerTap];
        for(int i = 0; i < ShotgunBulletPerTap; i++) 
        {
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);
            directionWithSpread[i] = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction
        }

        int[] bulletID = new int[ShotgunBulletPerTap];
        //Add forces to bullet
        if (starterAssetsInputs.aim)
        {
            AimShootRPC(ray.GetPoint(75), attackPoint.position);
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
        PlayerPrefs.SetInt("RefreshBag", 3);
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
        reloadSoundEffect.Play();
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
        reloading = false; 
    }
    private void ShootRPC(Vector3[] directionWithSpread, Vector3 directionWithoutSpread)
    {
        shootSoundEffect.Play();
        PV.RPC("RPC_Shoot", RpcTarget.All, directionWithSpread, directionWithoutSpread);
    }
    private void AimShootRPC(Vector3 target, Vector3 attackPoint)
    {
        shootSoundEffect.Play();
        PV.RPC("RPC_AimShoot", RpcTarget.All, target, attackPoint);

    }
    private void NotAimRPC()
    {
        PV.RPC("RPC_NotAim", RpcTarget.All);
    }
    private void AimRPC()
    {
        PV.RPC("RPC_Aim", RpcTarget.All);
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
            currentBullet.GetComponent<BulletProjectile>().account = PlayerPrefs.GetString("Account");
            currentBullet.GetComponent<BulletProjectile>().pid = PV.GetComponentInParent<FirstPersonController>().Pid;
            Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
            rb.AddForce(directionWithSpread[i].normalized * shootForce, ForceMode.Impulse);
        }
    }
    [PunRPC]
    void RPC_AimShoot(Vector3 target, Vector3 attackPoint)
    {
        Vector3 directionWithoutSpread = target - attackPoint;
        Vector3[] directionWithlittleSpread = new Vector3[ShotgunBulletPerTap];
        //calculate spread bullet
        Random.InitState(42);
        for (int i = 0; i < ShotgunBulletPerTap; i++)
        {
            float x = Random.Range(-spread / 2, spread / 2);
            float y = Random.Range(-spread / 2, spread / 2);
            directionWithlittleSpread[i] = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction
        }
        //fire
        Instantiate(muzzleFlash, attackPoint, Quaternion.LookRotation(directionWithoutSpread));
        for (int i = 0; i < ShotgunBulletPerTap; i++)
        {
            GameObject currentBullet = Instantiate(bullet, attackPoint, Quaternion.identity);
            currentBullet.transform.forward = directionWithlittleSpread[i].normalized;
            currentBullet.GetComponent<BulletProjectile>().owner = PV.Owner.NickName;
            currentBullet.GetComponent<BulletProjectile>().account = PlayerPrefs.GetString("Account");
            currentBullet.GetComponent<BulletProjectile>().pid = PV.GetComponentInParent<FirstPersonController>().Pid;
            Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
            rb.AddForce(directionWithlittleSpread[i].normalized * shootForce, ForceMode.Impulse);
        }

    }

    [PunRPC]

    void RPC_NotAim()
    {
        GetComponentInParent<FirstPersonController>().constraint.data.target = GetComponentInParent<FirstPersonController>().PistolInitPos.transform;
        GetComponentInParent<FirstPersonController>().rigBuilder.Build();
    }

    [PunRPC]

    void RPC_Aim()
    {
        GetComponentInParent<FirstPersonController>().constraint.data.target = GetComponentInParent<FirstPersonController>().PistolAimPos.transform;
        GetComponentInParent<FirstPersonController>().rigBuilder.Build();
    }
}
