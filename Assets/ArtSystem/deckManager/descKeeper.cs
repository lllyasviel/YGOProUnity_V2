using UnityEngine;
using System.Collections;

public class descKeeper : MonoBehaviour {
    public UITexture card;
    public UITexture back;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update () {
        if (back.width < card.width)
        {
            back.width = card.width + 2;
        }
        back.transform.localPosition = new Vector3(back.width / 2f, 0);
        Vector3 leftTop = new Vector3(-back.width / 2 + 2 + back.transform.localPosition.x, +back.height / 2 - 2 + back.transform.localPosition.y);
        card.transform.localPosition = new Vector3(leftTop.x + card.width / 2, leftTop.y - card.height / 2);
        Program.I().cardDescription.width = back.width-2;
        Program.I().cardDescription.cHeight = card.height;
    }
}
