//using UnityEngine;
//using System.Collections;
//using System;

//public class good_list : MonoBehaviour {
//   public Camera cam;
//   public PictureLoader pic_loader;
//   public CLIENT_SERVANT_DECKMANAGER father;
//   public delegate void refresh_function();

//   public refresh_function onchange;

//	// Use this for initialization
//	void Start () {
	
//	}
//    Vector3 pre_vector = Vector3.zero;
//	// Update is called once per frame
//    int pre_time = 0;
//	void Update () {
//        if (Program.TimePassed() - pre_time>500)
//        {
//            pre_time = Program.TimePassed();
//            if (gameObject.transform.position != pre_vector)
//            {
//                Vector3 up = Vector3.zero;
//                Vector3 down = Vector3.zero; ;
//                if (cam != null)
//                {
//                    up = cam.ScreenToWorldPoint(new Vector3(0, Screen.height + 200, 0));
//                    down = cam.ScreenToWorldPoint(new Vector3(0, -200, 0));
//                }
//                for (int i = 0; i < transform.childCount; i++)
//                {
//                    GameObject obj = transform.GetChild(i).gameObject;
//                    if (obj.transform.position.y > down.y && obj.transform.position.y < up.y)
//                    {
//                        obj.GetComponent<pic_shower>().show_picture(pic_loader);
//                    }
//                }
//            }
//        }
//        if (gameObject.transform.position != pre_vector)
//        {
//            Vector3 up=Vector3.zero;
//            Vector3 down = Vector3.zero; ;
//            if (cam!=null)
//            {
//                up = cam.ScreenToWorldPoint(new Vector3(0,Screen.height+200,0));
//                down = cam.ScreenToWorldPoint(new Vector3(0, -200, 0));
//            }
//            for (int i = 0; i < transform.childCount;i++ )
//            {
//                GameObject obj = transform.GetChild(i).gameObject;
//                if (obj.transform.position.y > down.y && obj.transform.position.y<up.y)
//                {
//                    obj.SetActive(true);
//                    obj.GetComponent<pic_shower>().show_picture(pic_loader);
//                    try
//                    {
//                        obj.transform.FindChild("ban").GetComponent<ban_icon>().show(father.check_lflist(father.code_list_to_print[father.now_index].code));
//                    }
//                    catch (Exception)
//                    {
//                    }
                   
//                }
//                else
//                {
//                    obj.SetActive(false);
//                }
//            }
//            pre_vector = gameObject.transform.position;
//            onchange();
//        }
//    }
//}
