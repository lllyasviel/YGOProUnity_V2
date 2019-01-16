using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public class SelectServer : WindowServantSP
{
    UIPopupList list;

    UIInput inputIP;
    UIInput inputPort;
    UIInput inputPsw;
    UIInput inputVersion;

    public override void initialize()
    {
        createWindow(Program.I().new_ui_selectServer);
        UIHelper.registEvent(gameObject, "exit_", onClickExit);
        UIHelper.registEvent(gameObject, "face_", onClickFace);
        UIHelper.registEvent(gameObject, "join_", onClickJoin);
        UIHelper.getByName<UIInput>(gameObject, "name_").value = Config.Get("name","一秒一喵机会");
        list = UIHelper.getByName<UIPopupList>(gameObject, "history_");
        UIHelper.registEvent(gameObject,"history_", onSelected);
        name = Config.Get("name", "一秒一喵机会");
        inputIP = UIHelper.getByName<UIInput>(gameObject, "ip_");
        inputPort = UIHelper.getByName<UIInput>(gameObject, "port_");
        inputPsw = UIHelper.getByName<UIInput>(gameObject, "psw_");
        inputVersion = UIHelper.getByName<UIInput>(gameObject, "version_");
        SetActiveFalse();
    }

    void onSelected()
    {
        if (list != null)
        {
            readString(list.value);
        }
    }

    private void readString(string str)
    {
        str = str.Substring(1, str.Length - 1);
        string version = "", remain = "";
        string[] splited;
        splited = str.Split(")");
        try
        {
            version = splited[0];
            remain = splited[1];
        }
        catch (Exception)
        {
        }
        splited = remain.Split(":");
        string ip = "";
        try
        {
            ip = splited[0];
            remain = splited[1];
        }
        catch (Exception)
        {
        }
        splited = remain.Split(" ");
        string psw = "", port = "";
        try
        {
            port = splited[0];
            psw = splited[1];
        }
        catch (Exception)
        {
        }
        inputIP.value = ip;
        inputPort.value = port;
        inputPsw.value = psw;
        inputVersion.value = version;
    }

    public override void show()
    {
        base.show();
        Program.I().room.RMSshow_clear();
        printFile(true);
        Program.charge();
    }

    public override void preFrameFunction()
    {
        base.preFrameFunction();
        Menu.checkCommend();
    }

    void printFile(bool first)
    {
        list.Clear();
        if (File.Exists("config\\hosts.conf") == false)
        {
            File.Create("config\\hosts.conf").Close();
        }
        string txtString = File.ReadAllText("config\\hosts.conf");
        string[] lines = txtString.Replace("\r", "").Split("\n");
        for (int i = 0; i < lines.Length; i++)
        {
            if (i == 0)
            {
                if (first)
                {
                    readString(lines[i]);
                }
            }
            list.AddItem(lines[i]);
        }
    }

    void onClickExit()
    {
        Program.I().shiftToServant(Program.I().menu);
        if (TcpHelper.tcpClient != null)
        {
            if (TcpHelper.tcpClient.Connected)
            {
                TcpHelper.tcpClient.Close();
            }
        }
    }

    public void set_version(string str)
    {
        UIHelper.getByName<UIInput>(gameObject, "version_").value = str;
    }

    void onClickJoin()
    {
        if (!isShowed)
        {
            return;
        }
        string Name = UIHelper.getByName<UIInput>(gameObject, "name_").value;
        string ipString = UIHelper.getByName<UIInput>(gameObject, "ip_").value;
        string portString = UIHelper.getByName<UIInput>(gameObject, "port_").value;
        string pswString = UIHelper.getByName<UIInput>(gameObject, "psw_").value;
        string versionString = UIHelper.getByName<UIInput>(gameObject, "version_").value;
        if (versionString=="")  
        {
            UIHelper.getByName<UIInput>(gameObject, "version_").value = "0x1348";
            versionString = "0x1348";
        }
        KF_onlineGame(Name, ipString, portString, versionString, pswString);
    }

    public void KF_onlineGame(string Name,string ipString, string portString, string versionString, string pswString="")
    {
        name = Name;
        Config.Set("name", name);
        if (ipString == "" || portString == "" || versionString == "")
        {
            RMSshow_onlyYes("", InterString.Get("非法输入！请检查输入的主机名。"), null);
        }
        else
        {
            if (name != "")
            {
                string fantasty = "(" + versionString + ")" + ipString + ":" + portString + " " + pswString;
                list.items.Remove(fantasty);
                list.items.Insert(0, fantasty);
                list.value = fantasty;
                if (list.items.Count>5) 
                {
                    list.items.RemoveAt(list.items.Count - 1);
                }
                string all = "";
                for (int i = 0; i < list.items.Count; i++)
                {
                    all += list.items[i] + "\r\n";
                }
                File.WriteAllText("config\\hosts.conf", all);
                printFile(false);
                (new Thread(() => { TcpHelper.join(ipString, name, portString, pswString,versionString); })).Start();
            }
            else
            {
                RMSshow_onlyYes("", InterString.Get("昵称不能为空。"), null);
            }
        }
    }

    GameObject faceShow = null;

    public string name = "";

    void onClickFace()
    {
        name = UIHelper.getByName<UIInput>(gameObject, "name_").value;
        RMSshow_face("showFace", name);
        Config.Set("name", name);
    }

}
