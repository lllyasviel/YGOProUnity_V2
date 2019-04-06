using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Percy
{
    #region DoNotCareAboutThis
    class Deck
    {
        public List<int> Main = new List<int>();
        public List<int> Extra = new List<int>();
        public List<int> Side = new List<int>();
    }
    class Package
    {
        public int Fuction = 0;
        public BinaryMaster Data = null;
        public Package()
        {
            Fuction = (int)0;
            Data = new BinaryMaster();
        }
    }
    class BinaryMaster
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
    class BinaryExtensions
    {
        public static byte[] ReadToEnd(BinaryReader reader)
        {
            return reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
        }
    }
    enum GameMessage
    {
        Retry = 1,
        Hint = 2,
        Waiting = 3,
        Start = 4,
        Win = 5,
        UpdateData = 6,
        UpdateCard = 7,
        RequestDeck = 8,
        SelectBattleCmd = 10,
        SelectIdleCmd = 11,
        SelectEffectYn = 12,
        SelectYesNo = 13,
        SelectOption = 14,
        SelectCard = 15,
        SelectChain = 16,
        SelectPlace = 18,
        SelectPosition = 19,
        SelectTribute = 20,
        SortChain = 21,
        SelectCounter = 22,
        SelectSum = 23,
        SelectDisfield = 24,
        SortCard = 25,
        SelectUnselectCard = 26,
        ConfirmDecktop = 30,
        ConfirmCards = 31,
        ShuffleDeck = 32,
        ShuffleHand = 33,
        RefreshDeck = 34,
        SwapGraveDeck = 35,
        ShuffleSetCard = 36,
        ReverseDeck = 37,
        DeckTop = 38,
        ShuffleExtra = 39,
        NewTurn = 40,
        NewPhase = 41,
        ConfirmExtratop = 42,
        Move = 50,
        PosChange = 53,
        Set = 54,
        Swap = 55,
        FieldDisabled = 56,
        Summoning = 60,
        Summoned = 61,
        SpSummoning = 62,
        SpSummoned = 63,
        FlipSummoning = 64,
        FlipSummoned = 65,
        Chaining = 70,
        Chained = 71,
        ChainSolving = 72,
        ChainSolved = 73,
        ChainEnd = 74,
        ChainNegated = 75,
        ChainDisabled = 76,
        CardSelected = 80,
        RandomSelected = 81,
        BecomeTarget = 83,
        Draw = 90,
        Damage = 91,
        Recover = 92,
        Equip = 93,
        LpUpdate = 94,
        Unequip = 95,
        CardTarget = 96,
        CancelTarget = 97,
        PayLpCost = 100,
        AddCounter = 101,
        RemoveCounter = 102,
        Attack = 110,
        Battle = 111,
        AttackDiabled = 112,
        DamageStepStart = 113,
        DamageStepEnd = 114,
        MissedEffect = 120,
        BeChainTarget = 121,
        CreateRelation = 122,
        ReleaseRelation = 123,
        TossCoin = 130,
        TossDice = 131,
        RockPaperScissors = 132,
        HandResult = 133,
        AnnounceRace = 140,
        AnnounceAttrib = 141,
        AnnounceCard = 142,
        AnnounceNumber = 143,
        AnnounceCardFilter = 144,
        CardHint = 160,
        TagSwap = 161,
        ReloadField = 162,
        AiName = 163,
        ShowHint = 164,
        PlayerHint = 165,
        MatchKill = 170,
        CustomMsg = 180,
        DuelWinner = 200,
    }
    enum CardLocation
    {
        Deck = 0x01,
        Hand = 0x02,
        MonsterZone = 0x04,
        SpellZone = 0x08,
        Grave = 0x10,
        Removed = 0x20,
        Extra = 0x40,
        Overlay = 0x80,
        Onfield = 0x0C
    }
    enum CardPosition
    {
        FaceUpAttack = 0x1,
        FaceDownAttack = 0x2,
        FaceUpDefence = 0x4,
        FaceDownDefence = 0x8,
        FaceUp = 0x5,
        FaceDown = 0xA,
        Attack = 0x3,
        Defence = 0xC
    }
    public struct CardData
    {
        public int Code;
        public int Alias;
        public long Setcode;
        public int Type;
        public int Level;
        public int Attribute;
        public int Race;
        public int Attack;
        public int Defense;
        public int LScale;
        public int RScale;
    }
    unsafe static class dll
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr ScriptReader(String scriptName, Int32* len);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate UInt32 CardReader(UInt32 code, CardData* pData);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate UInt32 MessageHandler(IntPtr pDuel, UInt32 messageType);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern void set_card_reader(CardReader f);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern void set_message_handler(MessageHandler f);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern void set_chat_handler(MessageHandler f);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        static extern void set_script_reader(ScriptReader f);
        static smallYgopro.cardHandler card_handler;
        public static void set_card_api(smallYgopro.cardHandler h)
        {
            card_handler = h;
            set_card_reader(OnCardReader);
        }
        static smallYgopro.chatHandler chat_handler;
        public static void set_chat_api(smallYgopro.chatHandler h)
        {
            chat_handler = h;
            set_message_handler(OnMessageHandler);
            set_chat_handler(OnMessageHandler);
        }
        private static UInt32 OnCardReader(UInt32 code, CardData* pData)
        {
            *pData = card_handler(code);
            return code;
        }
        static IntPtr _buffer_2 = Marshal.AllocHGlobal(65536);
        private static UInt32 OnMessageHandler(IntPtr pDuel, UInt32 messageType)
        {
            byte[] arr = new byte[256];
            get_log_message(pDuel, _buffer_2);
            Marshal.Copy(_buffer_2, arr, 0, 256);
            string message = System.Text.Encoding.UTF8.GetString(arr);
            if (message.Contains("\0"))
                message = message.Substring(0, message.IndexOf('\0'));
            chat_handler(message);
            return 0;
        }
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr create_duel(UInt32 seed);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void start_duel(IntPtr pduel, Int32 options);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 get_ai_going_first_second(IntPtr pduel, IntPtr deckname);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 set_player_going_first_second(IntPtr pduel, Int32 first, IntPtr deckname);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void set_ai_id(IntPtr pduel, int playerid);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void end_duel(IntPtr pduel);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void set_player_info(IntPtr pduel, Int32 playerid, Int32 lp, Int32 startcount, Int32 drawcount);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void new_card(IntPtr pduel, UInt32 code, Byte owner, Byte playerid, Byte location, Byte sequence, Byte position);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void new_tag_card(IntPtr pduel, UInt32 code, Byte owner, Byte location);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 process(IntPtr pduel);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 get_message(IntPtr pduel, IntPtr buf);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void get_log_message(IntPtr pduel, IntPtr buf);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void set_responseb(IntPtr pduel, IntPtr buf);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void set_responsei(IntPtr pduel, UInt32 value);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 query_card(IntPtr pduel, Byte playerid, Byte location, Byte sequence, Int32 queryFlag, IntPtr buf, Int32 useCache);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 query_field_count(IntPtr pduel, Byte playerid, Byte location);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 query_field_card(IntPtr pduel, Byte playerid, Byte location, Int32 queryFlag, IntPtr buf, Int32 useCache);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 query_field_info(IntPtr pduel, IntPtr buf);
        [DllImport("ocgcore", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 preload_script(IntPtr pduel, IntPtr script, Int32 len);
    }
    #endregion
    public class smallYgopro
    {
        #region DoNotCareAboutThis

        //public
        public delegate CardData cardHandler(long code);
        public delegate void chatHandler(string str);
        chatHandler cast;

        public Action<string> m_log;        

        void DebugLog(string obj) 
        {
            if (m_log != null)  
            {
                m_log(obj);
            }
        }

        public smallYgopro(Action<byte[]> HowToSendBufferToPlayer,cardHandler HowToReadCard,chatHandler HowToShowLog)
        {
            sendToPlayer = HowToSendBufferToPlayer;
            dll.set_card_api(HowToReadCard);
            dll.set_chat_api(HowToShowLog);
            cast = HowToShowLog;
            Random ran = new Random(Environment.TickCount);
            duel = dll.create_duel((UInt32)ran.Next(100, 99999));
        }

        public void dispose()
        {
            dll.end_duel(duel);
            Random ran = new Random(Environment.TickCount);
            duel =dll.create_duel((UInt32)ran.Next(100, 99999));
        }

        public bool startPuzzle(System.String path)
        {
            godMode = true;
            isFirst = true;
           dll.set_player_info(duel, 0, 8000, 5, 1);
           dll.set_player_info(duel, 1, 8000, 5, 1);
            var reult = 0;
            for (int i = 0; i < 10; i++)
            {
                reult =dll.preload_script(duel, getPtrString(path), path.Length);
                if (reult > 0)
                {
                    break;
                }
            }
            if (reult == 0)
            {
                return false;
            }
           dll.start_duel(duel, 0);
            Refresh();
            (new Thread(Process)).Start();
            return true;
        }

        public bool startAI(string playerDek, string aiDeck, string aiScript, bool playerGoFirst, bool unrand, int life, bool god,int mr)
        {
            godMode = god;
            isFirst = playerGoFirst;
           dll.set_player_info(duel, 0, life, 5, 1);
           dll.set_player_info(duel, 1, life, 5, 1);
            var reult = 0;
            for (int i = 0; i < 10; i++)
            {
                reult =dll.preload_script(duel, getPtrString(aiScript), aiScript.Length);
                if (reult > 0)
                {
                    break;
                }
            }
            if (reult == 0)
            {
                return false;
            }
            addDeck(playerDek, (playerGoFirst ? 0 : 1), !unrand);
            addDeck(aiDeck, (playerGoFirst ? 1 : 0), true);
           dll.set_ai_id(duel, playerGoFirst ? 1 : 0);
            int opt = 0;
            opt |= 0x80;
            if (unrand)
            {
                opt |= 0x10;
            }
            BinaryMaster master = new BinaryMaster();
            master.writer.Write((char)GameMessage.Start);
            master.writer.Write((byte)(playerGoFirst ? 0xf0 : 0xff));
            master.writer.Write((int)life);
            master.writer.Write((int)life);
            master.writer.Write((UInt16)dll.query_field_count(duel, 0, 0x1));
            master.writer.Write((UInt16)dll.query_field_count(duel, 0, 0x40));
            master.writer.Write((UInt16)dll.query_field_count(duel, 1, 0x1));
            master.writer.Write((UInt16)dll.query_field_count(duel, 1, 0x40));
            sendToPlayer(master.get());
            dll.start_duel(duel, (opt | (mr << 16)));
            Refresh();
            (new Thread(Process)).Start();
            return true;
        }

        public void response(byte[] resp)
        {
            if (resp.Length > 64) return;
            IntPtr buf = Marshal.AllocHGlobal(64);
            Marshal.Copy(resp, 0, buf, resp.Length);
           dll.set_responseb(duel, buf);
            Marshal.FreeHGlobal(buf);
            (new Thread(Process)).Start();
        }

        //private

        private IntPtr _buffer = Marshal.AllocHGlobal(4096);

        private IntPtr duel = default(IntPtr);

        private Action<byte[]> sendToPlayer;

        private bool godMode = false;

        private IntPtr getPtrString(string path)
        {
            IntPtr ptrFileName = Marshal.AllocHGlobal(path.Length + 1);
            byte[] s = System.Text.Encoding.UTF8.GetBytes(path);
            Marshal.Copy(s, 0, ptrFileName, s.Length);
            return ptrFileName;
        }

        private Deck FromYDKtoDeck(string path)
        {
            Deck deck = new Deck();
            try
            {
                string text = System.IO.File.ReadAllText(path);
                string st = text.Replace("\r", "");
                string[] lines = st.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                int flag = -1;
                foreach (string line in lines)
                {
                    if (line == "#main")
                    {
                        flag = 1;
                    }
                    else if (line == "#extra")
                    {
                        flag = 2;
                    }
                    else if (line == "!side")
                    {
                        flag = 3;
                    }
                    else
                    {
                        int code = 0;
                        try
                        {
                            code = Int32.Parse(line);
                        }
                        catch (Exception)
                        {

                        }
                        if (code > 100)
                        {
                            switch (flag)
                            {
                                case 1:
                                    {
                                        deck.Main.Add(code);
                                    }
                                    break;
                                case 2:
                                    {
                                        deck.Extra.Add(code);
                                    }
                                    break;
                                case 3:
                                    {
                                        deck.Side.Add(code);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            return deck;
        }

        private void addDeck(string playerDek, int playerId, bool rand)
        {
            var deck_player = FromYDKtoDeck(playerDek);
            if (rand)
            {
                System.Random seed = new System.Random();
                for (int i = 0; i < deck_player.Main.Count; i++)
                {
                    int random_index = seed.Next() % deck_player.Main.Count;
                    var t = deck_player.Main[i];
                    deck_player.Main[i] = deck_player.Main[random_index];
                    deck_player.Main[random_index] = t;
                }
            }
            for (int i = deck_player.Main.Count - 1; i >= 0; i--)
            {
               dll.new_card(duel, (uint)deck_player.Main[i],
                    (byte)playerId, (byte)playerId, (byte)CardLocation.Deck, 0, 0);
            }
            for (int i = 0; i < deck_player.Extra.Count; i++)
            {
               dll.new_card(duel, (uint)deck_player.Extra[i],
                    (byte)playerId, (byte)playerId, (byte)CardLocation.Extra, 0, 0);
            }
        }

        void sendToYrp(byte[] buffer)
        {
            yrp3dbuilder.Write(buffer[0]);
            yrp3dbuilder.Write(buffer.Length-1);
            for (int i = 1; i < buffer.Length; i++) 
            {
                yrp3dbuilder.Write(buffer[i]);
            }
        }

        BinaryWriter yrp3dbuilder;
        public byte[] getYRP3dBuffer(YRP yrp)
        {
            var tempS = sendToPlayer;
            sendToPlayer = sendToYrp;
            MemoryStream stream = new MemoryStream();
            yrp3dbuilder = new BinaryWriter(stream);
            sendToPlayer(yrp.getNamePacket());
            dll.end_duel(duel);
            duel = dll.create_duel(yrp.Seed);
            godMode = true;
            isFirst = true;
            dll.set_player_info(duel, 0, yrp.StartLp, yrp.StartHand, yrp.DrawCount);
            dll.set_player_info(duel, 1, yrp.StartLp, yrp.StartHand, yrp.DrawCount);
            if (yrp.playerData.Count == 4)
            {
                foreach (var item in yrp.playerData[0].main)
                {
                    dll.new_card(duel, (uint)item, (byte)0, (byte)0, (byte)CardLocation.Deck, 0, 0);
                }
                foreach (var item in yrp.playerData[0].extra)
                {
                    dll.new_card(duel, (uint)item, (byte)0, (byte)0, (byte)CardLocation.Extra, 0, 0);
                }
                foreach (var item in yrp.playerData[1].main)
                {
                    dll.new_tag_card(duel, (uint)item, (byte)0, (byte)CardLocation.Deck);
                }
                foreach (var item in yrp.playerData[1].extra)
                {
                    dll.new_tag_card(duel, (uint)item, (byte)0, (byte)CardLocation.Extra);
                }

                foreach (var item in yrp.playerData[2].main)
                {
                    dll.new_card(duel, (uint)item, (byte)1, (byte)1, (byte)CardLocation.Deck, 0, 0);
                }
                foreach (var item in yrp.playerData[2].extra)
                {
                    dll.new_card(duel, (uint)item, (byte)1, (byte)1, (byte)CardLocation.Extra, 0, 0);
                }
                foreach (var item in yrp.playerData[3].main)
                {
                    dll.new_tag_card(duel, (uint)item, (byte)1, (byte)CardLocation.Deck);
                }
                foreach (var item in yrp.playerData[3].extra)
                {
                    dll.new_tag_card(duel, (uint)item, (byte)1, (byte)CardLocation.Extra);
                }
            }
            else
            {
                foreach (var item in yrp.playerData[0].main)
                {
                    dll.new_card(duel, (uint)item, (byte)0, (byte)0, (byte)CardLocation.Deck, 0, 0);
                }
                foreach (var item in yrp.playerData[0].extra)
                {
                    dll.new_card(duel, (uint)item, (byte)0, (byte)0, (byte)CardLocation.Extra, 0, 0);
                }

                foreach (var item in yrp.playerData[1].main)
                {
                    dll.new_card(duel, (uint)item, (byte)1, (byte)1, (byte)CardLocation.Deck, 0, 0);
                }
                foreach (var item in yrp.playerData[1].extra)
                {
                    dll.new_card(duel, (uint)item, (byte)1, (byte)1, (byte)CardLocation.Extra, 0, 0);
                }
            }
            BinaryMaster master = new BinaryMaster();
            master.writer.Write((char)GameMessage.Start);
            master.writer.Write((byte)0);
            master.writer.Write(yrp.StartLp);
            master.writer.Write(yrp.StartLp);
            master.writer.Write((UInt16)dll.query_field_count(duel, 0, 0x1));
            master.writer.Write((UInt16)dll.query_field_count(duel, 0, 0x40));
            master.writer.Write((UInt16)dll.query_field_count(duel, 1, 0x1));
            master.writer.Write((UInt16)dll.query_field_count(duel, 1, 0x40));
            sendToPlayer(master.get());
            dll.start_duel(duel, yrp.opt);
            Refresh();
            end = false;
            err = false;
            try
            {
                while (true)
                {
                    //log("process");
                    Process();
                    if (yrp.gameData.Count==0)  
                    {
                        break;
                    }
                    if (yrp.gameData[0].Length > 64) break;
                    IntPtr buf = Marshal.AllocHGlobal(64);
                    Marshal.Copy(yrp.gameData[0], 0, buf, yrp.gameData[0].Length);
                    dll.set_responseb(duel, buf);
                    Marshal.FreeHGlobal(buf);
                    DebugLog("Push:  "+BitConverter.ToString(yrp.gameData[0]));
                    yrp.gameData.RemoveAt(0);
                    if (end)    
                    {
                        break;
                    }
                }
            }
            catch (Exception) 
            {
            }
            if (err)    
            {
                if (cast != null)
                {
                    cast("Error Occurred.");
                }
            }

            dispose();
            sendToPlayer = tempS;
            yrp3dbuilder.Close();
            stream.Close();
            return stream.ToArray();
        }

        #endregion

        //you can edit all codes safely after this line 

        //the HintInGame will be showed in ai mode window
        public static string HintInGame = "PercyAI Pro2Team 1033.D";

        void Process()
        {
            while (true)
            {
                int result =dll.process(duel);
                int len = result & 0xFFFF;
                if (len > 0)
                {
                    byte[] arr = new byte[4096];
                   dll.get_message(duel, _buffer);
                    Marshal.Copy(_buffer, arr, 0, 4096);
                    bool breakOut = false;
                    MemoryStream stream = new MemoryStream(arr);
                    BinaryReader reader = new BinaryReader(stream);
                    while (stream.Position < len)
                    {
                        //log("Analyse");
                        breakOut = Analyse(reader);
                    }
                    if (breakOut)
                    {
                        break;
                    }
                }
                //else
                //{
                //    log("len == 0");
                //    end = true;
                //    break;
                //}
            }
        }

        BinaryReader currentReader;

        BinaryWriter currentWriter;

        int move(int length, bool erase = false)
        {
            int returnValue = 0;
            if (length > 0)
            {
                if (currentReader != null)
                {
                    if (currentWriter != null)
                    {
                        try
                        {
                            byte[] readed = currentReader.ReadBytes(length);
                            if (readed.Length > 0)
                            {
                                returnValue = readed[0];
                            }
                            if (erase)
                            {
                                for (int i = 0; i < length; i++)
                                {
                                    currentWriter.Write((byte)0);
                                }
                            }
                            else
                            {
                                currentWriter.Write(readed);
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
            }
            return returnValue;
        }

        void flush()
        {
            sendToPlayer(((MemoryStream)currentWriter.BaseStream).ToArray());
        }

        bool isFirst = true;

        int localPlayer(int p)
        {
            if (isFirst)
            {
                return p;
            }
            else
            {
                return 1 - p;
            }
        }

        void Refresh()
        {
            if (godMode)
            {
                RefreshMonsters(0);
                RefreshMonsters(1);
                RefreshSpells(0);
                RefreshSpells(1);
                RefreshHand(0);
                RefreshHand(1);
                RefreshGrave(0);
                RefreshGrave(1);
                RefreshExtra(0);
                RefreshExtra(1);
                RefreshDeck(0);
                RefreshDeck(1);
                RefreshRemoved(0);
                RefreshRemoved(1);
            }
            else
            {
                if (isFirst)
                {
                    RefreshMonsters(0);
                    RefreshMonsters(1);
                    RefreshSpells(0);
                    RefreshSpells(1);
                    RefreshGrave(0);
                    RefreshGrave(1);
                    RefreshHand(0);
                    RefreshExtra(0);
                    RefreshRemoved(0);
                }
                else
                {
                    RefreshMonsters(0);
                    RefreshMonsters(1);
                    RefreshSpells(0);
                    RefreshSpells(1);
                    RefreshGrave(0);
                    RefreshGrave(1);
                    RefreshHand(1);
                    RefreshExtra(1);
                    RefreshRemoved(1);
                }
            }
        }

        byte[] QueryFieldCard(int player, CardLocation location, int flag, bool useCache)
        {
            int len =dll.query_field_card(duel, (byte)player, (byte)location, flag, _buffer, useCache ? 1 : 0);
            byte[] result = new byte[len];
            Marshal.Copy(_buffer, result, 0, len);
            return result;
        }

        void RefreshMonsters(int player, int flag = 0x81fff | 0x10000)
        {
            byte[] result = QueryFieldCard(player, CardLocation.MonsterZone, flag, false);
            var binary = new BinaryMaster();
            binary.writer.Write((byte)GameMessage.UpdateData);
            binary.writer.Write((byte)player);
            binary.writer.Write((byte)CardLocation.MonsterZone);

            MemoryStream ms = new MemoryStream(result);
            BinaryReader reader = new BinaryReader(ms);
            for (int i = 0; i < 5; i++)
            {
                int len = reader.ReadInt32();
                if (len == 4)
                {
                    binary.writer.Write(4);
                    continue;
                }

                byte[] raw = reader.ReadBytes(len - 4);
                if ((raw[11] & (int)CardPosition.FaceDown) != 0 && godMode == false && localPlayer(player) != 0)
                {
                    binary.writer.Write(8);
                    binary.writer.Write(0);
                }
                else
                {
                    binary.writer.Write(len);
                    binary.writer.Write(raw);
                }
            }

            sendToPlayer(binary.get());
        }

        void RefreshSpells(int player, int flag = 0x681fff)
        {
            byte[] result = QueryFieldCard(player, CardLocation.SpellZone, flag, false);
            var binary = new BinaryMaster();
            binary.writer.Write((byte)GameMessage.UpdateData);
            binary.writer.Write((byte)player);
            binary.writer.Write((byte)CardLocation.SpellZone);

            MemoryStream ms = new MemoryStream(result);
            BinaryReader reader = new BinaryReader(ms);
            for (int i = 0; i < 8; i++)
            {
                int len = reader.ReadInt32();
                if (len == 4)
                {
                    binary.writer.Write(4);
                    continue;
                }

                byte[] raw = reader.ReadBytes(len - 4);
                if ((raw[11] & (int)CardPosition.FaceDown) != 0 && godMode == false && localPlayer(player) != 0)
                {
                    binary.writer.Write(8);
                    binary.writer.Write(0);
                }
                else
                {
                    binary.writer.Write(len);
                    binary.writer.Write(raw);
                }
            }
            sendToPlayer(binary.get());
        }

        void RefreshHand(int player, int flag = 0x181fff)
        {
            byte[] result = QueryFieldCard(player, CardLocation.Hand, flag, false);
            var binary = new BinaryMaster();
            binary.writer.Write((byte)GameMessage.UpdateData);
            binary.writer.Write((byte)player);
            binary.writer.Write((byte)CardLocation.Hand);
            binary.writer.Write(result);
            sendToPlayer(binary.get());
        }

        void RefreshGrave(int player, int flag = 0x81fff)
        {
            byte[] result = QueryFieldCard(player, CardLocation.Grave, flag, false);
            var binary = new BinaryMaster();
            binary.writer.Write((byte)GameMessage.UpdateData);
            binary.writer.Write((byte)player);
            binary.writer.Write((byte)CardLocation.Grave);
            binary.writer.Write(result);
            sendToPlayer(binary.get());
        }

        void RefreshDeck(int player, int flag = 0x81fff)
        {
            byte[] result = QueryFieldCard(player, CardLocation.Deck, flag, false);
            var binary = new BinaryMaster();
            binary.writer.Write((byte)GameMessage.UpdateData);
            binary.writer.Write((byte)player);
            binary.writer.Write((byte)CardLocation.Deck);
            binary.writer.Write(result);
            sendToPlayer(binary.get());
        }

        void RefreshExtra(int player, int flag = 0x81fff)
        {
            byte[] result = QueryFieldCard(player, CardLocation.Extra, flag, false);
            var binary = new BinaryMaster();
            binary.writer.Write((byte)GameMessage.UpdateData);
            binary.writer.Write((byte)player);
            binary.writer.Write((byte)CardLocation.Extra);
            binary.writer.Write(result);
            sendToPlayer(binary.get());
        }

        void RefreshRemoved(int player, int flag = 0x81fff)
        {
            byte[] result = QueryFieldCard(player, CardLocation.Removed, flag, false);
            var binary = new BinaryMaster();
            binary.writer.Write((byte)GameMessage.UpdateData);
            binary.writer.Write((byte)player);
            binary.writer.Write((byte)CardLocation.Removed);
            binary.writer.Write(result);
            sendToPlayer(binary.get());
        }

        bool end = false;
        bool err = false;   
        bool Analyse(BinaryReader reader)
        {
            bool returnValue = false;
            currentReader = reader;
            MemoryStream me = new MemoryStream();
            currentWriter = new BinaryWriter(me);
            int player = 0;
            int count = 0;
            GameMessage mes = (GameMessage)move(1);
            //log(mes.ToString());
            switch (mes)
            {
                case GameMessage.Retry:
                    returnValue = true;
                    err = true;
                    //end = true;
                    break;
                case GameMessage.Hint:
                    move(6);
                    break;
                case GameMessage.Waiting:
                    break;
                case GameMessage.Start:
                    break;
                case GameMessage.Win:
                    move(2);
                    returnValue = true;
                    end = true;
                    break;
                case GameMessage.UpdateData:
                    break;
                case GameMessage.UpdateCard:
                    break;
                case GameMessage.RequestDeck:
                    break;
                case GameMessage.SelectBattleCmd:
                    move(1);
                    move(move(1) * 11);
                    move(move(1) * 8 + 2);
                    returnValue = true;
                    break;
                case GameMessage.SelectIdleCmd:
                    move(1);
                    move(move(1) * 7);
                    move(move(1) * 7);
                    move(move(1) * 7);
                    move(move(1) * 7);
                    move(move(1) * 7);
                    move(move(1) * 11 + 3);
                    returnValue = true;
                    break;
                case GameMessage.SelectEffectYn:
                    move(13);
                    returnValue = true;
                    break;
                case GameMessage.SelectYesNo:
                    move(5);
                    returnValue = true;
                    break;
                case GameMessage.SelectOption:
                    move(1);
                    move(move(1) * 4);
                    returnValue = true;
                    break;
                case GameMessage.SelectTribute:
                case GameMessage.SelectCard:
                    player = move(1);
                    move(3);
                    count = move(1);
                    for (int i = 0; i < count; i++)
                    {
                        int code = currentReader.ReadInt32();
                        int p = currentReader.ReadByte();
                        currentWriter.Write(((int)(p == player ? code : 0)));
                        currentWriter.Write((byte)p);
                        move(3);
                    }
                    returnValue = true;
                    break;
                case GameMessage.SelectUnselectCard:
                    player = move(1);
                    int buttonok = move(1);
                    move(3);
                    int count1 = move(1);
                    for (int i = 0; i < count1; i++)
                    {
                        int code = currentReader.ReadInt32();
                        int p = currentReader.ReadByte();
                        currentWriter.Write(((int)(p == player ? code : 0)));
                        currentWriter.Write((byte)p);
                        move(3);
                    }
                    int count2 = move(1);
                    for (int i = 0; i < count2; i++)
                    {
                        int code = currentReader.ReadInt32();
                        int p = currentReader.ReadByte();
                        //currentWriter.Write(((int)(p == player ? code : 0)));
                        //currentWriter.Write((byte)p);
                        move(3);
                    }
                    returnValue = true;
                    break;
                case GameMessage.SelectChain:
                    move(1);
                    count = move(1);
                    move(1);
                    move(1);
                    move(4);
                    move(4);
                    for (int i = 0; i < count; i++)
                    {
                        move(1);
                        move(4);
                        move(4);
                        move(4);
                    }
                    returnValue = true;
                    break;
                case GameMessage.SelectDisfield:
                case GameMessage.SelectPlace:
                case GameMessage.SelectPosition:
                    move(6);
                    returnValue = true;
                    break;
                case GameMessage.SelectCounter:
                    move(5);
                    move(move(1) * 9);
                    returnValue = true;
                    break;
                case GameMessage.SelectSum:
                    move(8);
                    move(move(1) * 11);
                    move(move(1) * 11);
                    returnValue = true;
                    break;
                case GameMessage.SortChain:
                case GameMessage.SortCard:
                    move(1);
                    move(move(1) * 7);
                    returnValue = true;
                    break;
                case GameMessage.ConfirmDecktop:
                    move(1);
                    move(move(1) * 7);
                    break;
                case GameMessage.ConfirmCards:
                    move(1);
                    move(move(1) * 7);
                    break;
                case GameMessage.RefreshDeck:
                case GameMessage.ShuffleDeck:
                    move(1);
                    break;
                case GameMessage.ShuffleHand:
                    move(1);
                    move(move(1) * 4);
                    break;
                case GameMessage.SwapGraveDeck:
                    move(1);
                    break;
                case GameMessage.ShuffleSetCard:
                    move(1);
                    move(move(1) * 8);
                    break;
                case GameMessage.ReverseDeck:
                    break;
                case GameMessage.DeckTop:
                    move(6);
                    break;
                case GameMessage.NewTurn:
                    move(1);
                    break;
                case GameMessage.NewPhase:
                    move(2);
                    break;
                case GameMessage.Move:
                    byte[] raw = currentReader.ReadBytes(16);
                    int pc = raw[4];
                    int pl = raw[5];
                    int cc = raw[8];
                    int cl = raw[9];
                    int cs = raw[10];
                    int cp = raw[11];

                    if (!Convert.ToBoolean((cl & ((int)CardLocation.Grave + (int)CardLocation.Overlay))) && Convert.ToBoolean((cl & ((int)CardLocation.Deck + (int)CardLocation.Hand)))
                        || Convert.ToBoolean((cp & (int)CardPosition.FaceDown)))
                    {
                        raw[0] = 0;
                        raw[1] = 0;
                        raw[2] = 0;
                        raw[3] = 0;
                    }
                    currentWriter.Write(raw);
                    break;
                case GameMessage.PosChange:
                    move(9);
                    break;
                case GameMessage.Set:
                    move(4, true);
                    move(4);
                    break;
                case GameMessage.Swap:
                    move(16);
                    break;
                case GameMessage.FieldDisabled:
                    move(4);
                    break;
                case GameMessage.Summoning:
                    move(8);
                    break;
                case GameMessage.Summoned:
                    break;
                case GameMessage.SpSummoning:
                    move(8);
                    break;
                case GameMessage.SpSummoned:
                    break;
                case GameMessage.FlipSummoning:
                    move(8);
                    break;
                case GameMessage.FlipSummoned:
                    break;
                case GameMessage.Chaining:
                    move(16);
                    break;
                case GameMessage.Chained:
                    move(1);
                    break;
                case GameMessage.ChainSolving:
                    move(1);
                    break;
                case GameMessage.ChainSolved:
                    move(1);
                    break;
                case GameMessage.ChainEnd:
                    break;
                case GameMessage.ChainNegated:
                case GameMessage.ChainDisabled:
                    move(1);
                    break;
                case GameMessage.CardSelected:
                    move(1);
                    move(move(1) * 4);
                    break;
                case GameMessage.RandomSelected:
                    move(1);
                    move(move(1) * 4);
                    break;
                case GameMessage.BecomeTarget:
                    move(move(1) * 4);
                    break;
                case GameMessage.Draw:
                    player = move(1);
                    count = move(1);
                    for (int i = 0; i < count; i++)
                    {
                        int code = currentReader.ReadInt32() & 0x7fffffff;
                        if (isFirst)
                        {
                            if (player == 0)
                            {
                                currentWriter.Write(code);
                            }
                            else
                            {
                                currentWriter.Write(0);
                            }
                        }
                        else
                        {
                            if (player == 0)
                            {
                                currentWriter.Write(0);
                            }
                            else
                            {
                                currentWriter.Write(code);
                            }
                        }
                    }
                    break;
                case GameMessage.PayLpCost:
                case GameMessage.LpUpdate:
                case GameMessage.Damage:
                case GameMessage.Recover:
                    move(5);
                    break;
                case GameMessage.Equip:
                    move(8);
                    break;
                case GameMessage.Unequip:
                    move(4);
                    break;
                case GameMessage.CardTarget:
                case GameMessage.CancelTarget:
                    move(8);
                    break;
                case GameMessage.AddCounter:
                case GameMessage.RemoveCounter:
                    move(7);
                    break;
                case GameMessage.Attack:
                    move(8);
                    break;
                case GameMessage.Battle:
                    move(26);
                    break;
                case GameMessage.AttackDiabled:
                    break;
                case GameMessage.DamageStepStart:
                    break;
                case GameMessage.DamageStepEnd:
                    break;
                case GameMessage.MissedEffect:
                    move(8);
                    break;
                case GameMessage.BeChainTarget:
                    break;
                case GameMessage.CreateRelation:
                    break;
                case GameMessage.ReleaseRelation:
                    break;
                case GameMessage.TossCoin:
                case GameMessage.TossDice:
                    move(1);
                    move(move(1));
                    break;
                case GameMessage.AnnounceRace:
                    move(6);
                    returnValue = true;
                    break;
                case GameMessage.AnnounceAttrib:
                    move(6);
                    returnValue = true;
                    break;
                case GameMessage.AnnounceCard:
                    move(5);
                    returnValue = true;
                    break;
                case GameMessage.AnnounceCardFilter:
                case GameMessage.AnnounceNumber:
                    move(1);
                    move(move(1) * 4);
                    returnValue = true;
                    break;
                case GameMessage.CardHint:
                    move(9);
                    break;
                case GameMessage.TagSwap:
                    player = move(1);
                    move(1);
                    int ecount = move(1);
                    move(1);
                    int hcount = move(1);
                    move(4);
                    for (int i = 0; i < hcount + ecount; i++)
                    {
                        uint code = currentReader.ReadUInt32();
                        if ((code & 0x80000000) != 0)
                            currentWriter.Write(code);
                        else
                            currentWriter.Write(0);
                    }
                    break;
                case GameMessage.ReloadField:
                    move(1);
                    for (int i_ = 0; i_ < 2; i_++)
                    {
                        move(4);
                        for (int i = 0; i < 7; i++)
                        {
                            int val = move(1);
                            if (val > 0)
                            {
                                move(2);
                            }
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            int val = move(1);
                            if (val > 0)
                            {
                                move(1);
                            }
                        }
                        move(1);
                        move(1);
                        move(1);
                        move(1);
                        move(1);
                        move(1);
                        move(move(1) * 15);
                    }
                    break;
                case GameMessage.AiName:
                    var length = currentReader.ReadUInt16();
                    currentWriter.Write(length);
                    move(length + 1);
                    break;
                case GameMessage.ShowHint:
                    var length2 = currentReader.ReadUInt16();
                    currentWriter.Write(length2);
                    move(length2 + 1);
                    break;
                case GameMessage.PlayerHint:
                    move(6);
                    break;
                case GameMessage.MatchKill:
                    move(4);
                    break;
                case GameMessage.CustomMsg:
                    break;
                case GameMessage.DuelWinner:
                    break;
                default:
                    returnValue = true;
                    break;
            }
            flush();
            switch (mes)
            {
                case GameMessage.RefreshDeck:
                case GameMessage.SwapGraveDeck:
                case GameMessage.ShuffleSetCard:
                case GameMessage.ShuffleDeck:
                case GameMessage.ShuffleHand:
                case GameMessage.ReverseDeck:
                case GameMessage.DeckTop:
                case GameMessage.Summoned:
                case GameMessage.SpSummoned:
                case GameMessage.FlipSummoned:
                case GameMessage.ChainSolved:
                case GameMessage.ChainEnd:
                case GameMessage.ChainNegated:
                case GameMessage.ChainDisabled:
                case GameMessage.Battle:
                case GameMessage.DamageStepEnd:
                case GameMessage.TagSwap:
                case GameMessage.ReloadField:
                case GameMessage.Draw:
                case GameMessage.Set:
                    Refresh();
                    break;
            }
            DebugLog(mes.ToString() + (returnValue ? (" Wating Buffer:\n"+BitConverter.ToString(((MemoryStream)(currentWriter.BaseStream)).ToArray())) : ""));
            return returnValue;
        }
    }
    public class YRP    
    {
        public int ID = 0;
        public int Version = 0;
        public int Flag = 0;
        public uint Seed = 0;
        public long DataSize = 0;
        public int Hash = 0;
        public byte[] Props = new byte[8];
        public int StartLp = 0;
        public int StartHand = 0;
        public int DrawCount = 0;
        public int opt = 0;

        public class PlayerData
        {
            public string name;
            public List<int> main = new List<int>();
            public List<int> extra = new List<int>();
        }
        public List<PlayerData> playerData = new List<PlayerData>();
        public List<byte[]> gameData = new List<byte[]>();

        public byte[] getNamePacket()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            if (playerData.Count == 4)
            {
                WriteUnicode(writer, playerData[0].name, 50);
                WriteUnicode(writer, playerData[1].name, 50);
                WriteUnicode(writer, playerData[0].name, 50);
                WriteUnicode(writer, playerData[2].name, 50);
                WriteUnicode(writer, playerData[3].name, 50);
                WriteUnicode(writer, playerData[2].name, 50);
            }
            else
            {
                WriteUnicode(writer, playerData[0].name, 50);
                WriteUnicode(writer, playerData[0].name, 50);
                WriteUnicode(writer, playerData[0].name, 50);
                WriteUnicode(writer, playerData[1].name, 50);
                WriteUnicode(writer, playerData[1].name, 50);
                WriteUnicode(writer, playerData[1].name, 50);
            }
            BinaryWriter Rwriter = new BinaryWriter(new MemoryStream());
            Rwriter.Write((byte)235);
            Rwriter.Write(stream.ToArray());
            return ((MemoryStream)(Rwriter.BaseStream)).ToArray();
        }

        void WriteUnicode(BinaryWriter writer, string text, int len)
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
    }
}
