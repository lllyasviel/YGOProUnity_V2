//using UnityEngine;
//using System.Collections;

//public class pic_shower : MonoBehaviour {
//    public UITexture sp;
//    PictureLoader pl;
//	// Use this for initialization
//	void Start () {
	
//	}
	
//	// Update is called once per frame
//    void Update()
//    {
//        if (pic_loading == true && pic_loaded==false)
//        {
//            Texture2D pic = pl.get(long.Parse(gameObject.name),ocgcore_picture_type.card_picture);
//            if (pic!=null)
//            {
//                sp.mainTexture = pic;
//                pic_loading = false;
//                pic_loaded = true;
//            }
//        }
//    }
//    bool pic_loading = false;
//    bool pic_loaded = false;
//    public void show_picture(PictureLoader loader)
//    {
//        pl = loader;
//        pic_loading = true;
//    }
//}
