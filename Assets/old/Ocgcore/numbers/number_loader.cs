using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class number_loader : MonoBehaviour {

    public Sprite[] sps = new Sprite[60];
    public GameObject mod;
	// Use this for initialization
	void Start () 
    {
	
	}

    List<GameObject> obj_nums = new List<GameObject>();
    Color ccc = Color.white;
    int n = -1;
    public void set_number(int number,int color)
    {
        if (n!=number)
        {
            n = number;
            string number_strig = number.ToString();
            for (int i = 0; i < obj_nums.Count; i++)
            {
                iTween.ScaleTo(obj_nums[i], Vector3.zero, 0.6f);
                Destroy(obj_nums[i], 0.6f);
            }
            obj_nums.Clear();
            if (number > -1)
            {
                for (int i = 0; i < number_strig.Length; i++)
                {
                    if (color * 10 + int.Parse(number_strig[i].ToString()) < 60)
                    {
                        Sprite sprite = sps[color * 10 + int.Parse(number_strig[i].ToString())];
                        GameObject obj = (GameObject)MonoBehaviour.Instantiate(mod, Vector3.zero, Quaternion.identity);
                        obj.transform.SetParent(gameObject.transform, false);
                        obj.GetComponent<SpriteRenderer>().sprite = sprite;
                        obj.transform.localPosition = new Vector3(-(float)number_strig.Length / 2f + 0.5f + i, 0, 0);
                        obj.transform.localScale = Vector3.zero;
                        iTween.ScaleTo(obj, new Vector3(1, 1, 1), 0.6f);
                        obj_nums.Add(obj);
                    }
                }
            }
            set_color(ccc);
        }
    }

    public void set_color(Color c)
    {
        ccc = c;
        //var sps=GetComponentsInChildren<SpriteRenderer>();
        //for (int i = 0; i < sps.Length;i++ )
        //{
        //    Color aaa = c;
        //    aaa.a = 0.8f;
        //    sps[i].color = aaa;
        //}
    }
	
	// Update is called once per frame
	void Update () 
    {
	
	}
}
