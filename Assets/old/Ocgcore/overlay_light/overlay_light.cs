using UnityEngine;
using System.Collections;

public class overlay_light : MonoBehaviour {
    Vector3 v = Vector3.right;
    GameObject l;
    void Start()
    {
        try
        {
            l = gameObject.transform.Find("light").gameObject;
            v = new Vector3(get_rand(), get_rand(), get_rand());
            Vector3 chuizhi = (new Vector3(1, 1, -(v.x + v.y) / v.z)) / Vector3.Distance(Vector3.zero, new Vector3(1, 1, -(v.x + v.y) / v.z));
            l.transform.localPosition = chuizhi * 5;
        }
        catch (System.Exception e)  
        {
        }
    }
    float get_rand()
    {
        float r = ((float)Random.Range(-100f, 100f)) / 10f;
        if (r == 0)
        {
            r = 0.001f;
        }
        return r;
    }
    // Update is called once per frame
    void Update()
    {
        l.transform.RotateAround(gameObject.transform.position, v, 90 * Time.deltaTime);
    }
}
