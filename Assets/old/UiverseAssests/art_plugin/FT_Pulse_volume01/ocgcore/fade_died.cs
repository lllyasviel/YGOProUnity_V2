using UnityEngine;
using System.Collections;

public class fade_died : MonoBehaviour {
    public float time = 3;
    public Vector3 scale = new Vector3(1,1,1);
	// Use this for initialization
	void Start () {
        gameObject.transform.localScale = Vector3.zero;
        iTween.ScaleTo(gameObject, scale, 0.6f);
        iTween.ScaleTo(gameObject, iTween.Hash(
                           "delay",time,
                           "x", 0,
                           "y", 0,
                           "z", 0,
                           "time", 0.6f
                           ));
        Destroy(gameObject, time + 0.6f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
