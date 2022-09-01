using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using StarterAssets;

public class BulletProjectile : MonoBehaviour
{

    public int damage;
    Vector3 mPrevious;
    public string owner = "123";
    public ParticleSystem blood;

    void Start()
    {
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
                    Instantiate(blood, (transform.position + mPrevious) / 2, Quaternion.LookRotation((transform.position + mPrevious) / 2 - hits[i].transform.position));
                }
                Destroy(gameObject);
            }
            
        }
        Destroy(gameObject, 3);

        mPrevious = transform.position;
    }
    
}
