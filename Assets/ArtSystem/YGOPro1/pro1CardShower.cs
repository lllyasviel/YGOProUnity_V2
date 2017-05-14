using UnityEngine;
using System.Collections;

public class pro1CardShower : MonoBehaviour {
    public UITexture card;
    public UITexture mask;
    public UITexture disable;
    public TweenAlpha t;

    public void run()
    {
        mask.gameObject.transform.localPosition = new Vector3(-140, 0, 0);
        iTween.MoveToLocal(mask.gameObject, new Vector3(140, 0, 0), 0.7f);
    }

    public void Dis()
    {
        iTween.ScaleTo(disable.gameObject, new Vector3(1, 1, 1), 0.7f);
        t.duration = 0.7f;
        t.enabled = true;
    }
}
