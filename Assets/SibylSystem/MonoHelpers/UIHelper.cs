using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
public static class UIHelper
{
    [DllImport("user32.dll")]
    static extern bool FlashWindow(IntPtr handle, bool invert);

    public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr GetParent(IntPtr hWnd);
    [DllImport("user32.dll")]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

    [DllImport("kernel32.dll")]
    static extern void SetLastError(uint dwErrCode);

    static IntPtr GetProcessWnd()
    {
        IntPtr ptrWnd = IntPtr.Zero;
        uint pid = (uint)System.Diagnostics.Process.GetCurrentProcess().Id;  // 当前进程 ID

        bool bResult = EnumWindows(new WNDENUMPROC(delegate (IntPtr hwnd, uint lParam)
        {
            uint id = 0;

            if (GetParent(hwnd) == IntPtr.Zero)
            {
                GetWindowThreadProcessId(hwnd, ref id);
                if (id == lParam)    // 找到进程对应的主窗口句柄
                {
                    ptrWnd = hwnd;   // 把句柄缓存起来
                    SetLastError(0);    // 设置无错误
                    return false;   // 返回 false 以终止枚举窗口
                }
            }

            return true;

        }), pid);

        return (!bResult && Marshal.GetLastWin32Error() == 0) ? ptrWnd : IntPtr.Zero;
    }

    public static void Flash()
    {
        FlashWindow(GetProcessWnd(),true);
    }

    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent,
    }

    public static void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
    {
        switch (renderingMode)
        {
            case RenderingMode.Opaque:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case RenderingMode.Cutout:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case RenderingMode.Fade:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case RenderingMode.Transparent:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }

    internal static void registEvent(UIButton btn, Action function) 
    {
        if (btn != null)
        {
            MonoDelegate d = btn.gameObject.GetComponent<MonoDelegate>();
            if (d == null)
            {
                d = btn.gameObject.AddComponent<MonoDelegate>();
            }
            d.actionInMono = function;
            btn.onClick.Clear();
            btn.onClick.Add(new EventDelegate(d, "function"));
            return;
        }
    }

    internal static Texture2D[] sliceField(Texture2D textureField_) 
    {
        Texture2D textureField = ScaleTexture(textureField_,1024,819);
        Texture2D[] returnValue = new Texture2D[3];
        returnValue[0] = new Texture2D(textureField.width, textureField.height);
        returnValue[1] = new Texture2D(textureField.width, textureField.height);
        returnValue[2] = new Texture2D(textureField.width, textureField.height);
        float zuo = (float)textureField.width * 69f / 320f;
        float you = (float)textureField.width * 247f / 320f;
        for (int w = 0; w < textureField.width; w++)
        {
            for (int h = 0; h < textureField.height; h++)
            {
                Color c = textureField.GetPixel(w, h);
                if (c.a < 0.05f)
                {
                    c.a = 0;
                }
                if (w < zuo)
                {
                    returnValue[0].SetPixel(w, h, c);
                    returnValue[1].SetPixel(w, h, new Color(0, 0, 0, 0));
                    returnValue[2].SetPixel(w, h, new Color(0, 0, 0, 0));
                }
                else if (w > you)
                {
                    returnValue[2].SetPixel(w, h, c);
                    returnValue[0].SetPixel(w, h, new Color(0, 0, 0, 0));
                    returnValue[1].SetPixel(w, h, new Color(0, 0, 0, 0));
                }
                else
                {
                    returnValue[1].SetPixel(w, h, c);
                    returnValue[0].SetPixel(w, h, new Color(0, 0, 0, 0));
                    returnValue[2].SetPixel(w, h, new Color(0, 0, 0, 0));
                }
            }
        }
        returnValue[0].Apply();
        returnValue[1].Apply();
        returnValue[2].Apply();
        return returnValue;
    }

    static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);

        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);

        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }

        result.Apply();
        return result;
    }


    internal static IntPtr getPtrString(string path)
    {
        IntPtr ptrFileName = Marshal.AllocHGlobal(path.Length + 1);
        byte[] s = System.Text.Encoding.UTF8.GetBytes(path);
        Marshal.Copy(s, 0, ptrFileName, s.Length);
        return ptrFileName;
    }

    public static T getByName<T>(GameObject father,string name) where T:Component
    {
        T return_value = null;
        var all = father.transform.GetComponentsInChildren<T>();
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].name == name)
            {
                return_value = all[i];
            }
        }
        return return_value;
    }

    public static void InterGameObject(GameObject father)
    {
        var all = father.transform.GetComponentsInChildren<UILabel>();  
        for (int i = 0; i < all.Length; i++)
        {
            if ((all[i].name.Length > 1 && all[i].name[0] == '!') || all[i].name == "yes_" || all[i].name == "no_")
            {
                all[i].text = InterString.Get(all[i].text);
            }
        }
    } 

    public static GameObject getByName(GameObject father, string name)
    {
        GameObject return_value = null;
        var all = father.transform.GetComponentsInChildren<Transform>();
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].name == name)
            {
                return_value = all[i].gameObject;
            }
        }
        return return_value;
    }

    public static T getByName<T>(GameObject father) where T : Component
    {
        T return_value = father.transform.GetComponentInChildren<T>();
        return return_value;
    }

    public static UILabel getLabelName(GameObject father, string name)
    {
        UILabel return_value = null;
        var all = father.transform.GetComponentsInChildren<UILabel>();
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].name == name 
                ||
                (all[i].transform.parent != null && all[i].transform.parent.name == name)
                ||
                (all[i].transform.parent.parent != null && all[i].transform.parent.parent.name == name)
                 ||
                (all[i].transform.parent.parent.parent != null && all[i].transform.parent.parent.parent.name == name)
                )
            {
                return_value = all[i];
            }
        }
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].name == name)
            {
                return_value = all[i];
            }
        }
        return return_value;
    }

    internal static int[] get_decklieshuArray(int count)
    {
        int[] ret = new int[4];
        ret[0] = 10;
        ret[1] = 10;
        ret[2] = 10;
        ret[3] = 10;
        for (int i = 41; i <= count; i++)
        {
            int index = i % 4;
            index--;
            if (index == -1)
            {
                index = 3;
            }
            ret[index]++;
        }
        return ret;
    }



    public static void trySetLableText(GameObject father, string name, string text)
    {
        var l = getLabelName(father, name);
        if (l != null)
        {
            l.text = text;
        }
        else
        {
            Program.DEBUGLOG("NO Lable"+ name);
        }
    }

    public static string tryGetLableText(GameObject father, string name)
    {
        var l = getLabelName(father, name);
        if (l != null)
        {
            return l.text;
        }

        return "";
    }

    public static string[] Split(this string str,string s)
    {
        return str.Split(new string[] { s }, StringSplitOptions.RemoveEmptyEntries);
    }

    public static void registEvent(GameObject father, string name, Action<GameObject, Servant.messageSystemValue> function, Servant.messageSystemValue value,string name2="")
    {
        UIInput input = getByName<UIInput>(father, name);
        if (input != null)
        {
            MonoListenerRMS_ized d = input.gameObject.GetComponent<MonoListenerRMS_ized>();
            if (d == null)
            {
                d = input.gameObject.AddComponent<MonoListenerRMS_ized>();
            }
            d.actionInMono = function;
            d.value = value;
            input.onSubmit.Clear();
            input.onSubmit.Add(new EventDelegate(d, "function"));
            UIButton btns = getByName<UIButton>(father, name2);
            if (btns != null)
            {
                btns.onClick.Clear();
                btns.onClick.Add(new EventDelegate(d, "function"));
            }
            return;
        }
        UIButton btn = getByName<UIButton>(father, name);
        if (btn != null)
        {
            MonoListenerRMS_ized d = btn.gameObject.GetComponent<MonoListenerRMS_ized>();
            if (d == null)
            {
                d = btn.gameObject.AddComponent<MonoListenerRMS_ized>();
            }
            d.actionInMono = function;
            d.value = value;
            btn.onClick.Clear();
            btn.onClick.Add(new EventDelegate(d, "function"));
            return;
        }
    }



    public static void registEventbtn(GameObject father, string name, Action function)
    {
        UIButton btn = getByName<UIButton>(father, name);
        if (btn != null)
        {
            MonoDelegate d = btn.gameObject.GetComponent<MonoDelegate>();
            if (d == null)
            {
                d = btn.gameObject.AddComponent<MonoDelegate>();
            }
            d.actionInMono = function;
            btn.onClick.Clear();
            btn.onClick.Add(new EventDelegate(d, "function"));
            return;
        }
    }

    public static void registEvent(GameObject father, string name, Action function)
    {
        UISlider slider = getByName<UISlider>(father, name);    
        if (slider != null)
        {
            MonoDelegate d = slider.gameObject.GetComponent<MonoDelegate>();
            if (d == null)
            {
                d = slider.gameObject.AddComponent<MonoDelegate>();
            }
            d.actionInMono = function;
            slider.onChange.Add(new EventDelegate(d, "function"));
            return;
        }
        UIPopupList list = getByName<UIPopupList>(father, name);
        if (list != null)
        {
            MonoDelegate d = list.gameObject.GetComponent<MonoDelegate>();
            if (d == null)
            {
                d = list.gameObject.AddComponent<MonoDelegate>();
            }
            d.actionInMono = function;
            list.onChange.Add(new EventDelegate(d, "function"));
            return;
        }
        UIToggle tog = getByName<UIToggle>(father, name);
        if (tog != null)
        {
            MonoDelegate d = tog.gameObject.GetComponent<MonoDelegate>();
            if (d == null)
            {
                d = tog.gameObject.AddComponent<MonoDelegate>();
            }
            d.actionInMono = function;
            tog.onChange.Clear();
            tog.onChange.Add(new EventDelegate(d, "function"));
            return;
        }
        UIInput input = getByName<UIInput>(father, name);
        if (input != null)
        {
            MonoDelegate d = input.gameObject.GetComponent<MonoDelegate>();
            if (d == null)
            {
                d = input.gameObject.AddComponent<MonoDelegate>();
            }
            d.actionInMono = function;
            input.onSubmit.Clear();
            input.onSubmit.Add(new EventDelegate(d, "function"));
            return;
        }
        UIScrollBar bar = getByName<UIScrollBar>(father, name);
        if (bar != null)
        {
            MonoDelegate d = bar.gameObject.GetComponent<MonoDelegate>();
            if (d == null)
            {
                d = bar.gameObject.AddComponent<MonoDelegate>();
            }
            d.actionInMono = function;
            bar.onChange.Clear();
            bar.onChange.Add(new EventDelegate(d, "function"));
            return;
        }
        UIButton btn = getByName<UIButton>(father, name);
        if (btn != null)
        {
            MonoDelegate d = btn.gameObject.GetComponent<MonoDelegate>();
            if (d == null)
            {
                d = btn.gameObject.AddComponent<MonoDelegate>();
            }
            d.actionInMono = function;
            btn.onClick.Clear();
            btn.onClick.Add(new EventDelegate(d, "function"));
            return;
        }
    }

    public static void addButtonEvent_toolShift(GameObject father, string name, Action function)
    {
        UIButton btn = getByName<UIButton>(father, name);
        if (btn != null)
        {
            MonoDelegate d = btn.gameObject.GetComponent<MonoDelegate>();
            if (d == null)
            {
                d = btn.gameObject.AddComponent<MonoDelegate>();
            }
            d.actionInMono = function;
            btn.onClick.Clear();
            btn.onClick.Add(new EventDelegate(btn.gameObject.GetComponent<toolShift>(), "shift"));
            btn.onClick.Add(new EventDelegate(d, "function"));
        }
    }

    public static void registClickListener(GameObject father, string name, Action<GameObject> ES_listenerForGameObject)
    {
        UIButton btn = getByName<UIButton>(father, name);
        if (btn != null)
        {
            MonoListener d = btn.gameObject.GetComponent<MonoListener>();
            if (d == null)
            {
                d = btn.gameObject.AddComponent<MonoListener>();
            }
            d.actionInMono = ES_listenerForGameObject;
            btn.onClick.Clear();
            btn.onClick.Add(new EventDelegate(d, "function"));
        }
    }

    public static Vector2 get_hang_lie(int index, int meihangdegeshu)
    {
        Vector2 return_value = Vector2.zero;
        return_value.x = (int)index / meihangdegeshu;
        return_value.y = index % meihangdegeshu;
        return return_value;
    }

    internal static Vector2 get_hang_lieArry(int v, int[] hangshu)
    {
        Vector2 return_value = Vector2.zero;
        for (int i = 0; i < 4; i++) 
        {
            if (v < hangshu[i])
            {
                return_value.x = i;
                return_value.y = v;
                return return_value;
            }
            else
            {
                v -= hangshu[i];
            }
        }
        return return_value;
    }

    public static int get_zuihouyihangdegeshu(int zongshu, int meihangdegeshu)
    {
        int re = 0;
        re = zongshu % meihangdegeshu;
        if (re == 0)
        {
            re = meihangdegeshu;
        }
        return re;
    }

    public static bool get_shifouzaizuihouyihang(int zongshu, int meihangdegeshu, int index)
    {
        return (int)((index) / meihangdegeshu) == (int)(zongshu / meihangdegeshu);
    }

    public static int get_zonghangshu(int zongshu, int meihangdegeshu)
    {
        return ((int)(zongshu - 1) / meihangdegeshu) + 1;
    }

    public static void registEvent(UIScrollView uIScrollView, Action function)
    {
        uIScrollView.onScrolled = new UIScrollView.OnDragNotification(function);
    }

    public static void registEvent(UIScrollBar scrollBar, Action function)
    {
        MonoDelegate d = scrollBar.gameObject.GetComponent<MonoDelegate>();
        if (d == null)
        {
            d = scrollBar.gameObject.AddComponent<MonoDelegate>();
        }
        d.actionInMono = function;
        scrollBar.onChange.Clear();
        scrollBar.onChange.Add(new EventDelegate(d, "function"));
    }

    public static void registUIEventTriggerForClick(GameObject gameObject, Action<GameObject> listenerForClicked)
    {
        BoxCollider boxCollider = gameObject.transform.GetComponentInChildren<BoxCollider>();
        boxCollider.gameObject.name = gameObject.name;
        if (boxCollider != null)
        {
            UIEventTrigger uIEventTrigger = boxCollider.gameObject.AddComponent<UIEventTrigger>();
            MonoListener d = boxCollider.gameObject.AddComponent<MonoListener>();
            d.actionInMono = listenerForClicked;
            uIEventTrigger.onClick.Add(new EventDelegate(d, "function"));
        }
    }

    public static void registUIEventTriggerForHoverOver(GameObject gameObject, Action<GameObject> listenerForHoverOver)
    {
        BoxCollider boxCollider = gameObject.transform.GetComponentInChildren<BoxCollider>();
        if (boxCollider != null)
        {
            UIEventTrigger uIEventTrigger = boxCollider.gameObject.AddComponent<UIEventTrigger>();
            MonoListener d = boxCollider.gameObject.AddComponent<MonoListener>();
            d.actionInMono = listenerForHoverOver;
            uIEventTrigger.onHoverOver.Add(new EventDelegate(d, "function"));
        }
    }

    internal static GameObject getRealEventGameObject(GameObject gameObject)
    {
        GameObject re = null;
        BoxCollider boxCollider = gameObject.transform.GetComponentInChildren<BoxCollider>();
        if (boxCollider != null)
        {
            re = boxCollider.gameObject;
        }
        return re;
    }

    internal static GameObject getGameObject(GameObject gameObject, string name)
    {
        Transform t = getByName<Transform>(gameObject, name);
        if (t != null)
        {
            return t.gameObject;
        }
        else
        {
            return null;
        }
    }

    internal static void trySetLableText(GameObject gameObject, string p)
    {
        try
        {
            gameObject.GetComponentInChildren<UILabel>().text = p;
        }
        catch (Exception)   
        {
        }
    }

    internal static void registEvent(GameObject gameObject, Action act)
    {
        registEvent(gameObject, gameObject.name, act);
    }

    internal static void trySetLableTextList(GameObject father,string text)
    {
        try
        {
            var p = father.GetComponentInChildren<UITextList>();
            p.Clear();
            p.Add(text);
        }
        catch (Exception)
        {
            Program.DEBUGLOG("NO LableList");
        }
    }

    internal static int get_decklieshu(int zongshu)
    {
        int return_value = 10;
        for (int i = 0; i < 100; i++)
        {
            if ((zongshu + i) % 4 == 0)
            {
                return_value = (zongshu + i) / 4;
                break;
            }
        }
        return return_value;
    }

    internal static void clearITWeen(GameObject gameObject)
    {
        iTween[] iTweens = gameObject.GetComponents<iTween>();
        for (int i=0;i< iTweens.Length;i++)
        {
            MonoBehaviour.DestroyImmediate(iTweens[i]);
        }
    }

    internal static float get_left_right_index(float left, float right, int i, int count)
    {
        float return_value = 0;
        if (count == 1)
        {
            return_value = left + right;
            return_value /= 2;
        }
        else
        {
            return_value = left + (right - left) * (float)i / ((float)(count - 1));
        }
        return return_value; 
    }

    internal static float get_left_right_indexZuo(float v1, float v2, int v3, int count, int v4)
    {
        if (count >= v4)
        {
            return get_left_right_index(v1, v2, v3, count);
        }
        else
        {
            return get_left_right_index(v1, v2, v3, v4);
        }
    }

    internal static float get_left_right_indexEnhanced(float left, float right, int i, int count, int illusion)
    {
        float return_value = 0;
        if (count > illusion)
        {
            if (count == 1)
            {
                return_value = left + right;
                return_value /= 2;
            }
            else
            {
                return_value = left + (right - left) * (float)i / ((float)(count - 1));
            }
        }
        else
        {
            if (illusion == 1)
            {
                return_value = left + right;
                return_value /= 2;
            }
            else
            {
                float l = left;
                float r = right;
                float per = ((right - left) / (illusion - 1));
                float length = per * (count + 1);
                l = (left + right) / 2f - length / 2f;
                r = (left + right) / 2f + length / 2f;
                return_value = l + per * (float)(i + 1);
            }
        }
        return return_value;
    }

    internal static void registUIEventTriggerForMouseDown(GameObject gameObject, Action<GameObject> listenerForMouseDown)
    {
        BoxCollider boxCollider = gameObject.transform.GetComponentInChildren<BoxCollider>();
        if (boxCollider != null)
        {
            UIEventTrigger uIEventTrigger = boxCollider.gameObject.AddComponent<UIEventTrigger>();
            MonoListener d = boxCollider.gameObject.AddComponent<MonoListener>();
            d.actionInMono = listenerForMouseDown;
            uIEventTrigger.onPress.Add(new EventDelegate(d, "function"));
        }
    }

    public static Dictionary<string, Texture2D> faces = new Dictionary<string, Texture2D>();

    internal static void iniFaces()
    {
        try
        {
            FileInfo[] fileInfos = (new DirectoryInfo("texture/face")).GetFiles();
            for (int i = 0; i < fileInfos.Length; i++)
            {
                if (fileInfos[i].Name.Length > 4)
                {
                    if (fileInfos[i].Name.Substring(fileInfos[i].Name.Length - 4, 4) == ".png")
                    {
                        string name = fileInfos[i].Name.Substring(0, fileInfos[i].Name.Length - 4);
                        if (!faces.ContainsKey(name))
                        {
                            try
                            {
                                faces.Add(name, UIHelper.getTexture2D("texture/face/" + fileInfos[i].Name));
                            }
                            catch (Exception e)
                            {
                                Debug.Log(e);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    internal static Texture2D getFace(string name)
    {
        Texture2D re = null;
        if (faces.TryGetValue(name, out re))
        {
            if (re != null)
            {
                return re;
            }
        }
        byte[] buffer= System.Text.Encoding.UTF8.GetBytes(name);
        int sum = 0;
        for (int i=0;i< buffer.Length;i++)
        {
            sum += buffer[i];
        }
        sum = sum % 100;
        return Program.I().face.faces[sum];
    }

    public static Texture2D getTexture2D(string path) 
    {
        Texture2D pic = null;
        try
        {
            if (!File.Exists(path))
            {
                return null;
            }
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            file.Seek(0, SeekOrigin.Begin);
            byte[] data = new byte[file.Length];
            file.Read(data, 0, (int)file.Length);
            file.Close();
            file.Dispose();
            file = null;
            pic = new Texture2D(1024, 600);
            pic.LoadImage(data);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return pic;
    }


    internal static void shiftButton(UIButton btn,bool enabled)
    {
        if (enabled)
        {
            btn.gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            btn.gameObject.transform.localScale = new Vector3(0, 0, 0);
        }
        //try
        //{
        //    BoxCollider boxCollider = btn.gameObject.GetComponentInChildren<BoxCollider>();
        //    UILabel label = btn.gameObject.GetComponentInChildren<UILabel>();
        //    label.text = hint;
        //    boxCollider.enabled = enabled;
        //    if (enabled)
        //    {
        //        label.color = Color.white;
        //    }
        //    else
        //    {
        //        label.color = Color.gray;
        //    }
        //}
        //catch (Exception)   
        //{
        //}
    }

    internal static void shiftUIToggle(UIToggle tog, bool canClick,bool canChange, string hint)      
    {
        try
        {
            tog.canChange = canChange;
            BoxCollider boxCollider = tog.gameObject.GetComponentInChildren<BoxCollider>();
            UILabel label = tog.gameObject.GetComponentInChildren<UILabel>();
            label.text = hint;
            boxCollider.enabled = canClick;
            if (canClick)
            {
                getByName<UISprite>(tog.gameObject, "Background").color= Color.white;
                //getByName<UISprite>(tog.gameObject, "Checkmark").color = Color.white;
            }
            else
            {
                getByName<UISprite>(tog.gameObject, "Background").color = Color.black;
                //getByName<UISprite>(tog.gameObject, "Checkmark").color = Color.gray;
            }
        }
        catch (Exception)
        {
        }
    }

    internal static string getBufferString(byte[] buffer)
    {
        string returnValue = "";
        foreach (var item in buffer)    
        {
            returnValue += ((int)item).ToString() + ".";
        }
        return returnValue;
    }

    internal static string getTimeString()
    {
        return (DateTime.Now.ToString("MM-dd「HH：mm：ss」"));
    }
    internal static bool fromStringToBool(string s)
    {
        return s == "1";
    }

    internal static string  fromBoolToString(bool s)
    {
        if (s)
        {
            return "1";
        }
        else
        {
            return "0";
        }
    }

    internal static Vector3 getCamGoodPosition(Vector3 v, float l)
    {
        Vector3 screenposition = Program.camera_game_main.WorldToScreenPoint(v);
        return Program.camera_game_main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z + l));
    }


    public static int CompareTime(object x, object y)   
    {
        if (x == null && y == null)
        {
            return 0;
        }
        if (x == null)
        {
            return -1;
        }
        if (y == null)
        {
            return 1;
        }
        FileInfo xInfo = (FileInfo)x;
        FileInfo yInfo = (FileInfo)y;
        return yInfo.LastWriteTime.CompareTo(xInfo.LastWriteTime);
    }

    public static int CompareName(object x, object y)   
    {
        if (x == null && y == null)
        {
            return 0;
        }
        if (x == null)
        {
            return -1;
        }
        if (y == null)
        {
            return 1;
        }
        FileInfo xInfo = (FileInfo)x;
        FileInfo yInfo = (FileInfo)y;
        return xInfo.FullName.CompareTo(yInfo.FullName);
    }

    internal static void playSound(string p, float val) 
    {
        if (Ocgcore.inSkiping) 
        {
            return;
        }
        string path = "sound/" + p + ".mp3";
        if (File.Exists(path) == false)
        {
            path = "sound/" + p + ".wav";
        }
        if (File.Exists(path) == false)
        {
            path = "sound/" + p + ".ogg";
        }
        if (File.Exists(path) == false)
        {
            return;
        }
        path = Environment.CurrentDirectory.Replace("\\", "/") + "/" + path;
        path = new Uri(new Uri("file:///"), path).AbsolutePath;
        GameObject audio_helper = Program.I().ocgcore.create_s(Program.I().mod_audio_effect);
        audio_helper.GetComponent<audio_helper>().play(path, Program.I().setting.soundValue());
        Program.I().destroy(audio_helper,5f);
    }

    internal static string getGPSstringLocation(GPS p1)
    {
        string res = "";
        if (p1.controller == 0)
        {
            res += "";
        }
        else
        {
            res += InterString.Get("对方");
        }
        if ((p1.location & (UInt32)game_location.LOCATION_DECK) > 0)
        {
            res += InterString.Get("卡组");
        }
        if ((p1.location & (UInt32)game_location.LOCATION_EXTRA) > 0)
        {
            res += InterString.Get("额外");
        }
        if ((p1.location & (UInt32)game_location.LOCATION_GRAVE) > 0)
        {
            res += InterString.Get("墓地");
        }
        if ((p1.location & (UInt32)game_location.LOCATION_HAND) > 0)
        {
            res += InterString.Get("手牌");
        }
        if ((p1.location & (UInt32)game_location.LOCATION_MZONE) > 0)
        {
            res += InterString.Get("前场");
        }
        if ((p1.location & (UInt32)game_location.LOCATION_REMOVED) > 0)
        {
            res += InterString.Get("除外");
        }
        if ((p1.location & (UInt32)game_location.LOCATION_SZONE) > 0)
        {
            res += InterString.Get("后场");
        }
        return res;
    }

    //internal static string getGPSstringPosition(GPS p1) 
    //{
    //    string res = "";
    //    if ((p1.location & (UInt32)game_location.LOCATION_OVERLAY) > 0)
    //    {
    //        res += InterString.Get("(被叠放)");
    //    }
    //    else
    //    {
    //        if ((p1.position & (UInt32)game_position.POS_FACEUP_ATTACK) > 0)
    //        {
    //            res += InterString.Get("(表侧攻击)");
    //        }
    //        else if ((p1.position & (UInt32)game_position.POS_FACEUP_DEFENSE) > 0)
    //        {
    //            res += InterString.Get("(表侧防御)");
    //        }
    //        else if ((p1.position & (UInt32)game_position.POS_FACEDOWN_ATTACK) > 0)
    //        {
    //            res += InterString.Get("(里侧攻击)");
    //        }
    //        else if ((p1.position & (UInt32)game_position.POS_FACEDOWN_DEFENSE) > 0)
    //        {
    //            res += InterString.Get("(里侧防御)");
    //        }
    //        else if ((p1.position & (UInt32)game_position.POS_ATTACK) > 0)
    //        {
    //            res += InterString.Get("(攻击)");
    //        }
    //        else if ((p1.position & (UInt32)game_position.POS_DEFENSE) > 0)
    //        {
    //            res += InterString.Get("(防御)");
    //        }
    //        else if ((p1.position & (UInt32)game_position.POS_FACEUP) > 0)
    //        {
    //            res += InterString.Get("(表侧)");
    //        }
    //        else if ((p1.position & (UInt32)game_position.POS_DEFENSE) > 0)
    //        {
    //            res += InterString.Get("(里侧)");
    //        }
    //    }

    //    return res;
    //}

    internal static string getGPSstringName(gameCard card, bool green = false)
    {
        string res = "";
        res += getGPSstringLocation(card.p) + "\n「" + getSuperName(card.get_data().Name, card.get_data().Id) + "」";
        if (green)
        {
            return "[00ff00]" + res + "[-]";
        }
        return res;
    }

    internal static string getSuperName(string name,int code)
    {
        string res = "";
        res = "[url=" + code.ToString() + "][u]" + name + "[/u][/url]";
        return res;
    }

    internal static string getDName(string name, int code)  
    {
        string res = "";
        res = "「[url=" + code.ToString() + "][u]" + name + "[/u][/url]」";
        return res;
    }

    internal static float getScreenDistance(GameObject a,GameObject b)
    {
        Vector3 sa = Program.camera_game_main.WorldToScreenPoint(a.transform.position);sa.z = 0;
        Vector3 sb = Program.camera_game_main.WorldToScreenPoint(b.transform.position);sb.z = 0;
        return Vector3.Distance(sa, sb);
    }

    internal static void setParent(GameObject child, GameObject parent)
    {
        child.transform.SetParent(parent.transform, true);
        Transform[] Transforms = child.GetComponentsInChildren<Transform>();
        foreach (Transform achild in Transforms)
            achild.gameObject.layer = parent.layer;
    }
}
