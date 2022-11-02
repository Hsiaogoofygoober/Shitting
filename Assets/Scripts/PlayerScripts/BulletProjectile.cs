using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using StarterAssets;

public class BulletProjectile : MonoBehaviour
{

    public int damage;
    Vector3 mPrevious;
    public string owner;
    public ParticleSystem blood;
    public ParticleSystem HitPartical;

    public string account;

    void Start()
    {
        mPrevious = transform.position;
    }

    private void Update()
    {
        
        RaycastHit[] hits = Physics.RaycastAll(new Ray(mPrevious, (transform.position - mPrevious).normalized), (transform.position - mPrevious).magnitude);
        for(int i = 0; i < hits.Length; i++)
        {
            //Debug.DrawLine(mPrevious,transform.position);
            if (hits[i].collider != null && !hits[i].collider.CompareTag("bullet"))
            {
                if (hits[i].collider.CompareTag("Player"))
                {
                    Debug.Log("hit => " + hits[i].point);
                    hits[i].collider.GetComponent<IDamageable>()?.TakeDamage(damage);
                    //PlayerPrefs.SetInt("SlotID", slotID);
                    hits[i].collider.GetComponent<FirstPersonController>().killer = owner;
                    hits[i].collider.GetComponent<FirstPersonController>().killerAccount = PlayerPrefs.GetString("Account");
                    Instantiate(blood, (transform.position + mPrevious) / 2, Quaternion.LookRotation((transform.position + mPrevious) / 2 - hits[i].transform.position));
                    if (HitPartical != null)
                    {
                        Instantiate(HitPartical, (transform.position + mPrevious) / 2, Quaternion.LookRotation((transform.position + mPrevious) / 2 - hits[i].transform.position));
                    }
                }
                Destroy(gameObject);
            }
            
        }
        Destroy(gameObject, 3);

        mPrevious = transform.position;
    }
    
}
