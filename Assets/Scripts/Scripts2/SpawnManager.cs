using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance;

    Spawnpoint[] spawnpoints;

    PhotonView pv;

    [SerializeField]
    public GameObject items;

    void Awake()
    {
        instance = this;
        pv = GetComponent<PhotonView>();
        spawnpoints = GetComponentsInChildren<Spawnpoint>();
    }

    void Start()
    {
        Weapon[] weapons = new Weapon[4];
        weapons[0] = new Weapon("PistolItem", 7);
        weapons[1] = new Weapon("RifleItem", 6);
        weapons[2] = new Weapon("ShotgunItem", 6);
        weapons[3] = new Weapon("SniperItem", 1);

        Ammo[] ammos = new Ammo[4];
        ammos[0] = new Ammo("PistolBullet", 20);
        ammos[1] = new Ammo("RifleBullet", 20);
        ammos[2] = new Ammo("ShotgunBullet", 20);
        ammos[3] = new Ammo("SniperBullet", 20);
        bool flag = true;
        int ammoCount = 12;

        int count = 0;
        if (PhotonNetwork.IsMasterClient) { 
            Debug.Log("一共有 " + spawnpoints.Length + " 個重生點");
            while (flag) 
            {
                int num1 = Random.Range(0, 1000)%4;
                int num2 = Random.Range(0, 1000)%4;
                if (weapons[num1].getAmount() > 0) 
                {
                    CreateGun(count, weapons[num1].getWeaponName());
                    weapons[num1].decreaceAmount();
                    count++;
                }

                int randomPos = Random.Range(0, 1000) % 20;
                if (ammoCount != 0) 
                {
                    if (ammos[num2].getAmount() > 0)
                    {
                        CreateAmmo(randomPos, ammos[num1].getAmmoName());
                        weapons[num1].decreaceAmount();
                        ammoCount++;
                    }
                }

                if (count == 20) 
                {
                    flag = false;
                }
            }
        }
        Debug.Log("一共產生 " + count + " 把槍");
    }

    void CreateGun(int index, string str) 
    {
        Vector3 pos = spawnpoints[index].transform.position;
        pos.y += 502;
        spawnpoints[index].transform.position = pos;
        Transform spawnpoint = spawnpoints[index].transform;
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Gun", str), spawnpoint.position, spawnpoint.rotation, 0, new object[] { pv.ViewID });
    }

    void CreateAmmo(int index, string str) 
    {
        Vector3 pos = spawnpoints[index].transform.position;
        pos.y += 502;
        pos.z += 40;
        spawnpoints[index].transform.position = pos;
        Transform spawnpoint = spawnpoints[index].transform;
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs/Ammo", str), spawnpoint.position, spawnpoint.rotation, 0, new object[] { pv.ViewID });
    }

    public Spawnpoint[] GetSpawnpoint()
    {
        return spawnpoints;
        // spawnpoints[Random.Range(0, spawnpoints.Length)].transform
    }
}
