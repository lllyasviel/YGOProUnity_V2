using UnityEngine;
using System.Collections;

public class blinding : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.gameObject.GetComponent<Renderer>().material.SetFloat("_Alpha", (1f + Mathf.Sin(Time.time*10)) / 2f);
    }
}
