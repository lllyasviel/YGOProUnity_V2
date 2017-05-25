using UnityEngine;
using System.Collections;

public class UIDeckPanel : MonoBehaviour {
    public UILabel leftMain;
    public UILabel leftExtra;
    public UILabel leftSide;
    public UILabel rightMain;
    public UILabel rightExtra;
    public UILabel rightSide;
    public Transform cardsBeginner;
    public GameObject cardMod;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public UILabel[] labs = new UILabel[6];

    public cardPicLoader createCard()
    {
        GameObject obj = (GameObject)Instantiate(cardMod);
        obj.transform.SetParent(cardsBeginner);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localEulerAngles = Vector3.zero;
        var ret = obj.GetComponentInChildren<cardPicLoader>();
        ret.clear();
        return ret;
    }
}
