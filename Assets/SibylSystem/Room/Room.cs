using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class Room : WindowServantSP
{
    UIselectableList superScrollView = null;

    string sort = "sortByTimeDeck";

    public override void initialize()
    {
        instanceHide = true;

        SetBar(Program.I().new_bar_room, 0, 0);
    }

    void onSelected()
    {
        Config.Set("deckInUse", superScrollView.selectedString);

        if (selftype < realPlayers.Length)
        {
            if (realPlayers[selftype] != null)
            {
                if (realPlayers[selftype].getIfPreped() == true)
                {
                    TcpHelper.CtosMessage_HsNotReady();
                    TcpHelper.CtosMessage_UpdateDeck(new YGOSharp.Deck("deck/" + Config.Get("deckInUse","wizard") + ".ydk"));
                    TcpHelper.CtosMessage_HsReady();
                }
            }
        }


    }

    void printFile()
    {
        string deckInUse = Config.Get("deckInUse","wizard");
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
    }

    public override void show()
    {
        if (isShowed == true)
        {
            Menu.deleteShell();
        }
        base.show();
        Program.I().ocgcore.returnServant = Program.I().selectServer;
        Program.I().ocgcore.handler = handler;
        UIHelper.registEvent(toolBar, "input_", onChat);
        Program.charge();
    }

    public void onSubmit(string val)
    {
        if (val != "")
        {
            TcpHelper.CtosMessage_Chat(val);
            //AddChatMsg(val, -1);
        }
    }

    public void onChat()
    {
        onSubmit(UIHelper.getByName<UIInput>(toolBar, "input_").value);
        UIHelper.getByName<UIInput>(toolBar, "input_").value = "";
    }

    void handler(byte[] buffer)
    {
        TcpHelper.CtosMessage_Response(buffer);
    }

    #region STOC

    int animationTime = 0;

    public void StocMessage_HsWatchChange(BinaryReader r)
    {
        countOfObserver = r.ReadUInt16();
        realize();
    }

    public void StocMessage_HsPlayerChange(BinaryReader r)
    {
        int status = r.ReadByte();
        int pos = (status >> 4) & 0xf;
        int state = status & 0xf;
        if (pos < 4)
        {
            if (state < 8)
            {
                roomPlayers[state] = roomPlayers[pos];
                roomPlayers[pos] = null;
            }
            if (state == 0x9)
            {
                roomPlayers[pos].prep = true;
            }
            if (state == 0xa)
            {
                roomPlayers[pos].prep = false;
            }
            if (state == 0xb)
            {
                roomPlayers[pos] = null;
            }
            if (state == 0x8)
            {
                roomPlayers[pos] = null;
                countOfObserver++;
            }
            realize();
        }
    }

    public void StocMessage_HsPlayerEnter(BinaryReader r)
    {
        string name = r.ReadUnicode(20);
        int pos = r.ReadByte()&3;//Fuck this
        RoomPlayer player = new RoomPlayer();
        player.name = name;
        player.prep = false;
        roomPlayers[pos] = player;
        realize();
        UIHelper.Flash();
    }

    public void StocMessage_Chat(BinaryReader r)
    {
        int player = r.ReadInt16();
        long length = r.BaseStream.Length - 3;
        string str = r.ReadUnicode((int)length);

        if (player < 4)
        {
            if (UIHelper.fromStringToBool(Config.Get("ignoreOP_","0")) == true)
                return;
            if (mode != 2)
            {
                if (Program.I().ocgcore.isShowed)
                    player = Program.I().ocgcore.localPlayer(player);
            }
            else
            {
                if (Program.I().ocgcore.isShowed && !Program.I().ocgcore.isFirst)
                    player ^= 2;
                if (player == 0)
                    player = 0;
                else if (player == 1)
                    player = 2;
                else if (player == 2)
                    player = 1;
                else if (player == 3)
                    player = 3;
                else
                    player = 10;
            }
        }
        else
        {
            if (UIHelper.fromStringToBool(Config.Get("ignoreWatcher_","0")) == true)
                return;
        }
        AddChatMsg(str, player);
    }

    public void AddChatMsg(string msg, int player)
    {
        string result = "";
        switch (player)
        {
            case -1: //local name
                result += Program.I().selectServer.name;
                result += ":";
                break;
            case 0: //from host
                result += Program.I().ocgcore.name_0;
                result += ":";
                break;
            case 1: //from client
                result += Program.I().ocgcore.name_1;
                result += ":";
                break;
            case 2: //host tag
                result += Program.I().ocgcore.name_0_tag;
                result += ":";
                break;
            case 3: //client tag
                result += Program.I().ocgcore.name_1_tag;
                result += ":";
                break;
            case 7: //---
                result += "[---]";
                result += ":";
                break;
            case 8: //system custom message, no prefix.
                result += "[System]";
                result += ":";
                break;
            case 9: //error message
                result += "[Script error]";
                result += ":";
                break;
            default: //from watcher or unknown
                result += "[---]";
                result += ":";
                break;
        }
        result += msg;
        string res = "[888888]" + result + "[-]";
        Program.I().book.add(res);
        Package p = new Package();
        p.Fuction = (int)YGOSharp.OCGWrapper.Enums.GameMessage.sibyl_chat;
        p.Data = new BinaryMaster();
        p.Data.writer.WriteUnicode(res, res.Length + 1);
        TcpHelper.AddRecordLine(p);
        switch ((YGOSharp.Network.Enums.PlayerType)player)
        {
            case YGOSharp.Network.Enums.PlayerType.Red:
                result = "[FF3030]" + result + "[-]";
                break;
            case YGOSharp.Network.Enums.PlayerType.Green:
                result = "[7CFC00]" + result + "[-]";
                break;
            case YGOSharp.Network.Enums.PlayerType.Blue:
                result = "[4876FF]" + result + "[-]";
                break;
            case YGOSharp.Network.Enums.PlayerType.BabyBlue:
                result = "[63B8FF]" + result + "[-]";
                break;
            case YGOSharp.Network.Enums.PlayerType.Pink:
                result = "[EED2EE]" + result + "[-]";
                break;
            case YGOSharp.Network.Enums.PlayerType.Yellow:
                result = "[EEEE00]" + result + "[-]";
                break;
            case YGOSharp.Network.Enums.PlayerType.White:
                result = "[FAF0E6]" + result + "[-]";
                break;
            case YGOSharp.Network.Enums.PlayerType.Gray:
                result = "[CDC9C9]" + result + "[-]";
                break;
        }
        RMSshow_none(result);
    }

    public void StocMessage_Replay(BinaryReader r)
    {
        byte[] data = r.ReadToEnd();
        Package p = new Package();
        p.Fuction = (int)YGOSharp.OCGWrapper.Enums.GameMessage.sibyl_replay;
        p.Data = new BinaryMaster();
        p.Data.writer.Write(data);
        TcpHelper.AddRecordLine(p);
        TcpHelper.SaveRecord(); 
    }

    public bool duelEnded = false;

    public void StocMessage_DuelEnd(BinaryReader r)
    {
        duelEnded = true;
        Program.I().ocgcore.forceMSquit();
    }

    public void StocMessage_DuelStart(BinaryReader r)
    {

        Program.I().ocgcore.returnServant = Program.I().selectServer;
        needSide = false;
        if (Program.I().deckManager.isShowed)
        {
            Program.I().deckManager.hide();
            RMSshow_onlyYes("",InterString.Get("更换副卡组成功，请等待对手更换副卡组。"),null);
        }
        if (isShowed)
        {
            hide();
        }
        if (selftype < 4)
        {
            Program.I().ocgcore.shiftCondition(Ocgcore.Condition.duel);
        }
        else
        {
            Program.I().ocgcore.shiftCondition(Ocgcore.Condition.watch);
        }
        Program.I().ocgcore.showBarOnly();
    }

    public void StocMessage_TypeChange(BinaryReader r)
    {
        int type = r.ReadByte();
        selftype = type & 0xf;
        is_host = ((type >> 4) & 0xf) != 0;
        if (is_host)
        {
            UIHelper.shiftButton(startButton(), true);
            lazyRoom.start.localScale = Vector3.one;
            lazyRoom.duelist.localPosition = new Vector3(lazyRoom.duelist.localPosition.x, -94.2f, 0);
            lazyRoom.observer.localPosition = new Vector3(lazyRoom.duelist.localPosition.x, -94.2f-30f, 0);
            lazyRoom.start.localPosition = new Vector3(lazyRoom.duelist.localPosition.x, -94.2f - 30f - 30f, 0);
        }
        else
        {
            UIHelper.shiftButton(startButton(), false);
            lazyRoom.start.localScale = Vector3.zero;
            lazyRoom.duelist.localPosition = new Vector3(lazyRoom.duelist.localPosition.x, -94.2f - 30f, 0);
            lazyRoom.observer.localPosition = new Vector3(lazyRoom.duelist.localPosition.x, -94.2f - 30f - 30f, 0);
            lazyRoom.start.localPosition = new Vector3(lazyRoom.duelist.localPosition.x, -94.2f - 30f - 30f - 30f, 0);
        }
        realize();
    }

    public void StocMessage_JoinGame(BinaryReader r)
    {
        lflist = r.ReadUInt32();
        rule = r.ReadByte();
        mode = r.ReadByte();
        Program.I().ocgcore.MasterRule = r.ReadChar();
        no_check_deck = r.ReadBoolean();
        no_shuffle_deck = r.ReadBoolean();
        r.ReadByte();
        r.ReadByte();
        r.ReadByte();
        start_lp = r.ReadInt32();
        start_hand = r.ReadByte();
        draw_count = r.ReadByte();
        time_limit = r.ReadInt16();
        ini();
        Program.I().shiftToServant(Program.I().room);
    }

    public bool sideWaitingObserver = false;

    public void StocMessage_WaitingSide(BinaryReader r)
    {
        sideWaitingObserver = true;
        RMSshow_none(InterString.Get("请耐心等待双方玩家更换副卡组。"));
    }

    public bool needSide = false;

    public void StocMessage_ChangeSide(BinaryReader r)
    {
        Program.I().ocgcore.surrended = false;
        Program.I().ocgcore.returnServant = Program.I().deckManager;
        needSide = true;
        
    }

    GameObject handres = null;
    public void StocMessage_HandResult(BinaryReader r)
    {
        if (isShowed)
        {
            hide();
        }
        int meResult = r.ReadByte();  
        int opResult = r.ReadByte();
        Program.I().new_ui_handShower.GetComponent<handShower>().me = meResult - 1;
        Program.I().new_ui_handShower.GetComponent<handShower>().op = opResult - 1;
        handres = create(Program.I().new_ui_handShower, Vector3.zero, Vector3.zero, false, Program.ui_main_2d);
        destroy(handres, 10f);
        animationTime = 1300;
    }

    public void StocMessage_SelectTp(BinaryReader r)
    {
        if (animationTime != 0)
        {
            Program.go(animationTime, () =>
            {
                RMSshow_FS("StocMessage_SelectTp", new messageSystemValue { hint = InterString.Get("先攻"), value = "first" }, new messageSystemValue { hint = InterString.Get("后攻"), value = "second" });
            });
            animationTime = 0;
        }
        else
        {
            RMSshow_FS("StocMessage_SelectTp", new messageSystemValue { hint = InterString.Get("先攻"), value = "first" }, new messageSystemValue { hint = InterString.Get("后攻"), value = "second" });
        }
    }

    public override void ES_RMS(string hashCode, List<messageSystemValue> result)
    {
        base.ES_RMS(hashCode, result);
        if (hashCode == "StocMessage_SelectTp")
        {
            if (result[0].value == "first")
            {
                TcpHelper.CtosMessage_TpResult(true);
            }
            if (result[0].value == "second")
            {
                TcpHelper.CtosMessage_TpResult(false);
            }
        }
        if (hashCode == "StocMessage_SelectHand")
        {
            if (result[0].value == "jiandao")
            {
                TcpHelper.CtosMessage_HandResult(1);
            }
            if (result[0].value == "shitou")
            {
                TcpHelper.CtosMessage_HandResult(2);
            }
            if (result[0].value == "bu")
            {
                TcpHelper.CtosMessage_HandResult(3);
            }
        }
    }

    public void StocMessage_SelectHand(BinaryReader r)
    {

        if (animationTime != 0)
        {
            Program.go(animationTime, () =>
            {
                hide();
                RMSshow_tp("StocMessage_SelectHand"
                    , new messageSystemValue { hint = "jiandao", value = "jiandao" }
                    , new messageSystemValue { hint = "shitou", value = "shitou" }
                    , new messageSystemValue { hint = "bu", value = "bu" });
            });
            animationTime = 0;
        }
        else
        {
            hide();
            RMSshow_tp("StocMessage_SelectHand"
                    , new messageSystemValue { hint = "jiandao", value = "jiandao" }
                    , new messageSystemValue { hint = "shitou", value = "shitou" }
                    , new messageSystemValue { hint = "bu", value = "bu" });
        }
    }

    public void StocMessage_ErrorMsg(BinaryReader r)
    {
        int msg = r.ReadByte();
        int code = 0;
        switch (msg)    
        {
            case 1:
                r.ReadByte();
                r.ReadByte();
                r.ReadByte();
                code = r.ReadInt32();
                switch (code)
                {
                    case 0:
                        RMSshow_onlyYes("", GameStringManager.get_unsafe(1403), null);
                        break;
                    case 1:
                        RMSshow_onlyYes("", GameStringManager.get_unsafe(1404), null);
                        break;
                    case 2:
                        RMSshow_onlyYes("", GameStringManager.get_unsafe(1405), null);
                        break;
                }
                break;
            case 2:
                r.ReadByte();
                r.ReadByte();
                r.ReadByte();
                code = r.ReadInt32();
                int flag = code >> 28;
                code = code & 0xFFFFFFF;
                switch (flag)
                {
                    case 1: // DECKERROR_LFLIST
                        RMSshow_onlyYes("", InterString.Get("卡组非法，请检查：[?]", YGOSharp.CardsManager.Get(code).Name) + "（数量不符合禁限卡表）", null);
                        break;
                    case 2: // DECKERROR_OCGONLY
                        RMSshow_onlyYes("", InterString.Get("卡组非法，请检查：[?]", YGOSharp.CardsManager.Get(code).Name) + "（OCG独有卡，不能在当前设置使用）", null);
                        break;
                    case 3: // DECKERROR_TCGONLY
                        RMSshow_onlyYes("", InterString.Get("卡组非法，请检查：[?]", YGOSharp.CardsManager.Get(code).Name) + "（TCG独有卡，不能在当前设置使用）", null);
                        break;
                    case 4: // DECKERROR_UNKNOWNCARD
                        if (code < 100000000)
                            RMSshow_onlyYes("", InterString.Get("卡组非法，请检查：[?]", YGOSharp.CardsManager.Get(code).Name) + "（服务器无法识别此卡，可能是服务器未更新）", null);
                        else
                            RMSshow_onlyYes("", InterString.Get("卡组非法，请检查：[?]", YGOSharp.CardsManager.Get(code).Name) + "（服务器无法识别此卡，可能是服务器不支持先行卡或此先行卡已正式更新）", null);
                        break;
                    case 5: // DECKERROR_CARDCOUNT
                        RMSshow_onlyYes("", InterString.Get("卡组非法，请检查：[?]", YGOSharp.CardsManager.Get(code).Name) + "（数量过多）", null);
                        break;
                    case 6: // DECKERROR_MAINCOUNT
                        RMSshow_onlyYes("", "主卡组数量应为40-60张", null);
                        break;
                    case 7: // DECKERROR_EXTRACOUNT
                        RMSshow_onlyYes("", "额外卡组数量应为0-15张", null);
                        break;
                    case 8: // DECKERROR_SIDECOUNT
                        RMSshow_onlyYes("", "副卡组数量应为0-15", null);
                        break;
                    default:
                        RMSshow_onlyYes("", GameStringManager.get_unsafe(1406), null);
                        break;
                }
                break;
            case 3:
                RMSshow_onlyYes("", InterString.Get("更换副卡组失败，请检查卡片张数是否一致。"), null);
                break;
            case 4:
                r.ReadByte();
                r.ReadByte();
                r.ReadByte();
                code = r.ReadInt32();
                string hexOutput = "0x"+String.Format("{0:X}", code);
                Program.I().selectServer.set_version(hexOutput);
                RMSshow_none(InterString.Get("你输入的版本号和服务器不一致,[7CFC00]YGOPro2已经智能切换版本号[-]，请重新链接。"));
                break;
            default:
                break;
        }
    }

    public void StocMessage_GameMsg(BinaryReader r)
    {
        showOcgcore();  
        Package p = new Package();
        p.Fuction = r.ReadByte();
        p.Data = new BinaryMaster(r.ReadToEnd());
        Program.I().ocgcore.addPackage(p);
    }

    void showOcgcore()
    {
        if (handres != null)
        {
            destroy(handres);
            handres = null;
        }
        if (Program.I().ocgcore.isShowed == false)
        {
            Program.camera_game_main.transform.position = new Vector3(0, 230, -230);
            if (mode != 2)
            {
                if (selftype == 1)
                {
                    Program.I().ocgcore.name_0 = roomPlayers[1].name;
                    Program.I().ocgcore.name_1 = roomPlayers[0].name;
                    Program.I().ocgcore.name_0_tag = "---";
                    Program.I().ocgcore.name_1_tag = "---";
                }
                else
                {
                    Program.I().ocgcore.name_0 = roomPlayers[0].name;
                    Program.I().ocgcore.name_1 = roomPlayers[1].name;
                    Program.I().ocgcore.name_0_tag = "---";
                    Program.I().ocgcore.name_1_tag = "---";
                }
            }
            else
            {
                if (selftype == 2 || selftype == 3)
                {
                    Program.I().ocgcore.name_0 = roomPlayers[2].name;
                    Program.I().ocgcore.name_1 = roomPlayers[0].name;
                    Program.I().ocgcore.name_0_tag = roomPlayers[3].name;
                    Program.I().ocgcore.name_1_tag = roomPlayers[1].name;
                }
                else
                {
                    Program.I().ocgcore.name_0 = roomPlayers[0].name;
                    Program.I().ocgcore.name_1 = roomPlayers[2].name;
                    Program.I().ocgcore.name_0_tag = roomPlayers[1].name;
                    Program.I().ocgcore.name_1_tag = roomPlayers[3].name;
                }
            }
            Program.I().ocgcore.timeLimit = time_limit;
            Program.I().ocgcore.lpLimit = start_lp;
            Program.I().ocgcore.InAI = false;
            Program.notGo(showCoreHandler);
            Program.go(10, showCoreHandler);
        }
    }

    private static void showCoreHandler()
    {
        Program.I().shiftToServant(Program.I().ocgcore);
    }

    public void StocMessage_CreateGame(BinaryReader r)
    {
    }

    public void StocMessage_LeaveGame(BinaryReader r)
    {
    }

    public void StocMessage_TpResult(BinaryReader r)
    {
    }

    public class RoomPlayer
    {
        public string name = "";
        public bool prep = false;
    }

    public RoomPlayer[] roomPlayers = new RoomPlayer[32];

    lazyPlayer[] realPlayers = new lazyPlayer[4];

    public UInt32 lflist;
    public byte rule;
    public byte mode;
    public bool no_check_deck;
    public bool no_shuffle_deck;
    public int start_lp = 8000;
    public byte start_hand;
    public byte draw_count;
    public short time_limit = 180;
    public int countOfObserver = 0;

    public int selftype;
    public bool is_host;

    void realize()
    {
        string description = "";
        if (mode == 0)
        {
            description += InterString.Get("单局模式");
        }
        if (mode == 1)
        {
            description += InterString.Get("比赛模式");
        }
        if (mode == 2)
        {
            description += InterString.Get("双打模式");
        }
        if (Program.I().ocgcore.MasterRule == 4)
        {
            description += InterString.Get("/新大师规则") + "\r\n";
        }
        else
        {
            description += InterString.Get("/大师规则[?]", Program.I().ocgcore.MasterRule.ToString()) + "\r\n";
        }
        description += InterString.Get("禁限卡表:[?]", YGOSharp.BanlistManager.GetName(lflist)) + "\r\n";
        if (rule == 0)
        {
            description += InterString.Get("(OCG卡池)") + "\r\n";
        }
        if (rule == 1)
        {
            description += InterString.Get("(TCG卡池)") + "\r\n";
        }
        if (rule == 2)
        {
            description += InterString.Get("(混合卡池)") + "\r\n";
        }
        if (no_check_deck)
        {
            description += InterString.Get("*不检查卡组") + "\r\n";
        }
        if (no_shuffle_deck)
        {
            description += InterString.Get("*不洗牌") + "\r\n";
        }
        description += InterString.Get("LP:[?]", start_lp.ToString()) + " ";
        description += InterString.Get("手牌:[?]", start_hand.ToString()) + " \r\n";
        description += InterString.Get("抽卡:[?]", draw_count.ToString()) + " ";
        description += InterString.Get("时间:[?]", time_limit.ToString()) + "\r\n";
        description += InterString.Get("观战者人数:[?]", countOfObserver.ToString());
        UIHelper.trySetLableText(gameObject, "description_", description);
        Program.I().ocgcore.name_0 = "---";
        Program.I().ocgcore.name_1 = "---";
        Program.I().ocgcore.name_0_tag = "---";
        Program.I().ocgcore.name_1_tag = "---";
        for (int i = 0; i < 4; i++)
        {
            realPlayers[i] = UIHelper.getByName<lazyPlayer>(gameObject, i.ToString());
            if (roomPlayers[i] == null)
            {
                realPlayers[i].SetNotNull(false);
            }
            else
            {
                realPlayers[i].SetNotNull(true);
                realPlayers[i].setName(roomPlayers[i].name);
                realPlayers[i].SetIFcanKick(is_host&&(i != selftype));
                realPlayers[i].setIfMe(i == selftype);
                realPlayers[i].setIfprepared(roomPlayers[i].prep);
                if (mode != 2)
                {
                    if (i == 0)
                    {
                        Program.I().ocgcore.name_0 = roomPlayers[i].name;
                    }
                    if (i == 1)
                    {
                        Program.I().ocgcore.name_1 = roomPlayers[i].name;
                    }
                    Program.I().ocgcore.name_0_tag = "---";
                    Program.I().ocgcore.name_1_tag = "---";
                }
                else
                {
                    if (i == 0)
                    {
                        Program.I().ocgcore.name_0 = roomPlayers[i].name;
                    }
                    if (i == 1)
                    {
                        Program.I().ocgcore.name_0_tag = roomPlayers[i].name;
                    }
                    if (i == 2)
                    {
                        Program.I().ocgcore.name_1 = roomPlayers[i].name;
                    }
                    if (i == 3)
                    {
                        Program.I().ocgcore.name_1_tag = roomPlayers[i].name;
                    }
                }
            }
        }
    }

    lazyRoom lazyRoom = null;
    void ini()
    {
        for (int i = 0; i < 4; i++)
        {
            roomPlayers[i] = null;
        }
        if (gameObject != null)
        {
            ES_quit();
        }
        MonoBehaviour.DestroyImmediate(gameObject);
        if (mode == 2)
        {
            createWindow(Program.I().remaster_tagRoom);
        }
        else
        {
            createWindow(Program.I().remaster_room);
        }
        lazyRoom = gameObject.GetComponent<lazyRoom>();
        fixScreenProblem();
        superScrollView = gameObject.GetComponentInChildren<UIselectableList>();
        superScrollView.selectedAction = onSelected;
        superScrollView.install();
        printFile();
        superScrollView.selectedString = Config.Get("deckInUse", "miaowu");
        superScrollView.toTop();
        if (mode == 0)
        {
            UIHelper.trySetLableText(gameObject, "Rname_", InterString.Get("单局房间"));
        }
        if (mode == 1)
        {
            UIHelper.trySetLableText(gameObject, "Rname_", InterString.Get("比赛房间"));
        }
        if (mode == 2)
        {
            UIHelper.trySetLableText(gameObject, "Rname_", InterString.Get("双打房间"));
        }

        UIHelper.trySetLableText(gameObject, "description_", "");
        for (int i = 0; i < 4; i++) 
        {
            realPlayers[i] = UIHelper.getByName<lazyPlayer>(gameObject, i.ToString());
        }

        for (int i = 0; i < 4; i++)
        {
            realPlayers[i].ini();
            realPlayers[i].onKick = OnKick;
            realPlayers[i].onPrepareChanged = onPrepareChanged;
        }

        UIHelper.shiftButton(startButton(), false);
        UIHelper.registUIEventTriggerForClick(startButton().gameObject, listenerForClicked);
        UIHelper.registUIEventTriggerForClick(exitButton().gameObject, listenerForClicked);
        UIHelper.registUIEventTriggerForClick(duelistButton().gameObject, listenerForClicked);
        UIHelper.registUIEventTriggerForClick(observerButton().gameObject, listenerForClicked);
        realize();
        superScrollView.refreshForOneFrame();
    }

    private void onPrepareChanged(int arg1, bool arg2)
    {
        if (roomPlayers[arg1] != null)
        {
            roomPlayers[arg1].prep = arg2;
        }
        if (arg2)
        {
            TcpHelper.CtosMessage_UpdateDeck(new YGOSharp.Deck("deck/" + Config.Get("deckInUse","miaouwu") + ".ydk"));
            TcpHelper.CtosMessage_HsReady();
        }
        else
        {
            TcpHelper.CtosMessage_HsNotReady();
        }
    }

    private void OnKick(int pos)
    {
        TcpHelper.CtosMessage_HsKick(pos);
    }

    private UIButton startButton()
    {
        return UIHelper.getByName<UIButton>(gameObject, "start_");
    }

    private UIButton exitButton()
    {
        return UIHelper.getByName<UIButton>(gameObject, "exit_");
    }

    private UIButton duelistButton()
    {
        return UIHelper.getByName<UIButton>(gameObject, "duelist_");
    }

    private UIButton observerButton()
    {
        return UIHelper.getByName<UIButton>(gameObject, "observer_");
    }

    void listenerForClicked(GameObject gameObjectListened)
    {
        if (gameObjectListened.name == "exit_")
        {
            Program.I().ocgcore.onExit();
        }
        if (gameObjectListened.name == "duelist_")
        {
            TcpHelper.CtosMessage_HsToDuelist();
        }
        if (gameObjectListened.name == "observer_")
        {
            TcpHelper.CtosMessage_HsToObserver();
        }
        if (gameObjectListened.name == "start_")
        {
            TcpHelper.CtosMessage_HsStart();
        }
    }

    #endregion

}
