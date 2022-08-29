using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using StarterAssets;

public class BulletProjectile : MonoBehaviour
{
    Rigidbody rb;
    public float damage;
    Vector3 mPrevious;
    BulletProjectile bullet;
    int ownerID;
    public string owner = "123";

    void Start()
    {
        /*rb = this.gameObject.GetComponent<Rigidbody>();
        bullet = gameObject.GetComponent<BulletProjectile>();

        StartCoroutine(Predict());*/
        mPrevious = transform.position;
    }

    private void Update()
    {
        
        RaycastHit[] hits = Physics.RaycastAll(new Ray(mPrevious, (transform.position - mPrevious).normalized), (transform.position - mPrevious).magnitude);
        Debug.Log(owner);
        for(int i = 0; i < hits.Length; i++)
        {
            if(hits[i].collider != null && !hits[i].collider.CompareTag("bullet"))
            {
                if (hits[i].collider.CompareTag("Player"))
                {
                    hits[i].collider.GetComponent<IDamageable>()?.TakeDamage(damage);
                    hits[i].collider.GetComponent<FirstPersonController>().killer = owner;
                }
                Destroy(gameObject);
            }
            
        }
        Destroy(gameObject, 3);

        mPrevious = transform.position;
    }
    /*void FixedUpdate()

    {

        StartCoroutine(Predict());
        Destroy(gameObject, 3);
    }*/

    /* IEnumerator Predict()

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
     }*/

    /*void OnTriggerEnterFixed(Collider other)
   {
       //if (other.CompareTag("Target"))
       if (other.CompareTag("Player"))
       {
           Debug.Log("----------shoot-----------");
           other.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);
           Debug.Log("----------takedamage-----------");
           //other.gameObject.GetComponent<FirstPersonController>().killer = bullet.PV.Owner.NickName;
           if (bullet.PV.Owner.NickName == "") 
           {
               Debug.Log("·F§A®QÓf¾÷ÙT");
           }            
       }
       Destroy(gameObject);
       Debug.Log("should destroy");
   }*/
}
