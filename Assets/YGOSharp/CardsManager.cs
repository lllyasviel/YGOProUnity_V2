using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace YGOSharp
{
    internal static class CardsManager
    {
        private static IDictionary<int, Card> _cards = new Dictionary<int, Card>();

        public static string nullName = "";

        public static string nullString = "";

        internal static void initialize(string databaseFullPath)
        {
            nullName = InterString.Get("未知卡片");
            nullString = "";
            nullString += "欢迎使用 YGOPro2 233测试版";
            nullString += "\r\n\r\n";
            nullString += "详见官方网站说明：";
            nullString += "\r\n";
            nullString += "[url=https://ygo233.com/ygopro2_233_test][u]https://ygo233.com/ygopro2_233_test[/u][/url]";
            nullString += "\r\n\r\n";
            nullString += "公测玩家交流群：\r\n[url=https://jq.qq.com/?_wv=1027&k=42Gsxgd][u]580454979[/u][/url]";
            nullString += "\r\n[url=https://jq.qq.com/?_wv=1027&k=44aGRzz][u]428563714[/u][/url]";
            using (SqliteConnection connection = new SqliteConnection("Data Source=" + databaseFullPath))
            {
                connection.Open();

                using (IDbCommand command = new SqliteCommand("SELECT datas.*, texts.* FROM datas,texts WHERE datas.id=texts.id;", connection))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LoadCard(reader);
                        }
                    }
                }
            }
        }

        internal static Card GetCard(int id)
        {
            if (_cards.ContainsKey(id))
                return _cards[id].clone();
            return null;
        }

        internal static Card GetCardRaw(int id)
        {
            if (_cards.ContainsKey(id))
                return _cards[id];
            return null;
        }

        internal static Card Get(int id)
        {
            Card returnValue = new Card();
            if (id > 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    returnValue = GetCard(id - i);
                    if (returnValue != null)
                    {
                        break;
                    }
                }
                if (returnValue == null)
                {
                    returnValue = new Card();
                }
            }
            return returnValue;
        }

        private static void LoadCard(IDataRecord reader)
        {
            Card card = new Card(reader);
            if (!_cards.ContainsKey(card.Id))
            {
                _cards.Add(card.Id, card);
            }
        }

        internal static List<Card> searchAdvanced(
            string getName,
            int getLevel,
            int getAttack,
            int getDefence,
            int getP,
            int getYear,
            int getLevel_UP,
            int getAttack_UP,
            int getDefence_UP,
            int getP_UP,
            int getYear_UP,
            string getPack,
            int getBAN,
            Banlist banlist,
            uint getTypeFilter,
            uint getRaceFilter,
            uint getAttributeFilter,
            uint getCatagoryFilter
            )
        {
            List<Card> returnValue = new List<Card>();
            foreach (var item in _cards)
            {
                Card card = item.Value;
                if ((card.Type & (uint)game_type.TYPE_TOKEN) == 0)
                {
                    if (getName == "" 
                        || Regex.Replace(card.Name, getName,"miaowu", RegexOptions.IgnoreCase) != card.Name
                        || Regex.Replace(card.Desc, getName, "miaowu", RegexOptions.IgnoreCase) != card.Desc
                        || Regex.Replace(card.strSetName, getName, "miaowu", RegexOptions.IgnoreCase) != card.strSetName
                        || card.Id.ToString() == getName
                        )
                    {
                        if (((card.Type & getTypeFilter)) == getTypeFilter || getTypeFilter == 0)
                        {
                            if ((card.Race & getRaceFilter) >0 || getRaceFilter == 0)
                            {
                                if ((card.Attribute & getAttributeFilter) > 0 || getAttributeFilter == 0)
                                {
                                    if (((card.Category & getCatagoryFilter))== getCatagoryFilter || getCatagoryFilter == 0)
                                    {
                                        if (judgeint(getAttack, getAttack_UP, card.Attack))
                                        {
                                            if (judgeint(getDefence, getDefence_UP, card.Defense))
                                            {
                                                if (judgeint(getLevel, getLevel_UP, card.Level))
                                                {
                                                    if (judgeint(getP, getP_UP, card.LScale))
                                                    {
                                                        if (judgeint(getYear, getYear_UP, card.year))
                                                        {
                                                            if (getBAN == -233 || banlist == null || banlist.GetQuantity(card.Id) == getBAN)
                                                            {
                                                                if (getPack == "" || card.packFullName == getPack)
                                                                {
                                                                    returnValue.Add(card);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            nameInSearch = getName;
            returnValue.Sort(comparisonOfCard());
            nameInSearch = "";
            return returnValue;
        }

        static string nameInSearch = "";

        static bool judgeint(int min, int max, int raw)
        {
            bool re = true;
            if (min == -233 && max == -233)
            {
                re = true;
            }
            if (min == -233 && max != -233)
            {
                re = max == raw;
            }
            if (min != -233 && max == -233)
            {
                re = min == raw;
            }
            if (min != -233 && max != -233)
            {
                re = min <= raw && raw <= max;
            }
            return re;
        }

        internal static List<Card> search(
          string getName,
            List<int> getsearchCode
            )
        {
            List<Card> returnValue = new List<Card>();
            foreach (var item in _cards)
            {
                Card card = item.Value;
                if (getName == ""
                        || Regex.Replace(card.Name, getName, "miaowu", RegexOptions.IgnoreCase) != card.Name
                        //|| Regex.Replace(card.Desc, getName, "miaowu", RegexOptions.IgnoreCase) != card.Desc
                        || Regex.Replace(card.strSetName, getName, "miaowu", RegexOptions.IgnoreCase) != card.strSetName
                        || card.Id.ToString() == getName
                        )
                {
                    if (getsearchCode.Count == 0|| is_declarable(card, getsearchCode))
                    {
                        returnValue.Add(card);
                    }
                }
            }
            nameInSearch = getName;
            returnValue.Sort(comparisonOfCard());
            nameInSearch = "";
            return returnValue;
        }

        private static bool is_declarable(Card card, List<int> getsearchCode)
        {
            Stack<int> stack = new Stack<int>();
            for (int i = 0; i < getsearchCode.Count; i++)
            {
                switch (getsearchCode[i])
                {
                    case (int)OCGWrapper.Enums.searchCode.OPCODE_ADD:
                        if (stack.Count >= 2)
                        {
                            int rhs = stack.Pop();
                            int lhs = stack.Pop();
                            stack.Push(lhs + rhs);
                        }
                        break;
                    case (int)OCGWrapper.Enums.searchCode.OPCODE_SUB:
                        if (stack.Count >= 2)
                        {
                            int rhs = stack.Pop();
                            int lhs = stack.Pop();
                            stack.Push(lhs - rhs);
                        }
                        break;
                    case (int)OCGWrapper.Enums.searchCode.OPCODE_MUL:
                        if (stack.Count >= 2)
                        {
                            int rhs = stack.Pop();
                            int lhs = stack.Pop();
                            stack.Push(lhs * rhs);
                        }
                        break;
                    case (int)OCGWrapper.Enums.searchCode.OPCODE_DIV:
                        if (stack.Count >= 2)
                        {
                            int rhs = stack.Pop();
                            int lhs = stack.Pop();
                            stack.Push(lhs / rhs);
                        }
                        break;
                    case (int)OCGWrapper.Enums.searchCode.OPCODE_AND:
                        if (stack.Count >= 2)
                        {
                            int rhs = stack.Pop();
                            int lhs = stack.Pop();
                            bool b0 = rhs != 0;
                            bool b1 = lhs != 0;
                            if (b0 && b1)
                            {
                                stack.Push(1);
                            }
                            else
                            {
                                stack.Push(0);
                            }
                        }
                        break;
                    case (int)OCGWrapper.Enums.searchCode.OPCODE_OR:
                        if (stack.Count >= 2)
                        {
                            int rhs = stack.Pop();
                            int lhs = stack.Pop();
                            bool b0 = rhs != 0;
                            bool b1 = lhs != 0;
                            if (b0 || b1)
                            {
                                stack.Push(1);
                            }
                            else
                            {
                                stack.Push(0);
                            }
                        }
                        break;
                    case (int)OCGWrapper.Enums.searchCode.OPCODE_NEG:
                        if (stack.Count >= 1)
                        {
                            int rhs = stack.Pop();
                            stack.Push(-rhs);
                        }
                        break;
                    case (int)OCGWrapper.Enums.searchCode.OPCODE_NOT:
                        if (stack.Count >= 1)
                        {
                            int rhs = stack.Pop();
                            bool b0 = rhs != 0;
                            if (b0)
                            {
                                stack.Push(0);
                            }
                            else
                            {
                                stack.Push(1);
                            }
                        }
                        break;
                    case (int)OCGWrapper.Enums.searchCode.OPCODE_ISCODE:
                        if (stack.Count >= 1)
                        {
                            int code = stack.Pop();
                            bool b0 = code == card.Id;
                            if (b0)
                            {
                                stack.Push(1);
                            }
                            else
                            {
                                stack.Push(0);
                            }
                        }
                        break;
                    case (int)OCGWrapper.Enums.searchCode.OPCODE_ISSETCARD:
                        if (stack.Count >= 1)
                        {
                            if (IfSetCard(stack.Pop(), card.Setcode))
                            {
                                stack.Push(1);
                            }
                            else
                            {
                                stack.Push(0);
                            }
                        }
                        break;
                    case (int)OCGWrapper.Enums.searchCode.OPCODE_ISTYPE:
                        if (stack.Count >= 1)
                        {
                            if ((stack.Pop() & card.Type) > 0)
                            {
                                stack.Push(1);
                            }
                            else
                            {
                                stack.Push(0);
                            }
                        }
                        break;
                    case (int)OCGWrapper.Enums.searchCode.OPCODE_ISRACE:
                        if (stack.Count >= 1)
                        {
                            if ((stack.Pop() & card.Race) > 0)
                            {
                                stack.Push(1);
                            }
                            else
                            {
                                stack.Push(0);
                            }
                        }
                        break;
                    case (int)OCGWrapper.Enums.searchCode.OPCODE_ISATTRIBUTE:
                        if (stack.Count >= 1)
                        {
                            if ((stack.Pop() & card.Attribute) > 0)
                            {
                                stack.Push(1);
                            }
                            else
                            {
                                stack.Push(0);
                            }
                        }
                        break;
                    default:
                        stack.Push(getsearchCode[i]);
                        break;
                }
            }
            if (stack.Count != 1 || stack.Pop() == 0)
                return false;
            return
                card.Id == (int)YGOSharp.OCGWrapper.Enums.Sp.CARD_MARINE_DOLPHIN
                ||
                card.Id == (int)YGOSharp.OCGWrapper.Enums.Sp.CARD_TWINKLE_MOSS
         ||
         (!(card.Alias != 0)
         && ((card.Type & ((int)YGOSharp.OCGWrapper.Enums.CardType.Monster + (int)YGOSharp.OCGWrapper.Enums.CardType.Token)))
         != ((int)YGOSharp.OCGWrapper.Enums.CardType.Monster + (int)YGOSharp.OCGWrapper.Enums.CardType.Token));
        }

        public static bool IfSetCard(int setCodeToAnalyse, long setCodeFromCard)
        {
            bool res = false;
            int settype = setCodeToAnalyse & 0xfff;
            int setsubtype = setCodeToAnalyse & 0xf000;
            long sc = setCodeFromCard;
            while (sc != 0)
            {
                if ((sc & 0xfff) == settype && (sc & 0xf000 & setsubtype) == setsubtype)
                    res = true;
                sc = sc >> 16;
            }

            return res;
        }

        internal static Comparison<Card> comparisonOfCard()
        {
            return (left, right) =>
            {
                int a = 1;
                if (left.Name == nameInSearch && right.Name != nameInSearch)
                {
                    a = -1;
                }
                else if (right.Name == nameInSearch && left.Name != nameInSearch)
                {
                    a = 1;
                }
                else
                {
                    if ((left.Type & 7) < (right.Type & 7))
                    {
                        a = -1;
                    }
                    else if ((left.Type & 7) > (right.Type & 7))
                    {
                        a = 1;
                    }
                    else
                    {
                        if ((left.Type >> 3) > (right.Type >> 3))
                        {
                            a = 1;
                        }
                        else if ((left.Type >> 3) < (right.Type >> 3))
                        {
                            a = -1;
                        }
                        else
                        {
                            if (left.Level > right.Level)
                            {
                                a = -1;
                            }
                            else if (left.Level < right.Level)
                            {
                                a = 1;
                            }
                            else
                            {
                                if (left.Attack > right.Attack)
                                {
                                    a = -1;
                                }
                                else if (left.Attack < right.Attack)
                                {
                                    a = 1;
                                }
                                else
                                {
                                    if (left.Attribute > right.Attribute)
                                    {
                                        a = 1;
                                    }
                                    else if (left.Attribute < right.Attribute)
                                    {
                                        a = -1;
                                    }
                                    else
                                    {
                                        if (left.Race > right.Race)
                                        {
                                            a = 1;
                                        }
                                        else if (left.Race < right.Race)
                                        {
                                            a = -1;
                                        }
                                        else
                                        {
                                            if (left.Category > right.Category)
                                            {
                                                a = 1;
                                            }
                                            else if (left.Category < right.Category)
                                            {
                                                a = -1;
                                            }
                                            else
                                            {
                                                if (left.Id > right.Id)
                                                {
                                                    a = 1;
                                                }
                                                else if (left.Id < right.Id)
                                                {
                                                    a = -1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return a;
            };
        }

    }

    internal static class PacksManager
    {
        public class packName
        {
           public string fullName;
            public string shortName;
            public int year;
            public int month;
            public int day;
        }

        public static List<packName> packs = new List<packName>();

        static Dictionary<string, string> pacDic = new Dictionary<string, string>();

        internal static void initialize(string databaseFullPath)
        {
            using (SqliteConnection connection = new SqliteConnection("Data Source=" + databaseFullPath))
            {
                connection.Open();
                using (IDbCommand command = new SqliteCommand("SELECT pack.* FROM pack;", connection))
                {
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                int Id = (int)reader.GetInt64(0);
                                Card c = CardsManager.GetCardRaw(Id);
                                if (c != null)
                                {
                                    c.packShortNam = reader.GetString(1);
                                    c.packFullName = reader.GetString(2);
                                    c.reality = reader.GetString(3);
                                    string temp = reader.GetString(4);
                                    string[] mats = temp.Split("/");
                                    if (mats.Length == 3)
                                    {
                                        c.month = int.Parse(mats[0]);
                                        c.day = int.Parse(mats[1]);
                                        c.year = int.Parse(mats[2]);
                                    }
                                    if (!pacDic.ContainsKey(c.packFullName))    
                                    {
                                        pacDic.Add(c.packFullName, c.packShortNam);
                                        packName p = new packName();
                                        p.day = c.day;
                                        p.year = c.year;
                                        p.month = c.month;
                                        p.fullName = c.packFullName;
                                        p.shortName = c.packShortNam;
                                        packs.Add(p);
                                    }
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
            }
        }

        internal static void initializeSec()
        {
            packs.Sort((left, right) => {
                if (left.year > right.year)
                {
                    return -1;
                }
                if (left.year < right.year)
                {
                    return 1;
                }
                if (left.month > right.month)
                {
                    return -1;
                }
                if (left.month < right.month)
                {
                    return 1;
                }
                if (left.day > right.day)
                {
                    return -1;
                }
                if (left.day < right.day)
                {
                    return 1;
                }
                return 1;
            });
        }

    }
}