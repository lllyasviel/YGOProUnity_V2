using UnityEngine;
using System.Collections;

public class spriteChanger : MonoBehaviour {
    public UITexture toBeChangedUITexture;
    public Texture2D[] texts = new Texture2D[6];
    public void change(int i)
    {
        if(i>0){
            if (i < texts.Length)
            {
                toBeChangedUITexture.mainTexture=texts[i];
            }
        }
    }
}
