using System;
using System.Net.Sockets;
using YGOSharp.Network.Enums;
using UnityEngine;
using System.IO;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using YGOSharp.OCGWrapper.Enums;

public static class TcpHelper
{
    public static TcpClient tcpClient = null;

    static  NetworkStream networkStream = null;

    static bool canjoin = true;

    public static void join(string ipString, string name, string portString, string pswString, string version)
    {
        if (canjoin)
        {
            if (tcpClient == null || tcpClient.Connected == false)
            {
                canjoin = false;
                try
                {
                    tcpClient = new TcpClientWithTimeout(ipString, int.Parse(portString), 3000).Connect();
                    networkStream = tcpClient.GetStream();
                    Thread t = new Thread(receiver);
                    t.Start();
                    CtosMessage_PlayerInfo(name);
                    CtosMessage_JoinGame(pswString, version);
                }
                catch (Exception e)
                {
                    Program.DEBUGLOG("onDisConnected 10");
                }
                canjoin = true;
            }
        }
        else
        {
            onDisConnected = true;
            Program.DEBUGLOG("onDisConnected 1");
        }
    }

    public static void receiver()
    {
        try
        {
            while (tcpClient != null && networkStream != null && tcpClient.Connected && Program.Running)
            {
                byte[] data = SocketMaster.ReadPacket(networkStream);
                addDateJumoLine(data);
            }
            onDisConnected = true;
            Program.DEBUGLOG("onDisConnected 2");
        }
        catch (Exception e)
        {
            onDisConnected = true;
            Program.DEBUGLOG("onDisConnected 3");
        }

    }

    public static void addDateJumoLine(byte[] data)
    {
        Monitor.Enter(datas);
        try
        {
            datas.Add(data);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
        Monitor.Exit(datas);
    }

    public static bool onDisConnected = false;

    static List<byte[]> datas = new List<byte[]>();

    public static void preFrameFunction()
    {
        if (datas.Count>0)
        {
            if (Monitor.TryEnter(datas))
            {
                for (int i = 0; i < datas.Count; i++)
                {
                    try
                    {
                        MemoryStream memoryStream = new MemoryStream(datas[i]);
                        BinaryReader r = new BinaryReader(memoryStream);
                        var ms = (YGOSharp.Network.Enums.StocMessage)(r.ReadByte());
                        switch (ms)
                        {
                            case YGOSharp.Network.Enums.StocMessage.GameMsg:
                                ((Room)Program.I().room).StocMessage_GameMsg(r);
                                break;
                            case YGOSharp.Network.Enums.StocMessage.ErrorMsg:
                                ((Room)Program.I().room).StocMessage_ErrorMsg(r);
                                break;
                            case YGOSharp.Network.Enums.StocMessage.SelectHand:
                                ((Room)Program.I().room).StocMessage_SelectHand(r);
                                break;
                            case YGOSharp.Network.Enums.StocMessage.SelectTp:
                                ((Room)Program.I().room).StocMessage_SelectTp(r);
                                break;
                            case YGOSharp.Network.Enums.StocMessage.HandResult:
                                ((Room)Program.I().room).StocMessage_HandResult(r);
                                break;
                            case YGOSharp.Network.Enums.StocMessage.TpResult:
                                ((Room)Program.I().room).StocMessage_TpResult(r);
                                break;
                            case YGOSharp.Network.Enums.StocMessage.ChangeSide:
                                ((Room)Program.I().room).StocMessage_ChangeSide(r);
                                TcpHelper.SaveRecord();
                                break;
                            case YGOSharp.Network.Enums.StocMessage.WaitingSide:
                                ((Room)Program.I().room).StocMessage_WaitingSide(r);
                                TcpHelper.SaveRecord();
                                break;
                            case YGOSharp.Network.Enums.StocMessage.CreateGame:
                                ((Room)Program.I().room).StocMessage_CreateGame(r);
                                break;
                            case YGOSharp.Network.Enums.StocMessage.JoinGame:
                                ((Room)Program.I().room).StocMessage_JoinGame(r);
                                break;
                            case YGOSharp.Network.Enums.StocMessage.TypeChange:
                                ((Room)Program.I().room).StocMessage_TypeChange(r);
                                break;
                            case YGOSharp.Network.Enums.StocMessage.LeaveGame:
                                ((Room)Program.I().room).StocMessage_LeaveGame(r);
                                break;
                            case YGOSharp.Network.Enums.StocMessage.DuelStart:
                                ((Room)Program.I().room).StocMessage_DuelStart(r);
                                break;
                            case YGOSharp.Network.Enums.StocMessage.DuelEnd:
                                ((Room)Program.I().room).StocMessage_DuelEnd(r);
                                TcpHelper.SaveRecord();
                                break;
                            case YGOSharp.Network.Enums.StocMessage.Replay:
                                ((Room)Program.I().room).StocMessage_Replay(r);
                                TcpHelper.SaveRecord();
                                break;
                            case YGOSharp.Network.Enums.StocMessage.TimeLimit:
                                ((Ocgcore)Program.I().ocgcore).StocMessage_TimeLimit(r);
                                break;
                            case YGOSharp.Network.Enums.StocMessage.Chat:
                                ((Room)Program.I().room).StocMessage_Chat(r);
                                break;
                            case YGOSharp.Network.Enums.StocMessage.HsPlayerEnter:
                                ((Room)Program.I().room).StocMessage_HsPlayerEnter(r);
                                break;
                            case YGOSharp.Network.Enums.StocMessage.HsPlayerChange:
                                ((Room)Program.I().room).StocMessage_HsPlayerChange(r);
                                break;
                            case YGOSharp.Network.Enums.StocMessage.HsWatchChange:
                                ((Room)Program.I().room).StocMessage_HsWatchChange(r);
                                break;
                            default:
                                break;
                        }
                    }
                    catch (System.Exception e)
                    {
                       // Program.DEBUGLOG(e);
                    }
                }
                datas.Clear();
                Monitor.Exit(datas);
            }
        }
        if (onDisConnected == true)
        {
            onDisConnected = false;
            Program.I().ocgcore.returnServant = Program.I().selectServer;
            if (TcpHelper.tcpClient != null)
            {
                if (TcpHelper.tcpClient.Connected)
                {
                    tcpClient.Client.Shutdown(0);
                    tcpClient.Close();
                }
            }

            tcpClient = null;
            if (Program.I().ocgcore.isShowed == false)
            {
                if (Program.I().menu.isShowed == false) 
                {
                    Program.I().shiftToServant(Program.I().selectServer);
                }
                Program.I().cardDescription.RMSshow_none(InterString.Get("链接被断开。"));
            }
            else
            {
                Program.I().cardDescription.RMSshow_none(InterString.Get("对方离开游戏，您现在可以截图。"));
                Program.I().ocgcore.forceMSquit();
            }

        }
    }

    public static void Send(Package message)
    {
        if (tcpClient != null && tcpClient.Connected)
        {
            Thread t = new Thread(sender);
            t.Start(message);
        }
    }

    static object locker = new object();

    static void sender(object o)
    {
        try
        {
            lock (locker)
            {
                Package message = (Package)o;
                byte[] data = message.Data.get();
                MemoryStream memstream = new MemoryStream();
                BinaryWriter b = new BinaryWriter(memstream);
                b.Write(BitConverter.GetBytes((Int16)data.Length + 1), 0, 2);
                b.Write(BitConverter.GetBytes((byte)message.Fuction), 0, 1);
                b.Write(data, 0, data.Length);
                byte[] s = memstream.ToArray();
                tcpClient.Client.Send(s);
            }
        }
        catch (Exception e)
        {
            onDisConnected = true;
            Program.DEBUGLOG("onDisConnected 5");
        }
    }

    public static void CtosMessage_Response(byte[] response)
    {

        Package message = new Package();
        message.Fuction = (int)CtosMessage.Response;
        message.Data.writer.Write(response);
        Send(message);
    }

    public static YGOSharp.Deck deck;
    public static void CtosMessage_UpdateDeck(YGOSharp.Deck deckFor)
    {
        deckStrings.Clear();
        deck = deckFor;
        Package message = new Package();
        message.Fuction = (int)CtosMessage.UpdateDeck;
        message.Data.writer.Write((int)deckFor.Main.Count + deckFor.Extra.Count);
        message.Data.writer.Write((int)deckFor.Side.Count);
        for (int i = 0; i < deckFor.Main.Count; i++)
        {
            message.Data.writer.Write((int)deckFor.Main[i]);
            var c = YGOSharp.CardsManager.Get((int)deckFor.Main[i]);
            deckStrings.Add(c.Name);
        }
        for (int i = 0; i < deckFor.Extra.Count; i++)
        {
            message.Data.writer.Write((int)deckFor.Extra[i]);
        }
        for (int i = 0; i < deckFor.Side.Count; i++)
        {
            message.Data.writer.Write((int)deckFor.Side[i]);
        }
        Send(message);
    }

    public static void CtosMessage_HandResult(int res)
    {
        Package message = new Package();
        message.Fuction = (int)CtosMessage.HandResult;
        message.Data.writer.Write((byte)res);
        Send(message);
    }

    public static void CtosMessage_TpResult(bool tp)
    {
        Package message = new Package();
        message.Fuction = (int)CtosMessage.TpResult;
        if (tp)
        {
            message.Data.writer.Write((byte)1);
        }
        else
        {
            message.Data.writer.Write((byte)0);
        }
        Send(message);
    }

    public static void CtosMessage_PlayerInfo(string name)
    {
        Package message = new Package();
        message.Fuction = (int)CtosMessage.PlayerInfo;
        message.Data.writer.WriteUnicode(name, 20);
        Send(message);
    }

    public static void CtosMessage_CreateGame()
    {
    }

    public static List<string> deckStrings = new List<string>();
    public static void CtosMessage_JoinGame(string psw,string version)
    {
        deckStrings.Clear();
        Package message = new Package();
        message.Fuction = (int)CtosMessage.JoinGame;
        Config.ClientVersion = (uint)GameStringManager.helper_stringToInt(version);
        message.Data.writer.Write((Int16)Config.ClientVersion);
        message.Data.writer.Write((byte)204);
        message.Data.writer.Write((byte)204);
        message.Data.writer.Write((Int32)0);
        message.Data.writer.WriteUnicode(psw, 20);
        Send(message);
    }

    public static void CtosMessage_LeaveGame()
    {
        Package message = new Package();
        message.Fuction = (int)CtosMessage.LeaveGame;
        Send(message);
    }

    public static void CtosMessage_Surrender()
    {
        Package message = new Package();
        message.Fuction = (int)CtosMessage.Surrender;
        Send(message);
    }

    public static void CtosMessage_TimeConfirm()
    {
        Package message = new Package();
        message.Fuction = (int)CtosMessage.TimeConfirm;
        Send(message);
    }

    public static void CtosMessage_Chat(string str)
    {
        Package message = new Package();
        message.Fuction = (int)CtosMessage.Chat;
        message.Data.writer.WriteUnicode(str, str.Length + 1);
        Send(message);
    }

    public static void CtosMessage_HsToDuelist()
    {
        Package message = new Package();
        message.Fuction = (int)CtosMessage.HsToDuelist;
        Send(message);
    }

    public static void CtosMessage_HsToObserver()
    {
        Package message = new Package();
        message.Fuction = (int)CtosMessage.HsToObserver;
        Send(message);
    }

    public static void CtosMessage_HsReady()
    {
        Package message = new Package();
        message.Fuction = (int)CtosMessage.HsReady;
        Send(message);
    }

    public static void CtosMessage_HsNotReady()
    {
        Package message = new Package();
        message.Fuction = (int)CtosMessage.HsNotReady;
        Send(message);
    }

    public static void CtosMessage_HsKick(int pos)
    {
        Package message = new Package();
        message.Fuction = (int)CtosMessage.HsKick;
        message.Data.writer.Write((byte)pos);
        Send(message);
    }

    public static void CtosMessage_HsStart()
    {
        Package message = new Package();
        message.Fuction = (int)CtosMessage.HsStart;
        Send(message);
    }

    static List<Package> packagesInRecord = new List<Package>();

    public static List<Package> readPackagesInRecord(string path)
    {
        List<Package> re = null;
        try
        {
            re = getPackages(File.ReadAllBytes(path));
        }
        catch (System.Exception e)
        {
            re = new List<Package>();
            UnityEngine.Debug.Log(e);
        }
        return re;
    }

    public static List<Package> getPackages(byte[] buffer)
    {
        List<Package> re = new List<Package>();
        try
        {
            BinaryReader reader;
            using (reader = new BinaryReader(new MemoryStream(buffer)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    Package p = new Package();
                    p.Fuction = reader.ReadByte();
                    p.Data = new BinaryMaster(reader.ReadBytes((int)(reader.ReadUInt32())));
                    re.Add(p);
                }
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
        return re;
    }

    public static string lastRecordName = "";   

    public static void SaveRecord()
    {
        try
        {
            if (packagesInRecord.Count > 10)
            {
                bool write = false;
                int i = 0;
                int startI = 0;
                foreach (var item in packagesInRecord)
                {
                    i++;
                    try
                    {
                        if (item.Fuction == (int)YGOSharp.OCGWrapper.Enums.GameMessage.Start)
                        {
                            write = true;
                            startI = i;
                        }
                        if (item.Fuction == (int)YGOSharp.OCGWrapper.Enums.GameMessage.ReloadField)
                        {
                            write = true;
                            startI = i;
                        }
                    }
                    catch (System.Exception e)
                    {
                        UnityEngine.Debug.Log(e);
                    }
                }
                if (write)
                {
                    if (startI > packagesInRecord.Count)
                    {
                        startI = packagesInRecord.Count;
                    }
                    packagesInRecord.Insert(startI, Program.I().ocgcore.getNamePacket());
                    lastRecordName = UIHelper.getTimeString();
                    FileStream stream = File.Create("replay/" + lastRecordName + ".yrp3d");
                    BinaryWriter writer = new BinaryWriter(stream);
                    foreach (var item in packagesInRecord)
                    {
                        writer.Write((byte)item.Fuction);
                        writer.Write((UInt32)item.Data.getLength());
                        writer.Write(item.Data.get());
                    }
                    stream.Flush();
                    writer.Close();
                    stream.Close();
                }
            }
            packagesInRecord.Clear();
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    public static void AddRecordLine(Package p)
    {
        if (Program.I().ocgcore.condition != Ocgcore.Condition.record)
        {
            packagesInRecord.Add(p);
        }
    }
}

public class Package
{
    public int Fuction = 0;
    public BinaryMaster Data = null;
    public Package()
    {
        Fuction = (int)CtosMessage.Response;
        Data = new BinaryMaster();
    }
}

public class BinaryMaster
{
    MemoryStream memstream = null;
    public BinaryReader reader = null;
    public BinaryWriter writer = null;
    public BinaryMaster(byte[] raw = null)
    {
        if (raw == null)
        {
            memstream = new MemoryStream();
        }
        else
        {
            memstream = new MemoryStream(raw);
        }
        reader = new BinaryReader(memstream);
        writer = new BinaryWriter(memstream);
    }
    public void set(byte[] raw)
    {
        memstream = new MemoryStream(raw);
        reader = new BinaryReader(memstream);
        writer = new BinaryWriter(memstream);
    }
    public byte[] get()
    {
        byte[] bytes = memstream.ToArray();
        return bytes;
    }
    public int getLength()
    {
        return (int)memstream.Length;
    }
    public override string ToString()
    {
        string return_value = "";
        byte[] bytes = get();
        for (int i = 0; i < bytes.Length; i++)
        {
            return_value += ((int)bytes[i]).ToString();
            if (i < bytes.Length - 1) return_value += ",";
        }
        return return_value;
    }

}

public static class BinaryExtensions
{
    public static void WriteUnicode(this BinaryWriter writer, string text, int len)
    {
        try
        {
            byte[] unicode = Encoding.Unicode.GetBytes(text);
            byte[] result = new byte[len * 2];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = 204;
            }
            int max = len * 2 - 2;
            Array.Copy(unicode, result, unicode.Length > max ? max : unicode.Length);
            result[unicode.Length] = 0;
            result[unicode.Length + 1] = 0;
            writer.Write(result);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
        }

    }

    public static string ReadUnicode(this BinaryReader reader, int len)
    {
        byte[] unicode = reader.ReadBytes(len * 2);
        string text = Encoding.Unicode.GetString(unicode);
        text = text.Substring(0, text.IndexOf('\0'));
        return text;
    }

    public static string ReadALLUnicode(this BinaryReader reader)
    {
        byte[] unicode = reader.ReadToEnd();
        string text = Encoding.Unicode.GetString(unicode);
        text = text.Substring(0, text.IndexOf('\0'));
        return text;
    }

    public static byte[] ReadToEnd(this BinaryReader reader)
    {
        return reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
    }

    public static GPS ReadGPS(this BinaryReader reader)
    {
        GPS a = new GPS();
        a.controller = (UInt32)Program.I().ocgcore.localPlayer(reader.ReadByte());
        a.location = reader.ReadByte();
        a.sequence = reader.ReadByte();
        a.position = reader.ReadByte();
        return a;
    }

    public static GPS ReadShortGPS(this BinaryReader reader)
    {
        GPS a = new GPS();
        a.controller = (UInt32)Program.I().ocgcore.localPlayer(reader.ReadByte());
        a.location = reader.ReadByte();
        a.sequence = reader.ReadByte();
        a.position = (int)game_position.POS_FACEUP_ATTACK;
        return a;
    }

    public static void readCardData(this BinaryReader r, gameCard cardTemp=null)     
    {
        gameCard cardToRefresh = cardTemp;
        int flag = r.ReadInt32();
        int code = 0;
        GPS gps = new GPS();

        if ((flag & (int)Query.Code) != 0)
        {
            code= r.ReadInt32();
        }
        if ((flag & (int)Query.Position) != 0)
        {
            gps = r.ReadGPS();
            cardToRefresh = null;
            cardToRefresh = Program.I().ocgcore.GCS_cardGet(gps,false);
        }

        if (cardToRefresh == null)
        {
            return;
        }

        YGOSharp.Card data = cardToRefresh.get_data();

        if ((flag & (int)Query.Code) != 0)
        {
            if (data.Id != code)
            {
                data = YGOSharp.CardsManager.Get(code);
                data.Id = code;
            }
        }
        if ((flag & (int)Query.Position) != 0)
        {
            cardToRefresh.p = gps;
        }


        if (data.Id > 0)
        {
            if ((cardToRefresh.p.location & (UInt32)CardLocation.Hand) > 0)
            {
                if (cardToRefresh.p.controller == 1)
                {
                    cardToRefresh.p.position = (Int32)CardPosition.FaceUpAttack;
                }
            }
        }

        if ((flag & (int)Query.Alias) != 0)
            data.Alias = r.ReadInt32();
        if ((flag & (int)Query.Type) != 0)
            data.Type = r.ReadInt32();

        int l1 = 0;
        if ((flag & (int)Query.Level) != 0)
        {
            l1 = r.ReadInt32();
        }
        int l2 = 0;
        if ((flag & (int)Query.Rank) != 0)
        {
            l2 = r.ReadInt32();
        }
        if (((flag & (int)Query.Level) != 0) || ((flag & (int)Query.Rank) != 0))
        {
            if (l1 > l2)
            {
                data.Level = l1;
            }
            else
            {
                data.Level = l2;
            }
        }

        if ((flag & (int)Query.Attribute) != 0)
            data.Attribute = r.ReadInt32();
        if ((flag & (int)Query.Race) != 0)
            data.Race = r.ReadInt32();
        if ((flag & (int)Query.Attack) != 0)
            data.Attack = r.ReadInt32();
        if ((flag & (int)Query.Defence) != 0)
            data.Defense = r.ReadInt32();
        if ((flag & (int)Query.BaseAttack) != 0)
            r.ReadInt32();
        if ((flag & (int)Query.BaseDefence) != 0)
            r.ReadInt32();
        if ((flag & (int)Query.Reason) != 0)
            r.ReadInt32();
        //if ((flag & (int)Query.ReasonCard) != 0)
        //    r.ReadInt32(); 
        if ((flag & (int)Query.EquipCard) != 0)
        {
            cardToRefresh.addTarget(Program.I().ocgcore.GCS_cardGet(r.ReadGPS(), false));
        }
        if ((flag & (int)Query.TargetCard) != 0)
        {
            int count = r.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                cardToRefresh.addTarget(Program.I().ocgcore.GCS_cardGet(r.ReadGPS(), false));
            }
        }
        if ((flag & (int)Query.OverlayCard) != 0)
        {
            var overs = Program.I().ocgcore.GCS_cardGetOverlayElements(cardToRefresh);
            int count = r.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                if (i < overs.Count)
                {
                    overs[i].set_code(r.ReadInt32());
                }
                else
                {
                    r.ReadInt32();
                }
            }
        }
        if ((flag & (int)Query.Counters) != 0)
        {
            int count = r.ReadInt32();
            for (int i = 0; i < count; ++i)
                r.ReadInt32();
        }
        if ((flag & (int)Query.Owner) != 0)
            r.ReadInt32();
        if ((flag & (int)Query.Status) != 0)
        {
            int status = r.ReadInt32();
            cardToRefresh.disabled = (status & 0x0001) == 0x0001;
        }
        if ((flag & (int)Query.LScale) != 0)
            data.LScale = r.ReadInt32();
        if ((flag & (int)Query.RScale) != 0)
            data.RScale = r.ReadInt32();
        cardToRefresh.set_data(data);
        //
    }
}

public class SocketMaster
{
    static byte[] ReadFull(NetworkStream stream, int length)
    {
        var buf = new byte[length];
        int rlen = 0;
        while (rlen < buf.Length)
        {
            int currentLength = stream.Read(buf, rlen, buf.Length - rlen);
            rlen += currentLength;
            if (currentLength == 0)
            {
                TcpHelper.onDisConnected = true;
                Program.DEBUGLOG("onDisConnected 6");
                break;
            }
        }

        return buf;
    }

    public static byte[] ReadPacket(NetworkStream stream)
    {
        var hdr = ReadFull(stream, 2);
        var plen = BitConverter.ToUInt16(hdr, 0);
        var buf = ReadFull(stream, plen);
        return buf;
    }

}

public class TcpClientWithTimeout
{
    protected string _hostname;
    protected int _port;
    protected int _timeout_milliseconds;
    protected TcpClient connection;
    protected bool connected;
    protected Exception exception;

    public TcpClientWithTimeout(string hostname, int port, int timeout_milliseconds)
    {
        _hostname = hostname;
        _port = port;
        _timeout_milliseconds = timeout_milliseconds;
    }
    public TcpClient Connect()
    {
        // kick off the thread that tries to connect
        connected = false;
        exception = null;
        Thread thread = new Thread(new ThreadStart(BeginConnect));
        thread.IsBackground = true; // 作为后台线程处理
                                    // 不会占用机器太长的时间
        thread.Start();

        // 等待如下的时间
        thread.Join(_timeout_milliseconds);

        if (connected == true)
        {
            // 如果成功就返回TcpClient对象
            thread.Abort();
            return connection;
        }
        if (exception != null)
        {
            // 如果失败就抛出错误
            thread.Abort();
            TcpHelper.onDisConnected = true;
            Program.DEBUGLOG("onDisConnected 7");
            throw exception;
        }
        else
        {
            // 同样地抛出错误
            thread.Abort();
            string message = string.Format("TcpClient connection to {0}:{1} timed out",
              _hostname, _port);
            TcpHelper.onDisConnected = true;
            Program.DEBUGLOG("onDisConnected 8");
            throw new TimeoutException(message);
        }
    }
    protected void BeginConnect()
    {
        try
        {
            connection = new TcpClient(_hostname, _port);
            // 标记成功，返回调用者
            connected = true;
        }
        catch (Exception ex)
        {
            // 标记失败
            exception = ex;
        }
    }
}