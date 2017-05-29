﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class gameUIbutton
{
    public string hashString;
    public GameObject gameObject;
    public int response;
}

public class gameInfo : MonoBehaviour
{
    public UITexture instance_btnPan;
    public UITextList instance_lab;    
    public GameObject line;

    //public UITextList hinter;

    barPngLoader me;

    barPngLoader opponent;

    public GameObject mod_healthBar;

    public float width = 0;

    int lastTickTime = 0;

    // Use this for initialization
    void Start()
    {
        ini();
    }

    public void ini()
    {
        Update();
    }

    public bool swaped = false;

    void Update()
    {
        if (me == null || opponent == null)
        {
            me = ((GameObject)MonoBehaviour.Instantiate(mod_healthBar, new Vector3(1000, 0, 0), Quaternion.identity)).GetComponent<barPngLoader>();
            me.transform.SetParent(gameObject.transform);
            opponent = ((GameObject)MonoBehaviour.Instantiate(mod_healthBar, new Vector3(1000, 0, 0), Quaternion.identity)).GetComponent<barPngLoader>();
            opponent.transform.SetParent(gameObject.transform);
            
            Transform[] Transforms = me.GetComponentsInChildren<Transform>();
            foreach (Transform child in Transforms)
            {
                child.gameObject.layer = gameObject.layer;
            }
            Transforms = opponent.GetComponentsInChildren<Transform>();
            foreach (Transform child in Transforms)
            {
                child.gameObject.layer = gameObject.layer;
            }
            Color c;
            ColorUtility.TryParseHtmlString(Config.Getui("gameChainCheckArea.fadecolor").TrimStart('#'), out c);
            UIHelper.getByName<UISprite>(UIHelper.getByName<UIToggle>(gameObject, "ignore_").gameObject, "Background").color = c;
            UIHelper.getByName<UISprite>(UIHelper.getByName<UIToggle>(gameObject, "watch_").gameObject, "Background").color = c;
        }
        float k = ((float)(Screen.width - Program.I().cardDescription.width)) / 1200f;
        if (k > 1.2f)
        {
            k = 1.2f;
        }
        if (k <0.8f)
        {
            k = 0.8f;
        }
        Vector3 ks = new Vector3(k, k, k);
        float kb = ((float)(Screen.width - Program.I().cardDescription.width)) / 1200f;
        if (kb > 1.2f)
        {
            kb = 1.2f;
        }
        if (kb < 0.73f)
        {
            kb = 0.73f;
        }
        Vector3 ksb = new Vector3(kb, kb, kb);
        instance_btnPan.gameObject.transform.localScale = ksb;
        opponent.transform.localScale = ks;
        me.transform.localScale = ks;
        if (!swaped) 
        {
            opponent.transform.localPosition = new Vector3(Screen.width / 2-14, Screen.height / 2 - 14);
            me.transform.localPosition = new Vector3(Screen.width / 2 - 14, Screen.height / 2 - 14 - k * (float)(opponent.under.height));
        }
        else
        {
            me.transform.localPosition = new Vector3(Screen.width / 2 - 14, Screen.height / 2 - 14);
            opponent.transform.localPosition = new Vector3(Screen.width / 2-14, Screen.height / 2 - 14 - k * (float)(opponent.under.height));
        }

        float height = 100 + 50 * (HashedButtons.Count);
        if (HashedButtons.Count==0)  
        {
            height = 80;
        }
         width = (150 * kb) + 15f;
        float localPositionPanX = (((float)Screen.width - 150 * kb) / 2) - 15f;
        float localPositionPanY = 0;
        float localPositionPanY_ = instance_btnPan.transform.localPosition.y + (localPositionPanY - instance_btnPan.transform.localPosition.y) * 0.2f;
        instance_btnPan.height += (int)(((float)height - (float)instance_btnPan.height) * 0.2f);
        instance_btnPan.transform.localPosition = new Vector3(localPositionPanX, localPositionPanY_, 0);
        instance_lab.transform.localPosition = new Vector3(Screen.width/2-315, -Screen.height / 2+90, 0);
        for (int i = 0; i < HashedButtons.Count; i++)
        {
            if (HashedButtons[i].gameObject != null)
            {
                HashedButtons[i].gameObject.transform.localPosition += (new Vector3(0, height / 2 - 110 - i * 50, 0) - HashedButtons[i].gameObject.transform.localPosition) * Program.deltaTime * 10f;
            }
        }
        if (Program.TimePassed() - lastTickTime > 1000)
        {
            lastTickTime = Program.TimePassed();
            tick();
        }
    }

    List<gameUIbutton> HashedButtons = new List<gameUIbutton>();

    public void addHashedButton(string hashString_, int hashInt, superButtonType type, string hint)
    {
        gameUIbutton hashedButton = new gameUIbutton();
        string hashString = hashString_;
        if (hashString == "")
        {
            hashString = hashInt.ToString();
        }
        hashedButton.hashString = hashString;
        hashedButton.response = hashInt;
        hashedButton.gameObject = Program.I().create(Program.I().new_ui_superButtonTransparent);
        UIHelper.trySetLableText(hashedButton.gameObject, "hint_", hint);
        UIHelper.getRealEventGameObject(hashedButton.gameObject).name = hashString + "----" + hashInt.ToString();
        UIHelper.registUIEventTriggerForClick(hashedButton.gameObject, listenerForClicked);
        hashedButton.gameObject.GetComponent<iconSetForButton>().setTexture(type);
        hashedButton.gameObject.GetComponent<iconSetForButton>().setText(hint);
        Transform[] Transforms = hashedButton.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in Transforms)
        {
            child.gameObject.layer = instance_btnPan.gameObject.layer;
        }
        hashedButton.gameObject.transform.SetParent(instance_btnPan.transform,false);
        hashedButton.gameObject.transform.localScale = Vector3.zero;
        hashedButton.gameObject.transform.localPosition= Vector3.zero;
        hashedButton.gameObject.transform.localEulerAngles = Vector3.zero;
        iTween.ScaleTo(hashedButton.gameObject, new Vector3(0.9f, 0.9f, 0.9f), 0.3f);
        HashedButtons.Add(hashedButton);
        refreshLine();
    }

    void listenerForClicked(GameObject obj)
    {
        string[] mats = obj.name.Split("----");
        if (mats.Length == 2)
        {
            for (int i = 0; i < HashedButtons.Count; i++)
            {
                if (HashedButtons[i].hashString == mats[0])
                {
                    if (HashedButtons[i].response.ToString() == mats[1])
                    {
                        Program.I().ocgcore.ES_gameUIbuttonClicked(HashedButtons[i]);
                    }
                }
            }
        }
    }

    public bool queryHashedButton(string hashString)
    {
        for (int i = 0; i < HashedButtons.Count; i++)
        {
            if (HashedButtons[i].hashString == hashString)
            {
                return true;
            }
        }
        return false;
    }

    public void removeHashedButton(string hashString)
    {
        gameUIbutton remove = null;
        for (int i = 0; i < HashedButtons.Count; i++)
        {
            if (HashedButtons[i].hashString == hashString)
            {
                remove = HashedButtons[i];
            }
        }
        if (remove != null)
        {
            if (remove.gameObject != null)
            {
                Program.I().destroy(remove.gameObject, 0.6f, true);
            }
            HashedButtons.Remove(remove);
        }
        refreshLine();
    }

    public void removeAll()
    {
        if (HashedButtons.Count == 1)
        {
            if (HashedButtons[0].hashString == "swap")
            {
                return;
            }
        }
        for (int i = 0; i < HashedButtons.Count; i++)
        {
            if (HashedButtons[i].gameObject != null)
            {
                Program.I().destroy(HashedButtons[i].gameObject, 0.6f, true);
            }
        }
        HashedButtons.Clear();
        refreshLine();
    }

    void refreshLine()
    {
        line.SetActive(HashedButtons.Count > 0);
    }

    int[] time = new int[2];

    bool[] isTicking = new bool[2];

    public void setTime(int player, int t)
    {
        if (player < 2)
        {
            time[player] = t;
            setTimeAbsolutely(player, t);
            isTicking[player] = true;
            isTicking[1 - player] = false;

        }
    }

    public void setExcited(int player)
    {
        if (player == 0)
        {
            me.under.mainTexture = GameTextureManager.exBar;
            opponent.under.mainTexture = GameTextureManager.bar;
        }
        else
        {
            opponent.under.mainTexture = GameTextureManager.exBar;
            me.under.mainTexture = GameTextureManager.bar;
        }
    }


    public void setTimeStill(int player)    
    {
        time[0] = Program.I().ocgcore.timeLimit;
        time[1] = Program.I().ocgcore.timeLimit;
        setTimeAbsolutely(0, Program.I().ocgcore.timeLimit);
        setTimeAbsolutely(1, Program.I().ocgcore.timeLimit);
        isTicking[0] = false;
        isTicking[1] = false;
        if (player == 0)
        {
            me.under.mainTexture = GameTextureManager.exBar;
            opponent.under.mainTexture = GameTextureManager.bar;
        }
        else
        {
            opponent.under.mainTexture = GameTextureManager.exBar;
            me.under.mainTexture = GameTextureManager.bar;
        }
        me.api_timeHint.text = "paused";
        opponent.api_timeHint.text = "paused";
    }

    public bool amIdanger()
    {
        return time[0] < Program.I().ocgcore.timeLimit / 3;
    }

    void setTimeAbsolutely(int player, int t)
    {
        if (player == 0)
        {
            me.api_timeHint.text = t.ToString() + "/" + Program.I().ocgcore.timeLimit.ToString();
            opponent.api_timeHint.text = "waiting";
            UIHelper.clearITWeen(me.api_healthBar.gameObject);
            iTween.MoveToLocal(me.api_timeBar.gameObject, new Vector3((float)(me.api_timeBar.width) - (float)t / (float)Program.I().ocgcore.timeLimit * (float)(me.api_timeBar.width), me.api_healthBar.gameObject.transform.localPosition.y, me.api_healthBar.gameObject.transform.localPosition.z), 1f);
        }
        if (player == 1)
        {
            opponent.api_timeHint.text = t.ToString() + "/" + Program.I().ocgcore.timeLimit.ToString();
            me.api_timeHint.text = "waiting";
            UIHelper.clearITWeen(opponent.api_healthBar.gameObject);
            iTween.MoveToLocal(opponent.api_timeBar.gameObject, new Vector3((float)(opponent.api_timeBar.width) - (float)t / (float)Program.I().ocgcore.timeLimit * (float)(opponent.api_timeBar.width), opponent.api_healthBar.gameObject.transform.localPosition.y, opponent.api_healthBar.gameObject.transform.localPosition.z), 1f);
        }
    }

    public void realize()
    {
        me.api_healthHint.text = ((float)Program.I().ocgcore.life_0 > 0 ? Program.I().ocgcore.life_0 : 0).ToString();
        opponent.api_healthHint.text = ((float)Program.I().ocgcore.life_1 > 0 ? Program.I().ocgcore.life_1 : 0).ToString();
        me.api_name.text = Program.I().ocgcore.name_0_c;
        opponent.api_name.text = Program.I().ocgcore.name_1_c;
        me.api_face.mainTexture = UIHelper.getFace(Program.I().ocgcore.name_0_c);
        opponent.api_face.mainTexture = UIHelper.getFace(Program.I().ocgcore.name_1_c);
        iTween.MoveToLocal(me.api_healthBar.gameObject, new Vector3(
            (float)(me.api_healthBar.width) - getRealLife(Program.I().ocgcore.life_0) / ((float)Program.I().ocgcore.lpLimit) * (float)(me.api_healthBar.width),
            me.api_healthBar.gameObject.transform.localPosition.y,
            me.api_healthBar.gameObject.transform.localPosition.z), 1f);
        iTween.MoveToLocal(opponent.api_healthBar.gameObject, new Vector3(
            (float)(opponent.api_healthBar.width) - getRealLife(Program.I().ocgcore.life_1) / ((float)Program.I().ocgcore.lpLimit) * (float)(opponent.api_healthBar.width),
            opponent.api_healthBar.gameObject.transform.localPosition.y,
            opponent.api_healthBar.gameObject.transform.localPosition.z), 1f);
        instance_lab.Clear();
        if (Program.I().ocgcore.confirmedCards.Count>0)
        {
            instance_lab.Add(GameStringHelper.yijingqueren);
        }
        foreach (var item in Program.I().ocgcore.confirmedCards)    
        {
            instance_lab.Add(item);
        }
    }

    static float getRealLife(float in_)
    {
        if (in_ < 0)
        {
            return 0;
        }
        if (in_ > Program.I().ocgcore.lpLimit)
        {
            return Program.I().ocgcore.lpLimit;
        }
        return in_;
    }

    void tick()
    {
        if (isTicking[0])
        {
            if (time[0] > 0)
            {
                time[0]--;
            }
            if (amIdanger())  
            {
                if (Program.I().ocgcore != null)
                {
                    Program.I().ocgcore.dangerTicking();
                }
            }
            setTimeAbsolutely(0, time[0]);
        }
        if (isTicking[1])
        {
            if (time[1] > 0)
            {
                time[1]--;
            }
            setTimeAbsolutely(1, time[1]);
        }
    }

    public bool ignoreChain()
    {
        return UIHelper.getByName<UIToggle>(gameObject, "ignore_").value;
    }

    public bool keepChain()
    {
        return UIHelper.getByName<UIToggle>(gameObject, "watch_").value;
    }

    public void ignoreChain_set(bool val)
    {
        try
        {
            UIHelper.getByName<UIToggle>(gameObject, "ignore_").value = val;
        }
        catch (Exception)
        {
        }
    }

    public void keepChain_set(bool val)
    {
        try
        {
            UIHelper.getByName<UIToggle>(gameObject, "watch_").value = val;
        }
        catch (Exception)
        {
        }
    }

    //public void ignoreChain_E()
    //{
    //    UIHelper.getByName<UIToggle>(gameObject, "ignore_").SwEt();
    //}

    //public void keepChain_E()   
    //{
    //    UIHelper.getByName<UIToggle>(gameObject, "watch_").SwEt();
    //}
}
