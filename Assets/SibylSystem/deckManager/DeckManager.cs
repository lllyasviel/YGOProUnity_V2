using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YGOSharp.OCGWrapper.Enums;
public class DeckManager : ServantWithCardDescription
{
    #region UI

    public enum Condition
    {
        editDeck = 1,
        changeSide = 2,
    }

    public Condition condition = Condition.editDeck;

    public GameObject gameObjectSearch;

    public GameObject gameObjectDetailedSearch;

    UIPopupList UIPopupList_main;

    UIPopupList UIPopupList_ban;    

    UIPopupList UIPopupList_second;

    UIPopupList UIPopupList_race;

    UIPopupList UIPopupList_attribute;

    UIPopupList UIPopupList_pack;

    UIInput UIInput_level;

    UIInput UIInput_atk;

    UIInput UIInput_def;

    UIInput UIInput_search;

    UIToggle[] UIToggle_effects = new UIToggle[32];

    SuperScrollView superScrollView = null;

    UIPopupList UIPopupList_banlist;

    public override void initialize()
    {
        gameObjectSearch = create
            (
            Program.I().new_ui_search,
            Program.camera_back_ground_2d.ScreenToWorldPoint(new Vector3(Screen.width + 600, Screen.height / 2, 600)),
            new Vector3(0, 0, 0),
            false,
            Program.ui_back_ground_2d
            );
        gameObjectDetailedSearch = create
            (
            Program.I().new_ui_searchDetailed,
            Program.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height * 2f, 0)),
            new Vector3(0, 0, 0),
            false,
            Program.ui_main_2d
            );
        UIHelper.InterGameObject(gameObjectSearch);
        UIHelper.InterGameObject(gameObjectDetailedSearch);
        shiftCondition(Condition.editDeck);
        UIHelper.registEvent(gameObjectSearch, "detailed_", onClickDetail);
        UIHelper.registEvent(gameObjectSearch, "search_", onClickSearch);
        UIPopupList_main = UIHelper.getByName<UIPopupList>(gameObjectDetailedSearch, "main_");
        UIPopupList_ban = UIHelper.getByName<UIPopupList>(gameObjectDetailedSearch, "ban_");
        UIPopupList_second = UIHelper.getByName<UIPopupList>(gameObjectDetailedSearch, "second_");
        UIPopupList_race = UIHelper.getByName<UIPopupList>(gameObjectDetailedSearch, "race_");
        UIPopupList_attribute = UIHelper.getByName<UIPopupList>(gameObjectDetailedSearch, "attribute_");
        UIPopupList_pack = UIHelper.getByName<UIPopupList>(gameObjectDetailedSearch, "pack_");
        UIInput_search = UIHelper.getByName<UIInput>(gameObjectSearch, "input_");
        UIInput_search.value = "";
        UIInput_level = UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "level_");
        UIInput_atk = UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "atk_");
        UIInput_def = UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "def_");
        for (int i = 0; i < 32; i++)
        {
            UIToggle_effects[i] = UIHelper.getByName<UIToggle>(gameObjectDetailedSearch, "T (" + (i+1).ToString() + ")");
            UIHelper.trySetLableText(UIToggle_effects[i].gameObject, GameStringManager.get_unsafe(1100 + i));
            UIToggle_effects[i].GetComponentInChildren<UILabel>().overflowMethod = UILabel.Overflow.ClampContent;
        }
        UIPopupList_pack.Clear();
        UIPopupList_pack.AddItem(GameStringManager.get_unsafe(1310));
        foreach (var item in YGOSharp.PacksManager.packs)
        {
            UIPopupList_pack.AddItem(item.fullName);
        }
        UIPopupList_main.Clear();
        UIPopupList_main.AddItem(GameStringManager.get_unsafe(1310));
        UIPopupList_main.AddItem(GameStringManager.get_unsafe(1312));
        UIPopupList_main.AddItem(GameStringManager.get_unsafe(1313));
        UIPopupList_main.AddItem(GameStringManager.get_unsafe(1314));
        UIPopupList_ban.Clear();
        UIPopupList_ban.AddItem(GameStringManager.get_unsafe(1310));
        UIPopupList_ban.AddItem(GameStringManager.get_unsafe(1316));
        UIPopupList_ban.AddItem(GameStringManager.get_unsafe(1317));
        UIPopupList_ban.AddItem(GameStringManager.get_unsafe(1318));
        clearAll();
        UIHelper.registEvent(UIPopupList_main.gameObject, onUIPopupList_main);
        UIHelper.registEvent(UIPopupList_second.gameObject, onUIPopupList_second);
        superScrollView = new SuperScrollView
           (
           UIHelper.getByName<UIPanel>(gameObjectSearch, "panel_"),
           UIHelper.getByName<UIScrollBar>(gameObjectSearch, "bar_"),
           itemOnListProducer,
           86
           );
        Program.go(500, () => {
            List<MonoCardInDeckManager> cs = new List<MonoCardInDeckManager>();
            for (int i = 0; i < 300; i++)
            {
                cs.Add(createCard());
            }
            for (int i = 0; i < 300; i++)
            {
                destroyCard(cs[i]);
            }
        });


    }

    GameObject itemOnListProducer(string[] Args)
    {
        GameObject returnValue = null;
        returnValue = create(Program.I().new_ui_cardOnSearchList, Vector3.zero, Vector3.zero, false, Program.ui_back_ground_2d);
        UIHelper.getRealEventGameObject(returnValue).name = Args[0];
        UIHelper.trySetLableText(returnValue, Args[2]);
        cardPicLoader cardPicLoader_ = UIHelper.getRealEventGameObject(returnValue).AddComponent<cardPicLoader>();
        cardPicLoader_.code = int.Parse(Args[0]);
        cardPicLoader_.data = YGOSharp.CardsManager.Get(int.Parse(Args[0]));
        cardPicLoader_.uiTexture = UIHelper.getByName<UITexture>(returnValue, "pic_");
        cardPicLoader_.ico = UIHelper.getByName<ban_icon>(returnValue);
        cardPicLoader_.ico.show(3);
        return returnValue;
    }

    public override void applyHideArrangement()
    {
        base.applyHideArrangement();
        Program.cameraFacing = false;
        iTween.MoveTo(gameObjectSearch, Program.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width + 600, Screen.height / 2, 600)), 1.2f);
        refreshDetail();
    }

    public override void applyShowArrangement()
    {
        base.applyShowArrangement();
        Program.cameraFacing = true;
        UITexture tex = UIHelper.getByName<UITexture>(gameObjectSearch, "under_");
        tex.height = Screen.height;
        iTween.MoveTo(gameObjectSearch, Program.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width - tex.width / 2, Screen.height / 2, 0)), 1.2f);
        refreshDetail();
    }

    void onLF()
    {
        currentBanlist = YGOSharp.BanlistManager.GetByName(UIPopupList_banlist.value);
    }

    public void shiftCondition(Condition condition)
    {
        this.condition = condition;
        switch (condition)
        {
            case Condition.editDeck:
                UIHelper.setParent(gameObjectSearch, Program.ui_back_ground_2d);
                SetBar(Program.I().new_bar_editDeck, 0, 230);
                UIPopupList_banlist = UIHelper.getByName<UIPopupList>(toolBar, "lfList_");
                var banlistNames = YGOSharp.BanlistManager.getAllName();
                UIPopupList_banlist.items = banlistNames;
                UIPopupList_banlist.value = UIPopupList_banlist.items[0];
                currentBanlist = YGOSharp.BanlistManager.GetByName(UIPopupList_banlist.items[0]);
                UIHelper.registEvent(toolBar, "rand_", rand);
                UIHelper.registEvent(toolBar, "sort_", sort);
                UIHelper.registEvent(toolBar, "clear_", clear);
                UIHelper.registEvent(toolBar, "home_", home);
                UIHelper.registEvent(toolBar, "save_", () => { onSave(); });
                UIHelper.registEvent(toolBar, "lfList_", onLF);
                UIHelper.registEvent(toolBar, "copy_", onCopy);
                break;
            case Condition.changeSide:
                UIHelper.setParent(gameObjectSearch, Program.ui_main_2d);
                SetBar(Program.I().new_bar_changeSide, 0, 230);
                UIPopupList_banlist = null;
                UIHelper.registEvent(toolBar, "sort_", sort);
                UIHelper.registEvent(toolBar, "finish_", home);
                UIHelper.registEvent(toolBar, "input_", onChat);
                break;
            default:
                break;
        }
    }

    void onCopy()
    {
        string deckName = Config.Get("deckInUse", "miaowu");
        string newname = InterString.Get("[?]的副本", deckName);
        string newnamer = newname;
        int i = 1;
        while (File.Exists("deck/" + newnamer + ".ydk"))
        {
            newnamer = newname + i.ToString();
            i++;
        }
        RMSshow_input("onRename", InterString.Get("新的卡组名"), newnamer);
    }

    public override void ES_RMS(string hashCode, List<messageSystemValue> result) 
    {
        base.ES_RMS(hashCode, result);
        if (hashCode == "onRename")
        {
            string raw = Config.Get("deckInUse", "miaowu");
            Config.Set("deckInUse", result[0].value);
            if (onSave()) 
            {
                ((CardDescription)Program.I().cardDescription).setTitle(result[0].value);
            }
            else
            {
                Config.Set("deckInUse", raw);
            }
        }
    }

    public Action returnAction = null;

    public bool onSave()    
    {
        try
        {
            if (
           deck.IMain.Count <= 60
           &&
           deck.IExtra.Count <= 15
           &&
           deck.ISide.Count <= 15
           )
            {
                string deckInUse = Config.Get("deckInUse", "miaowu");
                if (canSave)
                {
                    ArrangeObjectDeck();
                    FromObjectDeckToCodedDeck(true);
                    string value = "#created by ygopro2\r\n#main\r\n";
                    for (int i = 0; i < deck.Main.Count; i++)
                    {
                        value += deck.Main[i].ToString() + "\r\n";
                    }
                    value += "#extra\r\n";
                    for (int i = 0; i < deck.Extra.Count; i++)
                    {
                        value += deck.Extra[i].ToString() + "\r\n";
                    }
                    value += "!side\r\n";
                    for (int i = 0; i < deck.Side.Count; i++)
                    {
                        value += deck.Side[i].ToString() + "\r\n";
                    }
                    System.IO.File.WriteAllText("deck/" + deckInUse + ".ydk", value, System.Text.Encoding.UTF8);
                }
                else
                {
                    string value = "#created by ygopro2\r\n#main\r\n";
                    for (int i = 0; i < deck.Deck_O.Main.Count; i++)
                    {
                        value += deck.Deck_O.Main[i].ToString() + "\r\n";
                    }
                    value += "#extra\r\n";
                    for (int i = 0; i < deck.Deck_O.Extra.Count; i++)
                    {
                        value += deck.Deck_O.Extra[i].ToString() + "\r\n";
                    }
                    value += "!side\r\n";
                    for (int i = 0; i < deck.Deck_O.Side.Count; i++)
                    {
                        value += deck.Deck_O.Side[i].ToString() + "\r\n";
                    }
                    System.IO.File.WriteAllText("deck/" + deckInUse + ".ydk", value, System.Text.Encoding.UTF8);
                }
                RMSshow_none(InterString.Get("卡组[?]已经被保存。", deckInUse));
                return true;
            }
            else
            {
                RMSshow_none(InterString.Get("卡组内卡片张数超过限制。"));
                return false;
            }
        }
        catch (Exception)
        {
            RMSshow_none(InterString.Get("卡组非法！"));
            return false;
        }
    }

    public void onChat()
    {
        Program.I().room.onSubmit(UIHelper.getByName<UIInput>(toolBar, "input_").value);
        UIHelper.getByName<UIInput>(toolBar, "input_").value = "";
    }

    void home()
    {
        if (returnAction != null)
        {
            returnAction();
        }
    }

    void sort()
    {
        //animationCameraPan();
        ArrangeObjectDeck();
        SortObjectDeck();
        ShowObjectDeck();
    }

    void rand()
    {
        //animationCameraPan();
        ArrangeObjectDeck();
        RandObjectDeck();
        ShowObjectDeck();
    }

    void clear()
    {
        var deckTemp = deck.getAllObjectCard();
        foreach (var item in deckTemp)  
        {
            try
            {
                UIHelper.clearITWeen(item.gameObject);
                var rid = item.gameObject.GetComponent<Rigidbody>();
                if (rid == null)
                {
                    rid= item.gameObject.AddComponent<Rigidbody>();
                }
                rid.AddForce(0.7f * (item.transform.position + new Vector3(0, 30 - Vector3.Distance(item.transform.position, Vector3.zero), 0)) / Program.deltaTime);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }

    bool detailShowed = false;

    void showDetail()
    {
        detailShowed = true;
        refreshDetail();
    }

    void hideDetail()
    {
        clearAll();
        detailShowed = false;
        refreshDetail();
    }


    bool detailPanelShiftedTemp = false;
    void shiftDetailPanel(bool dragged) 
    {
        detailPanelShiftedTemp = dragged;
        if (isShowed&&detailShowed) 
        {
            if (dragged)
            {
                gameObjectDetailedSearch.GetComponent<UITexture>().color = new Color(1,1,1,0.7f);
            }
            else
            {
                gameObjectDetailedSearch.GetComponent<UITexture>().color = Color.white;
            }
        }
    }


    void refreshDetail()
    {
        if (gameObjectDetailedSearch!=null) 
        {
            if (isShowed)
            {
                if (Screen.height < 700)
                {
                    gameObjectDetailedSearch.transform.localScale = new Vector3(Screen.height / 700f, Screen.height / 700f, Screen.height / 700f);
                    if (detailShowed)
                    {
                        gameObjectDetailedSearch.GetComponent<UITexture>().height = 700;
                        iTween.MoveTo(gameObjectDetailedSearch, Program.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width - 230 - 115f * Screen.height / 700f, Screen.height * 0.5f, 0)), 0.6f);
                        reShowBar(0, 230 + 230 * Screen.height / 700f);
                    }
                    else
                    {
                        gameObjectDetailedSearch.GetComponent<UITexture>().height = 700;
                        iTween.MoveTo(gameObjectDetailedSearch, Program.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width - 230 - 115f * Screen.height / 700f, Screen.height * 1.5f, 0)), 0.6f);
                        reShowBar(0, 230);
                    }
                }
                else
                {
                    gameObjectDetailedSearch.transform.localScale = Vector3.one;
                    if (detailShowed)
                    {
                        gameObjectDetailedSearch.GetComponent<UITexture>().height = Screen.height;
                        iTween.MoveTo(gameObjectDetailedSearch, Program.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width - 345f, Screen.height * 0.5f, 0)), 0.6f);
                        reShowBar(0, 460);
                    }
                    else
                    {
                        gameObjectDetailedSearch.GetComponent<UITexture>().height = Screen.height;
                        iTween.MoveTo(gameObjectDetailedSearch, Program.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width - 345f, Screen.height * 1.5f, 0)), 0.6f);
                        reShowBar(0, 230);
                    }
                }

            }
            else
            {
                gameObjectDetailedSearch.transform.localScale = Vector3.zero;
            }
        }
    }

    void onClickDetail()
    {
        if (detailShowed)
        {
            hideDetail();
        }
        else
        {
            showDetail();
        }
    }

    public override void ES_mouseDownEmpty()
    {
        //if (detailShowed)
        //{
        //    hideDetail();
        //}
    }

    void onExitDetail()
    {
        if (detailShowed)
        {
            hideDetail();
        }
    }

    void clearAll()
    {
        try
        {
            seconds.Clear();
            for (int i = 0; i < 32; i++)
            {
                UIToggle_effects[i].value = false;
            }
            UIPopupList_pack.value = GameStringManager.get_unsafe(1310);
            UIPopupList_main.value = GameStringManager.get_unsafe(1310);
            UIPopupList_ban.value = GameStringManager.get_unsafe(1310);
            UIPopupList_second.Clear();
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1310));
            UIPopupList_second.value = GameStringManager.get_unsafe(1310);
            UIPopupList_race.Clear();
            UIPopupList_race.AddItem(GameStringManager.get_unsafe(1310));
            UIPopupList_race.value = GameStringManager.get_unsafe(1310);
            UIPopupList_attribute.Clear();
            UIPopupList_attribute.AddItem(GameStringManager.get_unsafe(1310));
            UIPopupList_attribute.value = GameStringManager.get_unsafe(1310);
            UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "stars_").value = "";
            UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "p_").value = "";
            UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "atk_").value = "";
            UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "def_").value = "";
            UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "year_").value = "";

            UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "stars_UP").value = "";
            UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "p_UP").value = "";
            UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "atk_UP").value = "";
            UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "def_UP").value = "";
            UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "year_UP").value = "";

        }
        catch (System.Exception e)
        {
            //UnityEngine.Debug.Log(e);
        }
    }

    List<string> seconds = new List<string>();
    void onUIPopupList_second()
    {
        Program.notGo(printSecond);
        Program.go(100, printSecond);
    }

    private void printSecond()
    {
        Program.go(50, tempStep2);
        if (UIPopupList_main.value == GameStringManager.get_unsafe(1312))
        {
            if (UIPopupList_second.value == GameStringManager.get_unsafe(1310))
            {
                seconds.Clear();
            }
            else
            {
                seconds.Remove(UIPopupList_second.value);
                seconds.Add(UIPopupList_second.value);
                string all = "";
                foreach (var item in seconds)
                {
                    all += item + " ";
                }
                if (all == "")
                {
                    all = GameStringManager.get_unsafe(1310);
                }
                UIPopupList_second.value = all;
            }
        }
        else
        {
            seconds.Clear();
            seconds.Add(UIPopupList_second.value);
        }
    }

    void tempStep2()
    {
        Program.notGo(printSecond);
    }

    void onUIPopupList_main()
    {
        UIPopupList_second.Clear();
        UIPopupList_second.AddItem(GameStringManager.get_unsafe(1310));
        UIPopupList_second.value = GameStringManager.get_unsafe(1310);
        UIPopupList_race.Clear();
        UIPopupList_race.AddItem(GameStringManager.get_unsafe(1310));
        UIPopupList_race.value = GameStringManager.get_unsafe(1310);
        UIPopupList_attribute.Clear();
        UIPopupList_attribute.AddItem(GameStringManager.get_unsafe(1310));
        UIPopupList_attribute.value = GameStringManager.get_unsafe(1310);
        if (UIPopupList_main.value == GameStringManager.get_unsafe(1312))
        {
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1054));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1055));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1056));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1057));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1063));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1073));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1062));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1061));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1060));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1059));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1071));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1072));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1074));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1075));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1076));
            for (int i = 1020; i <= 1044; i++)
            {
                UIPopupList_race.AddItem(GameStringManager.get_unsafe(i));
            }
            for (int i = 1010; i <= 1016; i++)
            {
                UIPopupList_attribute.AddItem(GameStringManager.get_unsafe(i));
            }
        }
        if (UIPopupList_main.value == GameStringManager.get_unsafe(1313))
        {
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1054));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1066));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1067));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1057));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1068));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1069));
        }
        if (UIPopupList_main.value == GameStringManager.get_unsafe(1314))
        {
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1054));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1067));
            UIPopupList_second.AddItem(GameStringManager.get_unsafe(1070));
        }
    }

    void onClickSearch()
    {
        doSearch();
    }

    int lastRefreshTime = 0;

    string UIInput_searchValueLast = "";

    void doSearch()
    {
        superScrollView.toTop();
        Program.go(50, process);
    }

    private void process()
    {
        List<YGOSharp.Card> result = YGOSharp.CardsManager.searchAdvanced
                    (
                    getName(),
                    getLevel(),
                    getAttack(),
                    getDefence(),
                    getP(),
                    getYear(),
                    getLevel_UP(),
                    getAttack_UP(),
                    getDefence_UP(),
                    getP_UP(),
                    getYear_UP(),
                    getPack(),
                    getBanFilter(),
                    currentBanlist,
                    getTypeFilter(),
                    getRaceFilter(),
                    getAttributeFilter(),
                    getCatagoryFilter()
                    );
        print(result);
        UIHelper.trySetLableText(gameObjectSearch, "title_", result.Count.ToString());
        UIInput_search.isSelected = true;
    }

  public  YGOSharp.Banlist currentBanlist = null;

    List<YGOSharp.Card> PrintedResult = new List<YGOSharp.Card>();

    void print(List<YGOSharp.Card> result)
    {
        if (superScrollView!=null)
        {
            PrintedResult = result;
            if (condition == Condition.editDeck)
            {
                currentBanlist = YGOSharp.BanlistManager.GetByName(UIPopupList_banlist.value);
            }
            if (condition == Condition.changeSide)
            {
                currentBanlist = YGOSharp.BanlistManager.GetByHash(Program.I().room.lflist);
            }
            List<string[]> args = new List<string[]>();
            foreach (var item in result)
            {
                string[] arg = new string[5];
                arg[0] = item.Id.ToString();
                arg[1] = "3";
                arg[2] = item.Name + "\n" + GameStringHelper.getSmall(item);
                args.Add(arg);
            }
            superScrollView.print(args);
            superScrollView.toTop();
        }
    }

    bool ifType(string str)
    {
        bool re = false;
        foreach (var item in seconds)   
        {
            if (str==item)
            {
                re = true;
                break;
            }
        }
        return re;
    }

    UInt32 getTypeFilter()
    {
        UInt32 returnValue = 0;
        if (UIPopupList_main.value == GameStringManager.get_unsafe(1312))
        {
            returnValue = (UInt32)CardType.Monster;
            if (ifType(GameStringManager.get_unsafe(1054)))
            {
                returnValue |= (UInt32)CardType.Monster + (UInt32)CardType.Normal;
            }
            if (ifType(GameStringManager.get_unsafe(1055)))
            {
                returnValue |= (UInt32)CardType.Monster + (UInt32)CardType.Effect;
            }
            if (ifType(GameStringManager.get_unsafe(1056)))
            {
                returnValue |= (UInt32)CardType.Monster + (UInt32)CardType.Fusion;
            }
            if (ifType(GameStringManager.get_unsafe(1057)))
            {
                returnValue |= (UInt32)CardType.Monster + (UInt32)CardType.Ritual;
            }
            if (ifType(GameStringManager.get_unsafe(1063)))
            {
                returnValue |= (UInt32)CardType.Monster + (UInt32)CardType.Synchro;
            }
            if (ifType(GameStringManager.get_unsafe(1073)))
            {
                returnValue |= (UInt32)CardType.Monster + (UInt32)CardType.Xyz;
            }
            if (ifType(GameStringManager.get_unsafe(1062)))
            {
                returnValue |= (UInt32)CardType.Monster + (UInt32)CardType.Tuner;
            }
            if (ifType(GameStringManager.get_unsafe(1061)))
            {
                returnValue |= (UInt32)CardType.Monster + (UInt32)CardType.Dual;
            }
            if (ifType(GameStringManager.get_unsafe(1060)))
            {
                returnValue |= (UInt32)CardType.Monster + (UInt32)CardType.Union;
            }
            if (ifType(GameStringManager.get_unsafe(1059)))
            {
                returnValue |= (UInt32)CardType.Monster + (UInt32)CardType.Spirit;
            }
            if (ifType(GameStringManager.get_unsafe(1071)))
            {
                returnValue |= (UInt32)CardType.Monster + (UInt32)CardType.Flip;
            }
            if (ifType(GameStringManager.get_unsafe(1072)))
            {
                returnValue |= (UInt32)CardType.Monster + (UInt32)CardType.Toon;
            }
            if (ifType(GameStringManager.get_unsafe(1074)))
            {
                returnValue |= (UInt32)CardType.Monster + (UInt32)CardType.Pendulum;
            }
            if (ifType(GameStringManager.get_unsafe(1075)))
            {
                returnValue |= (UInt32)CardType.Monster + (UInt32)CardType.SpSummon;
            }
            if (ifType(GameStringManager.get_unsafe(1076)))
            {
                returnValue |= (UInt32)CardType.Monster + (UInt32)CardType.Link;
            }
        }
        if (UIPopupList_main.value == GameStringManager.get_unsafe(1313))
        {
            returnValue = (UInt32)CardType.Spell;
            if (ifType(GameStringManager.get_unsafe(1054)))
            {
                returnValue |= (UInt32)CardType.Spell;
            }
            if (ifType(GameStringManager.get_unsafe(1066)))
            {
                returnValue |= (UInt32)CardType.Spell + (UInt32)CardType.QuickPlay;
            }
            if (ifType(GameStringManager.get_unsafe(1067)))
            {
                returnValue |= (UInt32)CardType.Spell + (UInt32)CardType.Continuous;
            }
            if (ifType(GameStringManager.get_unsafe(1057)))
            {
                returnValue |= (UInt32)CardType.Spell + (UInt32)CardType.Ritual;
            }
            if (ifType(GameStringManager.get_unsafe(1068)))
            {
                returnValue |= (UInt32)CardType.Spell + (UInt32)CardType.Equip;
            }
            if (ifType(GameStringManager.get_unsafe(1069)))
            {
                returnValue |= (UInt32)CardType.Spell + (UInt32)CardType.Field;
            }
        }
        if (UIPopupList_main.value == GameStringManager.get_unsafe(1314))
        {
            returnValue = (UInt32)CardType.Trap;
            if (ifType(GameStringManager.get_unsafe(1054)))
            {
                returnValue |= (UInt32)CardType.Trap;
            }
            if (ifType(GameStringManager.get_unsafe(1067)))
            {
                returnValue |= (UInt32)CardType.Trap + (UInt32)CardType.Continuous;
            }
            if (ifType(GameStringManager.get_unsafe(1070)))
            {
                returnValue |= (UInt32)CardType.Trap + (UInt32)CardType.Counter;
            }
        }
        return returnValue;
    }

    int getBanFilter()
    {
        int returnValue = -233;
        if (UIPopupList_ban.value == GameStringManager.get_unsafe(1316))
        {
            returnValue = 0;
        }
        if (UIPopupList_ban.value == GameStringManager.get_unsafe(1317))
        {
            returnValue = 1;
        }
        if (UIPopupList_ban.value == GameStringManager.get_unsafe(1318))
        {
            returnValue = 2;
        }
        return returnValue;
    }

    UInt32 getRaceFilter()
    {
        UInt32 returnValue = 0;
        for (int i = 0; i < 25; i++)
        {
            if (UIPopupList_race.value == GameStringManager.get_unsafe(1020 + i))
            {
                returnValue |= (UInt32)Math.Pow(2, i);
            }
        }
        return returnValue;
    }

    UInt32 getAttributeFilter()
    {
        UInt32 returnValue = 0;
        for (int i = 0; i < 7; i++)
        {
            if (UIPopupList_attribute.value == GameStringManager.get_unsafe(1010 + i))
            {
                returnValue |= (UInt32)Math.Pow(2, i);
            }
        }
        return returnValue;
    }

    UInt32 getCatagoryFilter()
    {
        UInt32 returnValue = 0;
        for (int i = 0; i < 32; i++)
        {
            if (UIToggle_effects[i].value == true)
            {
                returnValue |= (UInt32)Math.Pow(2, i);
            }
        }
        return returnValue;
    }

    int getAttack()
    {
        int returnValue = 0;
        try
        {
            returnValue = int.Parse(UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "atk_").value);
        }
        catch (Exception)
        {
            returnValue = -2;
        }
        if (UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "atk_").value == "")
        {
            returnValue = -233;
        }
        return returnValue;
    }

    int getDefence()
    {
        int returnValue = 0;
        try
        {
            returnValue = int.Parse(UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "def_").value);
        }
        catch (Exception)
        {
            returnValue = -2;
        }
        if (UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "def_").value == "")
        {
            returnValue = -233;
        }
        return returnValue;
    }

    int getLevel()
    {
        int returnValue = 0;
        try
        {
            returnValue = int.Parse(UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "stars_").value);
        }
        catch (Exception)
        {
            returnValue = 0;
        }
        if (UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "stars_").value == "")
        {
            returnValue = -233;
        }
        return returnValue;
    }

    int getP()
    {
        int returnValue = 0;
        try
        {
            returnValue = int.Parse(UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "p_").value);
        }
        catch (Exception)
        {
            returnValue = 0;
        }
        if (UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "p_").value == "")
        {
            returnValue = -233;
        }
        return returnValue;
    }

    int getYear()
    {
        int returnValue = 0;
        try
        {
            returnValue = int.Parse(UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "year_").value);
        }
        catch (Exception)
        {
            returnValue = 0;
        }
        if (UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "year_").value == "")
        {
            returnValue = -233;
        }
        return returnValue;
    }

    int getAttack_UP()
    {
        int returnValue = 0;
        try
        {
            returnValue = int.Parse(UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "atk_UP").value);
        }
        catch (Exception)
        {
            returnValue = -2;
        }
        if (UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "atk_UP").value == "")
        {
            returnValue = -233;
        }
        return returnValue;
    }

    int getDefence_UP()
    {
        int returnValue = 0;
        try
        {
            returnValue = int.Parse(UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "def_UP").value);
        }
        catch (Exception)
        {
            returnValue = -2;
        }
        if (UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "def_UP").value == "")
        {
            returnValue = -233;
        }
        return returnValue;
    }

    int getLevel_UP()
    {
        int returnValue = 0;
        try
        {
            returnValue = int.Parse(UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "stars_UP").value);
        }
        catch (Exception)
        {
            returnValue = 0;
        }
        if (UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "stars_UP").value == "")
        {
            returnValue = -233;
        }
        return returnValue;
    }

    int getP_UP()
    {
        int returnValue = 0;
        try
        {
            returnValue = int.Parse(UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "p_UP").value);
        }
        catch (Exception)
        {
            returnValue = 0;
        }
        if (UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "p_UP").value == "")
        {
            returnValue = -233;
        }
        return returnValue;
    }

    int getYear_UP()
    {
        int returnValue = 0;
        try
        {
            returnValue = int.Parse(UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "year_UP").value);
        }
        catch (Exception)
        {
            returnValue = 0;
        }
        if (UIHelper.getByName<UIInput>(gameObjectDetailedSearch, "year_UP").value == "")
        {
            returnValue = -233;
        }
        return returnValue;
    }

    string getName()
    {
        return UIInput_search.value;
    }

    string getPack()
    {
        if (UIPopupList_pack.value == GameStringManager.get_unsafe(1310))
        {
            return "";
        }
        return UIPopupList_pack.value;
    }

    #endregion

    GameObject gameObjectDesk = null;

    number_loader main_unmber;

    number_loader side_number;

    number_loader extra_unmber;

    number_loader m_unmber;

    number_loader s_number;

    number_loader t_unmber;

    public override void show()
    {
        base.show();
        Program.camera_game_main.transform.position = new Vector3(0, 35, 0);
        Program.camera_game_main.transform.localEulerAngles = new Vector3(90, 0, 0);
        cameraAngle = 90;
        Program.cameraFacing = true;
        Program.cameraPosition = Program.camera_game_main.transform.position;
        camrem();
        Program.I().light.transform.eulerAngles = new Vector3(50, 0, 0);
        gameObjectDesk = create_s(Program.I().new_mod_tableInDeckManager);
        gameObjectDesk.layer = 16;
        gameObjectDesk.transform.position = new Vector3(0, 0, 0);
        gameObjectDesk.transform.eulerAngles = new Vector3(90, 0, 0);
        gameObjectDesk.transform.localScale = new Vector3(30, 30, 1);
        gameObjectDesk.GetComponent<Renderer>().material.mainTexture = Program.GetTextureViaPath("texture/duel/deckTable.png");
        //UIHelper.SetMaterialRenderingMode(gameObjectDesk.GetComponent<Renderer>().material, UIHelper.RenderingMode.Transparent);
        Rigidbody rigidbody = gameObjectDesk.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        BoxCollider boxCollider = gameObjectDesk.AddComponent<BoxCollider>();
        main_unmber = create_s(Program.I().mod_ocgcore_number, new Vector3(-16.5f, 0, 13.6f), new Vector3(90, 0, 0), true).GetComponent<number_loader>();
        m_unmber = create_s(Program.I().mod_ocgcore_number, new Vector3(-16.5f, 0, 6.6f), new Vector3(90, 0, 0), true).GetComponent<number_loader>();
        s_number = create_s(Program.I().mod_ocgcore_number, new Vector3(-16.5f, 0, 4.6f), new Vector3(90, 0, 0), true).GetComponent<number_loader>();
        t_unmber = create_s(Program.I().mod_ocgcore_number, new Vector3(-16.5f, 0, 2.6f), new Vector3(90, 0, 0), true).GetComponent<number_loader>();
        extra_unmber = create_s(Program.I().mod_ocgcore_number, new Vector3(-16.5f, 0, -5.3f), new Vector3(90, 0, 0), true).GetComponent<number_loader>();
        side_number = create_s(Program.I().mod_ocgcore_number, new Vector3(-16.5f, 0, -11f), new Vector3(90, 0, 0), true).GetComponent<number_loader>();
        switch (condition)  
        {
            case Condition.editDeck:
                boxCollider.size = new Vector3(1, 1, 1);
                break;
            case Condition.changeSide:
                boxCollider.size = new Vector3(100, 100, 1);
                break;
            default:
                break;
        }
        clearAll();
    }

    public override void hide()
    {
        if (isShowed)
        {
            hideDetail();
        }
        for (int i = 0; i < deck.IMain.Count; i++)
        {
            destroyCard(deck.IMain[i]);
        }
        for (int i = 0; i < deck.IExtra.Count; i++)
        {
            destroyCard(deck.IExtra[i]);
        }
        for (int i = 0; i < deck.ISide.Count; i++)
        {
            destroyCard(deck.ISide[i]);
        }
        for (int i = 0; i < deck.IRemoved.Count; i++)
        {
            destroyCard(deck.IRemoved[i]);
        }
        deck = new YGOSharp.Deck();
        ((CardDescription)Program.I().cardDescription).setTitle("");
        base.hide();
    }

    float cameraDistance = Vector3.Distance(new Vector3(0, 23f, -17.5f), Vector3.zero);

    float cameraAngle = Mathf.Atan(23 / 17.5f);

    public override void preFrameFunction()
    {
        base.preFrameFunction();
        if (cardInDragging != null)
        {
            if (detailPanelShiftedTemp == false)
            {
                shiftDetailPanel(true);
            }
        }
        else
        {
            if (detailPanelShiftedTemp == true)
            {
                shiftDetailPanel(false);
            }
        }
        camrem();
        if (Input.mousePosition.x < Screen.width - 280)
        {
            if (Input.mousePosition.x > 250)
            {
                cameraAngle += Program.wheelValue * 1.2f;
                if (cameraAngle < 0f)
                {
                    cameraAngle = 0f;
                }
                if (cameraAngle > 90f)
                {
                    cameraAngle = 90f;
                }
            }
        }
        cameraDistance = 29 - 3.1415926f / 180f * (cameraAngle - 60f) * 13f;
        Program.cameraPosition = new Vector3(0, cameraDistance * Mathf.Sin(3.1415926f / 180f * cameraAngle), -cameraDistance * Mathf.Cos(3.1415926f / 180f * cameraAngle));
        if (Program.TimePassed() - lastRefreshTime > 80)
        {
            lastRefreshTime = Program.TimePassed();
            FromObjectDeckToCodedDeck();
            main_unmber.set_number(deck.Main.Count, 3);
            side_number.set_number(deck.Side.Count, 4);
            extra_unmber.set_number(deck.Extra.Count, 0);
            int m = 0, s = 0, t = 0;
            foreach (var item in deck.IMain)
            {
                if ((item.cardData.Type & (int)CardType.Monster) > 0) m++;
                if ((item.cardData.Type & (int)CardType.Spell) > 0) s++;
                if ((item.cardData.Type & (int)CardType.Trap) > 0) t++;
            }
            m_unmber.set_number(m, 1);
            s_number.set_number(s, 2);
            t_unmber.set_number(t, 5);
        }
        if (Program.InputEnterDown)
        {
            if (condition == Condition.editDeck)
            {
                onClickSearch();
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetMouseButtonDown(2))
        {
            onClickDetail();
        }
    }

    private void camrem()
    {
        float l = Program.I().cardDescription.width + ((float)Screen.width) * 0.03f;
        float r = Screen.width - 230f;
        if (detailShowed)
        {
            if (gameObjectDetailedSearch != null)
            {
                r -= 230 * gameObjectDetailedSearch.transform.localScale.x;
            }
        }
        Program.reMoveCam((l + r) / 2f);
    }

    public override void ES_HoverOverGameObject(GameObject gameObject)
    {
        MonoCardInDeckManager MonoCardInDeckManager_ = gameObject.GetComponent<MonoCardInDeckManager>();
        if (MonoCardInDeckManager_ != null)
        {
            ((CardDescription)(Program.I().cardDescription)).setData(MonoCardInDeckManager_.cardData, GameTextureManager.myBack);
        }
        cardPicLoader cardPicLoader_ = gameObject.GetComponent<cardPicLoader>();
        if (cardPicLoader_ != null)
        {
            ((CardDescription)(Program.I().cardDescription)).setData(cardPicLoader_.data, GameTextureManager.myBack);
        }
    }

    public MonoCardInDeckManager cardInDragging = null;
    int timeLastDown = 0;
    GameObject goLast = null;

    public override void ES_mouseDownGameObject(GameObject gameObject)
    {
        bool doubleClick = false;
        if (goLast == gameObject)
        {
            if (Program.TimePassed() - timeLastDown < 300)
            {
                doubleClick = true;
            }
        }
        goLast = gameObject;
        timeLastDown = Program.TimePassed();
        MonoCardInDeckManager MonoCardInDeckManager_ = gameObject.GetComponent<MonoCardInDeckManager>();
        if (MonoCardInDeckManager_ != null)
        {
            if (doubleClick && condition == Condition.editDeck && deck.GetCardCount(MonoCardInDeckManager_.cardData.Id) < currentBanlist.GetQuantity(MonoCardInDeckManager_.cardData.Id))
            {
                MonoCardInDeckManager card = createCard();
                card.transform.position = MonoCardInDeckManager_.transform.position;
                MonoCardInDeckManager_.cardData.cloneTo(card.cardData);
                card.gameObject.layer = 16;
                deck.IMain.Add(card);
                ArrangeObjectDeck(true);
                ShowObjectDeck();
            }
            else
            {
                cardInDragging = MonoCardInDeckManager_;
                MonoCardInDeckManager_.beginDrag();
            }
        }

        if (condition == Condition.editDeck)
        {
            cardPicLoader cardPicLoader_ = gameObject.GetComponent<cardPicLoader>();
            if (cardPicLoader_ != null)
            {
                if (deck.GetCardCount(cardPicLoader_.data.Id) < currentBanlist.GetQuantity(cardPicLoader_.data.Id))
                {
                    if ((cardPicLoader_.data.Type & (UInt32)CardType.Token) == 0)
                    {
                        MonoCardInDeckManager card = createCard();
                        card.transform.position = card.getGoodPosition(4);
                        card.cardData = cardPicLoader_.data;
                        card.gameObject.layer = 16;
                        deck.IMain.Add(card);
                        cardInDragging = card;
                        card.beginDrag();
                    }
                }
            }
        }
    }

    public override void ES_mouseUp()
    {
        if (cardInDragging != null)
        {
            if (Input.GetKey(KeyCode.LeftControl)|| Input.GetKey(KeyCode.RightControl))     
            {
                //
            }
            else
            {
                ArrangeObjectDeck(true);
                ShowObjectDeck();
            }
            cardInDragging.endDrag();
            cardInDragging = null;
        }
    }

    public override void ES_mouseUpRight()
    {
        if (Program.pointedGameObject != null)
        {
            if (condition == Condition.editDeck)
            {
                MonoCardInDeckManager MonoCardInDeckManager_ = Program.pointedGameObject.GetComponent<MonoCardInDeckManager>();
                if (MonoCardInDeckManager_ != null)
                {
                    MonoCardInDeckManager_.killIt();
                    ArrangeObjectDeck(true);
                    ShowObjectDeck();
                }
                cardPicLoader cardPicLoader_ = Program.pointedGameObject.GetComponent<cardPicLoader>();
                if (cardPicLoader_ != null)
                {
                    CreateMonoCard(cardPicLoader_.data);
                    ShowObjectDeck();
                }
            }
            else
            {
                MonoCardInDeckManager MonoCardInDeckManager_ = Program.pointedGameObject.GetComponent<MonoCardInDeckManager>();
                if (MonoCardInDeckManager_ != null)
                {
                    bool isSide = false;
                    for (int i = 0; i < deck.ISide.Count; i++)  
                    {
                        if (MonoCardInDeckManager_== deck.ISide[i])
                        {
                            isSide = true;
                        }
                    }
                    if (isSide)
                    {
                        if (
                        (MonoCardInDeckManager_.cardData.Type & (UInt32)CardType.Fusion) > 0
                         ||
                        (MonoCardInDeckManager_.cardData.Type & (UInt32)CardType.Synchro) > 0
                         ||
                        (MonoCardInDeckManager_.cardData.Type & (UInt32)CardType.Xyz) > 0
                          ||
                        (MonoCardInDeckManager_.cardData.Type & (UInt32)CardType.Link) > 0
                        )
                        {
                            deck.IExtra.Add(MonoCardInDeckManager_);
                            deck.ISide.Remove(MonoCardInDeckManager_);
                        }
                        else
                        {
                            deck.IMain.Add(MonoCardInDeckManager_);
                            deck.ISide.Remove(MonoCardInDeckManager_);
                        }
                    }
                    else
                    {
                        deck.ISide.Add(MonoCardInDeckManager_);
                        deck.IMain.Remove(MonoCardInDeckManager_);
                        deck.IExtra.Remove(MonoCardInDeckManager_);
                    }
                    ShowObjectDeck();
                }
            }
        }
    }

    private void CreateMonoCard(YGOSharp.Card data)
    {
        if (deck.GetCardCount(data.Id) < currentBanlist.GetQuantity(data.Id))
        {
            MonoCardInDeckManager card = createCard();
            card.transform.position = card.getGoodPosition(4);
            card.cardData = data;
            card.gameObject.layer = 16;
            if (
                (data.Type & (UInt32)CardType.Fusion) > 0
                  ||
                (data.Type & (UInt32)CardType.Synchro) > 0
                  ||
                (data.Type & (UInt32)CardType.Xyz) > 0
                ||
                (data.Type & (UInt32)CardType.Link) > 0
                )
            {
                deck.IExtra.Add(card);
                deck.Extra.Add(card.cardData.Id);
            }
            else
            {
                deck.IMain.Add(card);
                deck.Main.Add(card.cardData.Id);
            }
        }
    }

    public YGOSharp.Deck deck = new YGOSharp.Deck();

    public void loadDeckFromYDK(string path)
    {
        FromYDKtoCodedDeck(path, out deck);
        FormCodedDeckToObjectDeck();
    }

    public static void FromYDKtoCodedDeck(string path, out YGOSharp.Deck deck)
    {
        deck = new YGOSharp.Deck();
        try
        {
            string text = System.IO.File.ReadAllText(path);
            string st = text.Replace("\r", "");
            string[] lines = st.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            int flag = -1;
            foreach (string line in lines)
            {
                if (line == "#main")
                {
                    flag = 1;
                }
                else if (line == "#extra")
                {
                    flag = 2;
                }
                else if (line == "!side")
                {
                    flag = 3;
                }
                else
                {
                    int code = 0;
                    try
                    {
                        code = Int32.Parse(line);
                    }
                    catch (Exception)
                    {

                    }
                    if (code > 100)
                    {
                        switch (flag)
                        {
                            case 1:
                                {
                                    deck.Main.Add(code);
                                    deck.Deck_O.Main.Add(code);
                                }
                                break;
                            case 2:
                                {
                                    deck.Extra.Add(code);
                                    deck.Deck_O.Extra.Add(code);
                                }
                                break;
                            case 3:
                                {
                                    deck.Side.Add(code);
                                    deck.Deck_O.Side.Add(code);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
        }
    }

    public YGOSharp.Deck getRealDeck()
    {
        if (canSave)
        {
            return deck;
        }
        else
        {
            YGOSharp.Deck r = new YGOSharp.Deck();
            foreach (var item in deck.Deck_O.Main)  
            {
                r.Main.Add(item);
                r.Deck_O.Main.Add(item);
            }
            foreach (var item in deck.Deck_O.Side)
            {
                r.Side.Add(item);
                r.Deck_O.Side.Add(item);
            }
            foreach (var item in deck.Deck_O.Extra)
            {
                r.Extra.Add(item);
                r.Deck_O.Extra.Add(item);
            }
            return r;   
        }
    }

    void ArrangeObjectDeck(bool order = false)
    {
        var deckTemp = deck.getAllObjectCardAndDeload();
        if (order)
        {
            deckTemp.Sort((left, right) =>
            {
                Vector3 leftPosition = left.gameObject.transform.position;
                Vector3 rightPosition = right.gameObject.transform.position;
                if (leftPosition.y > 3f)
                {
                    leftPosition = MonoCardInDeckManager.refLectPosition(leftPosition);
                }
                if (rightPosition.y > 3f)
                {
                    rightPosition = MonoCardInDeckManager.refLectPosition(rightPosition);
                }
                if (leftPosition.z > -3 && rightPosition.z > -3)
                {
                    float l = leftPosition.x + 1000f * (int)((13f - leftPosition.z) / 3.7f);
                    float r = rightPosition.x + 1000f * (int)((13f - rightPosition.z) / 3.7f);
                    if (l < r)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else if (leftPosition.z > -3 && rightPosition.z < -3)
                {
                    return 1;
                }
                else if (leftPosition.z < -3 && rightPosition.z > -3)
                {
                    return -1;
                }
                else
                {
                    float l = leftPosition.x;
                    float r = rightPosition.x;
                    if (l < r)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
            });
        }
        for (int i = 0; i < deckTemp.Count; i++)
        {
            Vector3 p = deckTemp[i].gameObject.transform.position;
            if (deckTemp[i].getIfAlive() == true)
            {
                if (p.z > -8)
                {
                    if (
                        (deckTemp[i].cardData.Type & (UInt32)CardType.Fusion) > 0
                         ||
                        (deckTemp[i].cardData.Type & (UInt32)CardType.Synchro) > 0
                         ||
                        (deckTemp[i].cardData.Type & (UInt32)CardType.Xyz) > 0
                        ||
                        (deckTemp[i].cardData.Type & (UInt32)CardType.Link) > 0
                        )
                    {
                        deck.IExtra.Add(deckTemp[i]);
                    }
                    else
                    {
                        deck.IMain.Add(deckTemp[i]);
                    }
                }
                else
                {
                    deck.ISide.Add(deckTemp[i]);
                }
            }
            else
            {
                deck.IRemoved.Add(deckTemp[i]);
            }
        }
    }

    void SortObjectDeck()
    {
        YGOSharp.Deck.sort((List<MonoCardInDeckManager>)deck.IMain);
        YGOSharp.Deck.sort((List<MonoCardInDeckManager>)deck.IExtra);
        YGOSharp.Deck.sort((List<MonoCardInDeckManager>)deck.ISide);
    }

    void RandObjectDeck()
    {
        YGOSharp.Deck.rand((List<MonoCardInDeckManager>)deck.IMain);
    }



    List<GameObject> diedCards = new List<GameObject>();

    MonoCardInDeckManager createCard()
    {
        MonoCardInDeckManager r = null;
        if (diedCards.Count>0)
        {
            r = diedCards[0].AddComponent<MonoCardInDeckManager>();
            diedCards.RemoveAt(0);
        }
        if (r == null)
        {
            r = Program.I().create(Program.I().new_mod_cardInDeckManager).AddComponent<MonoCardInDeckManager>();
            r.gameObject.transform.Find("back").gameObject.GetComponent<Renderer>().material.mainTexture = GameTextureManager.myBack;
            r.gameObject.transform.Find("face").gameObject.GetComponent<Renderer>().material.mainTexture = GameTextureManager.myBack;
        }
        r.gameObject.transform.position = new Vector3(0, 5, 0);
        r.gameObject.transform.eulerAngles = new Vector3(90, 0, 0);
        r.gameObject.transform.localScale = new Vector3(0, 0, 0);
        iTween.ScaleTo(r.gameObject, new Vector3(3, 4, 1), 0.4f);
        r.gameObject.SetActive(true);
        return r;
    }

    void destroyCard(MonoCardInDeckManager c)
    {
        try
        {
            c.gameObject.SetActive(false);
            diedCards.Add(c.gameObject);
            MonoBehaviour.DestroyImmediate(c);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    bool canSave = false;

    public void FormCodedDeckToObjectDeck()
    {
        canSave = false;
        safeGogo(4000, () =>
        {
            canSave = true;
        });
        int indexOfLogic = 0;
        int[] hangshu = UIHelper.get_decklieshuArray(deck.Main.Count);
        foreach (var item in deck.Main) 
        {
            Vector2 v = UIHelper.get_hang_lieArry(indexOfLogic, hangshu);
            Vector3 toVector = new Vector3(UIHelper.get_left_right_index(-12.5f, 12.5f, (int)v.y, hangshu[(int)v.x]), 0.5f + v.y / 3f + v.x / 3f, 11.8f - v.x * 4f);
            YGOSharp.Card data = YGOSharp.CardsManager.Get(item);
            safeGogo(indexOfLogic * 25, () =>
            {
                MonoCardInDeckManager card = createCard();
                card.cardData = data;
                card.gameObject.layer = 16;
                deck.IMain.Add(card);
                card.tweenToVectorAndFall(toVector,new Vector3(90,0,0));
            });
            indexOfLogic++;
        }
        indexOfLogic = 0;
        foreach (var item in deck.Extra)
        {
            Vector3 toVector = new Vector3(UIHelper.get_left_right_indexZuo(-12.5f, 12.5f, indexOfLogic, deck.Extra.Count ,10), 0.5f + (float)indexOfLogic / 3f, -6.2f);
            YGOSharp.Card data = YGOSharp.CardsManager.Get(item);
            safeGogo(indexOfLogic * 90, () =>
            {
                MonoCardInDeckManager card = createCard();
                card.cardData = data;
                card.gameObject.layer = 16;
                deck.IExtra.Add(card);
                card.tweenToVectorAndFall(toVector, new Vector3(90, 0, 0));
            });
            indexOfLogic++;
        }
        indexOfLogic = 0;
        foreach (var item in deck.Side)
        {
            Vector3 toVector = new Vector3(UIHelper.get_left_right_indexZuo(-12.5f, 12.5f, indexOfLogic, deck.Side.Count, 10), 0.5f + (float)indexOfLogic / 3f, -12f);
            YGOSharp.Card data = YGOSharp.CardsManager.Get(item);
            safeGogo(indexOfLogic * 90, () =>
            {
                MonoCardInDeckManager card = createCard();
                card.cardData = data;
                card.gameObject.layer = 16;
                deck.ISide.Add(card);
                card.tweenToVectorAndFall(toVector, new Vector3(90, 0, 0));
            });
            indexOfLogic++;
        }
    }

    void ShowObjectDeck()
    {
        float k = (float)(1.5 * 0.1 / 0.130733633);
        int[] hangshu = UIHelper.get_decklieshuArray(deck.IMain.Count);
        for (int i = 0; i < deck.IMain.Count; i++)
        {
            Vector2 v = UIHelper.get_hang_lieArry(i, hangshu);
            Vector3 toAngle = new Vector3(90, 0, 0);
            if ((int)v.y > 0)
            {
                toAngle = new Vector3(87, -90, -90);
                if (hangshu[(int)v.x] > 10)
                {
                    toAngle = new Vector3(87f - (hangshu[(int)v.x] - 10f) * 0.4f, -90, -90);
                }
            }
            Vector3 toVector = new Vector3(UIHelper.get_left_right_indexZuo(-12.5f, 12.5f, (int)v.y, hangshu[(int)v.x], 10), 0.6f + Mathf.Sin((90 - toAngle.x) / 180f * Mathf.PI) * k, 11.8f - v.x * 4f);
            deck.IMain[i].tweenToVectorAndFall(toVector, toAngle);
        }
        for (int i = 0; i < deck.IExtra.Count; i++)
        {
            Vector3 toAngle = new Vector3(90, 0, 0);
            if (i > 0)
            {
                toAngle = new Vector3(87, -90, -90);
                if (deck.IExtra.Count > 10)
                {
                    toAngle = new Vector3(87f - (deck.IExtra.Count - 10f) * 0.4f, -90, -90);
                }
            }
            Vector3 toVector = new Vector3(UIHelper.get_left_right_indexZuo(-12.5f, 12.5f, i, deck.IExtra.Count, 10), 0.6f + Mathf.Sin((90 - toAngle.x) / 180f * Mathf.PI) * k, -6.2f);
            deck.IExtra[i].tweenToVectorAndFall(toVector, toAngle);
        }

        for (int i = 0; i < deck.ISide.Count; i++)
        {
            Vector3 toAngle = new Vector3(90, 0, 0);
            if (i > 0)
            {
                toAngle = new Vector3(87, -90, -90);
                if (deck.ISide.Count > 10)
                {
                    toAngle = new Vector3(87f - (deck.ISide.Count - 10f) * 0.4f, -90, -90);
                }
            }
            Vector3 toVector = new Vector3(UIHelper.get_left_right_indexZuo(-12.5f, 12.5f, i, deck.ISide.Count ,10), 0.6f + Mathf.Sin((90 - toAngle.x) / 180f * Mathf.PI) * k, -12f);
            deck.ISide[i].tweenToVectorAndFall(toVector, toAngle);
        }
    }

    public void FromObjectDeckToCodedDeck(bool order=false)
    {
        ArrangeObjectDeck(order);
        deck.Main.Clear();
        deck.Extra.Clear();
        deck.Side.Clear();
        foreach (var item in deck.IMain)
        {
            deck.Main.Add(item.cardData.Id);
        }
        foreach (var item in deck.IExtra)
        {
            deck.Extra.Add(item.cardData.Id);
        }
        foreach (var item in deck.ISide)
        {
            deck.Side.Add(item.cardData.Id);
        }
    }

    public void setGoodLooking(bool side=false) 
    {
        try
        {
            ((CardDescription)(Program.I().cardDescription)).setData(YGOSharp.CardsManager.Get(deck.Main[0]), GameTextureManager.myBack);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
        List<YGOSharp.Card> result = new List<YGOSharp.Card>();
        if (side)   
        {
            foreach (var item in Program.I().ocgcore.sideReference) 
            {
                result.Add(YGOSharp.CardsManager.Get(item.Value));
            }
        }
        else
        {
            foreach (var item in deck.Main)
            {
                result.Add(YGOSharp.CardsManager.Get(item));
            }
            foreach (var item in deck.Extra)
            {
                result.Add(YGOSharp.CardsManager.Get(item));
            }
            foreach (var item in deck.Side)
            {
                result.Add(YGOSharp.CardsManager.Get(item));
            }
        }
        print(result);
        UIHelper.trySetLableText(gameObjectSearch, "title_", result.Count.ToString());
        Program.go(50, superScrollView.toTop);
        Program.go(100, superScrollView.toTop);
        Program.go(200, superScrollView.toTop);
        Program.go(300, superScrollView.toTop);
        Program.go(400, superScrollView.toTop);
        Program.go(500, superScrollView.toTop);
        if (side)   
        {
            UIInput_search.value = InterString.Get("对手使用过的卡↓");
            UIInput_search.isSelected = false;
        }
        else
        {
            UIInput_search.value = "";
            UIInput_search.isSelected = true;
        }
    }
}
