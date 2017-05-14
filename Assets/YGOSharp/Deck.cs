using System;
using System.Collections.Generic;
using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;

namespace YGOSharp
{
    public class Deck
    {
        public IList<int> Main { get; private set; }
        public IList<int> Extra { get; private set; }
        public IList<int> Side { get; private set; }

        public D Deck_O { get; private set; }

        public IList<MonoCardInDeckManager> IMain { get; private set; }
        public IList<MonoCardInDeckManager> IExtra { get; private set; }
        public IList<MonoCardInDeckManager> ISide { get; private set; }
        public IList<MonoCardInDeckManager> IRemoved { get; private set; }

        public class D
        {
            public IList<int> Main = new List<int>();
            public IList<int> Extra = new List<int>();
            public IList<int> Side = new List<int>();
        }

        public Deck()
        {
            Main = new List<int>();
            Extra = new List<int>();
            Side = new List<int>();
            Deck_O = new D();
            IMain = new List<MonoCardInDeckManager>();
            IExtra = new List<MonoCardInDeckManager>();
            IRemoved = new List<MonoCardInDeckManager>();
            ISide = new List<MonoCardInDeckManager>();
        }

        public Deck(string path)
        {
            Main = new List<int>();
            Extra = new List<int>();
            Side = new List<int>();
            Deck_O = new D();
            IMain = new List<MonoCardInDeckManager>();
            IExtra = new List<MonoCardInDeckManager>();
            IRemoved = new List<MonoCardInDeckManager>();
            ISide = new List<MonoCardInDeckManager>();
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
                                        this.Main.Add(code);
                                        this.Deck_O.Main.Add(code);
                                    }
                                    break;
                                case 2:
                                    {
                                        this.Extra.Add(code);
                                        this.Deck_O.Extra.Add(code);
                                    }
                                    break;
                                case 3:
                                    {
                                        this.Side.Add(code);
                                        this.Deck_O.Side.Add(code);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
        }

        public int Check(Banlist ban, bool ocg, bool tcg)
        {
            if (Main.Count < 40 ||
                Main.Count > 60 ||
                Extra.Count > 15 ||
                Side.Count > 15)
                return 1;

            IDictionary<int, int> cards = new Dictionary<int, int>();

            IList<int>[] stacks = { Main, Extra, Side };
            foreach (IList<int> stack in stacks)
            {
                foreach (int id in stack)
                {
                    Card card = Card.Get(id);
                    AddToCards(cards, card);
                    if (!ocg && card.Ot == 1 || !tcg && card.Ot == 2)
                        return id;
                    if (card.HasType(CardType.Token))
                        return id;
                }
            }

            if (ban == null)
                return 0;

            foreach (var pair in cards)
            {
                int max = ban.GetQuantity(pair.Key);
                if (pair.Value > max)
                    return pair.Key;
            }

            return 0;
        }

        public int GetCardCount(int code)
        {
            int al = 0;
            try
            {
                al = YGOSharp.CardsManager.Get(code).Alias;
            }
            catch (Exception)
            {
            }
            int returnValue = 0;
            IList<MonoCardInDeckManager>[] stacks = { IMain, IExtra, ISide };
            foreach (var stack in stacks)
            {
                foreach (var item in stack)
                {
                    if (item.cardData.Id == code && item.getIfAlive())
                    {
                        returnValue++;
                        continue;
                    }
                    if (item.cardData.Alias == code && item.getIfAlive())
                    {
                        returnValue++;
                        continue;
                    }
                    if (item.cardData.Id == al && item.getIfAlive())
                    {
                        returnValue++;
                        continue;
                    }
                    if (item.cardData.Alias == al && item.getIfAlive() && al > 0)
                    {
                        returnValue++;
                        continue;
                    }
                }
            }
            return returnValue;
        }

        public bool Check(Deck deck)
        {
            if (deck.Main.Count != Main.Count || deck.Extra.Count != Extra.Count)
                return false;

            IDictionary<int, int> cards = new Dictionary<int, int>();
            IDictionary<int, int> ncards = new Dictionary<int, int>();
            IList<int>[] stacks = { Main, Extra, Side };
            foreach (IList<int> stack in stacks)
            {
                foreach (int id in stack)
                {
                    if (!cards.ContainsKey(id))
                        cards.Add(id, 1);
                    else
                        cards[id]++;
                }
            }
            stacks = new[] { deck.Main, deck.Extra, deck.Side };
            foreach (var stack in stacks)
            {
                foreach (int id in stack)
                {
                    if (!ncards.ContainsKey(id))
                        ncards.Add(id, 1);
                    else
                        ncards[id]++;
                }
            }
            foreach (var pair in cards)
            {
                if (!ncards.ContainsKey(pair.Key))
                    return false;
                if (ncards[pair.Key] != pair.Value)
                    return false;
            }
            return true;
        }

        private static void AddToCards(IDictionary<int, int> cards, Card card)
        {
            int id = card.Id;
            if (card.Alias != 0)
                id = card.Alias;
            if (cards.ContainsKey(id))
                cards[id]++;
            else
                cards.Add(id, 1);
        }

        public List<MonoCardInDeckManager> getAllObjectCardAndDeload()
        {
            List<MonoCardInDeckManager> r = new List<MonoCardInDeckManager>();
            IList<MonoCardInDeckManager>[] stacks = { IMain, IExtra, ISide,IRemoved };
            foreach (var stack in stacks)
            {
                foreach (var item in stack)
                {
                    r.Add(item);
                }
                stack.Clear();
            }
            return r;
        }

        public List<MonoCardInDeckManager> getAllObjectCard()
        {
            List<MonoCardInDeckManager> r = new List<MonoCardInDeckManager>();
            IList<MonoCardInDeckManager>[] stacks = { IMain, IExtra, ISide, IRemoved };
            foreach (var stack in stacks)
            {
                foreach (var item in stack)
                {
                    r.Add(item);
                }
            }
            return r;
        }

        public static void sort(List<MonoCardInDeckManager> cards)
        {
            cards.Sort(comparisonOfCard());
        }

        internal static Comparison<MonoCardInDeckManager> comparisonOfCard()
        {
            return (left, right) =>
            {
                return CardsManager.comparisonOfCard()(left.cardData, right.cardData);
            };
        }

        public static void rand(List<MonoCardInDeckManager> cards)
        {
            System.Random rand = new System.Random();
            for (int i = 0; i < cards.Count; i++)
            {
                int random_index = rand.Next() % cards.Count;
                var t = cards[i];
                cards[i] = cards[random_index];
                cards[random_index] = t;
            }
        }



    }
}