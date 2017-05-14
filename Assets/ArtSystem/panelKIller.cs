using UnityEngine;
using System.Collections;

public class panelKIller : MonoBehaviour {
    UIPanel pan;
    bool on = false;

    public void ini()
    {
        pan = GetComponentInChildren<UIPanel>();
    }

    public void set(bool r)
    {
        on = r;
        if (pan != null)
        {
            if (r)
            {
                pan.alpha = 0;
            }
        }
    }
    // Update is called once per frame
    void Update () {
        if (pan != null)
        {
            float to =0;
            if (on) 
            {
                to = 1;
            }
            if (Mathf.Abs(to - pan.alpha) > 0.1f)
            {
                pan.alpha += (to - pan.alpha) * Program.deltaTime * 18;
            }
            else
            {
                if (pan.alpha != to)
                {
                    pan.alpha = to;
                }
            }
        }
    }
}
