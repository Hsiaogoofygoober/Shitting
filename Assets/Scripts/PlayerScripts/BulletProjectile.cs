using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using StarterAssets;

public class BulletProjectile : MonoBehaviour
{
    Rigidbody rb;
    public float damage;
    PhotonView PV;
    BulletProjectile bullet;
    int ownerID;

    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        PV = this.gameObject.GetComponent<PhotonView>();
        bullet = gameObject.GetComponent<BulletProjectile>();

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
        //if (other.CompareTag("Target"))
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);
            other.gameObject.GetComponent<FirstPersonController>().killer = bullet.PV.Owner.NickName;
            if (bullet.PV.Owner.NickName == "") 
            {
                Debug.Log("幹你娘耖機掰");
            }
            Debug.Log(bullet.PV.Owner.NickName + "是開槍的");
        }
        Destroy(gameObject);
        Debug.Log("should destroy");
    }
}
