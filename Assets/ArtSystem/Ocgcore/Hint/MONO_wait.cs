using UnityEngine;
using System.Collections;

public class MONO_wait : MonoBehaviour {
    public UILabel lab;
    string s = "";
	// Use this for initialization
	void Start ()
    {
        s = InterString.Get("等待对方行动中");
	}
    float a = 0;
	// Update is called once per frame
	void Update () {
        a += Time.deltaTime;
        string t = "";
        for (int i = 0; i < (((int)(a * 60)) / 20) % 4; i++)
        {
            t += InterString.Get("…");
        }
        lab.text = t + s + t;
    }
}
