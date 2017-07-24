using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using YGOSharp.OCGWrapper.Enums;

public class PrecyOcg
{
    public static string HintInGame = Percy.smallYgopro.HintInGame;
    
    public static bool godMode = false;

    public Percy.smallYgopro ygopro;

    static string error = "Error occurred.";

    public PrecyOcg()
    {
        error = InterString.Get("Error occurred! @nError occurred! @nError occurred! @nError occurred! @nError occurred! @nError occurred! @nYGOPro1旧版的录像崩溃了！您可以选择使用永不崩溃的新版录像。");
        ygopro = new Percy.smallYgopro(receiveHandler, cardHandler, chatHandler);
        ygopro.m_log = (a) => { Program.DEBUGLOG(a); };
    }

    public void dispose()
    {
        ygopro.dispose();
    }

    object locker = new object();
    void receiveHandler(byte[] buffer)
    {
        byte[] bufferR = new byte[buffer.Length + 1];
        bufferR[0] = 1;
        buffer.CopyTo(bufferR,1);
        TcpHelper.addDateJumoLine(bufferR);
    }

    public void startPuzzle(System.String path)
    {
        if (Program.I().ocgcore.isShowed == false)
        {
            Program.I().room.mode = 0;
            Program.I().ocgcore.MasterRule = 3;
            godMode = true;
            prepareOcgcore();
            Program.I().ocgcore.isFirst = true;
            Program.I().ocgcore.returnServant = Program.I().puzzleMode;
            if (!ygopro.startPuzzle(path))
            {
                Program.I().cardDescription.RMSshow_none(InterString.Get("游戏内部出错，请重试，文件名中不能包含中文。"));
                return;
            }
            else
            {
                Config.ClientVersion = 0x233c;
                Program.I().shiftToServant(Program.I().ocgcore);
            }
        ((CardDescription)Program.I().cardDescription).setTitle(path);
        }
    }

    public void startAI(string playerDek, string aiDeck, string aiScript, bool playerGo, bool unrand, int life,bool god,int rule)
    {
        if (Program.I().ocgcore.isShowed == false)
        {
            Program.I().room.mode = 0;
            Program.I().ocgcore.MasterRule = rule;
            godMode = god;
            prepareOcgcore();
            Program.I().ocgcore.lpLimit = life;
            Program.I().ocgcore.isFirst = playerGo;
            Program.I().ocgcore.returnServant = Program.I().aiRoom;
            if (!ygopro.startAI(playerDek, aiDeck, aiScript, playerGo, unrand, life, god, rule))
            {
                Program.I().cardDescription.RMSshow_none(InterString.Get("游戏内部出错，请重试，文件名中不能包含中文。"));
                return;
            }
            else
            {
                Config.ClientVersion = 0x233c;
                Program.I().shiftToServant(Program.I().ocgcore);
            }
        }
    }

    private void prepareOcgcore()
    {
        Program.I().ocgcore.name_0 = Config.Get("name","一秒一喵机会");
        Program.I().ocgcore.name_0_c = Program.I().ocgcore.name_0;
        Program.I().ocgcore.name_1 = "Percy AI";
        Program.I().ocgcore.name_1_c = "Percy AI";
        Program.I().ocgcore.name_0_tag = "---";
        Program.I().ocgcore.name_1_tag = "---";
        Program.I().ocgcore.timeLimit = 240;
        Program.I().ocgcore.lpLimit = 8000;
        Program.I().ocgcore.handler = response;
        Program.I().ocgcore.shiftCondition(Ocgcore.Condition.watch);
        Program.I().ocgcore.InAI = true;
    }

    public void response(byte[] resp)
    {
        ygopro.response(resp);
    }

    Percy.CardData cardHandler(long code)
    {
        YGOSharp.Card card = YGOSharp.CardsManager.GetCard((int)code);
        if (card==null) 
        {
            card = new YGOSharp.Card();
        }
        Percy.CardData retuvalue = new Percy.CardData();
        retuvalue.Alias = card.Alias;
        retuvalue.Attack = card.Attack;
        retuvalue.Attribute = card.Attribute;
        retuvalue.Code = card.Id;
        retuvalue.Defense = card.Defense;
        retuvalue.Level = card.Level;
        retuvalue.LScale = card.LScale;
        retuvalue.Race = card.Race;
        retuvalue.RScale = card.RScale;
        retuvalue.Setcode = card.Setcode;
        retuvalue.Type = card.Type;
        return retuvalue;
    }

    void chatHandler(string result) 
    {
        BinaryMaster p = new BinaryMaster();
        p.writer.Write((byte)YGOSharp.OCGWrapper.Enums.GameMessage.sibyl_chat);
        result = result.Replace("Error Occurred.", error);
        p.writer.WriteUnicode(result, result.Length + 1);
        receiveHandler(p.get());
    }
}