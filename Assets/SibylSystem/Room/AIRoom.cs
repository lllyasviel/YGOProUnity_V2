using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AIRoom : WindowServantSP
{
    #region ui
    UIselectableList superScrollView = null;
    string sort = "sortByTimeDeck";
    string suiji = "";

    UIPopupList list_aideck;
    UIPopupList list_airank;

    public override void initialize()
    {
        suiji = InterString.Get("随机卡组");
        createWindow(Program.I().new_ui_aiRoom);
        superScrollView = gameObject.GetComponentInChildren<UIselectableList>();
        superScrollView.selectedAction = onSelected;
        list_aideck = UIHelper.getByName<UIPopupList>(gameObject, "aideck_");
        list_airank = UIHelper.getByName<UIPopupList>(gameObject, "rank_");
        list_aideck.value = Config.Get("list_aideck", suiji);
        list_airank.value = Config.Get("list_airank", "ai");
        UIHelper.registEvent(gameObject, "aideck_", onSave);
        UIHelper.registEvent(gameObject, "rank_", onSave);
        UIHelper.registEvent(gameObject, "start_", onStart);
        UIHelper.registEvent(gameObject, "exit_", ()=> { Program.I().shiftToServant(Program.I().menu); });
        UIHelper.trySetLableText(gameObject,"percyHint",InterString.Get("人机模式"));
        superScrollView.install();
        SetActiveFalse();
    }

    void onSelected()
    {
        Config.Set("deckInUse", superScrollView.selectedString);
    }

    void onSave()
    {
        Config.Set("list_aideck", list_aideck.value);
        Config.Set("list_airank", list_airank.value);
    }

    void onStart()
    {
        if (!isShowed)
        {
            return;
        }
        int l = 8000;
        try
        {
            l = int.Parse(UIHelper.getByName<UIInput>(gameObject, "life_").value);
        }
        catch (Exception)
        {
        }
        string aideck = "";
        if (Config.Get("list_aideck", suiji) == suiji)
        {
            aideck= "ai/ydk/" + list_aideck.items[UnityEngine.Random.Range(1, list_aideck.items.Count)] + ".ydk";
        }
        else
        {
            aideck = "ai/ydk/" + Config.Get("list_aideck", suiji) + ".ydk";
        }
        launch("deck/" + Config.Get("deckInUse", "miaowu") + ".ydk", aideck, "ai/" + Config.Get("list_airank", "ai") + ".lua", UIHelper.getByName<UIToggle>(gameObject, "first_").value, UIHelper.getByName<UIToggle>(gameObject, "unrand_").value, l, UIHelper.getByName<UIToggle>(gameObject, "god_").value, UIHelper.getByName<UIToggle>(gameObject, "mr4_").value ? 4 : 3);
    }

    void printFile()
    {
        string deckInUse = Config.Get("deckInUse","miaowu");
        superScrollView.clear();
        FileInfo[] fileInfos = (new DirectoryInfo("deck")).GetFiles();
        if (Config.Get(sort,"1") == "1")
        {
            Array.Sort(fileInfos, UIHelper.CompareTime);
        }
        else
        {
            Array.Sort(fileInfos, UIHelper.CompareName);
        }
        for (int i = 0; i < fileInfos.Length; i++)
        {
            if (fileInfos[i].Name.Length > 4)
            {
                if (fileInfos[i].Name.Substring(fileInfos[i].Name.Length - 4, 4) == ".ydk")
                {
                    if (fileInfos[i].Name.Substring(0, fileInfos[i].Name.Length - 4) == deckInUse)
                    {
                        superScrollView.add(fileInfos[i].Name.Substring(0, fileInfos[i].Name.Length - 4));
                    }
                }
            }
        }
        for (int i = 0; i < fileInfos.Length; i++)
        {
            if (fileInfos[i].Name.Length > 4)
            {
                if (fileInfos[i].Name.Substring(fileInfos[i].Name.Length - 4, 4) == ".ydk")
                {
                    if (fileInfos[i].Name.Substring(0, fileInfos[i].Name.Length - 4) != deckInUse)
                    {
                        superScrollView.add(fileInfos[i].Name.Substring(0, fileInfos[i].Name.Length - 4));
                    }
                }
            }
        }
        list_aideck.Clear();
        fileInfos = (new DirectoryInfo("ai/ydk")).GetFiles();
        Array.Sort(fileInfos, UIHelper.CompareName);
        list_aideck.AddItem(suiji);
        for (int i = 0; i < fileInfos.Length; i++)
        {
            if (fileInfos[i].Name.Length > 4)
            {
                if (fileInfos[i].Name.Substring(fileInfos[i].Name.Length - 4, 4) == ".ydk")
                {
                    list_aideck.AddItem(fileInfos[i].Name.Substring(0, fileInfos[i].Name.Length - 4));
                }
            }
        }
        list_airank.Clear();
        fileInfos = (new DirectoryInfo("ai")).GetFiles();
        Array.Sort(fileInfos, UIHelper.CompareName);
        for (int i = 0; i < fileInfos.Length; i++)
        {
            if (fileInfos[i].Name.Length > 4)
            {
                if (fileInfos[i].Name.Substring(fileInfos[i].Name.Length - 4, 4) == ".lua")
                {
                    list_airank.AddItem(fileInfos[i].Name.Substring(0, fileInfos[i].Name.Length - 4));
                }
            }
        }
    }

    public override void show()
    {
        base.show();
        printFile();
        superScrollView.selectedString = Config.Get("deckInUse", "miaowu");
        superScrollView.toTop();
        Program.charge();
    }

    #endregion

    PrecyOcg precy;

    public void launch(string playerDek, string aiDeck, string aiScript, bool playerGo, bool suffle, int life,bool god,int rule)
    {
        if (precy != null)
        {
            precy.dispose();
        }
        precy = new PrecyOcg();
        precy.startAI(playerDek, aiDeck, aiScript, playerGo, suffle, life, god,rule);
        RMSshow_none(InterString.Get("AI模式还在开发中，您在AI模式下遇到的BUG不会在联机的时候出现。"));
    }

    public override void preFrameFunction()
    {
        base.preFrameFunction();
        Menu.checkCommend();
    }
}
