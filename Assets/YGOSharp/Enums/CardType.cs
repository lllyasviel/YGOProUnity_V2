namespace YGOSharp.OCGWrapper.Enums
{
    public enum CardType
    {
        Monster = 0x1,
        Spell = 0x2,
        Trap = 0x4,
        Normal = 0x10,
        Effect = 0x20,
        Fusion = 0x40,
        Ritual = 0x80,
        TrapMonster = 0x100,
        Spirit = 0x200,
        Union = 0x400,
        Dual = 0x800,
        Tuner = 0x1000,
        Synchro = 0x2000,
        Token = 0x4000,
        QuickPlay = 0x10000,
        Continuous = 0x20000,
        Equip = 0x40000,
        Field = 0x80000,
        Counter = 0x100000,
        Flip = 0x200000,
        Toon = 0x400000,
        Xyz = 0x800000,
        Pendulum = 0x1000000,
        SpSummon = 0x2000000,
        Link = 0x4000000
    }
}