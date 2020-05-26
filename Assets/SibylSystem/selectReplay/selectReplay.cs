using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class selectReplay : WindowServantSP
{
    UIselectableList superScrollView = null;

    string sort = "sortByTimeReplay";

    public override void initialize()
    {
        createWindow(Program.I().remaster_replayManager);
        UIHelper.registEvent(gameObject, "exit_", onClickExit);
        superScrollView = gameObject.GetComponentInChildren<UIselectableList>();
        superScrollView.selectedAction = onSelected;
        UIHelper.registEvent(gameObject, "sort_", onSort);
        UIHelper.registEvent(gameObject, "launch_", onLaunch);
        UIHelper.registEvent(gameObject, "rename_", onRename);
        UIHelper.registEvent(gameObject, "delete_", onDelete);
        UIHelper.registEvent(gameObject, "yrp_", onYrp);
        UIHelper.registEvent(gameObject, "ydk_", onYdk);
        UIHelper.registEvent(gameObject, "god_", onGod);
        UIHelper.registEvent(gameObject, "value_", onValue);
        setSortLable();
        superScrollView.install();
        SetActiveFalse();
    }

    void onValue()
    {
        RMSshow_yesOrNo(
                 "onValue",
                 InterString.Get("您确定要删除所有未命名的录像？"),
                 new messageSystemValue { hint = "yes", value = "yes" },
                 new messageSystemValue { hint = "no", value = "no" });

    }

    private void setSortLable()
    {
        if (Config.Get(sort,"1") == "1")
        {
            UIHelper.trySetLableText(gameObject, "sort_", InterString.Get("时间排序"));
        }
        else
        {
            UIHelper.trySetLableText(gameObject, "sort_", InterString.Get("名称排序"));
        }
    }

    private void onLaunch()
    {
        if (!superScrollView.Selected())
        {
            return;
        }
        if (!isShowed)
        {
            return;
        }
        KF_replay(superScrollView.selectedString);
    }

    PrecyOcg precy;

    private void onGod()    
    {
        if (!superScrollView.Selected())
        {
            return;
        }
        if (!isShowed)
        {
            return;
        }
        KF_replay(superScrollView.selectedString,true);
    }

    private void onSort()
    {
        if (Config.Get(sort,"1") == "1")
        {
            Config.Set(sort, "0");
        }
        else
        {
            Config.Set(sort, "1");
        }
        setSortLable();
        printFile();
    }

    bool opYRP = false;

    private void onRename()
    {
        if (!superScrollView.Selected())
        {
            return;
        }
        string name = superScrollView.selectedString;
        if (name.Length > 4 && name.Substring(name.Length - 4, 4) == ".yrp")
        {
            opYRP = true;
            RMSshow_input("onRename", InterString.Get("请输入重命名后的录像名"), name.Substring(0, name.Length - 4));
        }
        else
        {
            opYRP = false;
            RMSshow_input("onRename", InterString.Get("请输入重命名后的录像名"), name);
        }
    }

    private void onDelete()
    {
        if (!superScrollView.Selected())
        {
            return;
        }
        RMSshow_yesOrNo(
                 "onDelete",
                 InterString.Get("删除[?],@n请确认。",
                 superScrollView.selectedString),
                 new messageSystemValue { hint = "yes", value = "yes" },
                 new messageSystemValue { hint = "no", value = "no" });
    }

    byte[] getYRPbuffer(string path)
    {
        if (path.Substring(path.Length - 4, 4) == ".yrp")
        {
            return File.ReadAllBytes(path);
        }
        byte[] returnValue = null;
        try
        {
            var collection = TcpHelper.readPackagesInRecord(path);
            foreach (var item in collection)
            {
                if (item.Fuction == (int)YGOSharp.OCGWrapper.Enums.GameMessage.sibyl_replay)
                {
                    returnValue = item.Data.reader.ReadToEnd();
                    break;
                }
            }

        }
        catch (Exception e) 
        {
            Debug.Log(e);
        }
        if (returnValue != null)
        {
            if (returnValue.Length < 50)
            {
                returnValue = null;
            }
        }
        return returnValue;
    }

    Percy.YRP getYRP(byte[] buffer)
    {
        Percy.YRP returnValue = new Percy.YRP();
        try
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(buffer));
            returnValue.ID= reader.ReadInt32();
            returnValue.Version= reader.ReadInt32();
            returnValue.Flag= reader.ReadInt32();
            returnValue.Seed= reader.ReadUInt32();
            returnValue.DataSize = reader.ReadInt32();
            returnValue.Hash = reader.ReadInt32();
            returnValue.Props= reader.ReadBytes(8);
            byte[] raw = reader.ReadToEnd();
            if ((returnValue.Flag & 0x1) > 0)
            {
                SevenZip.Compression.LZMA.Decoder lzma = new SevenZip.Compression.LZMA.Decoder();
                lzma.SetDecoderProperties(returnValue.Props);
                MemoryStream decompressed = new MemoryStream();
                lzma.Code(new MemoryStream(raw), decompressed, raw.LongLength, returnValue.DataSize, null);
                raw = decompressed.ToArray();
            }
            reader = new BinaryReader(new MemoryStream(raw));
            if ((returnValue.Flag & 0x2) > 0)
            {
                returnValue.playerData.Add(new Percy.YRP.PlayerData());
                returnValue.playerData.Add(new Percy.YRP.PlayerData());
                returnValue.playerData.Add(new Percy.YRP.PlayerData());
                returnValue.playerData.Add(new Percy.YRP.PlayerData());
                returnValue.playerData[0].name = reader.ReadUnicode(20);
                returnValue.playerData[1].name = reader.ReadUnicode(20);
                returnValue.playerData[2].name = reader.ReadUnicode(20);
                returnValue.playerData[3].name = reader.ReadUnicode(20);
                returnValue.StartLp = reader.ReadInt32();
                returnValue.StartHand = reader.ReadInt32();
                returnValue.DrawCount = reader.ReadInt32();
                returnValue.opt = reader.ReadInt32();
                Program.I().ocgcore.MasterRule = returnValue.opt >> 16;
                for (int i = 0; i < 4; i++)
                {
                    int count = reader.ReadInt32();
                    for (int i2 = 0; i2 < count; i2++)
                    {
                        returnValue.playerData[i].main.Add(reader.ReadInt32());
                    }
                    count = reader.ReadInt32();
                    for (int i2 = 0; i2 < count; i2++)
                    {
                        returnValue.playerData[i].extra.Add(reader.ReadInt32());
                    }
                }
            }
            else
            {
                returnValue.playerData.Add(new Percy.YRP.PlayerData());
                returnValue.playerData.Add(new Percy.YRP.PlayerData());
                returnValue.playerData[0].name = reader.ReadUnicode(20);
                returnValue.playerData[1].name = reader.ReadUnicode(20);
                returnValue.StartLp = reader.ReadInt32();
                returnValue.StartHand = reader.ReadInt32();
                returnValue.DrawCount = reader.ReadInt32();
                returnValue.opt = reader.ReadInt32();
                Program.I().ocgcore.MasterRule = returnValue.opt >> 16;
                for (int i = 0; i < 2; i++)
                {
                    int count = reader.ReadInt32();
                    for (int i2 = 0; i2 < count; i2++)
                    {
                        returnValue.playerData[i].main.Add(reader.ReadInt32());
                    }
                    count = reader.ReadInt32();
                    for (int i2 = 0; i2 < count; i2++)
                    {
                        returnValue.playerData[i].extra.Add(reader.ReadInt32());
                    }
                }
            }
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                returnValue.gameData.Add(reader.ReadBytes(reader.ReadByte()));
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return returnValue;
    }

    private void onYdk()    
    {
        if (!superScrollView.Selected())
        {
            return;
        }
        try
        {
            Percy.YRP yrp;
            if (File.Exists("replay/" + superScrollView.selectedString))    
            {
                yrp = getYRP(File.ReadAllBytes("replay/" + superScrollView.selectedString));
            }
            else
            {
                yrp = getYRP(getYRPbuffer("replay/" + superScrollView.selectedString + ".yrp3d"));
            }
            for (int i = 0; i < yrp.playerData.Count; i++)  
            {
                string value = "#created by ygopro2\r\n#main\r\n";
                for (int i2 = 0; i2 < yrp.playerData[i].main.Count; i2++)
                {
                    value += yrp.playerData[i].main[i2].ToString() + "\r\n";
                }
                value += "#extra\r\n";
                for (int i2 = 0; i2 < yrp.playerData[i].extra.Count; i2++)
                {
                    value += yrp.playerData[i].extra[i2].ToString() + "\r\n";
                }
                string name = "deck/" + superScrollView.selectedString + "_" + (i + 1).ToString() + ".ydk";
                File.WriteAllText(name, value);
                RMSshow_none(InterString.Get("卡组入库：[?]", name));
            }
            if (yrp.playerData.Count == 0)
            {
                RMSshow_none(InterString.Get("录像没有录制完整。"));
            }
        }
        catch (Exception)
        {
            RMSshow_none(InterString.Get("录像没有录制完整。"));
        }
    }

    private void onYrp()
    {
        if (!superScrollView.Selected())
        {
            return;
        }
        try
        {
            if (File.Exists("replay/" + superScrollView.selectedString + ".yrp3d"))  
            {
                File.WriteAllBytes("replay/" + superScrollView.selectedString + ".yrp", getYRPbuffer("replay/" + superScrollView.selectedString + ".yrp3d"));
                RMSshow_none(InterString.Get("录像入库：[?]", "replay/" + superScrollView.selectedString + ".yrp"));
                printFile();
            }
            else
            {
                RMSshow_none(InterString.Get("录像没有录制完整。"));
            }
        }
        catch (Exception)
        {
            RMSshow_none(InterString.Get("录像没有录制完整。"));
        }
    }

    public override void ES_RMS(string hashCode, List<messageSystemValue> result)
    {
        base.ES_RMS(hashCode, result);
        if (hashCode == "onRename")
        {
            try
            {
                if (opYRP)
                {
                    System.IO.File.Move("replay/" + superScrollView.selectedString, "replay/" + result[0].value + ".yrp");

                }else
                {
                    System.IO.File.Move("replay/" + superScrollView.selectedString + ".yrp3d", "replay/" + result[0].value + ".yrp3d");

                }
                printFile();
                RMSshow_none(InterString.Get("重命名成功。"));
            }
            catch (Exception)
            {
                RMSshow_none(InterString.Get("非法输入！请检查输入的文件名。"));
            }
        }
        if (hashCode == "onDelete")
        {
            if (result[0].value == "yes")
            {
                try
                {
                    if (File.Exists("replay/" + superScrollView.selectedString + ".yrp3d"))
                    {
                        System.IO.File.Delete("replay/" + superScrollView.selectedString + ".yrp3d");
                        RMSshow_none(InterString.Get("[?]已经被删除。", superScrollView.selectedString));
                        printFile();
                    }
                    if (File.Exists("replay/" + superScrollView.selectedString))
                    {
                        System.IO.File.Delete("replay/" + superScrollView.selectedString);
                        RMSshow_none(InterString.Get("[?]已经被删除。", superScrollView.selectedString));
                        printFile();
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        if (hashCode == "onValue")
        {
            if (result[0].value == "yes")
            {
                FileInfo[] fileInfos = (new DirectoryInfo("replay")).GetFiles();
                for (int i = 0; i < fileInfos.Length; i++)
                {
                    if (fileInfos[i].Name.Length == 21)
                    {
                        if (fileInfos[i].Name[2] == '-')
                        {
                            if (fileInfos[i].Name[5] == '「')
                            {
                                if (fileInfos[i].Name[8] == '：')
                                {
                                    try
                                    {
                                        File.Delete("replay/" + fileInfos[i].Name);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
                RMSshow_none(InterString.Get("清理完毕。"));
                printFile();
            }
        }
    }

    string selectedTrace = "";    
    void onSelected()
    {
        if (selectedTrace == superScrollView.selectedString)    
        {
            KF_replay(selectedTrace);
        }
        selectedTrace = superScrollView.selectedString;
    }

    public override void preFrameFunction()
    {
        base.preFrameFunction();
        Menu.checkCommend();
    }

    public void KF_replay(string name, bool god = false)
    {
        try
        {
            if (File.Exists("replay/" + name + ".yrp3d"))
            {
                if (god)
                {
                    RMSshow_none(InterString.Get("您正在观看旧版的录像（上帝视角），不保证稳定性。"));
                    if (precy != null)
                        precy.dispose();
                    precy = new PrecyOcg();
                    var collections = TcpHelper.getPackages(precy.ygopro.getYRP3dBuffer(getYRP(getYRPbuffer("replay/" + name + ".yrp3d"))));
                    pushCollection(collections);
                }
                else
                {
                    var collection = TcpHelper.readPackagesInRecord("replay/" + name + ".yrp3d");
                    pushCollection(collection);
                }
            }
            else
            {
                if (name.Length>4&&name.Substring(name.Length - 4, 4) == ".yrp")
                {
                    if (File.Exists("replay/" + name))
                    {
                        RMSshow_none(InterString.Get("您正在观看旧版的录像（上帝视角），不保证稳定性。"));
                        if (precy != null)
                            precy.dispose();
                        precy = new PrecyOcg();
                        var collections = TcpHelper.getPackages(precy.ygopro.getYRP3dBuffer(getYRP(File.ReadAllBytes("replay/" + name))));
                        pushCollection(collections);
                    }
                }
            }
        }
        catch (Exception)   
        {
            RMSshow_none(InterString.Get("录像没有录制完整。"));
        }
    }

    private void pushCollection(List<Package> collection)
    {
        Program.I().ocgcore.returnServant = Program.I().selectReplay;
        Program.I().ocgcore.handler = (a) => { };
        Program.I().ocgcore.name_0 = Config.Get("name", "一秒一喵机会");
        Program.I().ocgcore.name_0_c = Program.I().ocgcore.name_0;
        Program.I().ocgcore.name_1 = "Percy AI";
        Program.I().ocgcore.name_0_tag = "---";
        Program.I().ocgcore.name_1_tag = "---";
        Program.I().ocgcore.timeLimit = 240;
        Program.I().ocgcore.lpLimit = 8000;
        Program.I().ocgcore.isFirst = true;
        Program.I().shiftToServant(Program.I().ocgcore);
        Program.I().ocgcore.InAI = false;
        Program.I().ocgcore.shiftCondition(Ocgcore.Condition.record);
        Program.I().ocgcore.flushPackages(collection);
    }

    public override void show()
    {
        base.show();
        printFile();
        Program.charge();
    }

    void printFile()
    {
        superScrollView.clear();
        FileInfo[] fileInfos = (new DirectoryInfo("replay")).GetFiles();
        if (Config.Get(sort, "1") == "1")
        {
            Array.Sort(fileInfos, UIHelper.CompareTime);
        }
        else
        {
            Array.Sort(fileInfos, UIHelper.CompareName);
        }
        for (int i = 0; i < fileInfos.Length; i++)
        {
            if (fileInfos[i].Name.Length > 6)
            {
                if (fileInfos[i].Name.Length > 6 && fileInfos[i].Name.Substring(fileInfos[i].Name.Length - 6, 6) == ".yrp3d")
                {
                    superScrollView.add(fileInfos[i].Name.Substring(0, fileInfos[i].Name.Length - 6));
                }
                if (fileInfos[i].Name.Length > 4 && fileInfos[i].Name.Substring(fileInfos[i].Name.Length - 4, 4) == ".yrp")
                {
                    superScrollView.add(fileInfos[i].Name);
                }
            }
        }
    }

    void onClickExit()
    {
        Program.I().shiftToServant(Program.I().menu);
    }

}
