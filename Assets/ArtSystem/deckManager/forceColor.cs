using UnityEngine;
using System.Collections;

public class forceColor : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var tweens = GetComponentsInChildren<TweenColor>();
        foreach (var item in tweens)
        {
            DestroyImmediate(item);
        }
        Color c;
        ColorUtility.TryParseHtmlString(Config.Getui("allUI.color"), out c);
        var sprites = GetComponentsInChildren<UISprite>();
        foreach (var item in sprites)   
        {
            if (item.color.a > 0.1f)
            {
                item.color = c;
                try
                {
                    item.gameObject.GetComponentInParent<UIButton>().defaultColor = c;
                }
                catch (System.Exception)
                {
                }
            }
        }
        ColorUtility.TryParseHtmlString(Config.Getui("List.color"), out c);
        var lists = GetComponentsInChildren<UIPopupList>();
        foreach (var item in lists)
        {
            item.backgroundColor = c;
            try
            {
                item.gameObject.GetComponent<UISprite>().color = c;
                item.gameObject.GetComponent<UIButton>().defaultColor = c;
            }
            catch (System.Exception)
            {
            }
        }
        ColorUtility.TryParseHtmlString(Config.Getui("lable.color"), out c);
        var ls = GetComponentsInChildren<UILabel>();
        foreach (var item in ls)
        {
            item.applyGradient = true;
            item.gradientTop = c;
            item.gradientBottom = Color.gray;
        }
        ColorUtility.TryParseHtmlString(Config.Getui("lable.color.fadecolor"), out c);
        ls = GetComponentsInChildren<UILabel>();
        foreach (var item in ls)
        {
            item.gradientBottom = c;
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
