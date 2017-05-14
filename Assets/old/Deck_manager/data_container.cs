//using UnityEngine;
//using System.Collections;

//public class data_container : MonoBehaviour {
//    public CardData data;
//    public CLIENT_SERVANT_DECKMANAGER father;
//    public GameObject event_obj;
//	// Use this for initialization
//	void Start () {
//        event_obj=gameObject.transform.FindChild("under_button").gameObject;
//	}
//	bool pre_left=false;
//    bool pre_right = false;
//	// Update is called once per frame
//    void Update()
//    {
//        if(father.is_siding==false){
//            if (father.father.pointed_game_object == event_obj && father.drag_card == null)
//            {
//                father.change_data(data);
//                if (father.father.left_mouse_button_is_down == true)
//                {
//                    if (father.is_draging == false)
//                    {
//                        father.is_draging = true;
//                        father.begin_drag(data.code);
//                    }
//                }
//                //pc
//                if (pre_left == true && Input.GetMouseButton(0) == false)
//                {
//                    father.rush_to_main(data);
//                }
//                if (pre_right == true && Input.GetMouseButton(1) == false)
//                {
//                    father.rush_to_side(data);
//                }
//                pre_left = Input.GetMouseButton(0);
//                pre_right = Input.GetMouseButton(1);
//            }
//            else
//            {
//                pre_left = false;
//                pre_right = false;
//            }
//        }
//    }
//}
