using UnityEngine;
using System.Collections;

public class smartSelection : MonoBehaviour {
    public UILabel name_lable;

    public UITexture under_;

    public BoxCollider collider;

    public GameObject obj;

    public void setName(string s)       
    {
        name_lable.text = s;
    }

    public void setSelected(bool s) 
    {
        under_.color = s ? Color.white : (new Color(0, 0, 0, 0));
    }

    public string getNmae() 
    {
        return name_lable.text;
    }

    public void set_width(int width)        
    {
        int w = width + 80;
        under_.width = w;
        collider.size = new Vector3(w, under_.height + 5, 1);
        collider.center = new Vector3(0, -2.5f, 0);
        obj.transform.localPosition = new Vector3(0, 0, 0);
    }

    public static void IselectedSetter(GameObject obj, bool selected)
    {
        smartSelection smartSelection = obj.GetComponent<smartSelection>();
        if (smartSelection != null)
        {
            smartSelection.setSelected(selected);
        }
    }
}
