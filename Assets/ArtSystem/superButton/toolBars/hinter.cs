using UnityEngine;
using System.Collections;

public class hinter : MonoBehaviour {
    public string str;
    bool loaded = false;
    UIEventTrigger trigger = null;
	// Use this for initialization
	void Start () {
        trigger = GetComponent<UIEventTrigger>();
        if (trigger==null)
        {
            trigger = gameObject.AddComponent<UIEventTrigger>();
        }
        trigger.onHoverOver.Add(new EventDelegate(this, "in_"));
        trigger.onHoverOut.Add(new EventDelegate(this, "out_"));
        trigger.onPress.Add(new EventDelegate(this, "out_"));
    }

    GameObject obj; 

    void in_()
    {
        Vector3 screenPosition = Program.camera_main_2d.WorldToScreenPoint(gameObject.GetComponentInChildren<UITexture>().gameObject.transform.position);
        screenPosition.y += 45;
        screenPosition.z = 0;
        Vector3 worldPositin = Program.camera_main_2d.ScreenToWorldPoint(screenPosition);
        obj = Program.I().create(Program.I().mod_simple_ngui_text, worldPositin, Vector3.zero, true, Program.ui_main_2d, true);
        obj.GetComponent<UILabel>().text = str;
        obj.GetComponent<UILabel>().effectStyle = UILabel.Effect.Outline;
        Program.I().destroy(obj, 5f, false, false);
    }

    void out_()
    {
        if (obj!=null)  
        {
            Program.I().destroy(obj, 0.6f, true, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (loaded == false)
        {
            if (InterString.loaded == true)
            {
                loaded = true;
                str = InterString.Get(str);
            }
        }
    }
}
