using UnityEngine;
public class OpenCardOnClick : MonoBehaviour
{
    void OnClick()
    {
        UILabel lbl = GetComponent<UILabel>();

        if (lbl != null)
        {
            try
            {
                string s = lbl.GetUrlAtPosition(UICamera.lastWorldPosition);
                if (string.IsNullOrEmpty(s))
                {
                    return;
                }
                int code = int.Parse(s);
                ((CardDescription)(Program.I().cardDescription)).setData(YGOSharp.CardsManager.Get(code), GameTextureManager.myBack,"",true);
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}