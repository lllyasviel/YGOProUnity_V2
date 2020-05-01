namespace YGOSharp.Network.Enums
{
    public enum StocMessage
    {
        GameMsg = 0x1,
        ErrorMsg = 0x2,
        SelectHand = 0x3,
        SelectTp = 0x4,
        HandResult = 0x5,
        TpResult = 0x6,
        ChangeSide = 0x7,
        WaitingSide = 0x8,
        DeckCount = 0x9,
        CreateGame = 0x11,
        JoinGame = 0x12,
        TypeChange = 0x13,
        LeaveGame = 0x14,
        DuelStart = 0x15,
        DuelEnd = 0x16,
        Replay = 0x17,
        TimeLimit = 0x18,
        Chat = 0x19,
        HsPlayerEnter = 0x20,
        HsPlayerChange = 0x21,
        HsWatchChange = 0x22
    }
}
