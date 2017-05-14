using System.IO;
class TXT_DEBUGER_ruined
{
    public static void debug(string raw)
    {
        FileStream fs = new FileStream("log.txt", FileMode.Append);
        StreamWriter sw = new StreamWriter(fs);
        sw.Write(raw + "\n");
        sw.Flush();
        sw.Close();
        fs.Close();
    }
}

