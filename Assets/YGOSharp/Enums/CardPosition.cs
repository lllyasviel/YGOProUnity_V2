namespace YGOSharp.OCGWrapper.Enums
{
    public enum CardPosition
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
}


class CardFac
{
   public static int zuoxia = 1 << (1 - 1);
    public static int xia = 1 << (2 - 1);
    public static int youxia = 1 << (3 - 1);
    public static int zuo = 1 << (4 - 1);
    public static int you = 1 << (6 - 1);
    public static int zuoshang = 1 << (7 - 1);
    public static int shang = 1 << (8 - 1);
    public static int youshang = 1 << (9 - 1);
}