using UnityEngine;
using System;
using System.IO;
public class BackGroundPic : Servant
{
    GameObject backGround;
    public override void initialize()
    {
        backGround = create(Program.I().mod_simple_ngui_background_texture, Vector3.zero, Vector3.zero, false, Program.ui_back_ground_2d);
        FileStream file = new FileStream("texture/common/desk.jpg", FileMode.Open, FileAccess.Read);
        file.Seek(0, SeekOrigin.Begin);
        byte[] data = new byte[file.Length];
        file.Read(data, 0, (int)file.Length);
        file.Close();
        file.Dispose();
        file = null;
        Texture2D pic = new Texture2D(1024, 600);
        pic.LoadImage(data);
        backGround.GetComponent<UITexture>().mainTexture = pic;
        backGround.GetComponent<UITexture>().depth = -100;
    }

    public override void applyShowArrangement()
    {
        UIRoot root = Program.ui_back_ground_2d.GetComponent<UIRoot>();
        float s = root.activeHeight / Screen.height;
        var tex = backGround.GetComponent<UITexture>().mainTexture;
        float ss = (float)tex.height / (float)tex.width;
        int width = (int)(Screen.width * s);
        int height = (int)(width * ss);
        if (height < Screen.height)
        {
            height = (int)(Screen.height * s);
            width = (int)(height / ss);
        }
        backGround.GetComponent<UITexture>().height = height+2;
        backGround.GetComponent<UITexture>().width = width+2;
    }

    public override void applyHideArrangement()
    {
        applyShowArrangement();
    }
}
