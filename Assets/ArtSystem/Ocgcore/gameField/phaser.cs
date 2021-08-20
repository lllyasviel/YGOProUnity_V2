using UnityEngine;
using System.Collections;
using System;

public class phaser : MonoBehaviour {

    public Action mp2Action;
    public Action bpAction;
    public Action epAction;

    public BoxCollider colliderMp2;
    public BoxCollider colliderBp;
    public BoxCollider colliderEp;

    public UILabel labDp;
    public UILabel labSp;
    public UILabel labMp1;  
    public UILabel labBp;
    public UILabel labMp2;
    public UILabel labEp;   

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Program.InputGetMouseButtonUp_0)
        {
            if (Program.pointedCollider== colliderMp2)  
            {
                if (mp2Action!=null)    
                {
                    mp2Action();
                }
            }
            if (Program.pointedCollider == colliderBp)
            {
                if (bpAction != null)
                {
                    bpAction();
                }
            }
            if (Program.pointedCollider == colliderEp)
            {
                if (epAction != null)
                {
                    epAction();
                }
            }
        }
	
	}
}
