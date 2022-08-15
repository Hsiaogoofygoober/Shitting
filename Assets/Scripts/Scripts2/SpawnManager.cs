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
        Debug.Log("一共有 " + spawnpoints.Length + " 個重生點");
        int count = 0;
        for (int i = 0; i < 20; i++) 
        {
            count++;
            CreateGun(i);
        }

        Debug.Log("一共產生 " + count + " 把槍");
    }

    void CreateGun(int index) 
    {
        Vector3 pos = spawnpoints[index].transform.position;
        pos.y += 502;
        spawnpoints[index].transform.position = pos;
        Transform spawnpoint = spawnpoints[index].transform;
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ShotgunItem"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { pv.ViewID });
    }

    public Spawnpoint[] GetSpawnpoint()
    {
        return spawnpoints;
        // spawnpoints[Random.Range(0, spawnpoints.Length)].transform
    }
}
