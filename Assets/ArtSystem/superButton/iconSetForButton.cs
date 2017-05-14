using UnityEngine;
using System.Collections;

public enum superButtonType
{
    act, attack, bp,
    change, ep, mp,
    no, see, set,
    spsummon, summon, yes,
}

public class iconSetForButton : MonoBehaviour
{
    
    public UITexture UITextureInButton;
    public UILabel UILabelInButton;
    public Texture2D act;
    public Texture2D attack;
    public Texture2D bp;
    public Texture2D change;
    public Texture2D ep;
    public Texture2D mp;
    public Texture2D no;
    public Texture2D see;
    public Texture2D set;
    public Texture2D spsummon;
    public Texture2D summon;
    public Texture2D yes;
    public void setTexture(superButtonType type)
    {
        switch (type)   
        {
            case superButtonType.act:
                UITextureInButton.mainTexture = act;
                break;
            case superButtonType.attack:
                UITextureInButton.mainTexture = attack;
                break;
            case superButtonType.bp:
                UITextureInButton.mainTexture = bp;
                break;
            case superButtonType.change:
                UITextureInButton.mainTexture = change;
                break;
            case superButtonType.ep:
                UITextureInButton.mainTexture = ep;
                break;
            case superButtonType.mp:
                UITextureInButton.mainTexture = mp;
                break;
            case superButtonType.no:
                UITextureInButton.mainTexture = no;
                break;
            case superButtonType.see:
                UITextureInButton.mainTexture = see;
                break;
            case superButtonType.set:
                UITextureInButton.mainTexture = set;
                break;
            case superButtonType.spsummon:
                UITextureInButton.mainTexture = spsummon;
                break;
            case superButtonType.summon:
                UITextureInButton.mainTexture = summon;
                break;
            case superButtonType.yes:
                UITextureInButton.mainTexture = yes;
                break;
        }
        Color c;
        UnityEngine.Color.TryParseHexString(Config.Getui("gameButtonSign.color"), out c);
        UITextureInButton.color = c;
    }
    public void setText(string hint)
    {
        UILabelInButton.text = hint;
    }
}
