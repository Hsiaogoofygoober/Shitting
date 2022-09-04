using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimetoLife : MonoBehaviour
{
    // Start is called before the first frame update
    public float AgeLimit;
    public float Age;

    // Update is called once per frame
    void Update()
    {
        if(Age > AgeLimit)
        {
            Destroy(gameObject);
            return;
        }
        Age += Time.deltaTime;
    }
}
