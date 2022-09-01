using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageIndicator : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float lifeTime;
    public float minDist;
    public float maxDist;
    public float positionRate;
    public float scaleRate;

    private Vector3 iniPos;
    private Vector3 targetPos;
    private float timer;
    private Vector3 camera;
    private float distanceBetweenTwoPos;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main.transform.position;
        transform.LookAt(2 * transform.position - camera);
        
        iniPos = transform.position;
        float dist = Random.Range(minDist, maxDist);
        distanceBetweenTwoPos = Vector3.Distance(camera, transform.position);
        targetPos = iniPos + Vector3.up * distanceBetweenTwoPos * positionRate;
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        float fraction = lifeTime / 2;

        if (timer > lifeTime)
        {
            Destroy(gameObject);
        }
        else if(timer > fraction)
        {
            text.color = Color.Lerp(text.color, Color.clear, (timer - fraction)/(lifeTime-fraction));
        }

        transform.position = Vector3.Lerp(iniPos, targetPos, Mathf.Sin(timer/lifeTime));
        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * (2 + distanceBetweenTwoPos * scaleRate), Mathf.Sin(timer / lifeTime));
    }

    public void SetDamageText(int Damage)
    {
        text.text = Damage.ToString();
    }
}
