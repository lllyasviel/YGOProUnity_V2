using UnityEngine;
using System.Collections;
using System;

public class sorder : MonoBehaviour {
    int time;

	// Use this for initialization
	void Start () {
        time = Program.TimePassed();
		iTween.ScaleTo(gameObject,new Vector3(3,3,3),1);
	}
	
	// Update is called once per frame
	void Update () {
        int delta_time = Program.TimePassed() - time;

		//gameObject.transform.Rotate((new Vector3(0, 0, 1)) * Time.deltaTime * 50);

        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z + (float)(
            
            Math.Sin((float)delta_time / 150f) / 40f

            ));
	}
}
