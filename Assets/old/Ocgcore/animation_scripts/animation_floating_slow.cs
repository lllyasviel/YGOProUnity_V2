using System;
using UnityEngine;
using System.Collections;

public class animation_floating_slow : MonoBehaviour
{
    float rad = 1;
    int time = 0;
    void Start()
    {
        rad = (float)UnityEngine.Random.Range(80, 120) / 100f;
        time = Program.TimePassed();
    }
    void Update()
    {
        //gameObject.transform.localPosition = new Vector3(0, (float)Math.Sin(((double)(Program.TimePassed() - time)) / 1000d * 3.1415926d * rad / 4) * rad / 4, 0);
    }

}
