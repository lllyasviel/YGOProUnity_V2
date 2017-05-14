using UnityEngine;
using System.Collections;

public class UImouseHint : MonoBehaviour {
    public GameObject point;
    public Camera cam;
	// Use this for initialization
	void Start () {
        try
        {
            cam = gameObject.transform.GetComponentInParent<UIRoot>().GetComponentInChildren<Camera>();
        }
        catch (System.Exception)
        {
            
        }
	}
	
	// Update is called once per frame
    void Update()
    {
        if (drag)
        {
            point.transform.position = cam.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x+5,
                Input.mousePosition.y-25,0
                ));
        }
    }


    bool drag = false;
    public void begin()
    {
        drag = true;
        point.SetActive(true);
    }

    public void end()
    {
        drag = false;
        point.SetActive(false);
    }
}
