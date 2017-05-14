using UnityEngine;
using System.Collections;

public class cage_dropper : MonoBehaviour {
    public GameObject obj1;
    public GameObject obj2;
	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (obj1 != null)
        {
            Destroy(obj1);
        }
        if (obj2 != null)
        {
            Destroy(obj2);
        }
    }
}
