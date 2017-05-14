using UnityEngine;
using System.Collections;

public class arrow : MonoBehaviour {
    public float speed = 1;
    public Transform from;
    public Transform to;
    public RendMega mega;
    public MegaShapeArc arc;
    // Use this for initialization
    void Start ()
    {
        updateSpeed();
    }

    public void updateSpeed()
    {
        var list = GetComponentsInChildren<AnimUnit>();
        foreach (var item in list)
        {
            item.m_fspeed = speed;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
