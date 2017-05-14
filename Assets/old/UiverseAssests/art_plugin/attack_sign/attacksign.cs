using UnityEngine;
using System.Collections;
using System;

public class attacksign : MonoBehaviour {
	public Vector3 from;
	public Vector3 to;
	// Use this for initialization
	int time = 0;
	void Start () {
		gameObject.transform.position = from;
		gameObject.transform.LookAt(to);
		time = Program.TimePassed();
        gameObject.transform.localScale = new Vector3(5, 5, 5);
    }

    // Update is called once per frame
    void Update () {
        Vector3 from_ = get_over(from);
        Vector3 to_ = get_over(to);
        gameObject.transform.position = (from_ + to_) * 0.5f + (to_ - from_) * 0.5f * (float)Math.Sin(3.1415926f * (Program.TimePassed() - time) / 500);
    }


    Vector3 get_over(Vector3 i)
    {
        Vector3 o=Vector3.zero;
        Vector3 scr = Camera.main.WorldToScreenPoint(i);
        scr.z -= 4.5f;
        o = Camera.main.ScreenToWorldPoint(scr);
        return o;
    }
}
