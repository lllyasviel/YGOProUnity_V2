using UnityEngine;
using System.Collections;
using System;
public class coiner : MonoBehaviour {
	int time;
	// Use this for initialization
	void Start () {
		
		gameObject.transform.localScale = Vector3.zero;
		gameObject.transform.eulerAngles = new Vector3(60, 0, 0);
		
		time = Program.TimePassed();
	
	}
	public void coin_app()
	{
		iTween.ScaleTo(gameObject, new Vector3(10, 10, 8.743f), 1f);
	}
	public void dice_app()
	{
		iTween.ScaleTo(gameObject, new Vector3(0.2f, 0.2f, 0.2f), 1f);
	}
	public void disapp()
	{
		iTween.ScaleTo(gameObject, new Vector3(0,0,0), 1f);
	}
	public void tocoin(bool up)
	{
		if (up)
		{
			iTween.RotateTo(gameObject, new Vector3(60, 0, 0) + (new Vector3(360, 0, 0)) * 15, 3f);
		}
		else
		{
            iTween.RotateTo(gameObject, new Vector3(240, 0, 0) + (new Vector3(360, 0, 0)) * 15, 3f);
		}
	}
	public void todice(int num)
	{
		switch (num)
		{
            case 1: iTween.RotateTo(gameObject, new Vector3(-60, 180, 180) + (new Vector3(360, 360, 360)) * 15, 3f);
				break;
            case 2: iTween.RotateTo(gameObject, new Vector3(40, 180, 180) + (new Vector3(360, 360, 360)) * 15, 3f);
				break;
            case 3: iTween.RotateTo(gameObject, new Vector3(-30, 0, -90) + (new Vector3(360, 360, 360)) * 15, 3f);
				break;
            case 4: iTween.RotateTo(gameObject, new Vector3(-30, 0, 90) + (new Vector3(360, 360, 360)) * 15, 3f);
				break;
            case 5: iTween.RotateTo(gameObject, new Vector3(-30, 0, 0) + (new Vector3(360, 360, 360)) * 15, 3f);
				break;
            case 6: iTween.RotateTo(gameObject, new Vector3(60, 0, 0) + (new Vector3(360, 360, 360)) * 15, 3f);
				break;

		}
	
	}
	// Update is called once per frame
	void Update () {
		if(Program.TimePassed()-time>4000){
			time = Program.TimePassed();
			Destroy(gameObject, 3000);
			iTween.ScaleTo(gameObject, Vector3.zero, 1f);
		}
	}
}
