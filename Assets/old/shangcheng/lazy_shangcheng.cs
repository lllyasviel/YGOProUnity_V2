//using UnityEngine;
//using System.Collections;

//public class lazy_shangcheng : MonoBehaviour {
//    public GameObject mod_shangcheng;
//    public GameObject mod_dongxi;
//    GameObject things_1;
//    GameObject things_2;
//    GameObject things_3;
//    GameObject things_4;
//    public GameObject gird_1;
//    public GameObject gird_2;
//    public GameObject gird_3;
//    public GameObject gird_4;
//    public CLIENT client;
//    public UIButton exit_btn;
//	// Use this for initialization
//	void Start () {




//	}

//    public void ini()
//    {
//        things_1 = (GameObject)Instantiate(mod_shangcheng, Vector3.zero, Quaternion.identity);
//        things_2 = (GameObject)Instantiate(mod_shangcheng, Vector3.zero, Quaternion.identity);
//        things_3 = (GameObject)Instantiate(mod_shangcheng, Vector3.zero, Quaternion.identity);
//        things_4 = (GameObject)Instantiate(mod_shangcheng, Vector3.zero, Quaternion.identity);
//        things_1.transform.SetParent(gameObject.transform, false);
//        things_2.transform.SetParent(gameObject.transform, false);
//        things_3.transform.SetParent(gameObject.transform, false);
//        things_4.transform.SetParent(gameObject.transform, false);
//        things_1.transform.position = UICamera.mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 4, Screen.height / 2 + 230));
//        things_2.transform.position = UICamera.mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 4 * 3, Screen.height / 2 + 230));
//        things_3.transform.position = UICamera.mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 4, Screen.height / 2 - 45));
//        things_4.transform.position = UICamera.mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 4 * 3, Screen.height / 2 - 45));
//        things_1.transform.FindChild("Label").GetComponent<UILabel>().text = "背景音乐包";
//        things_2.transform.FindChild("Label").GetComponent<UILabel>().text = "卡片大图包";
//        things_3.transform.FindChild("Label").GetComponent<UILabel>().text = "卡背卡套包";
//        things_4.transform.FindChild("Label").GetComponent<UILabel>().text = "卡片特写包";

//        gird_1 = things_1.transform.FindChild("Scroll View").FindChild("UIGrid").gameObject;
//        gird_2 = things_2.transform.FindChild("Scroll View").FindChild("UIGrid").gameObject;
//        gird_3 = things_3.transform.FindChild("Scroll View").FindChild("UIGrid").gameObject;
//        gird_4 = things_4.transform.FindChild("Scroll View").FindChild("UIGrid").gameObject;
//    }
	
//	// Update is called once per frame
//	void Update () {
        
//	}

//    internal void shuabiao(System.Collections.Generic.List<SHANGCHENG.shangping> goods, GameObject gird)
//    {
//        gird.transform.DestroyChildren();
//        for (int i = 0; i < goods.Count; i++)
//        {
//            GameObject obj= client.create_game_object(client.loader.mod_shangping, Vector3.zero, Quaternion.identity,client.ui_main_2d);
//            obj.transform.SetParent(gird.transform, false);
//            obj.transform.localPosition = new Vector3(150 * i, 0, 0);
//            var lazy_thing=obj.GetComponent<lazy_thing>();
//            lazy_thing.client = client;
//            lazy_thing.shangcheng = shangceng;
//            lazy_thing.name.text = goods[i].name;
//            lazy_thing.value.text = goods[i].value.ToString()+"Dp";
//            //lazy_thing.pic.mainTexture = goods[i].pic;
//            lazy_thing.tip.text = goods[i].tip;
//            lazy_thing.shangping = goods[i];
//        }
//    }

//    public SHANGCHENG shangceng { get; set; }
//}
