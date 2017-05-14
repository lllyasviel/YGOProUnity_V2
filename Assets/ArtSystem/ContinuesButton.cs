using UnityEngine;
using System.Collections;

public class ContinuesButton : MonoBehaviour {
    UIButton btn;
	// Use this for initialization
	void Start ()
    {
        btn = GetComponentInChildren<UIButton>();
        UIEventTrigger trigger= gameObject.GetComponentInChildren<UIEventTrigger>();
        if (trigger==null)  
        {
            trigger = gameObject.AddComponent<UIEventTrigger>();
        }
        trigger.onRelease.Add(new EventDelegate(this, "off"));
        trigger.onPress.Add(new EventDelegate(this, "on"));
    }

    void on()
    {
        isTrigging = true;
        time = 0;
    }

    void off()  
    {
        isTrigging = false;
        time = 0;
    }

    bool isTrigging = false;
    // Update is called once per frame
    float time = 0;
	void Update () {
        if (isTrigging) 
        {
            if (btn!=null)  
            {
                time += Time.deltaTime;
                if (time > 0.2f)
                {
                    time = 0;
                    EventDelegate.Execute(btn.onClick);
                }
            }
        }
    }
}
