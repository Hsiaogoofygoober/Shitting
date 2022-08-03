using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using StarterAssets;

public class BulletProjectile : MonoBehaviour
{
    Rigidbody rb;
    float damage = 10;
    PhotonView pv;
    void Start()

    {

        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();

        StartCoroutine(Predict());

    }
    void FixedUpdate()

    {

        StartCoroutine(Predict());
        Destroy(gameObject, 3);

    }

    IEnumerator Predict()

    {

        Vector3 prediction = transform.position + rb.velocity * Time.fixedDeltaTime;

        RaycastHit hit2;

        int layerMask = ~LayerMask.GetMask("Bullet");

        //Debug.DrawLine(transform.position, prediction);




        if (Physics.Linecast(transform.position, prediction, out hit2, layerMask))

        {

            transform.position = hit2.point;

            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            rb.isKinematic = true;

            yield return 0;

            OnTriggerEnterFixed(hit2.collider);
            

        }

    }

    void OnTriggerEnterFixed(Collider other)
    {
        BulletProjectile bullet = gameObject.GetComponent<BulletProjectile>();
        if (other.gameObject.tag == "Player")
        {

            other.gameObject.GetComponent<FirstPersonController>().killer = bullet.pv.Owner.NickName;
            //pv.RPC("RPC_SendMessage", RpcTarget.Others, bullet.pv.Owner.NickName, other.gameObject.GetComponent<playerName>().name);
            //CallRpcSendMessageToOthers(pv.GetComponent<playerName>().name, other.gameObject.GetComponent<playerName>().name);
            Debug.Log("·F§A®Q" + other.gameObject.GetComponent<playerName>().name);
        }

        //if (other.CompareTag("Target"))

        
        //Debug.Log(bullet.pv.Owner.NickName);
        other.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);
        
        Destroy(gameObject);
    }

    //[PunRPC]
    //void RPC_SendMessage(string msg1, string msg2, Collider other, PhotonMessageInfo info) 
    //{
    //    if (pv.GetComponent<playerName>().name != msg2)
    //        return;
        
    //    other.GetComponent<FirstPersonController>().killer = msg1;
    //} 
}
