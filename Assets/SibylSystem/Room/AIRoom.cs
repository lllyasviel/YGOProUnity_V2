using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class AIRoom : WindowServantSP
{
    #region ui
    UIselectableList superScrollView = null;
    string sort = "sortByTimeDeck";
    System.Diagnostics.Process serverProcess;
    System.Diagnostics.Process botProcess;

    public class BotInfo
    {
        public string name;
        public string command;
        public string desc;
        public string[] flags;
    }
    private IList<BotInfo> Bots = new List<BotInfo>();
    private void ReadBots(string confPath)
    {
        StreamReader reader = new StreamReader(new FileStream(confPath, FileMode.Open, FileAccess.Read));
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine().Trim();
            if (line.Length > 0 && line[0] == '!')
            {
                BotInfo newBot = new BotInfo();
                newBot.name = line.TrimStart('!');
                newBot.command = reader.ReadLine().Trim();
                newBot.desc = reader.ReadLine().Trim();
                line = reader.ReadLine().Trim();
                newBot.flags = line.Split(' ');
                if (Array.IndexOf(newBot.flags, "SELECT_DECKFILE") < 0)
                    Bots.Add(newBot);
            }
        }
    }

    private string GetRandomBot(string flag)
    {
        IList<BotInfo> foundBots = new List<BotInfo>();
        foreach (var bot in Bots)
        {
            if (Array.IndexOf(bot.flags, flag) >= 0) foundBots.Add(bot);
        }
        if (foundBots.Count > 0)
        {
            System.Random rand = new System.Random();
            BotInfo bot = foundBots[rand.Next(foundBots.Count)];
            return bot.command;
        }
        return "";
    }

    public override void initialize()
    {
        createWindow(Program.I().new_ui_aiRoom);
        superScrollView = gameObject.GetComponentInChildren<UIselectableList>();
        superScrollView.selectedAction = onSelected;
        UIHelper.registEvent(gameObject, "start_", onStart);
        UIHelper.registEvent(gameObject, "exit_", onClickExit);
        UIHelper.trySetLableText(gameObject, "percyHint", InterString.Get("人机模式"));
        UIHelper.trySetLableText(gameObject, "botdesc_", InterString.Get("请选择对手。"));
        superScrollView.install();
        ReadBots("config/bot.conf");
        SetActiveFalse();
    }

    void onSelected()
    {
        int sel = superScrollView.selectedIndex;
        if (sel >= 0 && sel < Bots.Count)
            UIHelper.trySetLableText(gameObject, "botdesc_", Bots[sel].desc);
        else
            UIHelper.trySetLableText(gameObject, "botdesc_", InterString.Get("请选择对手。"));
    }

    void onSave()
    {
        //Config.Set("list_aideck", list_aideck.value);
        //Config.Set("list_airank", list_airank.value);
    }

    void onClickExit()
    {
        killServerProcess();
        if (Program.exitOnReturn)
            Program.I().menu.onClickExit();
        else
            Program.I().shiftToServant(Program.I().menu);
    }

    public void killServerProcess()
    {
        if (serverProcess != null && !serverProcess.HasExited)
        {
            serverProcess.Kill();
        }
        serverProcess = null;
    }

    void onStart()
    {
        if (!isShowed)
        {
            return;
        }
        int sel = superScrollView.selectedIndex;
        if (sel < 0 || sel >= Bots.Count)
        {
            return;
        }

        string aiCommand = Bots[sel].command;
        Match match = Regex.Match(aiCommand, "Random=(\\w+)");
        if (match.Success)
        {
            string randomFlag = match.Groups[1].Value;
            string command = GetRandomBot(randomFlag);
            if (command != "")
            {
                aiCommand = command;
            }
        }

        launch(aiCommand, UIHelper.getByName<UIToggle>(gameObject, "lockhand_").value, UIHelper.getByName<UIToggle>(gameObject, "nocheck_").value, UIHelper.getByName<UIToggle>(gameObject, "noshuffle_").value);
    }

    void printFile()
    {
        superScrollView.clear();
        foreach (var bot in Bots)
        {
            superScrollView.add(bot.name);
        }
    }

    public override void show()
    {
        base.show();
        printFile();
        onSelected();
        Program.charge();
    }

    #endregion

    PrecyOcg precy;

    public void launch(string command, bool lockhand, bool nocheck, bool noshuffle)
    {
        killServerProcess();
        command = command.Replace("'", "\"");
        if (lockhand) command += " Hand=1";

        serverProcess = new System.Diagnostics.Process();
        serverProcess.StartInfo.UseShellExecute = false;
        serverProcess.StartInfo.FileName = "AI.Server.exe";
        serverProcess.StartInfo.Arguments = "7911 -1 5 0 F " + (nocheck ? "T" : "F") + " " + (noshuffle ? "T" : "F") + " 8000 5 1 0 0";
        serverProcess.StartInfo.CreateNoWindow = true;
        serverProcess.StartInfo.RedirectStandardOutput = true;
        serverProcess.Start();
        string port = serverProcess.StandardOutput.ReadLine();
        command += " Port=" + port;

        botProcess = new System.Diagnostics.Process();
        botProcess.StartInfo.UseShellExecute = false;
        botProcess.StartInfo.FileName = "WindBot/WindBot.exe";
        botProcess.StartInfo.WorkingDirectory = "WindBot";
        botProcess.StartInfo.Arguments = command;
        botProcess.StartInfo.CreateNoWindow = true;
        botProcess.StartInfo.RedirectStandardOutput = true;
        botProcess.Start();
        botProcess.StandardOutput.ReadLine();

        string name = Config.Get("name", "一秒一咕机会");
        Program.I().ocgcore.returnServant = Program.I().aiRoom;
        (new Thread(() => { Thread.Sleep(500); TcpHelper.join("127.0.0.1", name, port, "", ""); })).Start();
        RMSshow_none(InterString.Get("您在AI模式下遇到的BUG也极有可能会在联机的时候出现，所以请务必向我们报告。"));
    }

    public override void preFrameFunction()
    {
        base.preFrameFunction();
        Menu.checkCommend();
    }
}
