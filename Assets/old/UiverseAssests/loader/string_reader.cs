using System;
using System.Collections.Generic;
using UnityEngine;
public class string_reader
{
    class string_hash
    {
        public string father = "";
        public int id = 0;
        public string content = "";
    }
    List<string_hash> strs = new List<string_hash>();
    public int get_int(string str)
    {
        int return_value = 0;
        if(str.Length>2&&str.Substring(0,2)=="0x"){
            return_value = Convert.ToInt32(str,16);
        }
        else
        {
            return_value = Int32.Parse(str);
        }
        return return_value;
    }
    public string_reader(string path)
    {
        string text = System.IO.File.ReadAllText(path);
        string st = text.Replace("\r", "");
        string[] lines = st.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            if (line.Length>1&&line.Substring(0,1)=="!")
            {
                string[] mats = line.Substring(1, line.Length-1).Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (mats.Length > 2)
                {
                    string_hash a = new string_hash();
                    a.father = mats[0];
                    try
                    {
                        a.id = get_int(mats[1]);
                    }catch(Exception e){
                        Debug.Log(e);
                    }
                    a.content = mats[2];
                    strs.Add(a);
                    if (a.id==1160)
                    {
                        Debug.Log(a.content);
                    }
                }
            }
        }
    }
    public string get(string father,int id)
    {
        string re = "";
        foreach (string_hash s in strs)
        {
            if(s.father==father&&s.id==id){
                re = s.content;
            }
        }
        return re;
    }
}

