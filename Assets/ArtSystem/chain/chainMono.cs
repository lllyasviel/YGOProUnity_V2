using UnityEngine;
using System;

public class chainMono : MonoBehaviour {
    public Renderer circle;
    public TMPro.TextMeshPro text;
    public bool flashing = true;
    float all = 0;
    bool p = true;
    void Update()
    {
        if (flashing)   
        {
            all += Program.deltaTime;
            if (all>0.05)
            {
                all = 0;
                p = !p;
                if (p)
                {
                    circle.gameObject.SetActive(false);
                    text.gameObject.SetActive(false);
                }
                else
                {
                    circle.gameObject.SetActive(true);
                    text.gameObject.SetActive(true);
                }
            }
        }
    }
}
