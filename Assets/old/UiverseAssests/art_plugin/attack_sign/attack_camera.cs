using UnityEngine;
using System.Collections;
using System;

public class attack_camera : MonoBehaviour {
    public Vector3 from;
    public Vector3 to;
    int atk;
    GameObject target;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //target.transform.localPosition = get_over(to);
    }
    Vector3 get_over(Vector3 i)
    {
        Vector3 o = Vector3.zero;
        Vector3 scr = Camera.main.WorldToScreenPoint(i);
        //scr.z = 5f * 1800f / (float)atk;
        o = Camera.main.ScreenToWorldPoint(scr);
        return o;
    }
    public void set(Vector3 from_,Vector3 to_,int atk_)
    {
        from = from_;
        to = to_;
        atk = atk_;
        gameObject.transform.GetChild(1).localPosition = get_over(from);
        target = gameObject.transform.GetChild(0).gameObject;
        target.transform.localPosition = get_over(to);
    }
}
