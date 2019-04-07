using System;
using System.Collections.Generic;

public class MultiStringMaster
{
    public class part
    {
        public string str;
        public int count;
    }

    public List<part> strings = new List<part>();

    private string _managedString = "";

    public string managedString
    {
        get { return _managedString.TrimEnd('\n'); }
        set { _managedString = value; }
    }

    public void Add(string str)
    {
        bool exist = false;
        for (int i = 0; i < strings.Count; i++)
        {
            if (strings[i].str == str)
            {
                exist = true;
                strings[i].count++;
            }
        }
        if (exist == false)
        {
            part t = new part();
            t.count = 1;
            t.str = str;
            strings.Add(t);
        }
        _managedString = "";
        for (int i = 0; i < strings.Count; i++)
        {
            if (strings[i].count == 1)
            {
                _managedString += strings[i].str + "\n";
            }
            else
            {
                _managedString += strings[i].str + "*" + strings[i].count.ToString() + "\n";
            }
        }
    }

    public void clear()
    {
        strings.Clear();
        _managedString = "";
    }

    public void remove(string str)
    {
        part t = null;
        for (int i = 0; i < strings.Count; i++)
        {
            if (strings[i].str.Replace(str, "miaowu") != strings[i].str)
            {
                t = strings[i];
            }
        }
        if (t != null)
        {
            if (t.count == 1)
            {
                strings.Remove(t);
            }
            else
            {
                t.count--;
            }
        }
        _managedString = "";
        for (int i = 0; i < strings.Count; i++)
        {
            if (strings[i].count == 1)
            {
                _managedString += strings[i].str + "\n";
            }
            else
            {
                _managedString += strings[i].str + "*" + strings[i].count.ToString() + "\n";
            }
        }
    }

}