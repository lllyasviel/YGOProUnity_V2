using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class Program : MonoBehaviour
{

    #region Resources
    public Camera main_camera;
    public facer face;
    public Light light;
    public AudioSource audio;
    public AudioClip zhankai;
    public GameObject mod_ui_2d;
    public GameObject mod_ui_3d;
    public GameObject mod_winExplode;
    public GameObject mod_loseExplode;
    public GameObject mod_audio_effect;
    public GameObject mod_ocgcore_card;
    public GameObject mod_ocgcore_card_cloude;
    public GameObject mod_ocgcore_card_number_shower;
    public GameObject mod_ocgcore_card_figure_line;
    public GameObject mod_ocgcore_hidden_button;
    public GameObject mod_ocgcore_coin;
    public GameObject mod_ocgcore_dice;
    public GameObject mod_simple_quad;
    public GameObject mod_simple_ngui_background_texture;
    public GameObject mod_simple_ngui_text;
    public GameObject mod_ocgcore_number;
    public GameObject mod_ocgcore_decoration_chain_selecting;
    public GameObject mod_ocgcore_decoration_card_selected;
    public GameObject mod_ocgcore_decoration_card_selecting;
    public GameObject mod_ocgcore_decoration_card_active;
    public GameObject mod_ocgcore_decoration_spsummon;
    public GameObject mod_ocgcore_decoration_thunder;
    public GameObject mod_ocgcore_decoration_trap_activated;
    public GameObject mod_ocgcore_decoration_magic_activated;
    public GameObject mod_ocgcore_decoration_magic_zhuangbei;
    public GameObject mod_ocgcore_decoration_removed;
    public GameObject mod_ocgcore_decoration_tograve;
    public GameObject mod_ocgcore_decoration_card_setted;
    public GameObject mod_ocgcore_blood;
    public GameObject mod_ocgcore_blood_screen;
    public GameObject mod_ocgcore_bs_atk_decoration;
    public GameObject mod_ocgcore_bs_atk_line_earth;
    public GameObject mod_ocgcore_bs_atk_line_water;
    public GameObject mod_ocgcore_bs_atk_line_fire;
    public GameObject mod_ocgcore_bs_atk_line_wind;
    public GameObject mod_ocgcore_bs_atk_line_dark;
    public GameObject mod_ocgcore_bs_atk_line_light;
    public GameObject mod_ocgcore_cs_chaining;
    public GameObject mod_ocgcore_cs_end;
    public GameObject mod_ocgcore_cs_bomb;
    public GameObject mod_ocgcore_cs_negated;
    public GameObject mod_ocgcore_cs_mon_earth;
    public GameObject mod_ocgcore_cs_mon_water;
    public GameObject mod_ocgcore_cs_mon_fire;
    public GameObject mod_ocgcore_cs_mon_wind;
    public GameObject mod_ocgcore_cs_mon_light;
    public GameObject mod_ocgcore_cs_mon_dark;
    public GameObject mod_ocgcore_ss_summon_earth;
    public GameObject mod_ocgcore_ss_summon_water;
    public GameObject mod_ocgcore_ss_summon_fire;
    public GameObject mod_ocgcore_ss_summon_wind;
    public GameObject mod_ocgcore_ss_summon_dark;
    public GameObject mod_ocgcore_ss_summon_light;
    public GameObject mod_ocgcore_ol_earth;
    public GameObject mod_ocgcore_ol_water;
    public GameObject mod_ocgcore_ol_fire;
    public GameObject mod_ocgcore_ol_wind;
    public GameObject mod_ocgcore_ol_dark;
    public GameObject mod_ocgcore_ol_light;
    public GameObject mod_ocgcore_ss_spsummon_normal;
    public GameObject mod_ocgcore_ss_spsummon_ronghe;
    public GameObject mod_ocgcore_ss_spsummon_tongtiao;
    public GameObject mod_ocgcore_ss_spsummon_yishi;
    public GameObject mod_ocgcore_ss_spsummon_link;
    public GameObject mod_ocgcore_ss_p_idle_effect;
    public GameObject mod_ocgcore_ss_p_sum_effect;
    public GameObject mod_ocgcore_ss_dark_hole;
    public GameObject mod_ocgcore_ss_link_mark;
    public GameObject new_ui_menu;
    public GameObject new_ui_setting;
    public GameObject new_ui_book;
    public GameObject new_ui_selectServer;
    public GameObject new_ui_gameInfo;
    public GameObject new_ui_cardDescription;
    public GameObject new_ui_search;
    public GameObject new_ui_searchDetailed;
    public GameObject new_ui_cardOnSearchList;
    public GameObject new_bar_changeSide;
    public GameObject new_bar_duel;
    public GameObject new_bar_room;
    public GameObject new_bar_editDeck;
    public GameObject new_bar_watchDuel;
    public GameObject new_bar_watchRecord;
    public GameObject new_mod_cardInDeckManager;
    public GameObject new_mod_tableInDeckManager;
    public GameObject new_ui_handShower;
    public GameObject new_ui_textMesh;
    public GameObject new_ui_superButton;
    public GameObject new_ui_superButtonTransparent;
    public GameObject new_ui_aiRoom;
    public GameObject new_ocgcore_field;
    public GameObject new_ocgcore_chainCircle;
    public GameObject new_ocgcore_wait;
    public GameObject new_mouse;
    public GameObject remaster_deckManager;
    public GameObject remaster_replayManager;
    public GameObject remaster_puzzleManager;
    public GameObject remaster_tagRoom;
    public GameObject remaster_room;
    public GameObject ES_1;
    public GameObject ES_2;
    public GameObject ES_2Force;
    public GameObject ES_3cancle;
    public GameObject ES_Single_multiple_window;
    public GameObject ES_Single_option;
    public GameObject ES_multiple_option;
    public GameObject ES_input;
    public GameObject ES_position;
    public GameObject ES_Tp;
    public GameObject ES_Face;
    public GameObject ES_FS;
    public GameObject Pro1_CardShower;
    public GameObject Pro1_superCardShower;
    public GameObject Pro1_superCardShowerA;
    public GameObject New_arrow;
    public GameObject New_selectKuang;
    public GameObject New_chainKuang;
    public GameObject New_phase;
    public GameObject New_decker;
    public GameObject New_winCaculator;
    public GameObject New_winCaculatorRecord;
    public GameObject New_ocgcore_placeSelector;
    #endregion

    #region Initializement

    private static Program instance;

    public static Program I()
    {
        return instance;
    }

    public static int TimePassed()
    {
        return (int)(Time.time * 1000f);
    }

    private List<GameObject> allObjects = new List<GameObject>();

    void loadResource(GameObject g)
    {
        try
        {
            GameObject obj = GameObject.Instantiate(g) as GameObject;
            obj.SetActive(false);
            allObjects.Add(obj);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    void loadResources()
    {

        loadResource(mod_audio_effect);
        loadResource(mod_ocgcore_card);
        loadResource(mod_ocgcore_card_cloude);
        loadResource(mod_ocgcore_card_number_shower);
        loadResource(mod_ocgcore_card_figure_line);
        loadResource(mod_ocgcore_hidden_button);
        loadResource(mod_ocgcore_coin);
        loadResource(mod_ocgcore_dice);

        loadResource(mod_ocgcore_decoration_chain_selecting);
        loadResource(mod_ocgcore_decoration_card_selected);
        loadResource(mod_ocgcore_decoration_card_selecting);
        loadResource(mod_ocgcore_decoration_card_active);
        loadResource(mod_ocgcore_decoration_spsummon);
        loadResource(mod_ocgcore_decoration_thunder);
        loadResource(mod_ocgcore_cs_mon_earth);
        loadResource(mod_ocgcore_cs_mon_water);
        loadResource(mod_ocgcore_cs_mon_fire);
        loadResource(mod_ocgcore_cs_mon_wind);
        loadResource(mod_ocgcore_cs_mon_light);
        loadResource(mod_ocgcore_cs_mon_dark);
        loadResource(mod_ocgcore_decoration_trap_activated);
        loadResource(mod_ocgcore_decoration_magic_activated);
        loadResource(mod_ocgcore_decoration_magic_zhuangbei);

        loadResource(mod_ocgcore_decoration_removed);
        loadResource(mod_ocgcore_decoration_tograve);
        loadResource(mod_ocgcore_decoration_card_setted);
        loadResource(mod_ocgcore_blood);
        loadResource(mod_ocgcore_blood_screen);


        loadResource(mod_ocgcore_bs_atk_decoration);
        loadResource(mod_ocgcore_bs_atk_line_earth);
        loadResource(mod_ocgcore_bs_atk_line_water);
        loadResource(mod_ocgcore_bs_atk_line_fire);
        loadResource(mod_ocgcore_bs_atk_line_wind);
        loadResource(mod_ocgcore_bs_atk_line_dark);
        loadResource(mod_ocgcore_bs_atk_line_light);

        loadResource(mod_ocgcore_cs_chaining);
        loadResource(mod_ocgcore_cs_end);
        loadResource(mod_ocgcore_cs_bomb);
        loadResource(mod_ocgcore_cs_negated);

        loadResource(mod_ocgcore_ss_summon_earth);
        loadResource(mod_ocgcore_ss_summon_water);
        loadResource(mod_ocgcore_ss_summon_fire);
        loadResource(mod_ocgcore_ss_summon_wind);
        loadResource(mod_ocgcore_ss_summon_dark);
        loadResource(mod_ocgcore_ss_summon_light);

        loadResource(mod_ocgcore_ol_earth);
        loadResource(mod_ocgcore_ol_water);
        loadResource(mod_ocgcore_ol_fire);
        loadResource(mod_ocgcore_ol_wind);
        loadResource(mod_ocgcore_ol_dark);
        loadResource(mod_ocgcore_ol_light);

        loadResource(mod_ocgcore_ss_spsummon_normal);
        loadResource(mod_ocgcore_ss_spsummon_ronghe);
        loadResource(mod_ocgcore_ss_spsummon_tongtiao);
        loadResource(mod_ocgcore_ss_spsummon_link);
        loadResource(mod_ocgcore_ss_spsummon_yishi);
        loadResource(mod_ocgcore_ss_p_idle_effect);
        loadResource(mod_ocgcore_ss_p_sum_effect);
        loadResource(mod_ocgcore_ss_dark_hole);
        loadResource(mod_ocgcore_ss_link_mark);
    }

    public static float transparency = 0;

    //public static bool YGOPro1 = true;

    public static float getVerticalTransparency()
    {
        if (I().setting.setting.closeUp.value == false)
        {
            return 0;
        }
        return transparency;
    }

    public static GameObject ui_back_ground_2d = null;
    public static Camera camera_back_ground_2d = null;
    public static GameObject ui_container_3d = null;
    public static Camera camera_container_3d = null;
    public static Camera camera_game_main = null;
    public static GameObject ui_windows_2d = null;
    public static Camera camera_windows_2d = null;
    public static GameObject ui_main_2d = null;
    public static Camera camera_main_2d = null;
    public static GameObject ui_main_3d = null;
    public static Camera camera_main_3d = null;

    public static Vector3 cameraPosition = new Vector3(0, 23, -23);
    public static Vector3 cameraRotation = new Vector3(60, 0, 0);
    public static bool cameraFacing = false;

    public static float verticleScale = 5f;

    void initialize()
    {

        go(1, () =>
        {
            UIHelper.iniFaces();
            initializeALLcameras();
            fixALLcamerasPreFrame();
            backGroundPic = new BackGroundPic();
            servants.Add(backGroundPic);
            backGroundPic.fixScreenProblem();
        });
        go(300, () =>
        {
            InterString.initialize("config\\translation.conf");
            GameTextureManager.initialize();
            Config.initialize("config\\config.conf");
            GameStringManager.initialize("config\\strings.conf");
            if (File.Exists("cdb\\strings.conf"))
            {
                GameStringManager.initialize("cdb\\strings.conf");
            }
            if (File.Exists("diy\\strings.conf"))
            {
                GameStringManager.initialize("diy\\strings.conf");
            }
            YGOSharp.BanlistManager.initialize("config\\lflist.conf");

            var fileInfos = (new DirectoryInfo("cdb")).GetFiles();
            for (int i = 0; i < fileInfos.Length; i++)
            {
                if (fileInfos[i].Name.Length > 4)
                {
                    if (fileInfos[i].Name.Substring(fileInfos[i].Name.Length - 4, 4) == ".cdb")
                    {
                        YGOSharp.CardsManager.initialize("cdb\\" + fileInfos[i].Name);
                    }
                }
            }

            if (Directory.Exists("diy"))
            {
                fileInfos = (new DirectoryInfo("diy")).GetFiles();
                for (int i = 0; i < fileInfos.Length; i++)
                {
                    if (fileInfos[i].Name.Length > 4)
                    {
                        if (fileInfos[i].Name.Substring(fileInfos[i].Name.Length - 4, 4) == ".cdb")
                        {
                            YGOSharp.CardsManager.initialize("diy\\" + fileInfos[i].Name);
                        }
                    }
                }
            }


            fileInfos = (new DirectoryInfo("pack")).GetFiles();
            for (int i = 0; i < fileInfos.Length; i++)
            {
                if (fileInfos[i].Name.Length > 3)
                {
                    if (fileInfos[i].Name.Substring(fileInfos[i].Name.Length - 3, 3) == ".db")
                    {
                        YGOSharp.PacksManager.initialize("pack\\" + fileInfos[i].Name);
                    }
                }
            }
            YGOSharp.PacksManager.initializeSec();
            initializeALLservants();
            loadResources();

        });

    }

    public GameObject mouseParticle;

    static int lastChargeTime = 0;
    public static void charge()
    {
        if (Program.TimePassed() - lastChargeTime > 5 * 60 * 1000)
        {
            lastChargeTime = Program.TimePassed();
            try
            {
                GameTextureManager.clearAll();
                Resources.UnloadUnusedAssets();
                GC.Collect();
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
        }
    }

    #endregion

    #region Tools

    public static GameObject pointedGameObject = null;

    public static Collider pointedCollider = null;

    public static bool InputGetMouseButtonDown_0;

    public static bool InputGetMouseButton_0;

    public static bool InputGetMouseButtonUp_0;

    public static bool InputGetMouseButtonDown_1;

    public static bool InputGetMouseButtonUp_1;

    public static bool InputEnterDown = false;

    public static float wheelValue = 0;

    public class delayedTask
    {
        public int timeToBeDone;
        public Action act;
    }

    static List<delayedTask> delayedTasks = new List<delayedTask>();

    public static void go(int delay_, Action act_)
    {
        delayedTasks.Add(new delayedTask
        {
            act = act_,
            timeToBeDone = delay_ + Program.TimePassed(),
        });
    }

    public static void notGo(Action act_)
    {
        List<delayedTask> rem = new List<delayedTask>();
        for (int i = 0; i < delayedTasks.Count; i++)
        {
            if (delayedTasks[i].act == act_)
            {
                rem.Add(delayedTasks[i]);
            }
        }
        for (int i = 0; i < rem.Count; i++)
        {
            delayedTasks.Remove(rem[i]);
        }
        rem.Clear();
    }

    int rayFilter = 0;

    public void initializeALLcameras()
    {
        for (int i = 0; i < 32; i++)
        {
            if (i == 15)
            {
                continue;
            }
            rayFilter |= (int)Math.Pow(2, i);
        }

        if (camera_game_main == null)
        {
            camera_game_main = this.main_camera;
        }
        camera_game_main.transform.position = new Vector3(0, 23, -23);
        camera_game_main.transform.eulerAngles = new Vector3(60, 0, 0);
        camera_game_main.transform.localScale = new Vector3(1, 1, 1);
        camera_game_main.rect = new Rect(0, 0, 1, 1);
        camera_game_main.depth = 0;
        camera_game_main.gameObject.layer = 0;
        camera_game_main.clearFlags = CameraClearFlags.Depth;

        if (ui_back_ground_2d == null)
        {
            ui_back_ground_2d = create(mod_ui_2d);
            camera_back_ground_2d = ui_back_ground_2d.transform.Find("Camera").GetComponent<Camera>();
        }
        camera_back_ground_2d.depth = -2;
        ui_back_ground_2d.layer = 8;
        ui_back_ground_2d.transform.Find("Camera").gameObject.layer = 8;
        camera_back_ground_2d.cullingMask = (int)Mathf.Pow(2, 8);
        camera_back_ground_2d.clearFlags = CameraClearFlags.Depth;

        if (ui_container_3d == null)
        {
            ui_container_3d = create(mod_ui_3d);
            camera_container_3d = ui_container_3d.transform.Find("Camera").GetComponent<Camera>();
        }
        camera_container_3d.depth = -1;
        ui_container_3d.layer = 9;
        ui_container_3d.transform.Find("Camera").gameObject.layer = 9;
        camera_container_3d.cullingMask = (int)Mathf.Pow(2, 9);
        camera_container_3d.fieldOfView = 75;
        camera_container_3d.rect = camera_game_main.rect;
        camera_container_3d.transform.position = new Vector3(0, 23, -23);
        camera_container_3d.transform.eulerAngles = new Vector3(60, 0, 0);
        camera_container_3d.transform.localScale = new Vector3(1, 1, 1);
        camera_container_3d.rect = new Rect(0, 0, 1, 1);
        camera_container_3d.clearFlags = CameraClearFlags.Depth;



        if (ui_main_2d == null)
        {
            ui_main_2d = create(mod_ui_2d);
            camera_main_2d = ui_main_2d.transform.Find("Camera").GetComponent<Camera>();
        }
        camera_main_2d.depth = 3;
        ui_main_2d.layer = 11;
        ui_main_2d.transform.Find("Camera").gameObject.layer = 11;
        camera_main_2d.cullingMask = (int)Mathf.Pow(2, 11);
        camera_main_2d.clearFlags = CameraClearFlags.Depth;


        if (ui_windows_2d == null)
        {
            ui_windows_2d = create(mod_ui_2d);
            camera_windows_2d = ui_windows_2d.transform.Find("Camera").GetComponent<Camera>();
        }
        camera_windows_2d.depth = 2;
        ui_windows_2d.layer = 19;
        ui_windows_2d.transform.Find("Camera").gameObject.layer = 19;
        camera_windows_2d.cullingMask = (int)Mathf.Pow(2, 19);
        camera_windows_2d.clearFlags = CameraClearFlags.Depth;


        if (ui_main_3d == null)
        {
            ui_main_3d = create(mod_ui_3d);
            camera_main_3d = ui_main_3d.transform.Find("Camera").GetComponent<Camera>();
        }
        camera_main_3d.depth = 1;
        ui_main_3d.layer = 10;
        ui_main_3d.transform.Find("Camera").gameObject.layer = 10;
        camera_main_3d.cullingMask = (int)Mathf.Pow(2, 10);
        camera_main_3d.fieldOfView = 75;
        camera_main_3d.rect = new Rect(0, 0, 1, 1);
        camera_main_3d.transform.position = new Vector3(0, 23, -23);
        camera_main_3d.transform.eulerAngles = new Vector3(60, 0, 0);
        camera_main_3d.transform.localScale = new Vector3(1, 1, 1);
        camera_main_3d.clearFlags = CameraClearFlags.Depth;




        camera_main_3d.transform.localPosition = camera_game_main.transform.position;
        camera_container_3d.transform.localPosition = camera_game_main.transform.position;

        camera_main_3d.transform.localEulerAngles = camera_game_main.transform.localEulerAngles;
        camera_container_3d.transform.localEulerAngles = camera_game_main.transform.localEulerAngles;

        camera_main_3d.fieldOfView = camera_game_main.fieldOfView;
        camera_container_3d.fieldOfView = camera_game_main.fieldOfView;

        camera_main_3d.rect = camera_game_main.rect;
        camera_container_3d.rect = camera_game_main.rect;
    }

    public static float deltaTime = 1f / 120f;

    public void fixALLcamerasPreFrame()
    {
        deltaTime = Time.deltaTime;
        if (deltaTime > 1f / 40f)
        {
            deltaTime = 1f / 40f;
        }
        if (camera_game_main != null)
        {
            camera_game_main.transform.position += (cameraPosition - camera_game_main.transform.position) * deltaTime * 3.5f;
            camera_container_3d.transform.localPosition = camera_game_main.transform.position;
            if (cameraFacing == false)
            {
                camera_game_main.transform.localEulerAngles += (cameraRotation - camera_game_main.transform.localEulerAngles) * deltaTime * 3.5f;
            }
            else
            {
                camera_game_main.transform.LookAt(Vector3.zero);
            }
            camera_container_3d.transform.localEulerAngles = camera_game_main.transform.localEulerAngles;
            camera_container_3d.fieldOfView = camera_game_main.fieldOfView;
            camera_container_3d.rect = camera_game_main.rect;
        }
    }

    public void fixScreenProblems()
    {
        for (int i = 0; i < servants.Count; i++)
        {
            servants[i].fixScreenProblem();
        }
    }

    public GameObject create(
        GameObject mod,
        Vector3 position = default(Vector3),
        Vector3 rotation = default(Vector3),
        bool fade = false,
        GameObject father = null,
        bool allParamsInWorld = true,
        Vector3 wantScale = default(Vector3)
        )
    {
        Vector3 scale = mod.transform.localScale;
        if (wantScale != default(Vector3))
        {
            scale = wantScale;
        }
        GameObject return_value = (GameObject)MonoBehaviour.Instantiate(mod);
        if (position != default(Vector3))
        {
            return_value.transform.position = position;
        }
        else
        {
            return_value.transform.position = Vector3.zero;
        }
        if (rotation != default(Vector3))
        {
            return_value.transform.eulerAngles = rotation;
        }
        else
        {
            return_value.transform.eulerAngles = Vector3.zero;
        }
        if (father != null)
        {
            return_value.transform.SetParent(father.transform, false);
            return_value.layer = father.layer;
            if (allParamsInWorld == true)
            {
                return_value.transform.position = position;
                return_value.transform.localScale = scale;
                return_value.transform.eulerAngles = rotation;
            }
            else
            {
                return_value.transform.localPosition = position;
                return_value.transform.localScale = scale;
                return_value.transform.localEulerAngles = rotation;
            }
        }
        else
        {
            return_value.layer = 0;
        }
        Transform[] Transforms = return_value.GetComponentsInChildren<Transform>();
        foreach (Transform child in Transforms)
        {
            child.gameObject.layer = return_value.layer;
        }
        if (fade == true)
        {
            return_value.transform.localScale = Vector3.zero;
            iTween.ScaleToE(return_value, scale, 0.3f);
        }
        return return_value;
    }

    public void destroy(GameObject obj, float time = 0, bool fade = false, bool instantNull = false)
    {
        try
        {
            if (obj != null)
            {
                if (fade)
                {
                    iTween.ScaleTo(obj, Vector3.zero, 0.4f);
                    MonoBehaviour.Destroy(obj, 0.6f);
                }
                else
                {
                    if (time != 0) MonoBehaviour.Destroy(obj, time);
                    else MonoBehaviour.Destroy(obj);
                }
                if (instantNull)
                {
                    obj = null;
                }
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    //public static void shiftCameraPan(Camera camera, bool enabled)
    //{
    //    cameraPaning = enabled;
    //    PanWithMouse panWithMouse = camera.gameObject.GetComponent<PanWithMouse>();
    //    if (panWithMouse == null)
    //    {
    //        panWithMouse = camera.gameObject.AddComponent<PanWithMouse>();
    //    }
    //    panWithMouse.enabled = enabled;
    //    if (enabled == false)
    //    {
    //        iTween.RotateTo(camera.gameObject, new Vector3(60, 0, 0), 0.6f);
    //    }
    //}

    public static void reMoveCam(float xINscreen)
    {
        float all = (float)Screen.width / 2f;
        float it = xINscreen - (float)Screen.width / 2f;
        float val = it / all;
        camera_game_main.rect = new Rect(val, 0, 1, 1);
        camera_container_3d.rect = camera_game_main.rect;
        camera_main_3d.rect = camera_game_main.rect;
    }

    public static void ShiftUIenabled(GameObject ui, bool enabled)
    {
        var all = ui.GetComponentsInChildren<BoxCollider>();
        for (int i = 0; i < all.Length; i++)
        {
            all[i].enabled = enabled;
        }
    }

    public static Texture2D GetTextureViaPath(string path)
    {
        FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
        file.Seek(0, SeekOrigin.Begin);
        byte[] data = new byte[file.Length];
        file.Read(data, 0, (int)file.Length);
        file.Close();
        file.Dispose();
        file = null;
        Texture2D pic = new Texture2D(1024, 600);
        pic.LoadImage(data);
        return pic;
    }

    #endregion

    #region Servants

    List<Servant> servants = new List<Servant>();

    public Servant backGroundPic;
    public Menu menu;
    public Setting setting;
    public selectDeck selectDeck;
    public selectReplay selectReplay;
    public Room room;
    public CardDescription cardDescription;
    public DeckManager deckManager;
    public Ocgcore ocgcore;
    public SelectServer selectServer;
    public Book book;
    public puzzleMode puzzleMode;
    public AIRoom aiRoom;

    void initializeALLservants()
    {
        menu = new Menu();
        servants.Add(menu);
        setting = new Setting();
        servants.Add(setting);
        selectDeck = new selectDeck();
        servants.Add(selectDeck);
        room = new Room();
        servants.Add(room);
        cardDescription = new CardDescription();
        deckManager = new DeckManager();
        servants.Add(deckManager);
        ocgcore = new Ocgcore();
        servants.Add(ocgcore);
        selectServer = new SelectServer();
        servants.Add(selectServer);
        book = new Book();
        servants.Add(book);
        selectReplay = new selectReplay();
        servants.Add(selectReplay);
        puzzleMode = new puzzleMode();
        servants.Add(puzzleMode);
        aiRoom = new AIRoom();
        servants.Add(aiRoom);
    }

    public void shiftToServant(Servant to)
    {
        if (to != backGroundPic && backGroundPic.isShowed)
        {
            backGroundPic.hide();
        }
        if (to != menu && menu.isShowed)
        {
            menu.hide();
        }
        if (to != setting && setting.isShowed)
        {
            setting.hide();
        }
        if (to != selectDeck && selectDeck.isShowed)
        {
            selectDeck.hide();
        }
        if (to != room && room.isShowed)
        {
            room.hide();
        }
        if (to != deckManager && deckManager.isShowed)
        {
            deckManager.hide();
        }
        if (to != ocgcore && ocgcore.isShowed)
        {
            ocgcore.hide();
        }
        if (to != selectServer && selectServer.isShowed)
        {
            selectServer.hide();
        }
        if (to != selectReplay && selectReplay.isShowed)
        {
            selectReplay.hide();
        }
        if (to != puzzleMode && puzzleMode.isShowed)
        {
            puzzleMode.hide();
        }
        if (to != aiRoom && aiRoom.isShowed)
        {
            aiRoom.hide();
        }

        if (to == backGroundPic && backGroundPic.isShowed == false) backGroundPic.show();
        if (to == menu && menu.isShowed == false) menu.show();
        if (to == setting && setting.isShowed == false) setting.show();
        if (to == selectDeck && selectDeck.isShowed == false) selectDeck.show();
        if (to == room && room.isShowed == false) room.show();
        if (to == deckManager && deckManager.isShowed == false) deckManager.show();
        if (to == ocgcore && ocgcore.isShowed == false) ocgcore.show();
        if (to == selectServer && selectServer.isShowed == false) selectServer.show();
        if (to == selectReplay && selectReplay.isShowed == false) selectReplay.show();
        if (to == puzzleMode && puzzleMode.isShowed == false) puzzleMode.show();
        if (to == aiRoom && aiRoom.isShowed == false) aiRoom.show();

    }

    #endregion

    #region MonoBehaviors

    void Start()
    {
        if (Screen.width < 100 || Screen.height < 100)
        {
            Screen.SetResolution(1300, 700, false);
        }
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 144;
        mouseParticle = Instantiate(new_mouse);
        instance = this;
        initialize();
        go(500, () => { gameStart(); });
    }

    int preWid = 0;

    int preheight = 0;

    public static float _padScroll = 0;

    void OnGUI()
    {
        if (Event.current.type == EventType.ScrollWheel)
            _padScroll = -Event.current.delta.y / 100;
        else
            _padScroll = 0;
    }

    void Update()
    {
       
        if (preWid != Screen.width || preheight != Screen.height)
        {
            Resources.UnloadUnusedAssets();
            onRESIZED();
        }
        fixALLcamerasPreFrame();
        wheelValue = UICamera.GetAxis("Mouse ScrollWheel") * 50;
        pointedGameObject = null;
        pointedCollider = null;
        Ray line = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(line, out hit, (float)1000, rayFilter))
        {
            pointedGameObject = hit.collider.gameObject;
            pointedCollider = hit.collider;
        }
        GameObject hoverobject = UICamera.Raycast(Input.mousePosition) ? UICamera.lastHit.collider.gameObject : null;
        if (hoverobject != null)
        {
            if (hoverobject.layer == 11 || pointedGameObject == null)
            {
                pointedGameObject = hoverobject;
                pointedCollider = UICamera.lastHit.collider;
            }
        }
        InputGetMouseButtonDown_0 = Input.GetMouseButtonDown(0);
        InputGetMouseButtonUp_0 = Input.GetMouseButtonUp(0);
        InputGetMouseButtonDown_1 = Input.GetMouseButtonDown(1);
        InputGetMouseButtonUp_1 = Input.GetMouseButtonUp(1);
        InputEnterDown = Input.GetKeyDown(KeyCode.Return);
        InputGetMouseButton_0 = Input.GetMouseButton(0);
        for (int i = 0; i < servants.Count; i++)
        {
            servants[i].Update();
        }
        TcpHelper.preFrameFunction();
        delayedTask remove = null;
        while (true)
        {
            remove = null;
            for (int i = 0; i < delayedTasks.Count; i++)
            {
                if (Program.TimePassed() > delayedTasks[i].timeToBeDone)
                {
                    remove = delayedTasks[i];
                    try
                    {
                        remove.act();
                    }
                    catch (System.Exception e)
                    {
                        UnityEngine.Debug.Log(e);
                    }
                    break;
                }
            }
            if (remove != null)
            {
                delayedTasks.Remove(remove);
            }
            else
            {
                break;
            }
        }

    }

    private void onRESIZED()
    {
        preWid = Screen.width;
        preheight = Screen.height;
        Program.notGo(fixScreenProblems);
        Program.go(500, fixScreenProblems);
    }

    public static void DEBUGLOG(object o)
    {
#if UNITY_EDITOR
        Debug.Log(o);
#endif
    }

    void gameStart()
    {
        backGroundPic.show();
        shiftToServant(menu);
    }

    public static bool Running = true;

    public static bool MonsterCloud = false;
    public static float fieldSize = 1;

    void OnApplicationQuit()
    {
        TcpHelper.SaveRecord();
        cardDescription.save();
        setting.saveWhenQuit();
        for (int i = 0; i < servants.Count; i++)
        {
            servants[i].OnQuit();
        }
        Running = false;
        try
        {
            TcpHelper.tcpClient.Close();
        }
        catch (System.Exception e)
        {
            //adeUnityEngine.Debug.Log(e);
        }
        Menu.deleteShell();
    }

    public void quit()
    {
        OnApplicationQuit();
    }

    #endregion

}
