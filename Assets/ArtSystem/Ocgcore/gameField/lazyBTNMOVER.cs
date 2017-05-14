using UnityEngine;
using System.Collections;

public class lazyBTNMOVER : MonoBehaviour {

    public UILabel g1;
    public UILabel g2;
    public UILabel g3;
    public UILabel g4;
    public UILabel g5;
    public UILabel g6;

    public void shift(bool ifnew)
    {
        if (ifnew)
        {
            g1.width = 50;
            g2.width = 50;
            g3.width = 50;
            g4.width = 50;
            g5.width = 50;
            g6.width = 50;
            g1.transform.localPosition = new Vector3(-258f, 1, 0);
            g2.transform.localPosition = new Vector3(-202.2f, 1, 0);
            g3.transform.localPosition = new Vector3(-37.6f, 1, 0);
            g4.transform.localPosition = new Vector3(19.6f, 1, 0);
            g5.transform.localPosition = new Vector3(189.2f, 1, 0);
            g6.transform.localPosition = new Vector3(243f, 1, 0);
            g3.text = "M1";
            g5.text = "M2";
        }
        else
        {
            g1.width = 84;
            g2.width = 84;
            g3.width = 84;
            g4.width = 84;
            g5.width = 84;
            g6.width = 84;
            g1.transform.localPosition = new Vector3(-238, 1, 0);
            g2.transform.localPosition = new Vector3(-140.2f, 1, 0);
            g3.transform.localPosition = new Vector3(-47.5f, 1, 0);
            g4.transform.localPosition = new Vector3(47.5f, 1, 0);
            g5.transform.localPosition = new Vector3(142.5f, 1, 0);
            g6.transform.localPosition = new Vector3(237.8f, 1, 0);
            g3.text = "MP1";
            g5.text = "MP2";
        }
    }
}
