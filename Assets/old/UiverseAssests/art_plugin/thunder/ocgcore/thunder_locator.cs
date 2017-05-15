using UnityEngine;
using System.Collections;

public class thunder_locator : MonoBehaviour {
    public bool needDestroy = false;
    public GameObject leftobj;
    public GameObject rightobj;
    private GameObject leftobj_left;
    private GameObject rightobj_right;
    public void set_objects(GameObject l,GameObject r)
    {
        leftobj = l;
        rightobj = r;
    }
	// Use this for initialization
	void Start () {
        leftobj_left = gameObject.transform.Find("left").gameObject;
        rightobj_right = gameObject.transform.Find("right").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        if (leftobj != null)
        {
            leftobj_left.transform.position = leftobj.transform.position;
        }
        if (rightobj != null)
        {
            rightobj_right.transform.position = rightobj.transform.position;
        }
	}
}
