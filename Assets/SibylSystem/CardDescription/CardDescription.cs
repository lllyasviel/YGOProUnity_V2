using UnityEngine;
using System;
using System.Collections.Generic;

public class CardDescription : Servant
{
    cardPicLoader picLoader;
    UIDragResize resizer;
    UITexture underSprite;
    UITexture picSprite;
    UISprite lineSprite;
    public UITextList description;
    GameObject line;
    public override void initialize()
    {
        gameObject = create
            (
            Program.I().new_ui_cardDescription,
            Program.camera_main_2d.ScreenToWorldPoint(new Vector3(-256, Screen.height /2, 600)),
            new Vector3(0, 0, 0),
            true,
            Program.ui_back_ground_2d
            );
        picLoader = gameObject.AddComponent<cardPicLoader>();
        picLoader.code = 0;
        picLoader.uiTexture = UIHelper.getByName<UITexture>(gameObject, "pic_");
        picLoader.loaded_code = -1;
        resizer = UIHelper.getByName<UIDragResize>(gameObject, "resizer");
        underSprite = UIHelper.getByName<UITexture>(gameObject, "under_");
        description = UIHelper.getByName<UITextList>(gameObject, "description_");
        line = UIHelper.getByName(gameObject, "line");
        UIHelper.registEvent(gameObject,"pre_", onPre);
        UIHelper.registEvent(gameObject, "next_", onNext); 
        UIHelper.registEvent(gameObject, "big_", onb);
        UIHelper.registEvent(gameObject, "small_", ons);
        picSprite = UIHelper.getByName<UITexture>(gameObject, "pic_");
        lineSprite = UIHelper.getByName<UISprite>(gameObject, "line");
        try
        {
            description.textLabel.fontSize = int.Parse(Config.Get("fontSize","24"));
        }
        catch (System.Exception e)
        {
        }
        read();
    }

    public float width = 0;
    public float cHeight = 0;

    void onb()
    {
        description.textLabel.fontSize += 1;
        description.Rebuild();
        Config.Set("fontSize", description.textLabel.fontSize.ToString());
    }

    void ons()
    {
        description.textLabel.fontSize -= 1;
        description.Rebuild();
        Config.Set("fontSize", description.textLabel.fontSize.ToString());
    }

    void onPre()
    {
        current--;
        loadData();
    }

    void onNext()
    {
        current++;
        loadData();
    }

    public override void applyHideArrangement()
    {
        if (gameObject != null)
        {
            underSprite.height = Screen.height + 4;
            iTween.MoveTo(gameObject, Program.camera_main_2d.ScreenToWorldPoint(new Vector3(-underSprite.width-20, Screen.height / 2, 0)), 1.2f);
            setTitle("");
            resizer.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public override void applyShowArrangement()
    {
        if (gameObject != null)
        {
            underSprite.height = Screen.height + 4;
            iTween.MoveTo(gameObject, Program.camera_main_2d.ScreenToWorldPoint(new Vector3(-2, Screen.height / 2, 0)), 1.2f);
            resizer.gameObject.GetComponent<BoxCollider>().enabled = true;
        }
    }

    public void read()
    {
        try
        {
            underSprite.width = int.Parse(Config.Get("CA","200"));
            picSprite.height = int.Parse(Config.Get("CB","100"));
        }
        catch (System.Exception e)
        {
        }
    }

    public void save()
    {
        Config.Set("CA", underSprite.width.ToString());
        Config.Set("CB", picSprite.height.ToString());
    }

    class data
    {
        public YGOSharp.Card card;
        public Texture2D def;
        public string tail = "";
    }

    int current = 0;

    List<data> datas = new List<data>();

    void loadData()
    {
        if (current < 0)
        {
            current = 0;
        }
        if (current > datas.Count - 1)
        {
            current = datas.Count - 1;
        }
        if (datas.Count == 0)
        {
            return;
        }
        data d = datas[current];
        apply(d.card, d.def, d.tail);
    }

    YGOSharp.Card currentCard = null;

    public bool ifShowingThisCard(YGOSharp.Card card)   
    {
        return currentCard == card;
    }

    private void apply(YGOSharp.Card card, Texture2D def, string tail)
    {
        if (card == null)
        {
            return;
        }
        string smallstr = "";
        if (card.Id != 0)
        {
            smallstr = GameStringHelper.getName(card) + GameStringHelper.getSmall(card);
            smallstr += "\n";
        }
        if (tail == "")
        {
            description.Clear();
            description.Add(smallstr + card.Desc);
        }
        else
        {
            description.Clear();
            description.Add(smallstr + "[FFD700]" + tail + "[-]" + card.Desc);
        }
        picLoader.code = card.Id;
        picLoader.defaults = def;
        picLoader.loaded_code = -1;
        currentCard = card;
    }

    public void setData(YGOSharp.Card card, Texture2D def, string tail = "")
    {
        if (card == null)
        {
            return;
        }
        if (card.Id == 0)
        {
            apply(card,def,tail);
            return;
        }
        if (datas.Count > 0)
        {
            if (datas[datas.Count - 1].card.Id == card.Id)
            {
                datas[datas.Count - 1] = new data
                {
                    card = card,
                    def = def,
                    tail = tail
                };
                if (datas.Count > 300)
                {
                    datas.RemoveAt(0);
                }
                current = datas.Count - 1;
                loadData();
                return;
            }
        }
        datas.Add(new data
        {
            card = card,
            def = def,
            tail = tail
        });
        if (datas.Count>300)    
        {
            datas.RemoveAt(0);
        }
        current = datas.Count - 1;
        loadData();
    }

    public void setTitle(string title)
    {
        UIHelper.trySetLableText(gameObject,"title_",title);
    }

    List<string> Logs = new List<string>();

    public void mLog(string result)
    {
        Logs.Add(result);
        string all = "";
        for (int i = 0; i < Logs.Count; i++)
        {
            if (i == Logs.Count - 1)
            {
                all += Logs[i].Replace("\0", "");
            }
            else
            {
                all += Logs[i].Replace("\0", "") + "\n";
            }
        }
        UIHelper.trySetLableTextList(UIHelper.getByName(gameObject, "chat_"), all);
        Program.go(8000, clearOneLog);
    }

    void clearOneLog()
    {
        if (Logs.Count>0)
        {
            Logs.RemoveAt(0);
            string all = "";
            foreach (var item in Logs)
            {
                all += item + "\n";
            }
            try
            {
                all = all.Substring(0, all.Length - 1);
            }
            catch (System.Exception e)
            {
            }
            UIHelper.trySetLableTextList(UIHelper.getByName(gameObject, "chat_"), all);
        }
        else
        {
            UIHelper.trySetLableTextList(UIHelper.getByName(gameObject, "chat_"), "");
        }

    }
    public void clearAllLog()
    {
        Program.notGo(clearOneLog);
        Logs.Clear();
        UIHelper.trySetLableTextList(UIHelper.getByName(gameObject, "chat_"), "");
    }
}
