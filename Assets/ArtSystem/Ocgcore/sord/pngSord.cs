using UnityEngine;
using System.Collections;

public class pngSord : MonoBehaviour {

	// Use this for initialization
	void Start () {
        gameObject.GetComponent<Renderer>().material.mainTexture = GameTextureManager.attack;
        gameObject.transform.localScale = new Vector3(1, 1f / (float)GameTextureManager.attack.width * (float)GameTextureManager.attack.height, 1);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
