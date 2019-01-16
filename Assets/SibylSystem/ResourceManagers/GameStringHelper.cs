using System;
using YGOSharp;

public class GameStringHelper
{
    public static string fen = "/";
    public static string xilie = "";
    public static string opHint = "";
    public static string licechuwai = "";  
    public static string biaoceewai = "";
    public static string teshuzhaohuan = "";
    public static string yijingqueren = "";

    public static string _zhukazu = "";
    public static string _fukazu = "";
    public static string _ewaikazu = "";
    public static string _guaishou = "";
    public static string _mofa = "";
    public static string _xianjing = "";
    public static string _ronghe = "";
    public static string _lianjie = "";
    public static string _tongtiao = "";
    public static string _chaoliang = "";

    public static string _wofang = "";
    public static string _duifang = "";

    public static string kazu = "";
    public static string mudi = "";
    public static string chuwai = "";
    public static string ewai = "";

    public static bool differ(long a, long b)
    {
        bool r = false;
        if ((a & b) > 0) r = true;
        return r;
    }

    public static string attribute(long a)
    {
        string r = "";
        bool passFirst = false;
        for (int i = 0; i < 7; i++)
        {
            if ((a & (1 << i)) > 0)
            {
                if (passFirst)
                {
                    r += fen;
                }
                r += GameStringManager.get_unsafe(1010 + i);
                passFirst = true;
            }
        }
        return r;
    }

    public static string race(long a)
    {
        string r = "";
        bool passFirst = false;
        for (int i = 0; i < 25; i++)
        {
            if ((a & (1 << i)) > 0)
            {
                if (passFirst)
                {
                    r += fen;
                }
                r += GameStringManager.get_unsafe(1020 + i);
                passFirst = true;
            }
        }
        return r;
    }

    public static string mainType(long a)
    {
        string r = "";
        bool passFirst = false;
        for (int i = 0; i < 3; i++)
        {
            if ((a & (1 << i)) > 0)
            {
                if (passFirst)
                {
                    r += fen;
                }
                r += GameStringManager.get_unsafe(1050 + i);
                passFirst = true;
            }
        }
        return r;
    }

    public static string secondType(long a)
    {
        string r = "";
        bool passFirst = false;
        for (int i = 4; i < 27; i++)
        {
            if ((a & (1 << i)) > 0)
            {
                if (passFirst)
                {
                    r += fen;
                }
                r += GameStringManager.get_unsafe(1050 + i);
                passFirst = true;
            }
        }
        if (r == "")
        {
            r += GameStringManager.get_unsafe(1054);
        }
        return r;
    }

    public static string getName(YGOSharp.Card card)
    {
        string limitot = "";
        switch(card.Ot)
        {
        case 1:
            limitot = "[OCG] ";
            break;
        case 2:
            limitot = "[TCG] ";
            break;
        case 3:
            limitot = "[OCG/TCG] ";
            break;
        case 4:
            limitot = "[Anime] ";
            break;
        }
        string re = "";
        try
        {
            re += "[b]" + card.Name + "[/b]";
            re += "\n";
            re += "[sup]" + limitot + "[/sup]";
            re += "\n";
            re += "[sup]" + card.Id.ToString() + "[/sup]";
            re += "\n";
        }
        catch (Exception e)
        {
        }

        if (differ(card.Attribute,(int)game_attributes.ATTRIBUTE_EARTH))     
        {
            re = "[F4A460]" + re + "[-]";
        }
        if (differ(card.Attribute, (int)game_attributes.ATTRIBUTE_WATER))
        {
            re = "[D1EEEE]" + re + "[-]";
        }
        if (differ(card.Attribute, (int)game_attributes.ATTRIBUTE_FIRE))
        {
            re = "[F08080]" + re + "[-]";
        }
        if (differ(card.Attribute, (int)game_attributes.ATTRIBUTE_WIND))
        {
            re = "[B3EE3A]" + re + "[-]";
        }
        if (differ(card.Attribute, (int)game_attributes.ATTRIBUTE_LIGHT))
        {
            re = "[EEEE00]" + re + "[-]";
        }
        if (differ(card.Attribute, (int)game_attributes.ATTRIBUTE_DARK))
        {
            re = "[FF00FF]" + re + "[-]";
        }

        return re;
    }

    public static string getType(Card card)
    {
        string re = "";
        try
        {
            if (differ(card.Type, (long)game_type.TYPE_MONSTER)) re += "[ff8000]" + mainType(card.Type);
            else if (differ(card.Type, (long)game_type.TYPE_SPELL)) re += "[7FFF00]" + mainType(card.Type);
            else if (differ(card.Type, (long)game_type.TYPE_TRAP)) re += "[dda0dd]" + mainType(card.Type);
            else re += "[ff8000]" + mainType(card.Type);
            re += "[-]";
        }
        catch (Exception e)
        {
        }

        return re;
    }

    public static string getSmall(YGOSharp.Card data)
    {
        string re = "";

        try
        {
            if ((data.Type & 0x1) > 0)
            {
                re += "[ff8000]";
                re += "["+secondType(data.Type)+"]";

                if ((data.Type & (int)game_type.link) == 0)
                {
                    if ((data.Type & (int)game_type.TYPE_XYZ) > 0)
                    {
                        re += " " + race(data.Race) + fen + attribute(data.Attribute) + fen + data.Level.ToString() + "[sup]☆[/sup]";
                    }
                    else
                    {
                        re += " " + race(data.Race) + fen + attribute(data.Attribute) + fen + data.Level.ToString() + "[sup]★[/sup]";
                    }
                }
                else
                {
                    re += " " + race(data.Race) + fen + attribute(data.Attribute) ;
                }

                if (data.LScale > 0) re += fen + data.LScale.ToString() + "[sup]P[/sup]";
                re += "\n";
                if (data.Attack < 0)
                {
                    re += "[sup]ATK[/sup]?  ";
                }
                else
                {
                    if (data.rAttack>0) 
                    {
                        int pos = data.Attack - data.rAttack;
                        if (pos>0)  
                        {
                            re += "[sup]ATK[/sup]" + data.Attack.ToString() + "(↑" + pos.ToString() + ")  ";
                        }
                        if (pos < 0)
                        {
                            re += "[sup]ATK[/sup]" + data.Attack.ToString() + "(↓" + (-pos).ToString() + ")  ";
                        }
                        if (pos == 0)
                        {
                            re += "[sup]ATK[/sup]" + data.Attack.ToString() + "  ";
                        }
                    }
                    else
                    {
                        re += "[sup]ATK[/sup]" + data.Attack.ToString() + "  ";
                    }
                }
                if ((data.Type & (int)game_type.link) == 0)
                {
                    if (data.Defense < 0)
                    {
                        re += "[sup]DEF[/sup]?";
                    }
                    else
                    {
                        if (data.rAttack > 0)
                        {
                            int pos = data.Defense - data.rDefense;
                            if (pos > 0)
                            {
                                re += "[sup]DEF[/sup]" + data.Defense.ToString() + "(↑" + pos.ToString() + ")";
                            }
                            if (pos < 0)
                            {
                                re += "[sup]DEF[/sup]" + data.Defense.ToString() + "(↓" + (-pos).ToString() + ")";
                            }
                            if (pos == 0)
                            {
                                re += "[sup]DEF[/sup]" + data.Defense.ToString();
                            }
                        }
                        else
                        {
                            re += "[sup]DEF[/sup]" + data.Defense.ToString();
                        }
                    }
                }
                else
                {
                    re += "[sup]LINK[/sup]" + data.Level.ToString();
                }
            }
            else if ((data.Type & 0x2) > 0)
            {
                re += "[7FFF00]";
                re += secondType(data.Type);
                if (data.LScale > 0) re += fen + data.LScale.ToString() + "[sup]P[/sup]";
            }
            else if ((data.Type & 0x4) > 0)
            {
                re += "[dda0dd]";
                re += secondType(data.Type);
            }
            else
            {
                re += "[ff8000]";
            }
            if (data.Alias > 0)
            {
                if (data.Alias != data.Id)
                {
                    string name = YGOSharp.CardsManager.Get(data.Alias).Name;
                    if (name != data.Name)
                    {
                        re += "\n[" + YGOSharp.CardsManager.Get(data.Alias).Name + "]";
                    }
                }
            }
            if (data.Setcode > 0)
            {
                re += "\n";
                re += xilie;
                re += getSetName(data.Setcode);
            }
            re += "[-]";
        }
        catch (Exception e)
        {
        }
        return re;
    }

    public static string getSetName(long Setcode)
    {
        string returnValue = "";
        for (int i = 0; i < GameStringManager.xilies.Count; i++)
        {
            if (YGOSharp.CardsManager.IfSetCard(GameStringManager.xilies[i].hashCode, Setcode))
            {
                returnValue = GameStringManager.xilies[i].content + " ";
            }
        }

        return returnValue;
    }
}
