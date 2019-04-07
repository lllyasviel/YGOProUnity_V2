using System;
using System.Collections.Generic;
using System.IO;
public static class InterString
{
    static Dictionary<string, string> translations = new Dictionary<string, string>();

    static string path;

    public static bool loaded = false;

    public static void initialize(string path)
    {
        InterString.path = path;
        if (File.Exists(path) == false)
        {
            File.Create(path).Close();
        }
        string txtString = File.ReadAllText(path);
        string[] lines = txtString.Replace("\r", "").Split("\n");
        for (int i = 0; i < lines.Length; i++)
        {
            string[] mats = lines[i].Split("->");
            if (mats.Length == 2)
            {
                if (!translations.ContainsKey(mats[0]))
                {
                    translations.Add(mats[0], mats[1]);
                }
            }
        }
        GameStringHelper.xilie = Get("系列：");
        GameStringHelper.opHint = Get("*控制权经过转移");
        GameStringHelper.licechuwai= Get("*里侧表示的除外卡片");
        GameStringHelper.biaoceewai = Get("*表侧表示的额外卡片");
        GameStringHelper.teshuzhaohuan= Get("*被特殊召唤出场");
        GameStringHelper.yijingqueren = Get("卡片展示简表※  ");
        GameStringHelper._chaoliang = Get("超量：");
        GameStringHelper._ewaikazu = Get("额外卡组：");
        GameStringHelper._fukazu = Get("副卡组：");
        GameStringHelper._guaishou = Get("怪兽：");
        GameStringHelper._mofa = Get("魔法：");
        GameStringHelper._ronghe = Get("融合：");
        GameStringHelper._lianjie = Get("连接：");
        GameStringHelper._tongtiao = Get("同调：");
        GameStringHelper._xianjing = Get("陷阱：");
        GameStringHelper._zhukazu = Get("主卡组：");

        GameStringHelper.kazu = Get("卡组");
        GameStringHelper.mudi = Get("墓地");
        GameStringHelper.chuwai = Get("除外");
        GameStringHelper.ewai = Get("额外");
        //GameStringHelper.diefang = Get("叠放");
        GameStringHelper._wofang = Get("我方");
        GameStringHelper._duifang = Get("对方");
        loaded = true;
    }

    public static string Get(string original)
    {
        
        string return_value = original;
        if (translations.TryGetValue(original, out return_value))
        {
            return return_value.Replace("@n", "\r\n").Replace("@ui", "");
        }
        else if (original != "")
        {
            File.AppendAllText(path, original + "->" + original + "\r\n");
            translations.Add(original, original);
            return original.Replace("@n", "\r\n").Replace("@ui", "");
        }
        else
            return original;
    }

    public static string Get(string original, string replace)
    {
        return Get(original).Replace("[?]", replace);
    }

}
