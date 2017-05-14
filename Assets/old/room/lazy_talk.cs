using UnityEngine;
using System.Collections;

public class lazy_talk : MonoBehaviour {
    public UIInput input;
    Vector3 input_vector = new Vector3(-425,-100,0);
    public UIButton btn;
    Vector3 btn_vector = new Vector3(-131.4f, -13, 0);
	// Use this for initialization
	void Start () {
	
	}

    public void shift()
    {
        if (input_vector.y == -100)
        {
            input_vector.y = 0;
            btn_vector.y = -100;
        }
        else
        {
            input_vector.y = -100;
            btn_vector.y = -13;
        }
    }
	
	// Update is called once per frame
	void Update () {
        input.gameObject.transform.localPosition += (input_vector - input.gameObject.transform.localPosition) * 0.2f;
        btn.gameObject.transform.localPosition += (btn_vector - btn.gameObject.transform.localPosition) * 0.2f;
	}
}
