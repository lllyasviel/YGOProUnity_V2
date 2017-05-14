using System;
using UnityEngine;
using System.Collections;

public class animation_floating_fast : MonoBehaviour
{
    int time_last=0;
	void Start () {
        int time_last = Program.TimePassed();
	}
	void Update () {
        int time_now = Program.TimePassed();
        float sin_last = (float)Math.Sin(((double)time_last) / 1000d * 3.1415926d * 2)/10;
        float sin_now = (float)Math.Sin(((double)time_now) / 1000d * 3.1415926d * 2)/10;
        gameObject.transform.position = new Vector3
            (
            gameObject.transform.position.x,
            gameObject.transform.position.y + sin_now - sin_last,
            gameObject.transform.position.z
            );
        time_last = time_now;
	}

}
