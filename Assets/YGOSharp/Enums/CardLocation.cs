namespace YGOSharp.OCGWrapper.Enums
{
    public enum CardLocation
    {
        Deck = 0x01,
        Hand = 0x02,
        MonsterZone = 0x04,
        SpellZone = 0x08,
        Grave = 0x10,
        Removed = 0x20,
        Extra = 0x40,
        Overlay = 0x80,
        Onfield = 0x0C,
		
		Unknown = 0,
		Search = 0x800
    }
}