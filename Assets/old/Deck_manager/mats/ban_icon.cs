using UnityEngine;
using System.Collections;

public class ban_icon : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void show(int i)
    {
        UITexture t = gameObject.GetComponent<UITexture>();
        if (t != null)
        {
            t.mainTexture = GameTextureManager.get("ban_" + i.ToString());
        }
        else
        {
            Renderer r = GetComponent<Renderer>();
            r.material.mainTexture = GameTextureManager.get("ban_" + i.ToString());
        }
    }
}
