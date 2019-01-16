using System;
using System.Collections.Generic;
using System.IO;
public static class Config
{
    public static uint ClientVersion = 0x1348;

    class oneString
    {
        public string original = "";
        public string translated = "";
    }

    static List<oneString> translations = new List<oneString>();

    static List<oneString> uits = new List<oneString>();    

    static string path;

    public static bool getEffectON(string raw)
    {
        return true;
    }

    public static void initialize(string path)
    {
        Config.path = path;   
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
                oneString s = new oneString();
                s.original = mats[0];
                s.translated = mats[1];
                translations.Add(s);
            }
        }
    }

    static bool loaded = false;

    public static string Getui(string original)
    {
        if (loaded == false)
        {
            loaded = true;
            string[] lines = File.ReadAllText("texture\\ui\\config.txt").Replace("\r", "").Replace(" ", "").Split("\n");
            for (int i = 0; i < lines.Length; i++)
            {
                string[] mats = lines[i].Split("=");
                if (mats.Length == 2)
                {
                    oneString s = new oneString();
                    s.original = mats[0];
                    s.translated = mats[1];
                    uits.Add(s);
                }
            }
        }
        string return_value = "";
        for (int i = 0; i < uits.Count; i++)
        {
            if (uits[i].original == original)
            {
                return_value = uits[i].translated;
                break;
            }
        }
        return return_value;
    }

    internal static float getFloat(string v)
    {
        int getted = 0;
        try
        {
            getted = Int32.Parse(Get(v, "0"));
        }
        catch (Exception)   
        {
        }
        return ((float)getted) / 100000f;
    }

    internal static void setFloat(string v,float f) 
    {
        Set(v,((int)(f* 100000f)).ToString());
    }

    public static string Get(string original,string defau)  
    {
        string return_value = defau;
        bool finded = false;
        for (int i = 0; i < translations.Count; i++)
        {
            if (translations[i].original == original)
            {
                return_value = translations[i].translated;
                finded = true;
                break;
            }
        }
        if (finded == false)
        {
            if (path != null)
            {
                File.AppendAllText(path, original + "->" + defau + "\r\n");
                oneString s = new oneString();
                s.original = original;
                s.translated = defau;
                return_value = defau;
                translations.Add(s);
            }
        }
        return return_value;
    }

    public static void Set(string original,string setted)
    {
        bool finded = false;
        for (int i = 0; i < translations.Count; i++)
        {
            if (translations[i].original == original)
            {
                finded = true;
                translations[i].translated = setted;
            }
        }
        if (finded == false)
        {
            oneString s = new oneString();
            s.original = original;
            s.translated = setted;
            translations.Add(s);
        }
        string all = "";
        for (int i = 0; i < translations.Count; i++)
        {
            all += translations[i].original + "->" + translations[i].translated + "\r\n";
        }
        File.WriteAllText(path, all);
    }
}
