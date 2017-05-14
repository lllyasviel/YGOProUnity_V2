using UnityEngine;
using System.Collections;

public class toolShift : MonoBehaviour {
    public GameObject ObjectMust = null;
    public GameObject ObjectOption = null;

    void Update()
    {
        if (Program.InputEnterDown)
        {
            if (ObjectMust.name == "input_")
            {
                UIInput input = UIHelper.getByName<UIInput>(ObjectMust, "input_");
                if (input != null)
                {
                    if (ObjectMust.transform.localPosition.y < 0)
                    {
                        shift();
                        input.isSelected = true;
                    }
                    else
                    {
                        if (input.isSelected)
                        {
                            if (input.value == "")
                            {
                                shift();
                                input.isSelected = false;
                            }
                        }
                        else
                        {
                            input.isSelected = true;
                        }
                    }
                }
            }
        }
    }

    public void shift()
    {
        Vector3 va = ObjectMust.transform.localPosition;
        if (ObjectOption == null)
        {
            if (va.y >= 0)
            {
                iTween.MoveToLocal(ObjectMust, new Vector3(va.x, -100, va.z), 0.6f);
            }
            else
            {
                iTween.MoveToLocal(ObjectMust, new Vector3(va.x, 0, va.z), 0.6f);
            }
        }
        else
        {
            Vector3 vb = ObjectOption.transform.localPosition;
            if (va.y > vb.y)
            {
                iTween.MoveToLocal(ObjectMust, new Vector3(va.x, -100, va.z), 0.6f);
                iTween.MoveToLocal(ObjectOption, new Vector3(vb.x, 0, vb.z), 0.6f);
            }
            else
            {
                iTween.MoveToLocal(ObjectMust, new Vector3(va.x, 0, va.z), 0.6f);
                iTween.MoveToLocal(ObjectOption, new Vector3(vb.x, -100, vb.z), 0.6f);
            }
        }
    }
}
