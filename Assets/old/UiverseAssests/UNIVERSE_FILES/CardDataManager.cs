//using Mono.Data.Sqlite;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using SQLiteConnection = Mono.Data.Sqlite.SqliteConnection;
//using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;
//using SQLiteDataReader = Mono.Data.Sqlite.SqliteDataReader;
//public class CardDataManager
//{
//    List<CardData> CardDatas;
//    public CardDataManager()
//    {
//        CardDatas = new List<CardData>();

//        DirectoryInfo TheFolder = new DirectoryInfo("data_card");
//        FileInfo[] fileInfo = TheFolder.GetFiles();
//        foreach (FileInfo NextFile in fileInfo)
//        {
//            string path = "data_card\\" + NextFile.Name;
//            load(path);
//        }

//    }




//    public void load(string path)
//    {

//        using (SQLiteConnection connection = new SQLiteConnection("Data Source=" + path))
//        {
//            connection.Open();

//            using (SQLiteCommand command = new SQLiteCommand("SELECT datas.*, texts.* FROM datas,texts WHERE datas.id=texts.id;",
//                                                            connection))
//            {
//                using (SQLiteDataReader reader = command.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        try
//                        {
//                            CardData c = new CardData();
//                            c.Str = new string[16];
//                            c.code = reader.GetInt64(0);
//                            c.Ot = reader.GetInt32(1);
//                            c.Alias = reader.GetInt64(2);
//                            c.SetCode = reader.GetInt64(3);
//                            c.Type = reader.GetInt64(4);
//                            c.Attack = reader.GetInt32(5);
//                            c.Defense = reader.GetInt32(6);
//                            long Level_raw = reader.GetInt64(7);
//                            c.Level = Level_raw & 0xff;
//                            c.LeftScale = (Level_raw >> 0x18) & 0xff;
//                            c.RightScale = (Level_raw >> 0x10) & 0xff;
//                            c.Race = reader.GetInt32(8);
//                            c.Attribute = reader.GetInt32(9);
//                            c.Category = reader.GetInt64(10);
//                            c.Name = reader.GetString(12);
//                            c.Desc = reader.GetString(13);
//                            c.tail = "";
//                            for (int ii = 0; ii < 0x10; ii++)
//                            {
//                                c.Str[ii] = reader.GetString(14 + ii);
//                            }
//                            CardDatas.Add(c);
//                        }
//                        catch (Exception e)
//                        {

//                        }
//                    }
//                }
//            }
//        }
//        /////////////////////////////////////////////////////
//        //string text_all = System.IO.File.ReadAllText(path);
//        //string[] card_strings = text_all.Split(new string[] { "/*/*/*/" }, StringSplitOptions.RemoveEmptyEntries);
//        //foreach (string one_card_string in card_strings)
//        //{
//        //    try
//        //    {
//        //        CardData c = new CardData();
//        //        c.Str = new string[16];
//        //        string[] elements = one_card_string.Split(new string[] { "|-" }, StringSplitOptions.RemoveEmptyEntries);



//        //        c.code = long.Parse(elements[0]);
//        //        c.Ot = int.Parse(elements[1]);
//        //        c.Alias = long.Parse(elements[2]);
//        //        c.SetCode = int.Parse(elements[3]);
//        //        c.Type = int.Parse(elements[4]);
//        //        c.Attack = int.Parse(elements[5]);
//        //        c.Defense = int.Parse(elements[6]);
//        //        c.Level = int.Parse(elements[7]);
//        //        c.LeftScale = int.Parse(elements[8]);
//        //        c.RightScale = int.Parse(elements[9]);
//        //        c.Race = int.Parse(elements[10]);
//        //        c.Attribute = int.Parse(elements[11]);
//        //        c.Name = elements[12];
//        //        c.Desc = elements[13];
//        //        for (int i = 0; i < 16;i++ )
//        //        {

//        //            if (elements.Length > 14 + i) 
//        //                c.Str[i] = elements[14 + i];
//        //        }

//        //        CardDatas.Add(c);
//        //    }
//        //    catch(Exception e)
//        //    {

//        //    }

//        //}
//    }




//    public CardData GetById(long id)
//    {
//        CardData card = new CardData();
//        card.code = 0;
//        card.Name = "预留广告位";
//        card.Desc = "预留广告位 \n联系作者qq:914847518";
//        if (id > 0) for (int i = 0; i < 10; i++)
//            {
//                foreach (CardData one_CardData in CardDatas)
//                {
//                    if (one_CardData.code == id - i)
//                    {
//                        card = one_CardData;
//                        break;
//                    }
//                }
//                if (card.code > 0) break;
//            }
//        return card;
//    }
//    public List<CardData> search(UInt32 type, string str)
//    {
//        List<CardData> return_valse = new List<CardData>();
//        if (str!="")
//        {
//            foreach (CardData one_CardData in CardDatas)
//            {
//                if (one_CardData.Name.Replace(str, "") != one_CardData.Name)
//                {
//                    if ((one_CardData.Type | type) > 0)
//                    {
//                        return_valse.Add(one_CardData);
//                    }
//                }
//            }
//        }
//        return return_valse;
//    }

//    public List<CardData> search_advanced
//        (
//        UInt32 fil_type_temp,
//        UInt32 fil_type_race_temp,
//        UInt32 fil_type_attribute_temp, 
//        UInt32 fil_type_catagory_temp, 
//        string str,
//        int atk=-233,
//        int def=-233,
//        int star=-233
//        )
//    {
//        //if (fil_type_temp==0)
//        //{
//        //    if (fil_type_race_temp == 0)
//        //    {
//        //        if (fil_type_attribute_temp == 0)
//        //        {
//        //            if (fil_type_catagory_temp == 0)
//        //            {
//        //                if (str=="")
//        //                {
//        //                    if (atk==-233)
//        //                    {
//        //                        if (def == -233)
//        //                        {
//        //                            if (star == -233)
//        //                            {
//        //                                return new List<CardData>();
//        //                            }
//        //                        }
//        //                    }
//        //                }
//        //            }
//        //        }
//        //    }
//        //}
//        UInt32 fil_type = fil_type_temp;
//        if (fil_type == 0)
//        {
//            //fil_type = 0xffffffff;
//        }
//        UInt32 fil_type_race = fil_type_race_temp;
//        if (fil_type_race == 0)
//        {
//            //fil_type_race = 0xffffffff;
//        }
//        UInt32 fil_type_attribute = fil_type_attribute_temp;
//        if (fil_type_attribute == 0)
//        {
//            //fil_type_attribute = 0xffffffff;
//        }
//        UInt32 fil_type_catagory = fil_type_catagory_temp;
//        if (fil_type_catagory == 0)
//        {
//           // fil_type_catagory = 0xffffffff;
//        }
//        List<CardData> return_valse = new List<CardData>();
//        //if (str != "")
//        {
//            foreach (CardData one_CardData in CardDatas)
//            {
//                string string_name = one_CardData.Name;
//                if (str == "" || string_name.Replace(str, "") != string_name || one_CardData.code.ToString() == str)
//                {
//                    if ((one_CardData.Type & fil_type) == fil_type || (fil_type == 0))
//                    {
//                        if ((one_CardData.Race & fil_type_race) == fil_type_race || (fil_type_race == 0))
//                        {
//                            if (((UInt32)one_CardData.Attribute & fil_type_attribute) == fil_type_attribute || (fil_type_attribute == 0))
//                            {
//                                if ((one_CardData.Category & fil_type_catagory) == fil_type_catagory || (fil_type_catagory == 0))
//                                {
//                                    if (one_CardData.Attack == atk || (atk == -233))
//                                    {
//                                        if (one_CardData.Defense == def || (def == -233))
//                                        {
//                                            if (one_CardData.Level == star || (star == -233))
//                                            {
//                                                return_valse.Add(one_CardData);
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        return_valse.Sort(comparisonOfCard());
//        return return_valse;
//    }

//    private static Comparison<CardData> comparisonOfCard()
//    {
//        return (left, right) =>
//        {
//            int a = 1;
//            if ((left.Type & 7) < (right.Type & 7))
//            {
//                a = -1;
//            }
//            else if ((left.Type & 7) > (right.Type & 7))
//            {
//                a = 1;
//            }
//            else
//            {
//                if (left.Level > right.Level)
//                {
//                    a = -1;
//                }
//                else if (left.Level < right.Level)
//                {
//                    a = 1;
//                }
//                else
//                {
//                    if (left.Attack > right.Attack)
//                    {
//                        a = -1;
//                    }
//                    else if (left.Attack < right.Attack)
//                    {
//                        a = 1;
//                    }
//                    else
//                    {
//                        if ((left.Type >> 3) > (right.Type >> 3))
//                        {
//                            a = 1;
//                        }
//                        else if ((left.Type >> 3) < (right.Type >> 3))
//                        {
//                            a = -1;
//                        }
//                        else
//                        {
//                            if (left.Attribute > right.Attribute)
//                            {
//                                a = 1;
//                            }
//                            else if (left.Attribute < right.Attribute)
//                            {
//                                a = -1;
//                            }
//                            else
//                            {
//                                if (left.Race > right.Race)
//                                {
//                                    a = 1;
//                                }
//                                else if (left.Race < right.Race)
//                                {
//                                    a = -1;
//                                }
//                                else
//                                {
//                                    if (left.Category > right.Category)
//                                    {
//                                        a = 1;
//                                    }
//                                    else if (left.Category < right.Category)
//                                    {
//                                        a = -1;
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            return a;
//        };
//    }
//    public string get_description(UInt32 l)
//    {
//        string re = "";

//        UInt32 code = l >> 4;
//        UInt32 index = l & 0xf;
//        try
//        {
//            re += GetById(code).Str[index];
//        }
//        catch (Exception e)
//        {

//        }


//        return re;
//    }
//}
