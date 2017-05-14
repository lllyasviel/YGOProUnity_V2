using UnityEngine;
using System;

public class MonoDelegate : MonoBehaviour
{
    public Action actionInMono;
    public void function()
    {
        if (actionInMono != null) actionInMono();
    }
}


public class MonoListener : MonoBehaviour
{
    public Action<GameObject> actionInMono;
    public void function()
    {
        if (actionInMono != null) actionInMono(this.gameObject);
    }
}

public class MonoListenerRMS_ized : MonoBehaviour       
{
    public Action<GameObject, Servant.messageSystemValue> actionInMono;
    public Servant.messageSystemValue value;
    public void function()
    {
        UIInput input = GetComponent<UIInput>();
        if (input != null)
        {
            value = new Servant.messageSystemValue();
            value.hint = input.name;
            value.value = input.value;
        }
        if (actionInMono != null) actionInMono(this.gameObject, value);
    }
}

