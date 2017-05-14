//using System;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Collections.Generic;
//using System.IO;
//using System.Threading;
//using UnityEngine;

//public enum ocgcore_picture_type
//{
//    card_picture = 0,
//    card_verticle_drawing = 1,
//    card_feature = 3,
//}

//public class PictureLoader
//{
//    private CLIENT client;

//    System.Object waitLoadListLock = new System.Object();

//    System.Object loadedListLock = new System.Object();

//    List<PictureResource> waitLoadList = new List<PictureResource>();

//    List<PictureResource> loadedList = new List<PictureResource>();

//    System.Collections.Generic.Dictionary<UInt64, bool> addedMap = new Dictionary<ulong, bool>();

//    private class BitmapHelper
//    {
//        public System.Drawing.Color[,] colors = null;
//        public BitmapHelper(string path)
//        {
//            Bitmap bitmap = (Bitmap)Image.FromFile(path);
//            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
//            IntPtr ptr = bmpData.Scan0;
//            int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
//            byte[] rgbValues = new byte[bytes];
//            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
//            colors = new System.Drawing.Color[bitmap.Width, bitmap.Height];
//            for (int counter = 0; counter < rgbValues.Length; counter += 4)
//            {
//                int i_am = counter / 4;

//                colors[i_am % bitmap.Width, i_am / bitmap.Width]
//                    =
//                    System.Drawing.Color.FromArgb(
//                    rgbValues[counter + 3],
//                    rgbValues[counter + 2],
//                    rgbValues[counter + 1],
//                    rgbValues[counter + 0]);
//            }

//            bitmap.UnlockBits(bmpData);
//            bitmap.Dispose();
//        }
//        public System.Drawing.Color GetPixel(int a, int b)
//        {
//            return colors[a, b];
//        }

//    }

//    private class PictureResource
//    {
//        public ocgcore_picture_type type;
//        public long code;
//        public byte[] data = null;
//        public float[, ,] hashed_data = null;
//        public PictureResource(ocgcore_picture_type t, long c)
//        {
//            type = t;
//            code = c;
//        }
//    }

//    public PictureLoader(CLIENT c)
//    {
//        client = c;
//        Thread main = new Thread(thread_run);
//        main.Start();
//        Debug.Log("picture_loader_begin");
//    }

//    void thread_run()
//    {
//        while (client.is_running)
//        {
//            try
//            {
//                lock (waitLoadListLock)
//                {
//                    //card_picture
//                    ProcessingCardPicture();

//                    //card_verticle_drawing
//                    ProcessingVerticleDrawing();

//                    //card_feature
//                    ProcessingCardFeature();
//                }
//            }
//            catch (Exception e)
//            {
//                Debug.Log(e);
//            }
//            Thread.Sleep(2000);
//        }
//    }

//    private void ProcessingCardFeature()
//    {
//        var removedList = new List<PictureResource>();

//        for (int i = 0; i < waitLoadList.Count; i++)
//        {
//            if (waitLoadList[i].hashed_data == null)
//            {
//                if (waitLoadList[i].type == ocgcore_picture_type.card_feature)
//                {
//                    if (File.Exists("picture_monster\\" + waitLoadList[i].code.ToString() + ".png"))
//                    {
//                        UsingAritificialPng(removedList, i);
//                    }
//                    else
//                    {
//                        UsingAutoPng(removedList, i);
//                    }
//                }

//            }
//        }

//        for (int i = 0; i < removedList.Count; i++)
//        {
//            waitLoadList.Remove(removedList[i]);
//        }
//        removedList.Clear();
//    }

//    private void UsingAutoPng(List<PictureResource> removedList, int i)
//    {
//        string path = "picture_card\\" + waitLoadList[i].code.ToString() + ".jpg";
//        if (File.Exists(path))
//        {
//            BitmapHelper bitmap = new BitmapHelper(path);
//            int width = bitmap.colors.GetLength(0);
//            int height = bitmap.colors.GetLength(1);
//            waitLoadList[i].hashed_data = new float[120, 120, 4];
//            for (int w = 0; w < 120; w++)
//            {
//                for (int h = 0; h < 120; h++)
//                {
//                    System.Drawing.Color color = bitmap.GetPixel((int)((float)(30 + w) / 177f * (float)width), (int)((float)(50 + h) / 254f * (float)height));
//                    float a = (float)color.A / 255f;
//                    if (w < 20) if (a > (float)w / (float)20) a = (float)w / (float)20;
//                    if (w > 100) if (a > 1f - (float)(w - 100) / (float)20) a = 1f - (float)(w - 100) / (float)20;
//                    if (h < 20) if (a > (float)h / (float)20) a = (float)h / (float)20;
//                    if (h > 100) if (a > 1f - (float)(h - 100) / (float)20) a = 1f - (float)(h - 100) / (float)20;
//                    waitLoadList[i].hashed_data[w, 120 - h - 1, 0] = (float)color.R / 255f;
//                    waitLoadList[i].hashed_data[w, 120 - h - 1, 1] = (float)color.G / 255f;
//                    waitLoadList[i].hashed_data[w, 120 - h - 1, 2] = (float)color.B / 255f;
//                    waitLoadList[i].hashed_data[w, 120 - h - 1, 3] = a;
//                }
//            }
//            loadedList.Add(waitLoadList[i]);
//            removedList.Add(waitLoadList[i]);
//        }
//    }

//    private void UsingAritificialPng(List<PictureResource> removedList, int i)
//    {
//        string path = "picture_monster\\" + waitLoadList[i].code.ToString() + ".png";
//        BitmapHelper bitmap = new BitmapHelper(path);
//        int left;
//        int right;
//        int up;
//        int down;
//        CutTop(bitmap, out left, out right, out up, out down);

//        up = CutLeft(bitmap, up);

//        down = CutRight(bitmap, down);

//        right = CutButton(bitmap, right);

//        int width = right - left;
//        int height = down - up;
//        waitLoadList[i].hashed_data = new float[width, height, 4];
//        for (int w = 0; w < width; w++)
//        {
//            for (int h = 0; h < height; h++)
//            {
//                System.Drawing.Color color = bitmap.GetPixel(left + w, up + h);
//                float a = (float)color.A / 255f;
//                if (w < 40) if (a > (float)w / (float)40) a = (float)w / (float)40;
//                if (w > (width - 40)) if (a > 1f - (float)(w - (width - 40)) / (float)40) a = 1f - (float)(w - (width - 40)) / (float)40;
//                if (h < 40) if (a > (float)h / (float)40) a = (float)h / (float)40;
//                if (h > (height - 40)) if (a > 1f - (float)(h - (height - 40)) / (float)40) a = 1f - (float)(h - (height - 40)) / (float)40;
//                waitLoadList[i].hashed_data[w, height - h - 1, 0] = (float)color.R / 255f;
//                waitLoadList[i].hashed_data[w, height - h - 1, 1] = (float)color.G / 255f;
//                waitLoadList[i].hashed_data[w, height - h - 1, 2] = (float)color.B / 255f;
//                waitLoadList[i].hashed_data[w, height - h - 1, 3] = a;
//            }
//        }
//        loadedList.Add(waitLoadList[i]);
//        removedList.Add(waitLoadList[i]);
//    }

//    private static int CutButton(BitmapHelper bitmap, int right)
//    {
//        for (int w = bitmap.colors.GetLength(0) - 1; w >= 0; w--)
//        {
//            for (int h = 0; h < bitmap.colors.GetLength(1); h++)
//            {
//                System.Drawing.Color color = bitmap.GetPixel(w, h);
//                if (color.A > 10)
//                {
//                    right = w;
//                    return right;
//                }
//            }
//        }
//        return right;
//    }

//    private static int CutRight(BitmapHelper bitmap, int down)
//    {
//        for (int h = bitmap.colors.GetLength(1) - 1; h >= 0; h--)
//        {
//            for (int w = 0; w < bitmap.colors.GetLength(0); w++)
//            {
//                System.Drawing.Color color = bitmap.GetPixel(w, h);
//                if (color.A > 10)
//                {
//                    down = h;
//                    return down;
//                }
//            }
//        }
//        return down;
//    }

//    private static int CutLeft(BitmapHelper bitmap, int up)
//    {
//        for (int h = 0; h < bitmap.colors.GetLength(1); h++)
//        {
//            for (int w = 0; w < bitmap.colors.GetLength(0); w++)
//            {
//                System.Drawing.Color color = bitmap.GetPixel(w, h);
//                if (color.A > 10)
//                {
//                    up = h;
//                    return up;
//                }
//            }
//        }
//        return up;
//    }

//    private static void CutTop(BitmapHelper bitmap, out int left, out int right, out int up, out int down)
//    {
//        ///////切边算法
//        left = 0;
//        right = bitmap.colors.GetLength(0);
//        up = 0;
//        down = bitmap.colors.GetLength(1);
//        for (int w = 0; w < bitmap.colors.GetLength(0); w++)
//        {
//            for (int h = 0; h < bitmap.colors.GetLength(1); h++)
//            {
//                System.Drawing.Color color = bitmap.GetPixel(w, h);
//                if (color.A > 10)
//                {
//                    left = w;
//                    return;
//                }
//            }
//        }
//    }

//    private void ProcessingVerticleDrawing()
//    {
//        var removedList = new List<PictureResource>();

//        for (int i = 0; i < waitLoadList.Count; i++)
//        {
//            if (waitLoadList[i].type == ocgcore_picture_type.card_verticle_drawing)
//            {
//                string path = "picture_card\\" + waitLoadList[i].code.ToString() + ".jpg";
//                if (!File.Exists(path))
//                {
//                    continue;
//                }
//                BitmapHelper bitmap = new BitmapHelper(path);
//                int width = bitmap.colors.GetLength(0);
//                int height = bitmap.colors.GetLength(1);
//                waitLoadList[i].hashed_data = new float[120, 120, 4];
//                for (int w = 0; w < 120; w++)
//                {
//                    for (int h = 0; h < 120; h++)
//                    {
//                        System.Drawing.Color color = bitmap.GetPixel((int)((float)(30 + w) / 177f * (float)width), (int)((float)(50 + h) / 254f * (float)height));
//                        float a = (float)color.A / 255f;
//                        if (h < 60)
//                        {
//                            if (w < 20) if (a > (float)w / (float)20) a = (float)w / (float)20;
//                            if (w > 100) if (a > 1f - (float)(w - 100) / (float)20) a = 1f - (float)(w - 100) / (float)20;
//                            if (h < 20) if (a > (float)h / (float)20) a = (float)h / (float)20;
//                            if (h > 100) if (a > 1f - (float)(h - 100) / (float)20) a = 1f - (float)(h - 100) / (float)20;
//                        }
//                        else
//                        {
//                            float l = (float)Math.Sqrt((60 - w) * (60 - w) + (60 - h) * (60 - h));
//                            float aaa = 1.0f - (l / 60);
//                            if (aaa < 0) { a = 0; }
//                            else
//                            {
//                                if (aaa > 0.4) aaa = 1;
//                                else aaa = (aaa) / 0.4f;
//                                a = aaa;
//                            }
//                        }
//                        waitLoadList[i].hashed_data[w, 120 - h - 1, 0] = (float)color.R / 255f;
//                        waitLoadList[i].hashed_data[w, 120 - h - 1, 1] = (float)color.G / 255f;
//                        waitLoadList[i].hashed_data[w, 120 - h - 1, 2] = (float)color.B / 255f;
//                        waitLoadList[i].hashed_data[w, 120 - h - 1, 3] = a;
//                    }
//                }

//                loadedList.Add(waitLoadList[i]);
//                removedList.Add(waitLoadList[i]);
//            }
//        }

//        for (int i = 0; i < removedList.Count; i++)
//        {
//            waitLoadList.Remove(removedList[i]);
//        }
//        removedList.Clear();
//    }

//    private void ProcessingCardPicture()
//    {
//        var removedList = new List<PictureResource>();
//        foreach (var val in waitLoadList)
//        {
//            if (val.type == ocgcore_picture_type.card_picture)
//            {
//                string path = "picture_card\\" + val.code.ToString() + ".jpg";
//                if (!File.Exists(path))
//                {
//                    path = "picture_card\\0.jpg";
//                }
//                byte[] data;
//                using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
//                {
//                    file.Seek(0, SeekOrigin.Begin);
//                    data = new byte[file.Length];
//                    file.Read(data, 0, (int)file.Length);
//                }
//                val.data = data;
//                loadedList.Add(val);
//                removedList.Add(val);
//            }
//        }
//        for (int i = 0; i < removedList.Count; i++)
//        {
//            waitLoadList.Remove(removedList[i]);
//        }
//        removedList.Clear();
//    }

//    public Texture2D get(long code, ocgcore_picture_type type)
//    {
//        lock (loadedListLock)
//        {
//            foreach (PictureResource r in loadedList)
//            {
//                if (r.type == type)
//                {
//                    if (r.code == code)
//                    {
//                        Texture2D re = null;
//                        if (r.data != null)
//                        {
//                            re = new Texture2D(400, 600);
//                            re.LoadImage(r.data);
//                            return re;
//                        }
//                        if (r.hashed_data != null)
//                        {
//                            int width = r.hashed_data.GetLength(0);
//                            int height = r.hashed_data.GetLength(1);
//                            re = new Texture2D(width, height);
//                            for (int w = 0; w < width; w++)
//                            {
//                                for (int h = 0; h < height; h++)
//                                {
//                                    re.SetPixel(w, h, new UnityEngine.Color(r.hashed_data[w, h, 0], r.hashed_data[w, h, 1], r.hashed_data[w, h, 2], r.hashed_data[w, h, 3]));
//                                }
//                            }
//                            re.Apply();
                          
//                            return re;
//                        }
//                    }
//                }
//            }
//        }

//        lock (waitLoadListLock)
//        {
//            if (!addedMap.ContainsKey((UInt64)type << 32 | (UInt64)code))
//            {
//                waitLoadList.Add(new PictureResource(type, code));
//                addedMap.Add((UInt64)type << 32 | (UInt64)code, true);
//            }
//        }

//        return null;
//    }
//}
