using System;
using System.IO;
using UnityEngine;
public class ui_helper
{
    public static Vector2 get_hang_lie(int index, int meihangdegeshu)
    {
        Vector2 return_value = Vector2.zero;
        return_value.x = (int)index / meihangdegeshu;
        return_value.y = index % meihangdegeshu;
        return return_value;
    }
    public static int get_zuihouyihangdegeshu(int zongshu,int meihangdegeshu)
    {
        int re = 0;
        re = zongshu % meihangdegeshu;
        if (re==0)
        {
            re = meihangdegeshu;
        }
        return re;
    }
    public static bool get_shifouzaizuihouyihang(int zongshu, int meihangdegeshu,int index)
    {
        return (int)((index) / meihangdegeshu) == (int)(zongshu / meihangdegeshu);
    }
    public static int get_zonghangshu(int zongshu, int meihangdegeshu)
    {
        return ((int)(zongshu - 1) / meihangdegeshu) + 1;
    }
    public static Vector3 get_close(Vector3 input_vector,Camera cam,float l)
    {
        Vector3 o = Vector3.zero;
        Vector3 scr = cam.WorldToScreenPoint(input_vector);
        scr.z -= l;
        o = cam.ScreenToWorldPoint(scr);
        return o;
    }

    public static Texture2D get_rand_face()
    {
        DirectoryInfo TheFolder = new DirectoryInfo("face");
        FileInfo[] fileInfo = TheFolder.GetFiles();
        FileInfo NextFile;
        Texture2D tex = new Texture2D(400, 600);
        if (fileInfo.Length > 0)
        {
            NextFile = fileInfo[UnityEngine.Random.Range(0, fileInfo.Length)];

            byte[] data;
            using (FileStream file = new FileStream(NextFile.FullName, FileMode.Open, FileAccess.Read))
            {
                file.Seek(0, SeekOrigin.Begin);
                data = new byte[file.Length];
                file.Read(data, 0, (int)file.Length);
            }
            tex.LoadImage(data);
        }
        return tex;
    }
}
