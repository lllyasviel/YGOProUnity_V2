namespace YGOSharp.OCGWrapper.Enums
{
    public enum DuelPhase
    {
        Draw = 0x01,
        Standby = 0x02,
        Main1 = 0x04,
        BattleStart = 0x08,
        BattleStep = 0x10,
        Damage = 0x20,
        DamageCal = 0x40,
        Battle = 0x80,
        Main2 = 0x100,
        End = 0x200
    }
}
