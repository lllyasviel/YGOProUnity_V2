using UnityEngine;
using System.Collections;

public class UIselectableListItem : MonoBehaviour {
    public GameObject selectedObject;
    public UILabel lable;
    public UIButton btn;
    public UIselectableList List;
    public void goList()
    {
        if (List!=null) 
        {
            if (List.selectedString == lable.text)
            {
                if (List.selectedAction != null)
                {
                    List.selectedAction();
                }
            }
            else
            {
                List.selectedString = lable.text;
            }
            List.mark();
        }
    }
}
