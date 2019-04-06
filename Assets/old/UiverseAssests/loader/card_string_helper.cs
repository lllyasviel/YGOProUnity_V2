//using System;
//public class GameStringHelper
//{
//    public static bool differ(long a, long b)
//    {
//        bool r = false;
//        if ((a & b) > 0) r = true;
//        return r;
//    }
//    public static string get_attribute_string(long a)
//    {
//        string r = "";
//        if (differ(a, (long)CardAttribute.Earth)) r += "地";
//        if (differ(a, (long)CardAttribute.Water)) r += "水";
//        if (differ(a, (long)CardAttribute.Fire)) r += "炎";
//        if (differ(a, (long)CardAttribute.Wind)) r += "风";
//        if (differ(a, (long)CardAttribute.Dark)) r += "暗";
//        if (differ(a, (long)CardAttribute.Light)) r += "光";
//        if (differ(a, (long)CardAttribute.Devine)) r += "神";
//        r += "属性";
//        return r;
//    }
//    public static string get_type_string(int a)
//    {
//        string r = "";
//        if (differ(a, (long)CardType.Monster)) r += "怪物卡";
//        if (differ(a, (long)CardType.Spell)) r += "魔法卡";
//        if (differ(a, (long)CardType.Trap)) r += "陷阱卡";
//        return r;
//    }
//    public static string get_detailed_type_string(int a)
//    {
//        string r = "";
//        if (differ(a, (long)CardType.Monster)) r += "怪物卡";
//        if (differ(a, (long)CardType.Spell)) r += "魔法卡";
//        if (differ(a, (long)CardType.Trap)) r += "陷阱卡";
//        return r;
//    }
//    public static string get_race_string(long a)
//    {
//        string r = "";
//        if (differ(a, (long)game_race.RACE_WARRIOR)) r += "战士";
//        if (differ(a, (long)game_race.RACE_SPELLCASTER)) r += "魔法师";
//        if (differ(a, (long)game_race.RACE_FAIRY)) r += "天使";
//        if (differ(a, (long)game_race.RACE_FIEND)) r += "恶魔";
//        if (differ(a, (long)game_race.RACE_ZOMBIE)) r += "不死";
//        if (differ(a, (long)game_race.RACE_MACHINE)) r += "机械";
//        if (differ(a, (long)game_race.RACE_AQUA)) r += "水";
//        if (differ(a, (long)game_race.RACE_PYRO)) r += "炎";
//        if (differ(a, (long)game_race.RACE_ROCK)) r += "岩石";
//        if (differ(a, (long)game_race.RACE_WINDBEAST)) r += "鸟兽";
//        if (differ(a, (long)game_race.RACE_PLANT)) r += "植物";
//        if (differ(a, (long)game_race.RACE_INSECT)) r += "昆虫";
//        if (differ(a, (long)game_race.RACE_THUNDER)) r += "雷";
//        if (differ(a, (long)game_race.RACE_DRAGON)) r += "龙";
//        if (differ(a, (long)game_race.RACE_BEAST)) r += "兽";
//        if (differ(a, (long)game_race.RACE_BEASTWARRIOR)) r += "兽战士";
//        if (differ(a, (long)game_race.RACE_DINOSAUR)) r += "恐龙";
//        if (differ(a, (long)game_race.RACE_FISH)) r += "鱼";
//        if (differ(a, (long)game_race.RACE_SEASERPENT)) r += "海龙";
//        if (differ(a, (long)game_race.RACE_REPTILE)) r += "爬虫";
//        if (differ(a, (long)game_race.RACE_PSYCHO)) r += "念动力";
//        if (differ(a, (long)game_race.RACE_DEVINE)) r += "幻神兽";
//        if (differ(a, (long)game_race.RACE_CREATORGOD)) r += "创造神";
//        if (differ(a, (long)game_race.RACE_PHANTOMDRAGON)) r += "幻龙";
//        r += "族";
//        return r;
//    }
//    public static string get_point_string(point p)
//    {
//        string re = "";
//        if (p.me)
//        {
//            re += "我的";
//        }
//        else
//        {
//            re += "对方的";
//        }
//        switch (p.location)
//        {
//            case CardLocation.Deck:
//                re += "卡组中的";
//                break;
//            case CardLocation.Extra:
//                re += "额外卡组中的";
//                break;
//            case CardLocation.Grave:
//                re += "墓地中的";
//                break;
//            case CardLocation.Hand:
//                re += "手上的";
//                break;
//            case CardLocation.MonsterZone:
//                re += "前场的";
//                break;
//            case CardLocation.Hand:
//                re += "被除外的";
//                break;
//            case CardLocation.SpellZone:
//                re += "后场的";
//                break;
//        }
//        re += "第" + p.index.ToString() + "张卡";
//        if (p.overlay_index > 0)
//        {
//            re += "下面第" + p.overlay_index.ToString() + "张被叠放的卡";
//        }
//        switch (p.position)
//        {
//            case CardPosition.FaceDownAttack:
//                re += "(里攻)";
//                break;
//            case CardPosition.FaceDown_DEFENSE:
//                re += "(里守)";
//                break;
//            case CardPosition.FaceUpAttack:
//                re += "(表攻)";
//                break;
//            case CardPosition.FaceUp_DEFENSE:
//                re += "(表守)";
//                break;
//        }
//        if (p.location == CardLocation.Unknown) re = "(未知区域)";
//        return re;
//    }
//    public static string get_name_string(CardData data)
//    {
//        string re = "";
//        if (differ(data.Type, (long)CardType.Monster)) re += "[ff8000]"+data.Name;
//        else if (differ(data.Type, (long)CardType.Spell)) re += "[7FFF00]" + data.Name;
//        else if (differ(data.Type, (long)CardType.Trap)) re += "[dda0dd]" + data.Name;
//        else re += "[ff8000]" + data.Name;
//        re += "[-]";
//        return re;
//    }
//    public static string get_string(CardData data,bool desc=true)
//    {
//        string re = "";
//        if ((data.Type & 0x1) > 0)
//        {
//            re += "[ff8000]";
//            if ((data.Type & 0x10) > 0)
//            {
//                re += "通常|";
//            }
//            if ((data.Type & 0x20) > 0)
//            {
//                re += "效果|";
//            }
//            if ((data.Type & 0x40) > 0)
//            {
//                re += "融合|";
//            }
//            if ((data.Type & 0x80) > 0)
//            {
//                re += "仪式|";
//            }
//            if ((data.Type & 0x100) > 0)
//            {
//                re += "陷阱怪物|";
//            }
//            if ((data.Type & 0x200) > 0)
//            {
//                re += "灵魂|";
//            }
//            if ((data.Type & 0x400) > 0)
//            {
//                re += "同盟|";
//            }
//            if ((data.Type & 0x800) > 0)
//            {
//                re += "二重|";
//            }
//            if ((data.Type & 0x1000) > 0)
//            {
//                re += "调整|";
//            }
//            if ((data.Type & 0x2000) > 0)
//            {
//                re += "同调|";
//            }
//            if ((data.Type & 0x4000) > 0)
//            {
//                re += "衍生物|";
//            }
//            if ((data.Type & 0x200000) > 0)
//            {
//                re += "翻转|";
//            }
//            if ((data.Type & 0x400000) > 0)
//            {
//                re += "卡通|";
//            }
//            if ((data.Type & 0x800000) > 0)
//            {
//                re += "超量|";
//            }
//            if ((data.Type & 0x1000000) > 0)
//            {
//                re += "灵摆|";
//            }
//            re += get_race_string(data.Race) + "|" + get_attribute_string(data.Attribute) + "|" + data.Level.ToString() + "[sup]★[/sup]";
//            if (data.LeftScale > 0) re += "|" + data.LeftScale.ToString() + "[sup]刻度[/sup]";
//            re += "\n";
//            if (data.Attack < 0)
//            {
//                re += "[sup]ATK[/sup]?  ";
//            }
//            else
//            {
//                re += "[sup]ATK[/sup]" + data.Attack.ToString() + "  ";
//            }
//            if (data.Defense < 0)
//            {
//                re += "[sup]DEF[/sup]?[-]\n";
//            }
//            else
//            {
//                re += "[sup]DEF[/sup]" + data.Defense.ToString() + "[-]\n";
//            }
//        }
//        if ((data.Type & 0x2) > 0)
//        {
//            re += "[7FFF00]";
//            if ((data.Type & 0x10000) > 0)
//            {
//                re += "速攻";
//            }
//            if ((data.Type & 0x20000) > 0)
//            {
//                re += "永续";
//            }
//            if ((data.Type & 0x40000) > 0)
//            {
//                re += "装备";
//            }
//            if ((data.Type & 0x80000) > 0)
//            {
//                re += "场地";
//            }
//            if ((data.Type & 0x80) > 0)
//            {
//                re += "仪式";
//            }
//            if (data.LeftScale > 0) re += "|" + data.LeftScale.ToString() + "[sup]刻度[/sup]";
//            re += "[-]\n";

//        }
//        if ((data.Type & 0x4) > 0)
//        {
//            re += "[dda0dd]";
//            if ((data.Type & 0x20000) > 0)
//            {
//                re += "永续";
//            }
//            if ((data.Type & 0x100000) > 0)
//            {
//                re += "反击";
//            }
//            re += "[-]\n";
//        }
//        if (data.tail.Length > 2)
//        {
//            re += data.tail;
//        }
//        if(desc)
//        {
//            if (re.Length > 12) re += data.Desc;
//            else re = data.Desc;
//        }
//        return re;
//    }
//    public static string get_phase_string(int a)
//    {
//        string r = "";
//        if (differ(a, (long)game_phrases.PHASE_BATTLE)) r += "战斗阶段";
//        if (differ(a, (long)game_phrases.PHASE_BATTLE_START)) r += "战斗开始";
//        if (differ(a, (long)game_phrases.PHASE_BATTLE_STEP)) r += "战斗步骤";
//        if (differ(a, (long)game_phrases.PHASE_DAMAGE)) r += "伤害步骤";
//        if (differ(a, (long)game_phrases.PHASE_DAMAGE_CAL)) r += "伤害计算";
//        if (differ(a, (long)game_phrases.PHASE_DRAW)) r += "抽卡阶段";
//        if (differ(a, (long)game_phrases.PHASE_END)) r += "回合结束";
//        if (differ(a, (long)game_phrases.PHASE_MAIN1)) r += "主要阶段";
//        if (differ(a, (long)game_phrases.PHASE_MAIN2)) r += "主要阶段";
//        if (differ(a, (long)game_phrases.PHASE_STANDBY)) r += "准备阶段";
//        return r;
//    }
//}
