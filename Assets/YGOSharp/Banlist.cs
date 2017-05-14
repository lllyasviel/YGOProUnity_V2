using System;
using System.Collections.Generic;

namespace YGOSharp
{
    public class Banlist
    {
        public IList<int> BannedIds { get; private set; }
        public IList<int> LimitedIds { get; private set; }
        public IList<int> SemiLimitedIds { get; private set; }
        public uint Hash { get; private set; }
        public string Name = "";

        public Banlist()
        {
            BannedIds = new List<int>();
            LimitedIds = new List<int>();
            SemiLimitedIds = new List<int>();
            Hash = 0x7dfcee6a;
        }

        public int GetQuantity(int cardId)
        {
            int al = 0;
            try
            {
                al = YGOSharp.CardsManager.Get(cardId).Alias;
            }
            catch (Exception)
            {
            }
            if (al==0)
            {
                if (BannedIds.Contains(cardId))
                    return 0;
                if (LimitedIds.Contains(cardId))
                    return 1;
                if (SemiLimitedIds.Contains(cardId))
                    return 2;
                return 3;
            }
            else
            {
                if (BannedIds.Contains(al))
                    return 0;
                if (LimitedIds.Contains(al))
                    return 1;
                if (SemiLimitedIds.Contains(al))
                    return 2;
                return 3;
            }
            
        }

        public void Add(int cardId, int quantity)
        {
            if (quantity < 0 || quantity > 2)
                return;
            switch (quantity)
            {
                case 0:
                    BannedIds.Add(cardId);
                    break;
                case 1:
                    LimitedIds.Add(cardId);
                    break;
                case 2:
                    SemiLimitedIds.Add(cardId);
                    break;
            }
            uint code = (uint)cardId;
            Hash = Hash ^ ((code << 18) | (code >> 14)) ^ ((code << (27 + quantity)) | (code >> (5 - quantity)));
        }
    }
}