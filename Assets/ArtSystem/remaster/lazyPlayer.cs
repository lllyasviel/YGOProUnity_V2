using UnityEngine;
using System.Collections;
using System;

public class lazyPlayer : MonoBehaviour {

    public UIToggle prep;

    public UIButton prepAsButton;

    public UIButton prepAsCollider; 
        
    public UIButton kickAsButton;

    public UISprite prepAsTexture; 

    public UILabel UILabel_name;

    public UITexture face;

    public GameObject line;

    public Transform transformOfPrepFore;

    int me = 0;  

    public void ini()
    {
        me = int.Parse(gameObject.name);
        setIfprepared(false);
        setIfMe(false);
        SetNotNull(false);
        SetIFcanKick(false);
        setName("");
    }

    // Use this for initialization
    void Start ()
    {
        UIHelper.registEvent(prepAsButton, OnPrepClicked);
        UIHelper.registEvent(kickAsButton, OnKickClicked);
       // ini();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnPrepClicked()
    {
        if (onPrepareChanged != null)
        {
            onPrepareChanged(me, prep.value);
        }
    }

    void OnKickClicked()
    {
        Program.DEBUGLOG("OnKickClicked  " + me.ToString());
        if (onKick != null)
        {
            onKick(me);
        }
        //setIfMe(!getIfMe());//bb
    }

    bool canKick = false;

    public void SetIFcanKick(bool canKick_)
    {
        canKick = canKick_;
        if (canKick)
        {
            kickAsButton.gameObject.SetActive(true);
        }
        else
        {
            kickAsButton.gameObject.SetActive(false);
        }
    }

    public bool getIfcanKick()
    {
        return canKick;
    }

    bool NotNull = false;

    public void SetNotNull(bool notNull_)
    {
        NotNull = notNull_;
        if (NotNull)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            setIfprepared(false);
            transformOfPrepFore.localScale = Vector3.zero;
            transform.localScale = new Vector3(0, 0, 0);
        }
    }

    public bool getIfNull()
    {
        return NotNull;
    }

    bool mIfMe = false;

    public void setIfMe(bool isMe)
    {
        mIfMe = isMe;
        if (mIfMe)
        {
            line.SetActive(true);
            prepAsTexture.color = Color.white;
            prepAsCollider.isEnabled = true;
        }
        else
        {
            line.SetActive(false);
            prepAsTexture.color = Color.gray;
            prepAsCollider.isEnabled = false;
        }
    }

    public bool getIfMe()
    {
        return mIfMe;
    }

    string mName;

    public void setName(string name_)
    {
        mName = name_;
        UILabel_name.text = name_;
        try
        {
            face.mainTexture = UIHelper.getFace(name_);
        }
        catch (Exception)
        {
            Debug.LogError("setName");
        }
    }

    public string getName()
    {
        return mName;
    }

    public Action<int, bool> onPrepareChanged = null;   

    public Action<int> onKick = null;

    public void setIfprepared(bool preped)
    {
        prep.value = preped;
    }

    public bool getIfPreped()
    {
        return prep.value;
    }
}
