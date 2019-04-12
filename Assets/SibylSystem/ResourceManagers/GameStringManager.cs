using System;
using System.Collections.Generic;
using UnityEngine;
public static class GameStringManager
{
    public class hashedString
    {
        public string region = "";
        public int hashCode = 0;
        public string content = "";
    }

    public static List<hashedString> hashedStrings = new List<hashedString>();

    public static List<hashedString> xilies = new List<hashedString>();

    public static int helper_stringToInt(string str)
    {
        int return_value = 0;
        try
        {
            if (str.Length > 2 && str.Substring(0, 2) == "0x")
            {
                return_value = Convert.ToInt32(str, 16);
            }
            else
            {
                return_value = Int32.Parse(str);
            }
        }
        catch (Exception)
        {
        }
        return return_value;
    }

    public static void initialize(string path)
    {
        string text = System.IO.File.ReadAllText(path);
        string st = text.Replace("\r", "");
        string[] lines = st.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            if (line.Length > 1 && line.Substring(0, 1) == "!")
            {
                string[] mats = line.Substring(1, line.Length - 1).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (mats.Length > 2)
                {
                    hashedString a = new hashedString();
                    a.region = mats[0];
                    try
                    {
                        a.hashCode = helper_stringToInt(mats[1]);
                    }
                    catch (Exception e)
                    {
                        Program.DEBUGLOG(e);
                    }
                    a.content = "";
                    for (int i = 2; i < mats.Length; i++)
                    {
                        a.content += mats[i] + " ";
                    }
                    a.content = a.content.Substring(0, a.content.Length - 1);
                    hashedStrings.Add(a);
                    if (a.region == "setname")
                    {
                        xilies.Add(a);
                    }
                }
            }
        }
    }

    public static string get(string region, int hashCode)
    {
        string re = "";
        foreach (hashedString s in hashedStrings)
        {
            if (s.region == region && s.hashCode == hashCode)
            {
                re = s.content;
                break;
            }
        }
        return re;
    }

    internal static string get_unsafe(int hashCode)
    {
        string re = "";
        foreach (hashedString s in hashedStrings)
        {
            if (s.region == "system" && s.hashCode == hashCode)
            {
                re = s.content;
                break;
            }
        }
        return re;
    }

    internal static string get(int description)
    {
        string a = "";
        if (description < 10000)
        {
            a = get("system", (int)description);
        }
        else
        {
            int code = description >> 4;
            int index = description & 0xf;
            try
            {
                a = YGOSharp.CardsManager.Get(code).Str[index];
            }
            catch (Exception e)
            {
                Program.DEBUGLOG(e);
            }
        }
        return a;
    }

    internal static string formatLocation(uint location, uint sequence)
    {
        if (location == 0x8)
        {
            if (sequence < 5)
                return get(1003);
            else if (sequence == 5)
                return get(1008);
            else
                return get(1009);
        }
        uint filter = 1;
        int i = 1000;
        for (; filter != 0x100 && filter != location; filter <<= 1)
            ++i;
        if (filter == location)
            return get(i);
        else
            return "???";
    }
    internal static string formatLocation(GPS gps)
    {
        return formatLocation(gps.location, gps.sequence);
    }
}

