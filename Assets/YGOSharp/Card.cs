using YGOSharp.OCGWrapper.Enums;
using System.Data;
using System;

namespace YGOSharp
{
    public class Card
    {
        public int Id;
        public int Ot;
        public int Alias;
        public long Setcode;
        public int Type;

        public int Level;
        public int LScale;
        public int RScale;
        public int LinkMarker;

        public int Attribute;
        public int Race;
        public int Attack;
        public int Defense;
        public int rAttack;
        public int rDefense;

        public Int64 Category;
        public string Name;
        public string Desc;
        public string[] Str;

        public string packShortNam = "";
        public string packFullName = "";
        public string reality = "";
        public string strSetName = "";  
        public int year = 0;
        public int month = 0;
        public int day = 0;

        public Card clone()
        {
            Card r = new Card();
            r.Id = Id;
            r.Ot = Ot;
            r.Alias = Alias;
            r.Setcode = Setcode;
            r.Type = Type;
            r.Level = Level;
            r.LScale = LScale;
            r.RScale = RScale;
            r.LinkMarker = LinkMarker;
            r.Attribute = Attribute;
            r.Race = Race;
            r.Attack = Attack;
            r.Defense = Defense;
            r.rAttack = rAttack;
            r.rDefense = rDefense;
            r.Category = Category;
            r.Name = Name;
            r.Desc = Desc;
            r.Str = new string[Str.Length];
            for (int ii = 0; ii < Str.Length; ii++)
            {
                r.Str[ii] = Str[ii];
            }
            return r;
        }

        public void  cloneTo(Card r)
        {
            r.Id = Id;
            r.Ot = Ot;
            r.Alias = Alias;
            r.Setcode = Setcode;
            r.Type = Type;
            r.Level = Level;
            r.LScale = LScale;
            r.RScale = RScale;
            r.Attribute = Attribute;
            r.Race = Race;
            r.Attack = Attack;
            r.Defense = Defense;
            r.rAttack = rAttack;
            r.rDefense = rDefense;
            r.Category = Category;
            r.Name = Name;
            r.Desc = Desc;
            r.Str = new string[Str.Length];
            for (int ii = 0; ii < Str.Length; ii++)
            {
                r.Str[ii] = Str[ii];
            }
        }

        public static Card Get(int id)
        {
            return CardsManager.GetCard(id);
        }

        public bool HasType(CardType type)
        {
            return ((Type & (int)type) != 0);
        }

        public bool HasLinkMarker(CardLinkMarker dir)
        {
            return (LinkMarker & (int)dir) != 0;
        }

        public bool IsExtraCard()
        {
            return (HasType(CardType.Fusion) || HasType(CardType.Synchro) || HasType(CardType.Xyz) || HasType(CardType.Link));
        }

        internal Card(IDataRecord reader)
        {
            this.Str = new string[16];
            this.Id = (int)reader.GetInt64(0);
            this.Ot = reader.GetInt32(1);
            this.Alias = (int)reader.GetInt64(2);
            this.Setcode = reader.GetInt64(3);
            this.Type = (int)reader.GetInt64(4);
            this.Attack = reader.GetInt32(5);
            this.Defense = reader.GetInt32(6);
            this.rAttack = this.Attack;
            this.rDefense = this.Defense;
            long Level_raw = reader.GetInt64(7);
            this.Level = (int)Level_raw & 0xff;
            this.LScale = (int)((Level_raw >> 0x18) & 0xff);
            this.RScale = (int)((Level_raw >> 0x10) & 0xff);
            this.LinkMarker = this.Defense;
            this.Race = reader.GetInt32(8);
            this.Attribute = reader.GetInt32(9);
            this.Category = reader.GetInt64(10);
            this.Name = reader.GetString(12);
            this.Desc = reader.GetString(13);
            for (int ii = 0; ii < 0x10; ii++)
            {
                this.Str[ii] = reader.GetString(14 + ii);
            }
            this.strSetName = GameStringHelper.getSetName(Setcode);
        }

        public Card()
        {
            this.Id = 0;
            this.Str = new string[16];
            this.Name = YGOSharp.CardsManager.nullName;
            this.Desc = YGOSharp.CardsManager.nullString;
        }

        
    }

    //public class cardInPack
    //{
    //    public int Id = 0;
    //    public string packShortNam = "";
    //    public string packFullName = "";
    //    public string reality = "";
    //    public int year = 0;
    //    public int month = 0;
    //    public int day = 0;
    //    internal cardInPack()
    //    {

    //    }
    //    internal cardInPack(IDataRecord reader)
    //    {
    //        try
    //        {
    //            Id = (int)reader.GetInt64(0);
    //            packShortNam = reader.GetString(1);
    //            packFullName = reader.GetString(2);
    //            reality = reader.GetString(3);
    //            string temp = reader.GetString(4);
    //            string[] mats = temp.Split("/");
    //            if (mats.Length == 3)
    //            {
    //                month = int.Parse(mats[0]);
    //                day = int.Parse(mats[1]);
    //                year = int.Parse(mats[2]);
    //            }
    //        }
    //        catch (Exception)
    //        {

    //        }
    //    }
    //}
}