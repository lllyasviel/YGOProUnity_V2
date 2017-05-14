using UnityEngine;
using System.Collections;

public class faceShower : MonoBehaviour {

	void Start () {
        gameObject.transform.position = Program.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width/2,Screen.height*1.5f,0));
        iTween.MoveToAction(gameObject, Program.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height/2, 0)),0.6f,()=>{
            iTween.ScaleToAction(gameObject,Vector3.zero, 0.6f, () => {   
                Destroy(gameObject);
            }, 1f);
        },0);
	} 

}
