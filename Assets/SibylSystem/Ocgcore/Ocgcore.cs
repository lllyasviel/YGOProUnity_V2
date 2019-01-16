using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YGOSharp.OCGWrapper.Enums;
public class Ocgcore : ServantWithCardDescription
{
    public enum Condition
    {
        N=0,
        duel = 1,
        watch = 2,
        record = 3,
    }

    public Condition condition = Condition.duel;

    public gameInfo gameInfo;

    public GameObject waitObject;

    public List<gameCard> cards = new List<gameCard>();

    bool flagForTimeConfirm = false;

    bool flagForCancleChain = false;

    public float getScreenCenter()
    {
        return ((float)Screen.width + Program.I().cardDescription.width - gameInfo.width) / 2f;
    }

    public int MasterRule = 0;

    class linkMask
    {
        public GPS p;
        public GameObject gameObject;
        public bool eff = false;
    }

    List<linkMask> linkMaskList = new List<linkMask>();

    linkMask makeLinkMask(GPS p)
    {
        linkMask ma = new linkMask();
        ma.p = p;
        ma.eff = !Program.I().setting.setting.Vlink.value;
        shift_effect(ma, Program.I().setting.setting.Vlink.value);
        return ma;
    }

    void shift_effect(linkMask target, bool value)
    {
        if (target.eff != value)
        {
            if (target.gameObject != null)
            {
                destroy(target.gameObject);
            }
            if (value)
            {
                target.gameObject = create_s(Program.I().mod_ocgcore_ss_link_mark, get_point_worldposition(target.p) + new Vector3(0, -0.1f, 0), Vector3.zero, true, null, true);
            }
            else
            {
                target.gameObject = create_s(Program.I().mod_simple_quad, get_point_worldposition(target.p) + new Vector3(0, -0.1f, 0), new Vector3(90, 0, 0), false, null, true);
                target.gameObject.transform.localScale = new Vector3(4, 4, 4);
                target.gameObject.GetComponent<Renderer>().material.mainTexture = GameTextureManager.LINKm;
                target.gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.8f);
            }
            target.eff = value;
        }
    }

    gameCardCondition get_point_worldcondition(GPS p)
    {
        gameCardCondition return_value = gameCardCondition.floating_clickable;
        if ((p.location & (UInt32)game_location.LOCATION_DECK) > 0)
        {
            return_value = gameCardCondition.still_unclickable;
        }
        if ((p.location & (UInt32)game_location.LOCATION_EXTRA) > 0)
        {
            return_value = gameCardCondition.still_unclickable;
        }
        if ((p.location & (UInt32)game_location.LOCATION_MZONE) > 0)
        {
            return_value = gameCardCondition.floating_clickable;
            if ((p.position & (UInt32)game_position.POS_FACEUP) > 0)
            {
                return_value = gameCardCondition.verticle_clickable;
            }
        }
        if ((p.location & (UInt32)game_location.LOCATION_SZONE) > 0)
        {
            return_value = gameCardCondition.floating_clickable;
        }
        if ((p.location & (UInt32)game_location.LOCATION_GRAVE) > 0)
        {
            return_value = gameCardCondition.still_unclickable;
        }
        if ((p.location & (UInt32)game_location.LOCATION_HAND) > 0)
        {
            return_value = gameCardCondition.floating_clickable;
        }
        if ((p.location & (UInt32)game_location.LOCATION_REMOVED) > 0)
        {
            return_value = gameCardCondition.still_unclickable;
        }
        if ((p.location & (UInt32)game_location.LOCATION_OVERLAY) > 0)
        {
            return_value = gameCardCondition.still_unclickable;
        }
        return return_value;
    }

    public Vector3 get_point_worldposition(GPS p, gameCard c = null)
    {
        Vector3 return_value = Vector3.zero;
        float real = (Program.fieldSize - 1) * 0.9f + 1f;
        if ((p.location & (UInt32)game_location.LOCATION_DECK) > 0)
        {
            if (p.controller==0)    
            {
                return_value = new Vector3(14.65f * real, 0, -14.6f);
            }
            else
            {
                return_value = new Vector3(-15.2f * real, 0, 14.6f);
            }
            return_value.y += p.sequence * 0.03f;
        }
        if ((p.location & (UInt32)game_location.LOCATION_EXTRA) > 0)
        {
            if (p.controller == 0)
            {
                return_value = new Vector3(-15.2f * real, 0, -14.6f);
            }
            else
            {
                return_value = new Vector3(14.65f * real, 0, 14.6f);
            }
            return_value.y += p.sequence * 0.03f;
        }
        if ((p.location & (UInt32)game_location.LOCATION_GRAVE) > 0)
        {
            if (MasterRule >= 4)
            {
                if (p.controller == 0)
                {
                    return_value = new Vector3(14.65f * real, 0, -9f);
                }
                else
                {
                    return_value = new Vector3(-15.2f * real, 0, 9f);
                }
            }
            else
            {
                if (p.controller == 0)
                {
                    return_value = new Vector3(14.65f * real, 0, -3f);
                }
                else
                {
                    return_value = new Vector3(-15.2f * real, 0, 3f);
                }
            }

            return_value.y += p.sequence * 0.03f;
        }
        if ((p.location & (UInt32)game_location.LOCATION_REMOVED) > 0)
        {
            if (MasterRule >= 4)
            {
                if (p.controller == 0)
                {
                    return_value = new Vector3(14.65f * real, 0, -3f);
                }
                else
                {
                    return_value = new Vector3(-15.2f * real, 0, 3f);
                }
            }
            else
            {
                if (p.controller == 0)
                {
                    return_value = new Vector3(14.65f * real + 19.15f - 14.65f, 0, -3f);
                }
                else
                {
                    return_value = new Vector3(-15.2f * real - 19.6f + 15.2f, 0, 3f);
                }
            }

            return_value.y += p.sequence * 0.03f;
        }
        if ((p.location & (UInt32)game_location.LOCATION_MZONE) > 0)
        {
            UInt32 realIndex = p.sequence;
            if (p.controller==0)    
            {
                realIndex = p.sequence;
                return_value.y = 0;
                return_value.z = -5.68f * real;
            }
            else
            {
                if (realIndex <= 4)
                {
                    realIndex = 4 - p.sequence;
                }else
                if (realIndex == 5)
                {
                    realIndex = 6;
                }else
                if (realIndex == 6)
                {
                    realIndex = 5;
                }
                return_value.y = 0;
                return_value.z = 5.65f * real;
            }
            switch (realIndex)  
            {
                case 0:
                    return_value.x = -10.1f;
                    break;
                case 1:
                    return_value.x = -5.17f;
                    break;
                case 2:
                    return_value.x = -0.27f;
                    break;
                case 3:
                    return_value.x = 4.72f;
                    break;
                case 4:
                    return_value.x = 9.62f;
                    break;
                case 5:
                    return_value.x = -5.17f;
                    return_value.z = 0;
                    break;
                case 6:
                    return_value.x = 4.72f;
                    return_value.z = 0;
                    break;
            }
            return_value.x *= real;
        }
        if ((p.location & (UInt32)game_location.LOCATION_SZONE) > 0)
        {
            if (p.sequence < 5 || ((p.sequence == 6 || p.sequence == 7) && MasterRule >= 4))
            {
                UInt32 realIndex = p.sequence;
                if (p.controller == 0)
                {
                    realIndex = p.sequence;
                    return_value.y = 0;
                    return_value.z = -11.5f * real;
                }
                else
                {
                    if (realIndex <= 4)
                    {
                        realIndex = 4 - p.sequence;
                    }else
                    if (realIndex == 7)
                    {
                        realIndex = 6;
                    }else
                    if (realIndex == 6)
                    {
                        realIndex = 7;
                    }
                    return_value.y = 0;
                    return_value.z = 11.5f * real;
                }
                switch (realIndex)
                {
                    case 0:
                        return_value.x = -10.1f;
                        break;
                    case 1:
                        return_value.x = -5.17f;
                        break;
                    case 2:
                        return_value.x = -0.27f;
                        break;
                    case 3:
                        return_value.x = 4.72f;
                        break;
                    case 4:
                        return_value.x = 9.62f;
                        break;
                    case 6:
                        return_value.x = -10.1f;
                        break;
                    case 7:
                        return_value.x = 9.62f;
                        break;
                }
                return_value.x *= real;
                if (gameField.isLong)
                {
                    if (p.controller == 1)
                    {
                        if (5.85f * real < 10f)
                        {
                            return_value.z = return_value.z - 5.85f * real + 10f;
                        }
                    }
                }
            }
            if (p.sequence == 5)
            {
                if (MasterRule >= 4)
                {
                    if (p.controller == 0)
                    {
                        return_value = new Vector3(-15.2f * real, 0, -9f);
                    }
                    else
                    {
                        return_value = new Vector3(14.65f * real, 0, 9f);
                    }
                }
                else
                {
                    if (p.controller == 0)
                    {
                        return_value = new Vector3(-15.2f * real, 0, -2.7f);
                    }
                    else
                    {
                        return_value = new Vector3(14.65f * real, 0, 2.75f);
                    }
                }
            }
            if (MasterRule <= 3)
            {
                if (p.sequence == 6)
                {
                    if (p.controller == 0)
                    {
                        return_value = new Vector3(-15.2f * real, 0, -9f);
                    }
                    else
                    {
                        return_value = new Vector3(14.65f * real, 0, 9f);
                    }
                }
                if (p.sequence == 7)
                {
                    if (p.controller == 0)
                    {
                        return_value = new Vector3(14.65f * real, 0, -9f);
                    }
                    else
                    {
                        return_value = new Vector3(-15.2f * real, 0, 9f);
                    }
                }
            }
        }
        if ((p.location & (UInt32)game_location.LOCATION_OVERLAY) > 0)
        {
            if (c != null)
            {
                int pposition = c.overFatherCount - 1 - p.position;
                return_value.y -= (pposition + 2) * 1f;
                return_value.x += (pposition + 1) * 0.6f;
            }
            else
            {
                return_value.y -= (p.position + 2) * 1f;
                return_value.x += (p.position + 1) * 0.6f;
            }

        }
        return return_value;
    }

    arrow Arrow;

    bool replayShowAll = false;
    bool reportShowAll = false;
    public override void initialize()
    {
        Arrow = ((GameObject)MonoBehaviour.Instantiate(Program.I().New_arrow)).GetComponent<arrow>();
        Arrow.gameObject.SetActive(false);
        replayShowAll = Config.Get("replayShowAll", "0") != "0";
        reportShowAll = Config.Get("reportShowAll", "0") != "0";
        gameInfo = create
            (
            Program.I().new_ui_gameInfo,
            Vector3.zero,
            Vector3.zero,
            false,
            Program.ui_back_ground_2d
            ).GetComponent<gameInfo>();
        gameInfo.ini();
        UIHelper.InterGameObject(gameInfo.gameObject);
        shiftCondition(Condition.duel);

        Program.go(1, () =>
        {
            MHS_creatBundle(60, localPlayer(0), game_location.LOCATION_DECK);
            MHS_creatBundle(15, localPlayer(0), game_location.LOCATION_EXTRA);
            MHS_creatBundle(60, localPlayer(1), game_location.LOCATION_DECK);
            MHS_creatBundle(15, localPlayer(1), game_location.LOCATION_EXTRA);
            for (int i = 0; i < cards.Count; i++)
            {
                cards[i].hide();
            }
        });
    }

    public override void applyHideArrangement()
    {
        base.applyHideArrangement();
        gameInfo.gameObject.SetActive(false);
        hideCaculator();
    }

    public override void applyShowArrangement()
    {
        base.applyShowArrangement();
        if (gameInfo.gameObject.activeInHierarchy == false)
        {
            gameInfo.gameObject.transform.localPosition = new Vector3(300, 0, 0);
            gameInfo.gameObject.SetActive(true);
            iTween.MoveToLocal(gameInfo.gameObject, Vector3.zero, 0.6f);
            gameInfo.ini();
            UIHelper.getByName<UIToggle>(gameInfo.gameObject, "ignore_").value = false;
            UIHelper.getByName<UIToggle>(gameInfo.gameObject, "watch_").value = false;
        }
    }

    public void shiftCondition(Condition condition)
    {
        this.condition = condition;
        switch (condition)
        {
            case Condition.duel:
                SetBar(Program.I().new_bar_duel, 0, 0);
                UIHelper.registEvent(toolBar, "input_", onChat);
                UIHelper.registEvent(toolBar, "gg_", onDuelResultConfirmed);
                UIHelper.registEvent(toolBar, "left_", on_left);
                UIHelper.registEvent(toolBar, "right_", on_right);
                UIHelper.registEvent(toolBar, "rush_", on_rush);
                UIHelper.addButtonEvent_toolShift(toolBar, "go_", on_go);
                UIHelper.addButtonEvent_toolShift(toolBar, "stop_", on_stop);
                break;
            case Condition.watch:
                SetBar(Program.I().new_bar_watchDuel, 0, 0);
                UIHelper.registEvent(toolBar, "input_", onChat);
                UIHelper.registEvent(toolBar, "exit_", onExit);
                UIHelper.registEvent(toolBar, "left_", on_left);
                UIHelper.registEvent(toolBar, "right_", on_right);
                UIHelper.addButtonEvent_toolShift(toolBar, "go_", on_go);
                UIHelper.addButtonEvent_toolShift(toolBar, "stop_", on_stop);
                break;
            case Condition.record:
                SetBar(Program.I().new_bar_watchRecord, 0, 0);
                UIHelper.registEvent(toolBar, "home_", onHome);
                UIHelper.registEvent(toolBar, "left_", on_left);
                UIHelper.registEvent(toolBar, "right_", on_right);
                UIHelper.addButtonEvent_toolShift(toolBar, "go_", on_go);
                UIHelper.addButtonEvent_toolShift(toolBar, "stop_", on_stop);
                break;
            default:
                break;
        }
    }


    int currentMessageIndex = -1;


    public void dangerTicking()
    {
        if (paused == true)
        {
            RMSshow_none(InterString.Get("您的时间不足无法使用ReadingSteiner，时间线强制收束！"));
            on_rush();
        }
    }


    public static bool inSkiping = false;
    void on_left()
    {
        if (winCaculator != null)
        {
            destroy(winCaculator.gameObject);
        }
        int preStepPackagesIndex = 0;
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i] < currentMessageIndex)
            {
                preStepPackagesIndex = keys[i];
                break;
            }
        }
        if (keys.Count>0)   
        {
            if (keys[0]!= currentMessageIndex)  
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    if (keys[i] < preStepPackagesIndex)
                    {
                        preStepPackagesIndex = keys[i];
                        break;
                    }
                }
            }
        }
        if (Packages_ALL.Count <= preStepPackagesIndex)
        {
            return;
        }
        if (condition == Condition.duel)
        {
            if (gameInfo.amIdanger())
            {
                RMSshow_none(InterString.Get("您的时间不足无法使用ReadingSteiner！"));
                return;
            }
        }
        bool needSwap = gameInfo.swaped;
        right = false;
        if (paused == false)
        {
            EventDelegate.Execute(UIHelper.getByName<UIButton>(toolBar, "stop_").onClick);
        }
        keys.Clear();
        currentMessageIndex = -1;
        Program.I().book.clear();
        inSkiping = true;
        for (int i = 0; i <= preStepPackagesIndex; i++)
        {
            if (i == preStepPackagesIndex)
            {
                currentMessage = (GameMessage)Packages_ALL[i].Fuction;
                try
                {
                    logicalizeMessage(Packages_ALL[i]);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e);
                }
                if (needSwap)
                {
                    GCS_swapALL(false);
                }
                try
                {
                    practicalizeMessage(Packages_ALL[i]);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e);
                }
                clearResponse();
            }
            else
            {
                currentMessage = (GameMessage)Packages_ALL[i].Fuction;
                try
                {
                    logicalizeMessage(Packages_ALL[i]);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e);
                }
            }
        }
        Packages.Clear();
        for (int i = 0; i < Packages_ALL.Count - preStepPackagesIndex - 1; i++)
        {
            Packages.Add(Packages_ALL[i + preStepPackagesIndex + 1]);
        }
        specialLR();
        inSkiping = false;
    }

    public static GameObject LRCgo = null;

    private void specialLR()
    {
        try
        {
            if (LRCgo != null)
            {
                destroy(LRCgo);
            }
            if (gameField != null)
            {
                gameField.shiftBlackHole(false, new Vector3(0, 0, 0));
            }
            Nconfirm();
            cardsForConfirm.Clear();
            if (flagForTimeConfirm)
            {
                flagForTimeConfirm = false;
                MessageBeginTime = Program.TimePassed();
                clearAllShowed();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    bool right = false;
    int keysTempCount = 0;

    void on_right()
    {
        specialLR();
        if (right)
        {
            inSkiping = true;
            while (keys.Count == keysTempCount && Packages.Count > 0)
            {
                currentMessage = (GameMessage)Packages[0].Fuction;
                try
                {
                    logicalizeMessage(Packages[0]);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e);
                }
                try
                {
                    practicalizeMessage(Packages[0]);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e);
                }
                Packages.RemoveAt(0);
            }
            inSkiping = false;
        }
        right = true;
        keysTempCount = keys.Count;
    }

    void on_rush()  
    {
        specialLR();
        while (Packages.Count > 0)
        {
            currentMessage = (GameMessage)Packages[0].Fuction;
            try
            {
                logicalizeMessage(Packages[0]);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
            if (Packages.Count==1)  
            {
                try
                {
                    practicalizeMessage(Packages[0]);
                    realize();
                    toNearest();
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e);
                }
            }
            Packages.RemoveAt(0);
        }
        keysTempCount = keys.Count;
        if (paused == true)
        {
            EventDelegate.Execute(UIHelper.getByName<UIButton>(toolBar, "go_").onClick);
        }
    }

    void on_go()
    {
        paused = false;
        if (condition == Condition.duel)
        {
            if (isShowed)
            {
                UIHelper.playSound("phase", 1f);
                gameField.animation_show_big_string(GameTextureManager.ts, true);
            }
            //Program.I().cardDescription.clearAllLog();
            RMSshow_none(InterString.Get("[7CFC00]ReadingSteiner结束，回归到主时间轴。[-]"));
            ((CardDescription)Program.I().cardDescription).setTitle("");
        }
    }

    void on_stop()
    {
        if (paused == false)
        {
            destroy(waitObject, 0, false, true);
            paused = true;
            if (currentMessageIndex > theWorldIndex)
            {
                theWorldIndex = currentMessageIndex;
            }
        }
        if (condition== Condition.record)   
        {
            return;
        }
        if (condition == Condition.duel)
        {
            if (isShowed)
            {
                UIHelper.playSound("nextturn", 1f);
                gameField.animation_show_big_string(GameTextureManager.rs, true);
            }
            Program.I().cardDescription.clearAllLog();
            RMSshow_none(InterString.Get("[FF3030]ReadingSteiner被启动成功！您现在可以随意操作时间。@n长按按钮跳跃时间，闪电按钮回到现在。[-]"));
            ((CardDescription)Program.I().cardDescription).setTitle(InterString.Get("[FF3030]ReadingSteiner 正在跨越时间线[-]"));
        }
    }

    public void onHome()
    {
        returnTo();
    }


    public Servant returnServant;
    public void returnTo()
    {
        TcpHelper.SaveRecord();
        if (returnServant != null)
        {
            Program.I().shiftToServant(returnServant);
        }
    }

    public void onExit()
    {
        if (TcpHelper.tcpClient != null)
        {
            if (TcpHelper.tcpClient.Connected)
            {
                Program.I().ocgcore.returnServant = Program.I().selectServer;
                TcpHelper.tcpClient.Client.Shutdown(0);
                TcpHelper.tcpClient.Close();
            }
            TcpHelper.tcpClient = null;
        }
        returnTo();
    }

    public bool surrended = false;

    public void onChat()
    {
        Program.I().room.onSubmit(UIHelper.getByName<UIInput>(toolBar, "input_").value);
        UIHelper.getByName<UIInput>(toolBar, "input_").value = "";
    }

    public int lpLimit = 8000;

    public int timeLimit = 180;

    public string name_0_c = "";

    public string name_1_c = "";

    public string name_0 = "";

    public string name_1 = "";

    public string name_0_tag = "";

    public string name_1_tag = "";

    public int life_0;

    public int life_1;

    List<Package> Packages = new List<Package>();
    List<Package> Packages_ALL = new List<Package>();

    public void addPackage(Package p)
    {
        TcpHelper.AddRecordLine(p);
        Packages.Add(p);
        Packages_ALL.Add(p);
    }

    public void flushPackages(List<Package> ps)
    {
        Packages.Clear();
        Packages = null;
        Packages = ps;
        Packages_ALL.Clear();
        foreach (var item in Packages)
        {
            Packages_ALL.Add(item);
        }
    }

    int MessageBeginTime = 0;

    int lastReszieTime = 0;

    public GameMessage currentMessage = GameMessage.Waiting;

    public bool paused = false;

    float lastSize = 0;
    float lastAlpha = 0;    

    public List<GameObject> allChainPanelFixedContainer = new List<GameObject>();

    void pre200Frame()
    {
        lastReszieTime = Program.TimePassed();
        if (lastSize != Program.fieldSize || lastAlpha != Program.getVerticalTransparency())
        {
            lastSize = Program.fieldSize;
            lastAlpha = Program.getVerticalTransparency();
            reSize();
        }
        if (allChainPanelFixedContainer.Count > 0)
        {
            allChainPanelFixedContainer.RemoveAll((a) => { return a == null; });
            for (int i = 0; i < allChainPanelFixedContainer.Count; i++)
            {
                allChainPanelFixedContainer[i].transform.localPosition = Vector3.zero;
            }
            List<List<GameObject>> groups = new List<List<GameObject>>();
            for (int i = 0; i < allChainPanelFixedContainer.Count; i++)
            {
                GameObject currentGameobject = allChainPanelFixedContainer[i];
                List<GameObject> toList = null;
                for (int a = 0; a < groups.Count; a++)  
                {
                    if (UIHelper.getScreenDistance(groups[a][0], currentGameobject) < 5f * ((float)Screen.height) / 700f)
                    {
                        toList = groups[a];
                    }
                }
                if (toList==null)   
                {
                    toList = new List<GameObject>();
                    groups.Add(toList);
                }
                toList.Add(currentGameobject);
            }
            for (int a = 0; a < groups.Count; a++)
            {
                for (int b = 0; b < groups[a].Count; b++)   
                {
                    groups[a][b].transform.localPosition = new Vector3(0.35f * (groups[a].Count - b - 1), 0, -0.05f * b - 0.2f);
                }
            }
        }
    }


    public override void preFrameFunction()
    {
        base.preFrameFunction();
        Program.reMoveCam(getScreenCenter());
        Program.cameraPosition.z += Program.wheelValue;
        if (Program.cameraPosition.z < camera_min)
        {
            Program.cameraPosition.z = camera_min;
        }
        if (Program.cameraPosition.z > camera_max)
        {
            Program.cameraPosition.z = camera_max;
        }

        if (Input.GetKeyDown(KeyCode.C) == true)
        {
            gameInfo.set_condition(gameInfo.chainCondition.smart);
        }
        if (Input.GetKeyDown(KeyCode.A) == true)
        {
            gameInfo.set_condition(gameInfo.chainCondition.all);
        }
        if (Input.GetKeyDown(KeyCode.S) == true)
        {
            gameInfo.set_condition(gameInfo.chainCondition.no);
        }

        if (Input.GetKeyUp(KeyCode.C) == true)
        {
            gameInfo.set_condition(gameInfo.chainCondition.standard);
        }
        if (Input.GetKeyUp(KeyCode.A) == true)
        {
            gameInfo.set_condition(gameInfo.chainCondition.standard);
        }
        if (Input.GetKeyUp(KeyCode.S) == true)
        {
            gameInfo.set_condition(gameInfo.chainCondition.standard);
        }

        if (Input.GetMouseButtonDown(2))
        {
            if (Program.I().book.isShowed)
            {
                Program.I().book.hide();
            }
            else
            {
                Program.I().book.show();
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))  
        {
            if (Program.I().book.isShowed==false)
            {
                Program.I().book.show();
            }
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (Program.I().book.isShowed==true)
            {
                Program.I().book.hide();
            }
        }
        if (paused == false)
        {
            sibyl();
        }
        if (right == true)
        {
            if (keys.Count == keysTempCount && Packages.Count > 0)
            {
                sibyl();
            }
            else
            {
                right = false;
            }
        }
        if (Program.TimePassed() > lastReszieTime + 200)
        {
            pre200Frame();
        }
    }

    void sibyl()
    {
        try
        {
            bool messageIsHandled = false;
            while (true)
            {
                if (Packages.Count == 0)
                {
                    break;
                }
                Package currentPackage = Packages[0];
                currentMessage = (GameMessage)currentPackage.Fuction;
                if (ifMessageImportant(currentPackage))
                {
                    if (Program.TimePassed() < MessageBeginTime)
                    {
                        break;
                    }
                }
                messageIsHandled = true;
                try
                {
                    logicalizeMessage(Packages[0]);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e);
                }
                try
                {
                    practicalizeMessage(Packages[0]);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e);
                }
                Packages.RemoveAt(0);
            }
            //if (messageIsHandled)
            //{
            //    realize(false);
            //}
            if (messageIsHandled)
            {
                if (condition == Condition.record)
                {
                    if (Packages.Count == 0)
                    {
                        RMSshow_none(InterString.Get("录像播放结束。"));
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    string winReason="";

    bool ifMessageImportant(Package package)
    {
        BinaryReader r = package.Data.reader;
        r.BaseStream.Seek(0, 0);
        GameMessage msg = (GameMessage)Packages[0].Fuction;
        switch (msg)    
        {
            case GameMessage.Start:
            case GameMessage.Win:
            case GameMessage.ConfirmDecktop:
            case GameMessage.ConfirmCards:
            case GameMessage.ShuffleDeck:
            case GameMessage.ShuffleHand:
            case GameMessage.SwapGraveDeck:
            case GameMessage.ShuffleSetCard:
            case GameMessage.ReverseDeck:
            case GameMessage.DeckTop:
            case GameMessage.NewTurn:
            case GameMessage.NewPhase:
            case GameMessage.Move:
            case GameMessage.PosChange:
            case GameMessage.Swap:
            case GameMessage.ChainSolved:
            case GameMessage.ChainNegated:
            case GameMessage.ChainDisabled:
            case GameMessage.RandomSelected:
            case GameMessage.BecomeTarget:
            case GameMessage.Draw:
            case GameMessage.Damage:
            case GameMessage.Recover:
            case GameMessage.PayLpCost:
            case GameMessage.TossCoin:
            case GameMessage.TossDice:
            case GameMessage.TagSwap:
            case GameMessage.ReloadField:
                return true;
            case GameMessage.FlipSummoning:
            case GameMessage.Summoning:
            case GameMessage.SpSummoning:
            case GameMessage.Chaining:
                return true;
            case GameMessage.Hint:
                int type = r.ReadChar();
                if (type == 8)
                {
                    return true;
                }
                if (type == 10)
                {
                    return true;
                }
                return false;
            case GameMessage.CardHint:
                r.ReadGPS();
                int ctype = r.ReadByte();
                if (ctype == 1)
                {
                    return true;
                }
                return false;
            case GameMessage.SelectBattleCmd:
            case GameMessage.SelectIdleCmd:
            case GameMessage.SelectEffectYn:
            case GameMessage.SelectYesNo:
            case GameMessage.SelectOption:
            case GameMessage.SelectCard:
            case GameMessage.SelectPosition:
            case GameMessage.SelectTribute:
            case GameMessage.SortChain:
            case GameMessage.SelectCounter:
            case GameMessage.SelectSum:
            case GameMessage.SortCard:
            case GameMessage.AnnounceRace:
            case GameMessage.AnnounceAttrib:
            case GameMessage.AnnounceCard:
            case GameMessage.AnnounceNumber:
            case GameMessage.AnnounceCardFilter:
            case GameMessage.SelectDisfield:
            case GameMessage.SelectPlace:
                if (inIgnoranceReplay() || currentMessageIndex + 1 < theWorldIndex)
                {
                    return false;
                }
                return true;
            case GameMessage.SelectChain:
                if (inIgnoranceReplay() || currentMessageIndex + 1 < theWorldIndex)
                {
                    return false;
                }
                r.ReadChar();
                int count = r.ReadByte();
                int spcount = r.ReadByte();
                int forced = r.ReadByte();
                int hint0 = r.ReadInt32();
                int hint1 = r.ReadInt32();
                bool ignore = false;
                if (forced == 0)    
                {
                    var condition = gameInfo.get_condition();
                    if (condition == gameInfo.chainCondition.no)
                    {
                        ignore = true;
                    }
                    else
                    {
                        if (condition == gameInfo.chainCondition.all)
                        {
                            ignore = false;
                        }
                        else
                        {
                            if (condition == gameInfo.chainCondition.smart)
                            {
                                if (count == 0)
                                {
                                    ignore = true;
                                }
                                else
                                {
                                    ignore = false;
                                }
                            }
                            else
                            {
                                if (spcount == 0)
                                {
                                    ignore = true;
                                }
                                else
                                {
                                    ignore = false;
                                }
                            }
                        }
                    }
                }
                if (ignore)      
                {
                    return false;
                }
                return true;
            case GameMessage.Attack:
                return true;
                //case GameMessage.Attack:
                //    if (Program.I().setting.setting.Vbattle.value)
                //    {
                //        return true;
                //    }
                //    else
                //    {
                //        return false;
                //    }
                //case GameMessage.Battle:
                //    if (Program.I().setting.setting.Vbattle.value)
                //    {
                //        return false;
                //    }
                //    else
                //    {
                //        return true;
                //    }
        }
        return false;
    }

    public void forceMSquit()
    {
        Package p = new Package();
        p.Fuction = (int)YGOSharp.OCGWrapper.Enums.GameMessage.sibyl_quit;
        Packages.Add(p);
    }

    //handle messages
    enum autoForceChainHandlerType
    {
        autoHandleAll,manDoAll,afterClickManDo
    }
    autoForceChainHandlerType autoForceChainHandler = autoForceChainHandlerType.manDoAll;
    bool deckReserved = false;
    public int turns = 0;
    public List<string> confirmedCards = new List<string>();
    void logicalizeMessage(Package p)
    {
        currentMessageIndex++;
        BinaryReader r = p.Data.reader;
        r.BaseStream.Seek(0, 0);
        int code = 0;
        int count = 0;
        int controller = 0;
        int location = 0;
        int sequence = 0;
        int player = 0;
        int data = 0;
        int type = 0;
        GPS gps;
        gameCard game_card;
        GPS from;
        GPS to;
        gameCard card;
        int val;
        string name;
        surrended = false;
        switch ((GameMessage)p.Fuction)
        {
            case GameMessage.sibyl_chat:
                printDuelLog(r.ReadALLUnicode());
                break;
            case GameMessage.sibyl_name:
                name_0 = r.ReadUnicode(50);
                name_0_tag = r.ReadUnicode(50);
                name_0_c = r.ReadUnicode(50);
                name_1 = r.ReadUnicode(50);
                name_1_tag = r.ReadUnicode(50);
                name_1_c = r.ReadUnicode(50);
                if (r.BaseStream.Position < r.BaseStream.Length)
                {
                    MasterRule = r.ReadInt32();
                }
                else
                {
                    MasterRule = 3;
                }
                break;
            case GameMessage.AiName:
                int length = r.ReadUInt16();
                byte[] buffer = r.ReadBytes(length + 1);
                string n = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                name_1 = n;
                name_1_tag = n;
                name_1_c = n;
                break;
            case GameMessage.Win:
                deckReserved = false;
                player = localPlayer(r.ReadByte());
                int winType = r.ReadByte();
                keys.Insert(0, currentMessageIndex);
                if (player == 2)
                {
                    result = duelResult.draw;
                    printDuelLog(InterString.Get("游戏平局！"));
                }
                else if (player == 0 || winType == 4)
                {
                    result = duelResult.win;
                    if (cookie_matchKill > 0)
                    {
                        winReason = YGOSharp.CardsManager.Get(cookie_matchKill).Name;
                        printDuelLog(InterString.Get("比赛胜利，卡片：[?]", winReason));
                    }
                    else
                    {
                        winReason = GameStringManager.get("victory", winType);
                        printDuelLog(InterString.Get("游戏胜利，原因：[?]", winReason));
                    }
                }
                else
                {
                    result = duelResult.lose;
                    if (cookie_matchKill > 0)
                    {
                        winReason = YGOSharp.CardsManager.Get(cookie_matchKill).Name;
                        printDuelLog(InterString.Get("比赛败北，卡片：[?]", winReason));
                    }
                    else
                    {
                        winReason = GameStringManager.get("victory", winType);
                        printDuelLog(InterString.Get("游戏败北，原因：[?]", winReason));
                    }
                }
                break;
            case GameMessage.Start:
                confirmedCards.Clear();
                gameField.currentPhase = GameField.ph.dp;
                result = duelResult.disLink;
                logicalClearChain();
                surrended = false;
                Program.I().room.duelEnded = false;
                turns = 0;
                deckReserved = false;
                keys.Insert(0, currentMessageIndex);
                RMSshow_clear();
                md5Maker = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    cards[i].p.location = (UInt32)game_location.LOCATION_UNKNOWN;
                }
                int playertype = r.ReadByte();
                isFirst = ((playertype & 0xf) > 0) ? false : true;
                gameInfo.swaped = false;
                isObserver = ((playertype & 0xf0) > 0) ? true : false;
                life_0 = r.ReadInt32();
                life_1 = r.ReadInt32();
                lpLimit = life_0;
                name_0_c = name_0;
                name_1_c = name_1;
                if (Program.I().room.mode == 2)
                {
                    if (isFirst)
                    {
                        name_1_c = name_1_tag;
                    }
                    else
                    {
                        name_0_c = name_0_tag;
                    }
                }
                cookie_matchKill = 0;
                MHS_creatBundle(r.ReadInt16(), localPlayer(0), game_location.LOCATION_DECK);
                MHS_creatBundle(r.ReadInt16(), localPlayer(0), game_location.LOCATION_EXTRA);
                MHS_creatBundle(r.ReadInt16(), localPlayer(1), game_location.LOCATION_DECK);
                MHS_creatBundle(r.ReadInt16(), localPlayer(1), game_location.LOCATION_EXTRA);
                gameField.clearDisabled();
                if (Program.I().room.mode == 0)
                {
                    printDuelLog(InterString.Get("单局模式 决斗开始！"));
                }
                if (Program.I().room.mode == 1)
                {
                    printDuelLog(InterString.Get("比赛模式 决斗开始！"));
                }
                if (Program.I().room.mode == 2)
                {
                    printDuelLog(InterString.Get("双打模式 决斗开始！"));
                }
                printDuelLog(InterString.Get("双方生命值：[?]", lpLimit.ToString()));
                printDuelLog(InterString.Get("Tip：鼠标中键/[FF0000]TAB键[-]可以打开/关闭哦。"));
                printDuelLog(InterString.Get("Tip：强烈建议使用[FF0000]TAB键[-]。"));
                arrangeCards();
                Sleep(21);
                break;
            case GameMessage.ReloadField:
                MasterRule = r.ReadByte() + 1;
                if (MasterRule > 255)
                {
                    MasterRule -= 255;
                }
                confirmedCards.Clear();
                gameField.currentPhase = GameField.ph.dp;
                result = duelResult.disLink;
                deckReserved = false;
                isFirst = true;
                gameInfo.swaped = false;
                logicalClearChain();
                surrended = false;
                Program.I().room.duelEnded = false;
                turns = 0;
                keys.Insert(0, currentMessageIndex);
                RMSshow_clear();
                md5Maker = 0;
                for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                    {
                        cards[i].p.location = (UInt32)game_location.LOCATION_UNKNOWN;
                    }
                cookie_matchKill = 0;
                
                if (Program.I().room.mode == 0)
                {
                    printDuelLog(InterString.Get("单局模式 决斗开始！"));
                }
                if (Program.I().room.mode == 1)
                {
                    printDuelLog(InterString.Get("比赛模式 决斗开始！"));
                }
                if (Program.I().room.mode == 2)
                {
                    printDuelLog(InterString.Get("双打模式 决斗开始！"));
                }
                printDuelLog(InterString.Get("双方生命值：[?]", lpLimit.ToString()));
                printDuelLog(InterString.Get("Tip：鼠标中键/[FF0000]TAB键[-]可以打开/关闭哦。"));
                printDuelLog(InterString.Get("Tip：强烈建议使用[FF0000]TAB键[-]。"));
                for (int p_ = 0; p_ < 2; p_++)
                {
                    player = localPlayer(p_);
                    if (player == 0)
                    {
                        life_0 = r.ReadInt32();
                    }
                    else
                    {
                        life_1 = r.ReadInt32();
                    }
                    for (int i = 0; i < 7; i++)
                    {
                        val = r.ReadByte();
                        if (val > 0)
                        {
                            gps = new GPS
                            {
                                controller = (UInt32)player,
                                location = (UInt32)game_location.LOCATION_MZONE,
                                position = (int)r.ReadByte(),
                                sequence = (UInt32)i,
                            };
                            GCS_cardCreate(gps);
                            val = r.ReadByte();
                            for (int xyz = 0; xyz < val; ++xyz)
                            {
                                gps.location |= (UInt32)game_location.LOCATION_OVERLAY;
                                gps.position = xyz;
                                GCS_cardCreate(gps);
                            }
                        }
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        val = r.ReadByte();
                        if (val > 0)
                        {
                            gps = new GPS
                            {
                                controller = (UInt32)player,
                                location = (UInt32)game_location.LOCATION_SZONE,
                                position = (int)r.ReadByte(),
                                sequence = (UInt32)i,
                            };
                            GCS_cardCreate(gps);
                        }
                    }
                    val = r.ReadByte();
                    for (int i = 0; i < val; i++)
                    {
                        gps = new GPS
                        {
                            controller = (UInt32)player,
                            location = (UInt32)game_location.LOCATION_DECK,
                            position = (int)game_position.POS_FACEDOWN_ATTACK,
                            sequence = (UInt32)i,
                        };
                        GCS_cardCreate(gps);
                    }
                    val = r.ReadByte();
                    for (int i = 0; i < val; i++)
                    {
                        gps = new GPS
                        {
                            controller = (UInt32)player,
                            location = (UInt32)game_location.LOCATION_HAND,
                            position = (int)game_position.POS_FACEDOWN_ATTACK,
                            sequence = (UInt32)i,
                        };
                        GCS_cardCreate(gps);
                    }
                    val = r.ReadByte();
                    for (int i = 0; i < val; i++)
                    {
                        gps = new GPS
                        {
                            controller = (UInt32)player,
                            location = (UInt32)game_location.LOCATION_GRAVE,
                            position = (int)game_position.POS_FACEUP_ATTACK,
                            sequence = (UInt32)i,
                        };
                        GCS_cardCreate(gps);
                    }
                    val = r.ReadByte();
                    for (int i = 0; i < val; i++)
                    {
                        gps = new GPS
                        {
                            controller = (UInt32)player,
                            location = (UInt32)game_location.LOCATION_REMOVED,
                            position = (int)game_position.POS_FACEUP_ATTACK,
                            sequence = (UInt32)i,
                        };
                        GCS_cardCreate(gps);
                    }
                    val = r.ReadByte();
                    int val_up = r.ReadByte();
                    for (int i = 0; i < val - val_up; i++)
                    {
                        gps = new GPS
                        {
                            controller = (UInt32)player,
                            location = (UInt32)game_location.LOCATION_EXTRA,
                            position = (int)game_position.POS_FACEDOWN_ATTACK,
                            sequence = (UInt32)i,
                        };
                        GCS_cardCreate(gps);
                    }
                    for (int i = 0; i < val_up; i++)
                    {
                        gps = new GPS
                        {
                            controller = (UInt32)player,
                            location = (UInt32)game_location.LOCATION_EXTRA,
                            position = (int)game_position.POS_FACEUP_ATTACK,
                            sequence = (UInt32)(val + i),
                        };
                        GCS_cardCreate(gps);
                    }
                }
                gameField.clearDisabled();
                arrangeCards();
                break;
            case GameMessage.UpdateData:
                controller = localPlayer(r.ReadChar());
                location = r.ReadChar();
                try
                {
                    while (true)
                    {
                        int len = r.ReadInt32();
                        if (len == 4) continue;
                        long pos = r.BaseStream.Position;
                        r.readCardData();
                        r.BaseStream.Position = pos + len - 4;
                    }
                }
                catch (System.Exception e)
                {
                   // UnityEngine.Debug.Log(e);
                }
                break;
            case GameMessage.UpdateCard:
                gps = r.ReadShortGPS();
                gameCard cardToRefresh = GCS_cardGet(gps, false);
                r.ReadUInt32();
                r.readCardData(cardToRefresh);
                break;
            case GameMessage.ReverseDeck:
                deckReserved = !deckReserved;
                break;
            case GameMessage.Move:
                keys.Insert(0, currentMessageIndex);
                code = r.ReadInt32();
                from = r.ReadGPS();
                to = r.ReadGPS();
                card = GCS_cardGet(from, false);
                if (card != null)
                {
                    card.set_code(code);
                }
                GCS_cardMove(from, to);
                break;
            case GameMessage.PosChange:
                keys.Insert(0, currentMessageIndex);
                ES_hint = GameStringManager.get_unsafe(1600);
                code = r.ReadInt32();
                from = r.ReadGPS();
                to = from;
                to.position = r.ReadByte();
                card = GCS_cardGet(from, false);
                if (card != null)
                {
                    card.set_code(code);
                }
                GCS_cardMove(from, to);
                break;
            case GameMessage.Set:
                ES_hint = GameStringManager.get_unsafe(1601);
                break;
            case GameMessage.Swap:
                keys.Insert(0, currentMessageIndex);
                ES_hint = GameStringManager.get_unsafe(1602);
                code = r.ReadInt32();
                from = r.ReadGPS();
                code = r.ReadInt32();
                to = r.ReadGPS();
                GCS_cardMove(from, to, true, true);
                break;
            case GameMessage.FlipSummoned:
                ES_hint = GameStringManager.get_unsafe(1608);
                break;
            case GameMessage.Summoned:
                ES_hint = GameStringManager.get_unsafe(1604);
                break;
            case GameMessage.SpSummoned:
                ES_hint = GameStringManager.get_unsafe(1606);
                break;
            case GameMessage.Chaining:
                code = r.ReadInt32();
                gps = r.ReadGPS();
                card = GCS_cardGet(gps, false);
                if (card != null)
                {
                    card.set_code(code);
                    cardsInChain.Add(card);
                    if (cardsInChain.Count == 1)
                    {
                        cardsInChain[0].CS_showBall();
                    }
                    else
                    {
                        cardsInChain[0].CS_ballToNumber();
                        cardsInChain[cardsInChain.Count - 1].CS_addChainNumber(cardsInChain.Count);
                    }
                    ES_hint = InterString.Get("「[?]」被发动时", card.get_data().Name);
                    if (card.p.controller == 0)
                    {
                        ///printDuelLog("●" + InterString.Get("[?]被发动", UIHelper.getGPSstringName(card)));
                    }
                    else
                    {
                       // printDuelLog("●" + InterString.Get("[?]被对方发动", UIHelper.getGPSstringName(card)));
                    }
                }
                break;
            case GameMessage.ChainSolved:
                int id = r.ReadByte() - 1;
                if (id < 0)
                {
                    id = 0;
                }
                if (id < cardsInChain.Count)
                {
                    card = cardsInChain[id];
                    card.CS_hideBall();
                    card.CS_removeOneChainNumber();
                }
                break;
            case GameMessage.ChainEnd:
                logicalClearChain();
                break;
            case GameMessage.ChainNegated:
            case GameMessage.ChainDisabled:
                int id_ = r.ReadByte() - 1;
                if (id_ < 0)
                {
                    id_ = 0;
                }
                if (id_ < cardsInChain.Count)
                {
                    card = cardsInChain[id_];
                    card.CS_hideBall();
                    card.CS_removeOneChainNumber();
                }
                break;
            case GameMessage.Damage:
                ES_hint = InterString.Get("玩家受到伤害时");
                player = localPlayer(r.ReadByte());
                player = unSwapPlayer(player);
                val = r.ReadInt32();
                if (player == 0)
                {
                    //printDuelLog(InterString.Get("受到伤害[?]", val.ToString()));
                }
                else
                {
                    //printDuelLog(InterString.Get("对方受到伤害[?]", val.ToString()));
                }
                if (player == 0)
                {
                    life_0 -= val;
                }
                else
                {
                    life_1 -= val;
                }
                break;
            case GameMessage.PayLpCost:
                player = localPlayer(r.ReadByte());
                player = unSwapPlayer(player);
                val = r.ReadInt32();
                if (player == 0)
                {
                    //printDuelLog(InterString.Get("支付生命值[?]", val.ToString()));
                }
                else
                {
                    //printDuelLog(InterString.Get("对方支付生命值[?]", val.ToString()));
                }
                if (player == 0)
                {
                    life_0 -= val;
                }
                else
                {
                    life_1 -= val;
                }
                break;
            case GameMessage.Recover:
                ES_hint = InterString.Get("玩家生命值回复时");
                player = localPlayer(r.ReadByte());
                player = unSwapPlayer(player);
                val = r.ReadInt32();
                if (player == 0)
                {
                    //printDuelLog(InterString.Get("回复生命值[?]", val.ToString()));
                }
                else
                {
                    //printDuelLog(InterString.Get("对方回复生命值[?]", val.ToString()));
                }
                if (player == 0)
                {
                    life_0 += val;
                }
                else
                {
                    life_1 += val;
                }
                break;
            case GameMessage.LpUpdate:
                player = localPlayer(r.ReadByte());
                player = unSwapPlayer(player);
                val = r.ReadInt32();
                if (player == 0)
                {
                    //printDuelLog(InterString.Get("刷新生命值[?]", val.ToString()));
                }
                else
                {
                    //printDuelLog(InterString.Get("对方刷新生命值[?]", val.ToString()));
                }
                if (player == 0)
                {
                    life_0 = val;
                }
                else
                {
                    life_1 = val;
                }
                break;
            case GameMessage.RandomSelected:
                player = localPlayer(r.ReadByte());
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    gps = r.ReadGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        printDuelLog(InterString.Get("对象选择：[?]", UIHelper.getGPSstringName(card)));
                    }
                }
                break;
            case GameMessage.BecomeTarget:
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    gps = r.ReadGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        printDuelLog(InterString.Get("对象选择：[?]", UIHelper.getGPSstringName(card)));
                    }
                }
                break;
            case GameMessage.TossCoin:
                player = r.ReadByte();
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    data = r.ReadByte();
                    if (data == 0)
                    {
                        printDuelLog(InterString.Get("硬币反面"));
                    }
                    else
                    {
                        printDuelLog(InterString.Get("硬币正面"));
                    }
                }
                break;
            case GameMessage.TossDice:
                player = r.ReadByte();
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    data = r.ReadByte();
                    printDuelLog(InterString.Get("骰子结果：[?]", data.ToString()));
                }
                break;
            case GameMessage.HandResult:
                data = r.ReadByte();
                int data1 = data & 0x3;
                int data2 = (data >> 2) & 0x3;
                string res1 = (data1 == 1 ? "剪刀" : (data1 == 2 ? "布" : "石头"));
                string res2 = (data2 == 1 ? "剪刀" : (data2 == 2 ? "布" : "石头"));
                if (isFirst)
                    printDuelLog("猜拳结果：你好像出了" + res2 + data2.ToString() + "，对方好像出了" + res1 + data1.ToString());
                else
                    printDuelLog("猜拳结果：你好像出了" + data1.ToString() + res1 + "，对方好像出了" + res2 + data2.ToString());
                break;
            case GameMessage.Attack:
                game_card = GCS_cardGet(r.ReadGPS(), false);
                string derectattack = "";
                if (game_card != null)
                {
                    name = game_card.get_data().Name;
                    ES_hint = InterString.Get("「[?]」攻击时", game_card.get_data().Name);
                    //printDuelLog("●" + InterString.Get("[?]发动攻击！", UIHelper.getGPSstringLocation(game_card.p) + UIHelper.getGPSstringName(game_card)));
                    if (game_card.p.controller == 0)
                    {
                        derectattack = "●" + InterString.Get("对方被直接攻击！");
                    }
                    else
                    {
                        derectattack = "●" + InterString.Get("被直接攻击！");
                    }
                }
                game_card = GCS_cardGet(r.ReadGPS(), false);
                if (game_card != null)
                {
                    name = game_card.get_data().Name;
                    //printDuelLog("●" + InterString.Get("[?]被攻击！", UIHelper.getGPSstringLocation(game_card.p) + UIHelper.getGPSstringName(game_card)));
                }
                else
                {
                    //printDuelLog(derectattack);
                }
                break;
            case GameMessage.AttackDiabled:
                ES_hint = InterString.Get("攻击被无效时");
                //printDuelLog(InterString.Get("攻击被无效"));
                break;
            case GameMessage.Battle:
                break;
            case GameMessage.FlipSummoning:
                code = r.ReadInt32();
                name = YGOSharp.CardsManager.Get(code).Name;
                card = GCS_cardGet(r.ReadShortGPS(), false);
                if (card != null)
                {
                    card.set_code(code);
                    card.p.position = (int)game_position.POS_FACEUP_ATTACK;
                    card.refreshData();
                    ES_hint = InterString.Get("「[?]」反转召唤宣言时", card.get_data().Name);
                    if (card.p.controller == 0)
                    {
                        //printDuelLog("●" + InterString.Get("[?]被反转召唤", UIHelper.getGPSstringName(card)));
                    }
                    else
                    {
                        //printDuelLog("●" + InterString.Get("[?]被对方反转召唤", UIHelper.getGPSstringName(card)));
                    }
                }
                break;
            case GameMessage.Summoning:
                code = r.ReadInt32();
                name = YGOSharp.CardsManager.Get(code).Name;
                card = GCS_cardGet(r.ReadShortGPS(), false);
                if (card != null)
                {
                    card.set_code(code);
                    ES_hint = InterString.Get("「[?]」通常召唤宣言时", card.get_data().Name);

                    if (card.p.controller == 0)
                    {
                        //printDuelLog("●" + InterString.Get("[?]被通常召唤", UIHelper.getGPSstringName(card)));
                    }
                    else
                    {
                        //printDuelLog("●" + InterString.Get("[?]被对方通常召唤", UIHelper.getGPSstringName(card)));
                    }
                }
                break;
            case GameMessage.SpSummoning:
                code = r.ReadInt32();
                name = YGOSharp.CardsManager.Get(code).Name;
                card = GCS_cardGet(r.ReadShortGPS(), false);
                if (card != null)
                {
                    card.set_code(code);
                    card.add_string_tail(GameStringHelper.teshuzhaohuan);
                    ES_hint = InterString.Get("「[?]」特殊召唤宣言时", card.get_data().Name);

                    if (card.p.controller == 0)
                    {
                        //printDuelLog("●" + InterString.Get("[?]被特殊召唤", UIHelper.getGPSstringName(card)));
                    }
                    else
                    {
                        //printDuelLog("●" + InterString.Get("[?]被对方特殊召唤", UIHelper.getGPSstringName(card)));
                    }
                }
                break;
            case GameMessage.Draw:
                keys.Insert(0, currentMessageIndex);
                ES_hint = InterString.Get("玩家抽卡时");
                controller = localPlayer(r.ReadByte());
                count = r.ReadByte();
                int deckCC = MHS_getBundle(controller, (int)game_location.LOCATION_DECK).Count;
                for (int isa = 0; isa < count; isa++)
                {
                    card = GCS_cardMove(
                        new GPS
                        {
                            controller = (UInt32)controller,
                            location = (UInt32)game_location.LOCATION_DECK,
                            sequence = (UInt32)(deckCC - 1 - isa),
                            position = (int)game_position.POS_FACEDOWN_ATTACK,
                        }
                    ,
                    new GPS
                    {
                        controller = (UInt32)controller,
                        location = (UInt32)game_location.LOCATION_HAND,
                        sequence = (UInt32)(1000),
                        position = (int)game_position.POS_FACEDOWN_ATTACK,
                    }
                    , false);
                    card.set_code(r.ReadInt32() & 0x7fffffff);
                    if (controller == 0)
                    {
                        //printDuelLog(InterString.Get("抽卡[?]", UIHelper.getGPSstringName(card)));
                    }
                    else
                    {
                        //printDuelLog(InterString.Get("对方抽卡[?]", UIHelper.getGPSstringName(card)));
                    }
                }
                break;
            case GameMessage.TagSwap:
                keys.Insert(0, currentMessageIndex);
                controller = localPlayer(r.ReadByte());
                if (controller == 0)
                {
                    if (name_0_c == name_0)
                    {
                        name_0_c = name_0_tag;
                    }
                    else
                    {
                        name_0_c = name_0;
                    }
                }
                else
                {
                    if (name_1_c == name_1)
                    {
                        name_1_c = name_1_tag;
                    }
                    else
                    {
                        name_1_c = name_1;
                    }
                }
                int mcount = r.ReadByte();
                var cardsInDeck = MHS_resizeBundle(mcount, controller, game_location.LOCATION_DECK);
                int ecount = r.ReadByte();
                var cardsInExtra = MHS_resizeBundle(ecount, controller, game_location.LOCATION_EXTRA);
                int pcount = r.ReadByte();
                int hcount = r.ReadByte();
                var cardsInHand = MHS_resizeBundle(hcount, controller, game_location.LOCATION_HAND);
                if (cardsInDeck.Count > 0)
                {
                    cardsInDeck[cardsInDeck.Count - 1].set_code(r.ReadInt32());
                }
                for (int i = 0; i < cardsInHand.Count; i++)
                {
                    cardsInHand[i].set_code(r.ReadInt32());
                }
                for (int i = 0; i < cardsInExtra.Count; i++)
                {
                    cardsInExtra[i].set_code(r.ReadInt32() & 0x7fffffff);
                }
                for (int i = 0; i < pcount; i++)
                {
                    if (cardsInExtra.Count - 1 - i > 0)
                    {
                        cardsInExtra[cardsInExtra.Count - 1 - i].p.position = (int)game_position.POS_FACEUP_ATTACK;
                    }
                }
                if (controller == 0)
                {
                    //printDuelLog(InterString.Get("切换玩家，手牌张数变为[?]", hcount.ToString()));
                }
                else
                {
                    //printDuelLog(InterString.Get("对方切换玩家，手牌张数变为[?]", hcount.ToString()));
                }
                //Program.DEBUGLOG("TAG SWAP->controller:" + controller + "mcount:" + mcount + "ecount:" + ecount + "pcount:" + pcount + "hcount:" + hcount);
                break;
            case GameMessage.MatchKill:
                cookie_matchKill = r.ReadInt32();
                break;
            case GameMessage.PlayerHint:
                controller = localPlayer(r.ReadByte());
                int ptype = r.ReadByte();
                int pvalue = r.ReadInt32();
                string valstring = GameStringManager.get(pvalue);
                if (ptype == 6)
                {
                    if (controller==0)  
                    {
                        printDuelLog(InterString.Get("我方状态：[?]", valstring));
                    }
                    else
                    {
                        printDuelLog(InterString.Get("对方状态：[?]", valstring));
                    }
                }
                else if (ptype == 7)
                {
                    if (controller == 0)
                    {
                        printDuelLog(InterString.Get("我方取消状态：[?]", valstring));
                    }
                    else
                    {
                        printDuelLog(InterString.Get("对方取消状态：[?]", valstring));
                    }
                }
                break;
            case GameMessage.CardHint:
                game_card = GCS_cardGet(r.ReadGPS(), false);
                int ctype = r.ReadByte();
                int value = r.ReadInt32();
                if (game_card != null)
                {
                    if (ctype == 1)
                    {
                        game_card.del_one_tail(InterString.Get("数字记录："));
                        game_card.add_string_tail(InterString.Get("数字记录：") + value.ToString());
                    }
                    if (ctype == 2)
                    {
                        game_card.del_one_tail(InterString.Get("卡片记录："));
                        game_card.add_string_tail(InterString.Get("卡片记录：") + UIHelper.getSuperName(YGOSharp.CardsManager.Get(value).Name, value));
                    }
                    if (ctype == 3)
                    {
                        game_card.del_one_tail(InterString.Get("种族记录："));
                        game_card.add_string_tail(InterString.Get("种族记录：") + GameStringHelper.race(value));
                    }
                    if (ctype == 4)
                    {
                        game_card.del_one_tail(InterString.Get("属性记录："));
                        game_card.add_string_tail(InterString.Get("属性记录：") + GameStringHelper.attribute(value));
                    }
                    if (ctype == 5)
                    {
                        game_card.del_one_tail(InterString.Get("数字记录："));
                        game_card.add_string_tail(InterString.Get("数字记录：") + value.ToString());
                    }
                    if (ctype == 6)
                    {
                        game_card.add_string_tail(GameStringManager.get(value));
                    }
                    if (ctype == 7)
                    {
                        game_card.del_one_tail(GameStringManager.get(value));
                    }
                }
                break;
           case GameMessage.Hint:
                Es_selectMSGHintType = r.ReadChar();
                Es_selectMSGHintPlayer = localPlayer(r.ReadChar());
                Es_selectMSGHintData = r.ReadInt32();
                type = Es_selectMSGHintType;
                player = Es_selectMSGHintPlayer;
                data = Es_selectMSGHintData;
                if (type == 1)
                {
                    ES_hint = GameStringManager.get(data);
                }
                if (type == 2)
                {
                    printDuelLog(GameStringManager.get(data));
                }
                if (type == 3)
                {
                    ES_selectHint = GameStringManager.get(data);
                }
                if (type == 4)
                {
                    printDuelLog(InterString.Get("效果选择：[?]", GameStringManager.get(data)));
                }
                if (type == 5)
                {
                    printDuelLog(GameStringManager.get(data));
                }
                if (type == 6)
                {
                    printDuelLog(InterString.Get("种族选择：[?]", GameStringHelper.race(data)));
                }
                if (type == 7)
                {
                    printDuelLog(InterString.Get("属性选择：[?]", GameStringHelper.attribute(data)));
                }
                if (type == 8)
                {
                    printDuelLog(InterString.Get("卡片展示：[?]", UIHelper.getSuperName(YGOSharp.CardsManager.Get(data).Name, data)));
                }
                if (type == 9)
                {
                    printDuelLog(InterString.Get("数字选择：[?]", data.ToString()));
                }
                if (type == 10)
                {
                    printDuelLog(InterString.Get("卡片展示：[?]", UIHelper.getSuperName(YGOSharp.CardsManager.Get(data).Name, data)));
                }
                break;
            case GameMessage.MissedEffect:
                r.ReadInt32();
                code = r.ReadInt32();
                printDuelLog(InterString.Get("「[?]」失去了时点。", UIHelper.getSuperName(YGOSharp.CardsManager.Get(code).Name, code)));
                break;
            case GameMessage.NewTurn:
                toDefaultHintLogical();
                gameField.currentPhase = GameField.ph.dp;
                //  keys.Insert(0, currentMessageIndex);
                player = localPlayer(r.ReadByte());
                if (player == 0)
                {
                    ES_turnString = InterString.Get("我方的");
                }
                else
                {
                    ES_turnString = InterString.Get("对方的");
                }
                turns++;
                ES_phaseString = InterString.Get("回合");
                //printDuelLog(InterString.Get("进入[?]", ES_turnString + ES_phaseString)+"  "+ InterString.Get("回合计数[?]", turns.ToString()));
                ES_hint = ES_turnString + ES_phaseString;
                break;
            case GameMessage.NewPhase:
                toDefaultHintLogical();
                autoForceChainHandler =  autoForceChainHandlerType.manDoAll;
               // keys.Insert(0, currentMessageIndex);
                ushort ph = r.ReadUInt16();
                if (ph == 0x01)
                {
                    ES_phaseString = InterString.Get("抽卡阶段");
                    gameField.currentPhase = GameField.ph.dp;
                }
                if (ph == 0x02)
                {
                    ES_phaseString = InterString.Get("准备阶段");
                    gameField.currentPhase = GameField.ph.sp;
                }
                if (ph == 0x04)
                {
                    ES_phaseString = InterString.Get("主要阶段1");
                    gameField.currentPhase = GameField.ph.mp1;
                }
                if (ph == 0x08)
                {
                    ES_phaseString = InterString.Get("战斗阶段");
                    gameField.currentPhase = GameField.ph.bp;
                }
                if (ph == 0x10)
                {
                    ES_phaseString = InterString.Get("战斗步骤");
                    gameField.currentPhase = GameField.ph.bp;
                }
                if (ph == 0x20)
                {
                    ES_phaseString = InterString.Get("伤害步骤");
                    gameField.currentPhase = GameField.ph.bp;
                }
                if (ph == 0x40)
                {
                    ES_phaseString = InterString.Get("伤害判定时");
                    gameField.currentPhase = GameField.ph.bp;
                }
                if (ph == 0x80)
                {
                    ES_phaseString = InterString.Get("战斗阶段");
                    gameField.currentPhase = GameField.ph.bp;
                }
                if (ph == 0x100)
                {
                    ES_phaseString = InterString.Get("主要阶段2");
                    gameField.currentPhase = GameField.ph.mp2;
                }
                if (ph == 0x200)
                {
                    ES_phaseString = InterString.Get("结束阶段");
                    gameField.currentPhase = GameField.ph.ep;
                }
                //printDuelLog(InterString.Get("进入[?]", ES_turnString + ES_phaseString));
                ES_hint = ES_turnString + ES_phaseString;
                break;
            case GameMessage.ConfirmDecktop:
                player = localPlayer(r.ReadByte());
                count = r.ReadByte();
                int countOfDeck = countLocation(player, game_location.LOCATION_DECK);
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(new GPS
                    {
                        controller = (UInt32)player,
                        location = (UInt32)game_location.LOCATION_DECK,
                        sequence = (UInt32)(countOfDeck - 1 - i),
                    }, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        printDuelLog(InterString.Get("[ff0000]确认卡片：[?][-]", UIHelper.getGPSstringName(card, true)));
                        confirmedCards.Add("「" + UIHelper.getSuperName(card.get_data().Name, card.get_data().Id) + "」");
                        if (confirmedCards.Count>=6)    
                        {
                            confirmedCards.RemoveAt(0);
                        }
                    }
                }
                break;
            case GameMessage.ConfirmCards:
                player = localPlayer(r.ReadByte());
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        printDuelLog(InterString.Get("[ff0000]确认卡片：[?][-]", UIHelper.getGPSstringName(card, true)));
                        confirmedCards.Add("「" + UIHelper.getSuperName(card.get_data().Name, card.get_data().Id) + "」");
                        if (confirmedCards.Count >= 6)
                        {
                            confirmedCards.RemoveAt(0);
                        }
                    }
                }
                break;
            case GameMessage.DeckTop:
                player = localPlayer(r.ReadByte());
                int countOfDeck_ = countLocation(player, game_location.LOCATION_DECK);
                gps = new GPS
                {
                    controller = (UInt32)player,
                    location = (UInt32)game_location.LOCATION_DECK,
                    sequence = (UInt32)(countOfDeck_ - 1 - r.ReadByte()),
                };
                code = r.ReadInt32();
                card = GCS_cardGet(gps, false);
                if (card != null)
                {
                    card.set_code(code);
                    printDuelLog(InterString.Get("确认卡片：[?]", UIHelper.getGPSstringName(card)));
                }
                break;
            case GameMessage.RefreshDeck:
            case GameMessage.ShuffleDeck:
                player = localPlayer(r.ReadByte());
                if (player == 0)
                {
                    //printDuelLog(InterString.Get("洗牌"));
                }
                else
                {
                    //printDuelLog(InterString.Get("对方洗牌"));
                }
                for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                    {
                        if ((cards[i].p.location & (UInt32)game_location.LOCATION_DECK) > 0)
                        {
                            if (cards[i].p.controller == player)
                            {
                                cards[i].erase_data();
                            }
                        }
                    }
                break;
            case GameMessage.ShuffleHand:
                player = localPlayer(r.ReadByte());
                for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                    {
                        if ((cards[i].p.location & (UInt32)game_location.LOCATION_HAND) > 0)
                        {
                            if (cards[i].p.controller == player)
                            {
                                cards[i].erase_data();
                            }
                        }
                    }
                break;
            case GameMessage.SwapGraveDeck:
                player = localPlayer(r.ReadByte());
                for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                    {
                        if (cards[i].p.controller == player)
                        {
                            if ((cards[i].p.location & (UInt32)game_location.LOCATION_DECK) > 0)
                            {
                                if (cards[i].p.controller == player)
                                {
                                    cards[i].p.location = (UInt32)game_location.LOCATION_GRAVE;
                                }
                            }
                            else if ((cards[i].p.location & (UInt32)game_location.LOCATION_GRAVE) > 0)
                            {
                                if (cards[i].p.controller == player)
                                {
                                    cards[i].p.location = (UInt32)game_location.LOCATION_DECK;
                                }
                            }
                        }
                    }
                break;
            case GameMessage.ShuffleSetCard:
                location = r.ReadByte();
                count = r.ReadByte();
                List<GPS> gpss = new List<GPS>();
                for (int i = 0; i < count; i++)
                {
                    gps = r.ReadGPS();
                    gpss.Add(gps);
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.erase_data();
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    gps = r.ReadGPS();
                    if (gps.location > 0)
                    {
                        GCS_cardMove(gpss[i], gps);
                    }
                }
                break;
            case GameMessage.FieldDisabled:
                UInt32 selectable_field = r.ReadUInt32();
                int filter = 0x1;
                for (int i = 0; i < 5; ++i, filter <<= 1)
                {
                    gps = new GPS
                    {
                        controller = (UInt32)localPlayer(0),
                        location = (UInt32)game_location.LOCATION_MZONE,
                        sequence = (UInt32)i
                    };
                    if ((selectable_field & filter) > 0)
                    {
                        gameField.set_point_disabled(gps, true);
                    }
                    else
                    {
                        gameField.set_point_disabled(gps, false);
                    }
                }
                filter = 0x100;
                for (int i = 0; i < 8; ++i, filter <<= 1)
                {
                    gps = new GPS
                    {
                        controller = (UInt32)localPlayer(0),
                        location = (UInt32)game_location.LOCATION_SZONE,
                        sequence = (UInt32)i
                    };
                    if ((selectable_field & filter) > 0)
                    {
                        gameField.set_point_disabled(gps, true);
                    }
                    else
                    {
                        gameField.set_point_disabled(gps, false);
                    }
                }
                filter = 0x10000;
                for (int i = 0; i < 5; ++i, filter <<= 1)
                {
                    gps = new GPS
                    {
                        controller = (UInt32)localPlayer(1),
                        location = (UInt32)game_location.LOCATION_MZONE,
                        sequence = (UInt32)i
                    };
                    if ((selectable_field & filter) > 0)
                    {
                        gameField.set_point_disabled(gps, true);
                    }
                    else
                    {
                        gameField.set_point_disabled(gps, false);
                    }
                }
                filter = 0x1000000;
                for (int i = 0; i < 8; ++i, filter <<= 1)
                {
                    gps = new GPS
                    {
                        controller = (UInt32)localPlayer(1),
                        location = (UInt32)game_location.LOCATION_SZONE,
                        sequence = (UInt32)i
                    };
                    if ((selectable_field & filter) > 0)
                    {
                        gameField.set_point_disabled(gps, true);
                    }
                    else
                    {
                        gameField.set_point_disabled(gps, false);
                    }
                }
                break;
            case GameMessage.CardTarget:
            case GameMessage.Equip:
                from = r.ReadGPS();
                to = r.ReadGPS();
                gameCard card_from = GCS_cardGet(from, false);
                gameCard card_to = GCS_cardGet(to, false);
                if (card_from != null)
                {
                    if ((int)GameMessage.Equip == p.Fuction)
                    {
                        card_from.target.Clear();
                    }
                    card_from.addTarget(card_to);
                }
                break;
            case GameMessage.CancelTarget:
            case GameMessage.Unequip:
                from = r.ReadGPS();
                card = GCS_cardGet(from, false);
                card.target.Clear();
                break;
            case GameMessage.AddCounter:
                type = r.ReadUInt16();
                gps = r.ReadShortGPS();
                card = GCS_cardGet(gps, false);
                count = r.ReadUInt16();
                if (card != null)
                {
                    name = GameStringManager.get("counter", type);
                    for (int i = 0; i < count; i++)
                    {
                        card.add_string_tail(name);
                    }
                }
                break;
            case GameMessage.RemoveCounter:
                type = r.ReadUInt16();
                gps = r.ReadShortGPS();
                card = GCS_cardGet(gps, false);
                count = r.ReadUInt16();
                if (card != null)
                {
                    name = GameStringManager.get("counter", type);
                    for (int i = 0; i < count; i++)
                    {
                        card.del_one_tail(name);
                    }
                }
                break;
        }
        r.BaseStream.Seek(0, 0);
    }

    private int unSwapPlayer(int player)
    {
        if (gameInfo.swaped)
        {
            return 1 - player;
        }
        else
        {
            return player;
        }
    }

    public Package getNamePacket()
    {
        Package p__ = new Package();
        p__.Fuction = (int)YGOSharp.OCGWrapper.Enums.GameMessage.sibyl_name;
        p__.Data = new BinaryMaster();
        p__.Data.writer.WriteUnicode(name_0, 50);
        p__.Data.writer.WriteUnicode(name_0_tag, 50);
        p__.Data.writer.WriteUnicode(name_0_c!=""? name_0_c: name_0, 50);
        p__.Data.writer.WriteUnicode(name_1, 50);
        p__.Data.writer.WriteUnicode(name_1_tag, 50);
        p__.Data.writer.WriteUnicode(name_1_c != "" ? name_1_c : name_1, 50);
        p__.Data.writer.Write(Program.I().ocgcore.MasterRule);
        return p__;
    }

    private static void printDuelLog(string toPrint)
    {
        Program.I().book.add(toPrint);
    }

    private int countLocation(int player, game_location location_)
    {
        int re = 0;

        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
            {
                if ((cards[i].p.location & (UInt32)location_) > 0)
                {
                    if (cards[i].p.controller == player)
                    {
                        re++;
                    }
                }
            }

        return re;
    }

    private int countLocationSequence(int player, game_location location_)  
    {
        int re = 0;

        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
            {
                if ((cards[i].p.location & (UInt32)location_) > 0)
                {
                    if (cards[i].p.controller == player)
                    {
                        if (cards[i].p.sequence > re)
                        {
                            re = (int)cards[i].p.sequence;
                        }
                    }
                }
            }

        return re;
    }

    public bool inIgnoranceReplay()
    {
        return InAI == false && condition != Condition.duel;
    }

    public void reSize()
    {
        realize(true);
    }

    static void shiftArrowHandlerF()
    {
        if (Program.I().ocgcore.Arrow != null)
        {
            Program.I().ocgcore.Arrow.gameObject.SetActive(false);
        }
    }

    static void shiftArrowHandlerT()
    {
        if (Program.I().ocgcore.Arrow != null)
        {
            Program.I().ocgcore.Arrow.gameObject.SetActive(true);
        }
    }

    void shiftArrow(Vector3 from, Vector3 to, bool on, int delay)
    {
        Program.notGo(shiftArrowHandlerT);
        Program.notGo(shiftArrowHandlerF);
        if (on)
        {
            Program.go(delay, shiftArrowHandlerT);
        }
        else
        {
            Program.go(delay, shiftArrowHandlerF);
        }
        if (on)
        {
            Arrow.from.position = from;
            Arrow.to.position = to;
        }
        else
        {
            Arrow.from.position = new Vector3(25, 0, 0);
            Arrow.to.position = new Vector3(25, 0, 5);
        }
        var collection = Arrow.GetComponentsInChildren<Transform>(true);
        foreach (var item in collection)
        {
            item.gameObject.layer = on ? 0 : 4;
        }
    }

    lazyWin winCaculator = null;

    void showCaculator()
    {
        if (winCaculator == null)
        {
            if (condition == Condition.watch)
            {
                if (paused == false)
                {
                    EventDelegate.Execute(UIHelper.getByName<UIButton>(toolBar, "stop_").onClick);
                }
            }
            UIHelper.playSound("explode", 0.4f);
            float real = (Program.fieldSize - 1) * 0.9f + 1f;
            RMSshow_clear();
            GameObject explode = create(result == duelResult.win ? Program.I().mod_winExplode : Program.I().mod_loseExplode);
            var co = explode.AddComponent<animation_screen_lock>();
            co.screen_point = Program.camera_game_main.WorldToScreenPoint(new Vector3(0, 0, -5.65f * real));
            co.screen_point.z = 2;
            explode.transform.position = Camera.main.ScreenToWorldPoint(co.screen_point);
            if (condition == Condition.record)
            {
                winCaculator = create
                (
                Program.I().New_winCaculatorRecord,
                Program.camera_main_2d.ScreenToWorldPoint(co.screen_point),
                new Vector3(0, 0, 0),
                true,
                Program.ui_main_2d,
                true,
                new Vector3(((float)Screen.height) / 700f, ((float)Screen.height) / 700f, ((float)Screen.height) / 700f)
                ).GetComponent<lazyWin>();
            }
            else
            {
                winCaculator = create
                (
                Program.I().New_winCaculator,
                Program.camera_main_2d.ScreenToWorldPoint(co.screen_point),
                new Vector3(0, 0, 0),
                true,
                Program.ui_main_2d,
                true,
                new Vector3(((float)Screen.height) / 700f, ((float)Screen.height) / 700f, ((float)Screen.height) / 700f)
                ).GetComponent<lazyWin>();
                winCaculator.input.value = UIHelper.getTimeString();
                UIHelper.registEvent(winCaculator.gameObject, "yes_", onSaveReplay);
                UIHelper.registEvent(winCaculator.gameObject, "no_", onGiveUpReplay);
            }
            switch (result)
            {
                case duelResult.disLink:
                    winCaculator.win.text = "Disconnected";
                    break;
                case duelResult.win:
                    winCaculator.win.text = "You Win";
                    break;
                case duelResult.lose:
                    winCaculator.win.text = "You Lose";
                    break;
                case duelResult.draw:
                    winCaculator.win.text = "Draw Game";
                    break;
                default:
                    winCaculator.win.text = "Disconnected";
                    break;
            }
        }
        else
        {
            switch (result)
            {
                case duelResult.win:
                    winCaculator.win.text = "You Win";
                    break;
                case duelResult.lose:
                    winCaculator.win.text = "You Lose";
                    break;
                case duelResult.draw:
                    winCaculator.win.text = "Draw Game";
                    break;
            }
        }
        winCaculator.reason.text = winReason;
    }

    void onSaveReplay()
    {
        if (winCaculator != null)
        {
            try
            {
                if (File.Exists("replay/" + TcpHelper.lastRecordName + ".yrp3d"))
                {
                    if (TcpHelper.lastRecordName != winCaculator.input.value)
                    {
                        if (File.Exists("replay/" + winCaculator.input.value + ".yrp3d"))
                        {
                            File.Delete("replay/" + winCaculator.input.value + ".yrp3d");
                        }
                    }
                    File.Move("replay/" + TcpHelper.lastRecordName + ".yrp3d", "replay/" + winCaculator.input.value + ".yrp3d");
                }
            }
            catch (Exception e)   
            {
                RMSshow_none(e.ToString());
            }
        }
        onDuelResultConfirmed();
    }

    void onGiveUpReplay()
    {
        if (winCaculator != null)
        {
            try
            {
                if (File.Exists("replay/" + TcpHelper.lastRecordName + ".yrp3d"))
                {
                    if (File.Exists("replay/" + "-lastReplay" + ".yrp3d"))
                    {
                        File.Delete("replay/" + "-lastReplay" + ".yrp3d");
                    }
                    File.Move("replay/" + TcpHelper.lastRecordName + ".yrp3d", "replay/-lastReplay.yrp3d");
                }
            }
            catch (Exception e)
            {
                RMSshow_none(e.ToString());
            }
        }
        onDuelResultConfirmed();
    }

    void hideCaculator()
    {
        if (winCaculator != null)
        {
            if (condition == Condition.watch)
            {
                if (paused == true)
                {
                    EventDelegate.Execute(UIHelper.getByName<UIButton>(toolBar, "go_").onClick);
                }
            }
            destroy(winCaculator.gameObject);
        }
    }

    void practicalizeMessage(Package p)
    {
        int player = 0;
        int count = 0;
        int code = 0;
        int min = 0;
        int max = 0;
        bool cancalable = false;
        GPS gps;
        gameCard card;
        BinaryReader r = p.Data.reader;
        r.BaseStream.Seek(0, 0);
        gameButton btn;
        string desc = "";
        UInt32 available;
        BinaryMaster binaryMaster;
        Vector3 VectorAttackCard;
        Vector3 VectorAttackTarget;
        char type;
        Int32 data;
        int val;
        int cctype;
        GameObject tempobj;
        bool psum = false;
        bool pIN = false;
        BinaryMaster bin;
        long length_of_message = r.BaseStream.Length;
        List<messageSystemValue> values;
        switch ((GameMessage)p.Fuction)
        {
            //case GameMessage.sibyl_clear:
            //    clearResponse();
            //    break;
            case GameMessage.sibyl_quit:
                Program.I().room.duelEnded = true;
                result = duelResult.disLink;
                showCaculator();
                break;
            case GameMessage.Retry:
                Debug.Log("Retry");
                break;
            //case GameMessage.sibyl_delay:
            //    if (inIgnoranceReplay())
            //    {
            //        break;
            //    }
            //    player = localPlayer(r.ReadChar());
            //    gameInfo.setTime(player, Program.I().room.time_limit);
            //    break;
            case GameMessage.sibyl_chat:
                string sss = r.ReadALLUnicode();
                RMSshow_none(sss);
                break;
            case GameMessage.ShowHint:
                int length = r.ReadUInt16();
                byte[] buffer = r.ReadToEnd();
                string n = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                RMSshow_none(n);
                break;
            case GameMessage.sibyl_name:
                gameInfo.realize();
                if (MasterRule >= 4)
                {
                    gameField.loadNewField();
                }
                else
                {
                    gameField.loadOldField();
                }
                break;
            case GameMessage.Hint:
                type = r.ReadChar();
                player = r.ReadChar();
                data = r.ReadInt32();
                if (type == 1)
                {
                    ES_hint = GameStringManager.get(data);
                }
                if (type == 2)
                {
                    RMSshow_none(GameStringManager.get(data));
                }
                if (type == 3)
                {
                    ES_selectHint = GameStringManager.get(data);
                }
                if (type == 4)
                {
                    RMSshow_none(InterString.Get("效果选择：[?]", GameStringManager.get(data)));
                }
                if (type == 5)
                {
                    RMSshow_none(GameStringManager.get(data));
                }
                if (type == 6)
                {
                    RMSshow_none(InterString.Get("种族选择：[?]", GameStringHelper.race(data)));
                }
                if (type == 7)
                {
                    RMSshow_none(InterString.Get("属性选择：[?]", GameStringHelper.attribute(data)));
                }
                if (type == 8)
                {
                    animation_show_card_code(data);
                }
                if (type == 9)
                {
                    RMSshow_none(InterString.Get("数字选择：[?]", data.ToString()));
                }
                if (type == 10)
                {
                    animation_show_card_code(data);
                }
                break;
            case GameMessage.MissedEffect:
                break;
            case GameMessage.Waiting:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                showWait();
                break;
            case GameMessage.Start:
                if (MasterRule >= 4)
                {
                    gameField.loadNewField();
                }
                else
                {
                    gameField.loadOldField();
                }
                realize(true);
                if (condition != Condition.record)
                {
                    if (isObserver)
                    {
                        if (condition != Condition.watch)
                        {
                            shiftCondition(Condition.watch);
                        }
                    }
                    else
                    {
                        if (condition != Condition.duel)
                        {
                            shiftCondition(Condition.duel);
                        }
                    }
                }
                else
                {
                    if (condition != Condition.record)
                    {
                        shiftCondition(Condition.record);
                    }
                }
                card = GCS_cardGet(new GPS
                {
                    controller = (UInt32)0,
                    location = (UInt32)game_location.LOCATION_DECK,
                    position = (int)game_position.POS_FACEDOWN_ATTACK,
                    sequence = (UInt32)0,
                }, false);
                if (card != null)
                {
                    Program.I().cardDescription.setData(card.get_data(), card.p.controller == 0 ? GameTextureManager.myBack : GameTextureManager.opBack, card.tails.managedString);
                }
                clearChainEnd();
                hideCaculator();
                break;
            case GameMessage.ReloadField:
                if (MasterRule >= 4)
                {
                    gameField.loadNewField();
                }
                else
                {
                    gameField.loadOldField();
                }
                realize(true);
                if (condition != Condition.record)
                {
                    if (isObserver)
                    {
                        if (condition != Condition.watch)
                        {
                            shiftCondition(Condition.watch);
                        }
                    }
                    else
                    {
                        if (condition != Condition.duel)
                        {
                            shiftCondition(Condition.duel);
                        }
                    }
                }
                else
                {
                    if (condition != Condition.record)
                    {
                        shiftCondition(Condition.record);
                    }
                }

                card = GCS_cardGet(new GPS
                {
                    controller = (UInt32)0,
                    location = (UInt32)game_location.LOCATION_HAND,
                    position = (int)game_position.POS_FACEDOWN_ATTACK,
                    sequence = (UInt32)0,
                }, false);
                if (card != null)
                {
                    Program.I().cardDescription.setData(card.get_data(), card.p.controller == 0 ? GameTextureManager.myBack : GameTextureManager.opBack, card.tails.managedString);
                }
                clearChainEnd();
                hideCaculator();
                break;
            case GameMessage.Win:
                player = localPlayer(r.ReadByte());
                int winType = r.ReadByte();
                showCaculator();
                Sleep(120);
                if (player == 2)
                {
                    RMSshow_none(InterString.Get("游戏平局！"));
                }
                else if (player == 0 || winType == 4)
                {
                    if (cookie_matchKill > 0)
                    {
                        RMSshow_none(InterString.Get("比赛胜利，卡片：[?]", winReason));
                    }
                    else
                    {
                        RMSshow_none(InterString.Get("游戏胜利，原因：[?]", winReason));
                    }
                }
                else
                {
                    if (cookie_matchKill > 0)
                    {
                        RMSshow_none(InterString.Get("比赛败北，卡片：[?]", winReason));
                    }
                    else
                    {
                        RMSshow_none(InterString.Get("游戏败北，原因：[?]", winReason));
                    }
                }
                break;
            case GameMessage.RequestDeck:
                break;
            case GameMessage.SelectBattleCmd:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                if (condition == Condition.record)
                {
                    Sleep(20);
                }
                destroy(waitObject, 0, false, true);
                toDefaultHint();
                player = localPlayer(r.ReadChar());
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    desc = GameStringManager.get(r.ReadInt32());
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        Effect eff = new Effect();
                        eff.ptr = ((i << 16) + 0);
                        eff.desc = desc;
                        card.effects.Add(eff);
                        if (card.query_hint_button(InterString.Get("发动效果@ui")) == false)
                        {
                            btn = new gameButton(((i << 16) + 0), InterString.Get("发动效果@ui"), superButtonType.act);
                            btn.cookieCard = card;
                            card.add_one_button(btn);
                            if (card.condition != gameCardCondition.verticle_clickable)
                            {
                                card.add_one_decoration(Program.I().mod_ocgcore_decoration_card_active, 2, Vector3.zero, "active", true, true);
                                if (card.isHided())
                                    card.currentFlash = gameCard.flashType.Active;
                            }
                        }
                    }
                }
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    r.ReadByte();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        btn = new gameButton(((i << 16) + 1), InterString.Get("攻击宣言@ui"), superButtonType.attack);
                        card.add_one_button(btn);
                        card.add_one_decoration(Program.I().mod_ocgcore_bs_atk_decoration, 5, Vector3.zero, "atk");
                    }
                }
                byte mp = r.ReadByte();
                byte ep = r.ReadByte();
                if (mp == 1)
                {
                    gameInfo.addHashedButton("", 2, superButtonType.mp, InterString.Get("主要阶段@ui"));
                    gameField.retOfMp = 2;
                    gameField.Phase.colliderMp2.enabled = true;
                }
                if (ep == 1)
                {
                    gameInfo.addHashedButton("", 3, superButtonType.ep, InterString.Get("结束回合@ui"));
                    gameField.retOfEp = 3;
                    gameField.Phase.colliderEp.enabled = true;
                }
                realize();
                break;
            case GameMessage.SelectIdleCmd:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                if (condition == Condition.record)
                {
                    Sleep(20);
                }
                destroy(waitObject, 0, false, true);
                toDefaultHint();
                player = localPlayer(r.ReadChar());
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        btn = new gameButton(((i << 16) + 0), InterString.Get("通常召唤@ui"), superButtonType.summon);
                        card.add_one_button(btn);
                    }
                }
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        if (card.query_hint_button(InterString.Get("特殊召唤@ui")) == false)
                        {
                            btn = new gameButton(((i << 16) + 1), InterString.Get("特殊召唤@ui"), superButtonType.spsummon);
                            card.add_one_button(btn);
                            if (card.condition != gameCardCondition.verticle_clickable)
                            {
                                card.add_one_decoration(Program.I().mod_ocgcore_decoration_spsummon, 2, Vector3.zero, "chain_selecting", true, true);
                                if (card.isHided())
                                    card.currentFlash = gameCard.flashType.SpSummon;
                            }
                        }
                    }
                }
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        btn = new gameButton(((i << 16) + 2), InterString.Get("表示形式@ui"), superButtonType.change);
                        card.add_one_button(btn);
                    }
                }
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        btn = new gameButton(((i << 16) + 3), InterString.Get("前场放置@ui"), superButtonType.set);
                        card.add_one_button(btn);
                    }
                }
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        btn = new gameButton(((i << 16) + 4), InterString.Get("后场放置@ui"), superButtonType.set);
                        card.add_one_button(btn);
                    }
                }
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    int descP = r.ReadInt32();
                    desc = GameStringManager.get(descP);
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        if (descP == 1160)
                        {
                            btn = new gameButton(((i << 16) + 5), InterString.Get("灵摆发动@ui"), superButtonType.act);
                            card.add_one_button(btn);
                            if (card.condition != gameCardCondition.verticle_clickable)
                            {
                                card.add_one_decoration(Program.I().mod_ocgcore_decoration_card_active, 2, Vector3.zero, "active", true, true);
                                if (card.isHided())
                                    card.currentFlash = gameCard.flashType.Active;
                            }
                        }
                        else
                        {
                            Effect eff = new Effect();
                            eff.ptr = ((i << 16) + 5);
                            eff.desc = desc;
                            card.effects.Add(eff);
                            if (card.query_hint_button(InterString.Get("发动效果@ui")) == false)
                            {
                                btn = new gameButton(((i << 16) + 5), InterString.Get("发动效果@ui"), superButtonType.act);
                                btn.cookieCard = card;
                                card.add_one_button(btn);
                                if (card.condition != gameCardCondition.verticle_clickable)
                                {
                                    card.add_one_decoration(Program.I().mod_ocgcore_decoration_card_active, 2, Vector3.zero, "active", true, true);
                                    if (card.isHided())
                                        card.currentFlash = gameCard.flashType.Active;
                                }
                            }
                        }
                    }
                }
                byte bp = r.ReadByte();
                byte ep2 = r.ReadByte();
                byte shuffle = r.ReadByte();
                if (bp == 1)
                {
                    gameInfo.addHashedButton("", 6, superButtonType.bp, InterString.Get("战斗阶段@ui"));
                    gameField.retOfbp = 6;
                    gameField.Phase.colliderBp.enabled = true;
                }
                if (ep2 == 1)
                {
                    gameInfo.addHashedButton("", 7, superButtonType.ep, InterString.Get("结束回合@ui"));
                    gameField.retOfEp = 7;
                    gameField.Phase.colliderEp.enabled = true;
                }
                if (shuffle == 1)
                {
                    gameInfo.addHashedButton("", 8, superButtonType.change, InterString.Get("洗切手牌@ui"));
                }
                realize();
                break;
            case GameMessage.SelectEffectYn:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                if (condition == Condition.record)
                {
                    Sleep(20);
                }
                destroy(waitObject, 0, false, true);
                player = localPlayer(r.ReadByte());
                code = r.ReadInt32();
                gps = r.ReadShortGPS();
                r.ReadByte();
                int cr = 95;
                if (Config.ClientVersion >= 0x233c)
                {
                    int cp = r.ReadInt32();
                    if (cp > 0)
                        cr = cp;
                }
                desc = GameStringManager.get(cr);
                card = GCS_cardGet(gps, false);
                desc = desc.Replace("[%ls]", "「" + card.get_data().Name + "」");
                if (card != null)
                {
                    string hin = ES_hint + "，\n" + desc;
                    RMSshow_yesOrNo("return", hin, new messageSystemValue { value = "1", hint = "yes" }, new messageSystemValue { value = "0", hint = "no" });
                    card.add_one_decoration(Program.I().mod_ocgcore_decoration_chain_selecting, 4, Vector3.zero, "chain_selecting");
                    card.currentFlash = gameCard.flashType.Active;
                }
                break;
            case GameMessage.SelectYesNo:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                if (condition == Condition.record)
                {
                    Sleep(20);
                }
                destroy(waitObject, 0, false, true);
                player = localPlayer(r.ReadByte());
                desc = GameStringManager.get(r.ReadInt32());
                RMSshow_yesOrNo("return", desc, new messageSystemValue { value = "1", hint = "yes" }, new messageSystemValue { value = "0", hint = "no" });
                break;
            case GameMessage.SelectOption:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                if (condition == Condition.record)
                {
                    Sleep(60);
                }
                destroy(waitObject, 0, false, true);
                player = localPlayer(r.ReadByte());
                count = r.ReadByte();
                if (count > 1)
                {
                    values = new List<messageSystemValue>();
                    for (int i = 0; i < count; i++)
                    {
                        desc = GameStringManager.get(r.ReadInt32());
                        values.Add(new messageSystemValue { hint = desc, value = i.ToString() });
                    }
                    RMSshow_singleChoice("return", values);
                }
                else
                {
                    binaryMaster = new BinaryMaster();
                    binaryMaster.writer.Write(0);
                    sendReturn(binaryMaster.get());
                }

                break;
            case GameMessage.SelectTribute:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                if (condition == Condition.record)
                {
                    Sleep(60);
                }
                destroy(waitObject, 0, false, true);
                player = localPlayer(r.ReadByte());
                cancalable = (r.ReadByte() != 0);
                ES_min = r.ReadByte();
                ES_max = r.ReadByte();
                ES_level = 0;
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        card.forSelect = true;
                        card.selectPtr = i;
                        int para = r.ReadByte();
                        card.levelForSelect_1 = para;
                        card.levelForSelect_2 = para;
                        allCardsInSelectMessage.Add(card);
                    }
                }
                if (cancalable)
                {
                    gameInfo.addHashedButton("cancleSelected", -1, superButtonType.no, InterString.Get("取消选择@ui"));
                }
                realizeCardsForSelect();
                if (ES_selectHint != "")
                {
                    gameField.setHint(ES_selectHint + " " + ES_min.ToString() + "-" + ES_max.ToString());
                }
                else
                {
                    gameField.setHint(InterString.Get("请选择卡片。") + " " + ES_min.ToString() + "-" + ES_max.ToString());
                }
                break;
            case GameMessage.SelectCard:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                if (condition == Condition.record)
                {
                    Sleep(60);
                }
                destroy(waitObject, 0, false, true);
                player = localPlayer(r.ReadByte());
                cancalable = (r.ReadByte() != 0);
                ES_min = r.ReadByte();
                ES_max = r.ReadByte();
                ES_level = 0;
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        card.forSelect = true;
                        card.selectPtr = i;
                        allCardsInSelectMessage.Add(card);
                    }
                }
                if (cancalable)
                {
                    gameInfo.addHashedButton("cancleSelected", -1, superButtonType.no, InterString.Get("取消选择@ui"));
                }
                realizeCardsForSelect();
                if (ES_selectHint != "")
                {
                    gameField.setHint(ES_selectHint + " " + ES_min.ToString() + "-" + ES_max.ToString());
                }
                else
                {
                    gameField.setHint(InterString.Get("请选择卡片。") + " " + ES_min.ToString() + "-" + ES_max.ToString());
                }
                break;
            case GameMessage.SelectUnselectCard:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                if (condition == Condition.record)
                {
                    Sleep(60);
                }
                destroy(waitObject, 0, false, true);
                player = localPlayer(r.ReadByte());
                bool finishable = (r.ReadByte() != 0);
                cancalable = (r.ReadByte() != 0) || finishable;
                ES_min = r.ReadByte();
                ES_max = r.ReadByte();
                ES_min = finishable ? 0 : 1; // SelectUnselectCard can actually always select 1 card
                ES_max = 1; // SelectUnselectCard can actually always select 1 card
                ES_level = 0;
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        card.forSelect = true;
                        card.selectPtr = i;
                        allCardsInSelectMessage.Add(card);
                    }
                }
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadGPS();
                    /*card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        card.forSelect = true;
                        card.selectPtr = i;
                        allCardsInSelectMessage.Add(card);
                    }*/
                }
                if (cancalable && !finishable)
                {
                    gameInfo.addHashedButton("cancleSelected", -1, superButtonType.no, InterString.Get("取消选择@ui"));
                }
                realizeCardsForSelect();
                if (ES_selectHint != "")
                {
                    gameField.setHint(ES_selectHint + " " + ES_min.ToString() + "-" + ES_max.ToString());
                }
                else
                {
                    gameField.setHint(InterString.Get("请选择卡片。") + " " + ES_min.ToString() + "-" + ES_max.ToString());
                }
                break;
            case GameMessage.SelectChain:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                destroy(waitObject, 0, false, true);
                player = localPlayer(r.ReadChar());
                count = r.ReadByte();
                int spcount = r.ReadByte();
                int forced = r.ReadByte();
                int hint0 = r.ReadInt32();
                int hint1 = r.ReadInt32();
                List<gameCard> chainCards = new List<gameCard>();
                for (int i = 0; i < count; i++)
                {
                    int flag = 0;
                    if (length_of_message % 12 != 0)
                    {
                        flag = r.ReadChar();
                    }
                    code = r.ReadInt32() % 1000000000;
                    gps = r.ReadGPS();
                    desc = GameStringManager.get(r.ReadInt32());
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        chainCards.Add(card);
                        card.set_code(code);
                        card.prefered = true;
                        Effect eff = new Effect();
                        eff.flag = flag;
                        eff.ptr = i;
                        eff.desc = desc;
                        card.effects.Add(eff);
                    }
                }
                var chain_condition = gameInfo.get_condition(); 
                int handle_flag = 0;
                if (forced == 0)
                {
                    //无强制发动的卡
                    if (spcount == 0)
                    {
                        //无关键卡
                        if (chain_condition == gameInfo.chainCondition.no)
                        {
                            //无关键卡 连锁被无视 直接回答---
                            handle_flag = 0;
                        }
                        else if (chain_condition == gameInfo.chainCondition.all)
                        {
                            //无关键卡但是连锁被监控
                            if (chainCards.Count == 0)
                            {
                                //欺骗--
                                handle_flag = -1;
                            }
                            else
                            {
                                if (chainCards.Count == 1 && chainCards[0].effects.Count == 1)
                                {
                                    //只有一张要处理的卡 常规处理 一张---
                                    handle_flag = 1;
                                }
                                else
                                {
                                    //常规处理 多张---
                                    handle_flag = 2;
                                }
                            }
                        }
                        else if (chain_condition == gameInfo.chainCondition.smart)
                        {
                            //无关键卡但是连锁被智能过滤
                            if (chainCards.Count == 0)
                            {
                                //根本没卡 直接回答---
                                handle_flag = 0;
                            }
                            else
                            {
                                if (chainCards.Count == 1 && chainCards[0].effects.Count == 1)
                                {
                                    //只有一张要处理的卡 常规处理 一张---
                                    handle_flag = 1;
                                }
                                else
                                {
                                    //常规处理 多张---
                                    handle_flag = 2;
                                }
                            }
                        }
                        else
                        {
                            //无关键卡而且连锁没有被监控    直接回答---
                            handle_flag = 0;
                        }
                    }
                    else
                    {
                        //有关键卡
                        if (chainCards.Count == 0)
                        {
                            //根本没卡 直接回答---
                            handle_flag = 0;
                            if (chain_condition == gameInfo.chainCondition.all)
                            {
                                //欺骗--
                                handle_flag = -1;
                            }
                        }
                        else if (chain_condition == gameInfo.chainCondition.no)
                        {
                            //有关键卡 连锁被无视 直接回答---
                            handle_flag = 0;
                        }
                        else
                        {
                            if (chainCards.Count == 1 && chainCards[0].effects.Count == 1)
                            {
                                //只有一张要处理的卡 常规处理 一张---
                                handle_flag = 1;
                            }
                            else
                            {
                                //常规处理 多张---
                                handle_flag = 2;
                            }
                        }
                    }
                }
                else
                {
                    if (chainCards.Count == 1 && chainCards[0].effects.Count == 1)
                    {
                        //有一张强制发动的卡 回应--
                        handle_flag = 4;
                    }
                    else
                    {
                        //有强制发动的卡 处理强制发动的卡--
                        handle_flag = 3;
                        if (autoForceChainHandler== autoForceChainHandlerType.autoHandleAll)
                        {
                            handle_flag = 4;
                        }
                        if (autoForceChainHandler == autoForceChainHandlerType.afterClickManDo)
                        {
                            handle_flag = 5;
                        }
                    }
                    if (UIHelper.fromStringToBool(Config.Get("autoChain_", "0")) == true)
                    {
                        //自动回应--
                        handle_flag = 4;
                    }
                }
                if (handle_flag == -1)
                {
                    //欺骗
                    RMSshow_onlyYes("return", InterString.Get("[?]，@n没有卡片可以连锁。", ES_hint), new messageSystemValue { hint = "yes", value = "-1" });
                    flagForCancleChain = true;
                    if (condition == Condition.record)
                    {
                        Sleep(60);
                    }
                }
                if (handle_flag == 0)
                {
                    //直接回答
                    binaryMaster = new BinaryMaster();
                    binaryMaster.writer.Write((Int32)(-1));
                    sendReturn(binaryMaster.get());
                }
                if (handle_flag == 1)
                {
                    //处理一张   废除
                    handle_flag = 2;
                }
                if (handle_flag == 2)
                {
                    //处理多张
                    for (int i = 0; i < chainCards.Count; i++)
                    {
                        chainCards[i].add_one_decoration(Program.I().mod_ocgcore_decoration_chain_selecting, 4, Vector3.zero, "chain_selecting");
                        chainCards[i].forSelect = true;
                        chainCards[i].currentFlash = gameCard.flashType.Active;
                    }
                    flagForCancleChain = true;
                    RMSshow_yesOrNo("return", InterString.Get("[?]，@n是否连锁？", ES_hint), new messageSystemValue { value = "hide", hint = "yes" }, new messageSystemValue { value = "-1", hint = "no" });
                    gameInfo.addHashedButton("cancleChain", -1, superButtonType.no, InterString.Get("取消连锁@ui"));
                    if (condition == Condition.record)
                    {
                        Sleep(60);
                    }
                }
                if (handle_flag == 3)
                {
                    //处理强制发动的卡
                    for (int i = 0; i < chainCards.Count; i++)
                    {
                        chainCards[i].add_one_decoration(Program.I().mod_ocgcore_decoration_chain_selecting, 4, Vector3.zero, "chain_selecting");
                        chainCards[i].forSelect = true;
                        chainCards[i].currentFlash = gameCard.flashType.Active;
                    }
                    RMSshow_yesOrNo("autoForceChainHandler", InterString.Get("[?]，@n自动处理强制发动的卡？", ES_hint), new messageSystemValue { value = "yes", hint = "yes" }, new messageSystemValue { value = "no", hint = "no" });
                    if (condition == Condition.record)
                    {
                        Sleep(60);
                    }
                }
                if (handle_flag == 5)
                {
                    //处理强制发动的卡 AfterClick
                    for (int i = 0; i < chainCards.Count; i++)
                    {
                        chainCards[i].add_one_decoration(Program.I().mod_ocgcore_decoration_chain_selecting, 4, Vector3.zero, "chain_selecting");
                        chainCards[i].forSelect = true;
                        chainCards[i].currentFlash = gameCard.flashType.Active;
                    }
                }
                if (handle_flag == 4)
                {
                    //有一张强制发动的卡 回应--
                    binaryMaster = new BinaryMaster();
                    binaryMaster.writer.Write((Int32)(chainCards[0].effects[0].ptr));
                    sendReturn(binaryMaster.get());
                }
                break;
            case GameMessage.SelectPosition:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                if (condition == Condition.record)
                {
                    Sleep(60);
                }
                destroy(waitObject, 0, false, true);
                player = localPlayer(r.ReadByte());
                code = r.ReadInt32();
                int positions = r.ReadByte();
                int op1 = 0x1;
                int op2 = 0x4;
                if (positions == 0x1 || positions == 0x2 || positions == 0x4 || positions == 0x8)
                {
                    binaryMaster = new BinaryMaster();
                    binaryMaster.writer.Write(positions);
                    sendReturn(binaryMaster.get());
                }
                else
                {
                    if ((positions & 0x1) > 0)
                    {
                        op1 = 0x1;
                    }
                    if ((positions & 0x2) > 0)
                    {
                        op1 = 0x2;
                    }
                    if ((positions & 0x4) > 0)
                    {
                        op2 = 0x4;
                    }
                    if ((positions & 0x8) > 0)
                    {
                        if ((positions & 0x4) > 0)
                        {
                            op1 = 0x4;
                        }
                        op2 = 0x8;
                    }
                    RMSshow_position("return", code, new messageSystemValue { value = op1.ToString(), hint = "atk" }, new messageSystemValue { value = op2.ToString(), hint = "def" });
                }
                break;
            case GameMessage.SortCard:
            case GameMessage.SortChain:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                if (condition == Condition.record)
                {
                    Sleep(60);
                }
                destroy(waitObject, 0, false, true);
                player = localPlayer(r.ReadByte());
                ES_sortSum = 0;
                count = r.ReadByte();
                cardsInSort.Clear();
                ES_sortResult.Clear();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        card.forSelect = true;
                        card.isShowed = true;
                        card.sortOptions.Add(i);
                        cardsInSort.Remove(card);
                        cardsInSort.Add(card);
                        ES_sortSum++;
                        card.add_one_decoration(Program.I().mod_ocgcore_decoration_card_selecting, 2, Vector3.zero, "card_selecting");
                        card.currentFlash = gameCard.flashType.Select;
                    }
                }
                if (UIHelper.fromStringToBool(Config.Get("autoChain_", "0")) == true)
                {
                    if (currentMessage == GameMessage.SortChain)
                    {
                        bin = new BinaryMaster();
                        for (int i = 0; i < count; i++)
                        {
                            bin.writer.Write((byte)(i));
                        }
                        sendReturn(bin.get());
                    }
                }
                realize();
                toNearest();
                if (currentMessage == GameMessage.SortCard)
                {
                    gameField.setHint(InterString.Get("请为卡片排序。"));
                }
                else
                {
                    gameField.setHint(InterString.Get("请为连锁手动排序。"));
                }
                break;
            case GameMessage.SelectCounter:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                bool Version1033b = (length_of_message - 5) % 8 == 0;
                if (condition == Condition.record)
                {
                    Sleep(60);
                }
                destroy(waitObject, 0, false, true);
                player = localPlayer(r.ReadByte());
                r.ReadInt16();
                if (Version1033b)   
                {
                    ES_min = r.ReadByte();
                }
                else
                {
                    ES_min = r.ReadUInt16();
                }
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    int pew = 0;
                    if (Version1033b)
                    {
                        pew = r.ReadByte();
                    }
                    else
                    {
                        pew = r.ReadUInt16();
                    }
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        card.counterCANcount = pew;
                        card.counterSELcount = 0;
                        allCardsInSelectMessage.Add(card);
                        card.selectPtr = i;
                        card.forSelect = true;
                        card.add_one_decoration(Program.I().mod_ocgcore_decoration_card_selecting, 2, Vector3.zero, "card_selecting");
                        card.isShowed = true;
                        card.currentFlash = gameCard.flashType.Select;
                    }
                }
                if (gameInfo.queryHashedButton("clearCounter") == false)
                {
                    gameInfo.addHashedButton("clearCounter", 0, superButtonType.no, InterString.Get("重新选择@ui"));
                }
                realize();
                toNearest();
                gameField.setHint(InterString.Get("请移除[?]个指示物。", ES_min.ToString()));
                break;
            case GameMessage.SelectSum:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                if (condition == Condition.record)
                {
                    Sleep(60);
                }
                destroy(waitObject, 0, false, true);
                ES_overFlow = r.ReadByte() != 0;
                player = localPlayer(r.ReadByte());
                ES_level = r.ReadInt32();
                ES_min = r.ReadByte();
                ES_max = r.ReadByte();
                if (ES_min < 1)
                {
                    ES_min = 1;
                }
                if (ES_max < 1)
                {
                    ES_max = 99;
                }
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    int para = r.ReadInt32();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.selectPtr = i;
                        card.levelForSelect_1 = para & 0xffff;
                        card.levelForSelect_2 = para >> 16;
                        if (card.levelForSelect_2 == 0)
                        {
                            card.levelForSelect_2 = card.levelForSelect_1;
                            if ((card.get_data().Type & (int)game_type.link) > 0)
                            {
                                card.levelForSelect_2 = 1;
                            }
                        }
                        allCardsInSelectMessage.Add(card);
                        cardsMustBeSelected.Add(card);
                        card.forSelect = true;
                    }
                }
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    int para = r.ReadInt32();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        card.selectPtr = i;
                        card.levelForSelect_1 = para & 0xffff;
                        card.levelForSelect_2 = para >> 16;
                        if (card.levelForSelect_2 == 0)
                        {
                            card.levelForSelect_2 = card.levelForSelect_1;
                            if ((card.get_data().Type & (int)game_type.link) > 0)
                            {
                                card.levelForSelect_2 = 1;
                            }
                        }
                        allCardsInSelectMessage.Add(card);
                        card.forSelect = true;
                    }
                }
                realizeCardsForSelect();
                gameField.setHint(ES_selectHint);
                break;
            case GameMessage.SelectPlace:
            case GameMessage.SelectDisfield:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                destroy(waitObject, 0, false, true);
                binaryMaster = new BinaryMaster();
                player = r.ReadByte();
                min = r.ReadByte();
                int _field = ~r.ReadInt32();
                if (Program.I().setting.setting.hand.value == true || Program.I().setting.setting.handm.value == true)
                {
                    
                    ES_min = min;
                    for (int i = 0; i < min; i++)
                    {
                        byte[] resp = new byte[3];
                        bool pendulumZone = false;
                        int filter;

                        /*if ((field & 0x7f0000) != 0)
                        {
                            resp[0] = (byte)(1 - player);
                            resp[1] = 0x4;
                            filter = (field >> 16) & 0x7f;
                        }
                        else if ((field & 0x1f000000) != 0)
                        {
                            resp[0] = (byte)(1 - player);
                            resp[1] = 0x8;
                            filter = (field >> 24) & 0x1f;
                        }
                        else if ((field & 0xc0000000) != 0)
                        {
                            resp[0] = (byte)(1 - player);
                            resp[1] = 0x8;
                            filter = (field >> 30) & 0x3;
                            pendulumZone = true;
                        }*/ 
                    for (int j=0; j<2; j++)
                    {
                        resp = new byte[3];
                        pendulumZone = false;
                        filter = 0;
                        int field;

                        if (j==0)
                        {
                            resp[0] = (byte)player;
                            field = _field & 0xffff;
                        }
                        else
                        {
                            resp[0] = (byte)(1 - player);
                            field = _field >> 16;
                        }

                        if ((field & 0x7f) != 0)
                        {
                            resp[1] = 0x4;
                            filter = field & 0x7f;
                        }
                        else if ((field & 0x1f00) != 0)
                        {
                            resp[1] = 0x8;
                            filter = (field >> 8) & 0x1f;
                        }
                        else if ((field & 0xc000) != 0)
                        {
                            resp[1] = 0x8;
                            filter = (field >> 14) & 0x3;
                            pendulumZone = true;
                        }

                        if (filter == 0)
                            continue;

                        if (!pendulumZone)
                        {
                            if ((filter & 0x4) != 0)
                            {
                                resp[2] = 2;
                                createPlaceSelector(resp);
                            }
                            if ((filter & 0x2) != 0)
                            {
                                resp[2] = 1;
                                createPlaceSelector(resp);
                            }
                            if ((filter & 0x8) != 0)
                            {
                                resp[2] = 3;
                                createPlaceSelector(resp);
                            }
                            if ((filter & 0x1) != 0)
                            {
                                resp[2] = 0;
                                createPlaceSelector(resp);
                            }
                            if ((filter & 0x10) != 0)
                            {
                                resp[2] = 4;
                                createPlaceSelector(resp);
                            }
                            if (resp[1] == 0x4)
                            {
                                if ((filter & 0x20) != 0)
                                {
                                    resp[2] = 5;
                                    createPlaceSelector(resp);
                                }
                                if ((filter & 0x40) != 0)
                                {
                                    resp[2] = 6;
                                    createPlaceSelector(resp);
                                }
                            }
                        }
                        else
                        {
                            if ((filter & 0x2) != 0)
                            {
                                resp[2] = 7;
                                createPlaceSelector(resp);
                            }
                            if ((filter & 0x1) != 0)
                            {
                                resp[2] = 6;
                                createPlaceSelector(resp);
                            }
                        }

                    }

                    }
                    if (Es_selectMSGHintType == 3)
                    {
                        if (Es_selectMSGHintPlayer == 0)
                        {
                            gameField.setHint(InterString.Get("请为我方的「[?]」选择位置。", YGOSharp.CardsManager.Get(Es_selectMSGHintData).Name));
                        }
                        else
                        {
                            gameField.setHint(InterString.Get("请为对方的「[?]」选择位置。", YGOSharp.CardsManager.Get(Es_selectMSGHintData).Name));
                        }
                    }
                }
                else
                {
                    int field = _field;
                    for (int i = 0; i < min; i++)
                    {
                        byte[] resp = new byte[3];
                        bool pendulumZone = false;
                        int filter;

                        if ((field & 0x7f0000) != 0)
                        {
                            resp[0] = (byte)(1 - player);
                            resp[1] = 0x4;
                            filter = (field >> 16) & 0x7f;
                        }
                        else if ((field & 0x1f000000) != 0)
                        {
                            resp[0] = (byte)(1 - player);
                            resp[1] = 0x8;
                            filter = (field >> 24) & 0x1f;
                        }
                        else if ((field & 0xc0000000) != 0)
                        {
                            resp[0] = (byte)(1 - player);
                            resp[1] = 0x8;
                            filter = (field >> 30) & 0x3;
                            pendulumZone = true;
                        }
                        else if ((field & 0x7f) != 0)
                        {
                            resp[0] = (byte)player;
                            resp[1] = 0x4;
                            filter = field & 0x7f;
                        }
                        else if ((field & 0x1f00) != 0)
                        {
                            resp[0] = (byte)player;
                            resp[1] = 0x8;
                            filter = (field >> 8) & 0x1f;
                        }
                        else
                        {
                            resp[0] = (byte)player;
                            resp[1] = 0x8;
                            filter = (field >> 14) & 0x3;
                            pendulumZone = true;
                        }

                        if (!pendulumZone)
                        {
                            if ((filter & 0x4) != 0) resp[2] = 2;
                            else if ((filter & 0x2) != 0) resp[2] = 1;
                            else if ((filter & 0x8) != 0) resp[2] = 3;
                            else if ((filter & 0x1) != 0) resp[2] = 0;
                            else if ((filter & 0x10) != 0) resp[2] = 4;
                            else
                            {
                                if (resp[1] == 0x4)
                                {
                                    if ((filter & 0x20) != 0) resp[2] = 5;
                                    else if ((filter & 0x40) != 0) resp[2] = 6;
                                }
                            }
                        }
                        else
                        {
                            if ((filter & 0x2) != 0) resp[2] = 7;
                            if ((filter & 0x1) != 0) resp[2] = 6;
                        }
                        binaryMaster.writer.Write(resp);
                    }
                    sendReturn(binaryMaster.get());
                }
                break;
            case GameMessage.RockPaperScissors:
                binaryMaster = new BinaryMaster();
                binaryMaster.writer.Write(UnityEngine.Random.Range(0, 2));
                sendReturn(binaryMaster.get());
                break;
            case GameMessage.ConfirmDecktop:
                player = localPlayer(r.ReadByte());
                count = r.ReadByte();
                int countOfDeck = countLocation(player, game_location.LOCATION_DECK);
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    gps = new GPS
                    {
                        controller = (UInt32)player,
                        location = (UInt32)game_location.LOCATION_DECK,
                        sequence = (UInt32)(countOfDeck - 1 - i),
                    };
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        confirm(card);
                    }
                }
                Sleep(count * 40);
                break;
            case GameMessage.ConfirmCards:
                player = localPlayer(r.ReadByte());
                count = r.ReadByte();
                int t2 = 0;
                int t3 = 0;
                bool pan_mode = false;
                for (int i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    bool showC = false;
                    if (gps.controller!=0)  
                    {
                        showC = true;
                    }
                    else
                    {
                        if (gps.location != (int)game_location.LOCATION_HAND)   
                        {
                            showC = true;
                        }
                        if (Program.I().room.mode == 2) 
                        {
                            showC = true;
                        }
                        if (condition != Condition.duel)
                        {
                            if (InAI == false)  
                            {
                                showC = true;
                            }
                        }
                    }
                    if (showC)  
                    {
                        if (card != null)
                        {
                            if (
                                (card.p.location & (UInt32)game_location.LOCATION_DECK) > 0
                                ||
                                (card.p.location & (UInt32)game_location.LOCATION_GRAVE) > 0
                                ||
                                (card.p.location & (UInt32)game_location.LOCATION_EXTRA) > 0
                                ||
                                (card.p.location & (UInt32)game_location.LOCATION_REMOVED) > 0
                                )
                            {
                                card.currentKuang = gameCard.kuangType.selected;
                                cardsInSelectAnimation.Add(card);
                                card.isShowed = true;
                                pan_mode = true;
                                if (condition != Condition.record)
                                {
                                    t2 += 100000;
                                    clearTimeFlag = true;
                                }
                                t3++;
                            }
                            else if (card.condition != gameCardCondition.verticle_clickable)
                            {
                                if ((card.p.location & (UInt32)game_location.LOCATION_HAND) > 0)
                                {
                                    if (i==0)   
                                    {
                                        confirm(card);
                                        t2 += 50;
                                    }
                                    else
                                    {
                                        Nconfirm();
                                        t2 = 50;
                                    }
                                }
                                else
                                {
                                    confirm(card);
                                    t2 += 50;
                                }
                            }
                            else
                            {
                                card.currentKuang = gameCard.kuangType.selected;
                                cardsInSelectAnimation.Add(card);
                            }
                        }
                    }
                }
                realize();
                toNearest();
                if (pan_mode)
                {
                    clearAllShowedB = true;
                    flagForTimeConfirm = true;
                    gameField.setHint(InterString.Get("请确认[?]张卡片。", t3.ToString()));
                    if (inIgnoranceReplay()||inTheWorld())
                    {
                        t2 = 0;
                        clearResponse();
                    }
                }
                Sleep(t2);
                break;
            case GameMessage.RefreshDeck:
            case GameMessage.ShuffleDeck:
                UIHelper.playSound("shuffle", 1f);
                player = localPlayer(r.ReadByte());
                for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                    {
                        if ((cards[i].p.location & (UInt32)game_location.LOCATION_DECK) > 0)
                        {
                            if (cards[i].p.controller == player)
                            {
                                if (i % 2 == 0) cards[i].animation_shake_to(1.2f);
                            }
                        }
                    }
                Sleep(30);
                break;
            case GameMessage.ShuffleHand:
                realize();
                UIHelper.playSound("shuffle", 1f);
                player = localPlayer(r.ReadByte());
                animation_suffleHand(player);
                Sleep(21);
                break;
            case GameMessage.SwapGraveDeck:
                realize();
                Sleep(120);
                break;
            case GameMessage.ShuffleSetCard:
                UIHelper.playSound("shuffle", 1f);
                count = r.ReadByte();
                List<GPS> gpss = new List<GPS>();
                for (int i = 0; i < count; i++)
                {
                    gps = r.ReadGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        Vector3 position = Vector3.zero;
                        if (card.p.controller == 1)
                        {
                            card.animation_confirm(new Vector3(0, 5, 5), new Vector3(0, 90, 180), 0.2f, 0.01f);
                        }
                        else
                        {
                            card.animation_confirm(new Vector3(0, 5, -5), new Vector3(0, -90, 180), 0.2f, 0.01f);
                        }
                    }
                }
                Sleep(30);
                break;
            case GameMessage.ReverseDeck:
                break;
            case GameMessage.DeckTop:
                break;
            case GameMessage.NewTurn:
                removeSelectedAnimations();
                player = localPlayer(r.ReadByte());
                if (condition != Condition.duel)
                {
                    gameInfo.setTimeStill(player);
                }
                //else
                //{
                //    gameInfo.setTime(player, timeLimit);
                //}
                toDefaultHint();
                UIHelper.playSound("nextturn", 1f);
                gameField.animation_show_big_string(GameTextureManager.nt);
                //if (player == 1 && InAI == true)
                //{
                //    showWait();
                //}
                gameInfo.setExcited((turns % 2 == (isFirst ? 0 : 1)) ? 1 : 0);
                break;
            case GameMessage.NewPhase:
                removeSelectedAnimations();
                toDefaultHint();
                UIHelper.playSound("phase", 1f);
                int phrase = r.ReadUInt16();
                if (GameStringHelper.differ(phrase, (long)DuelPhase.BattleStart))
                {
                    gameField.animation_show_big_string(GameTextureManager.bp);
                }
                if (GameStringHelper.differ(phrase, (long)DuelPhase.Draw))
                {
                    gameField.animation_show_big_string(GameTextureManager.dp);
                }
                if (GameStringHelper.differ(phrase, (long)DuelPhase.End))
                {
                    gameField.animation_show_big_string(GameTextureManager.ep);
                }
                if (GameStringHelper.differ(phrase, (long)DuelPhase.Main1))
                {
                    gameField.animation_show_big_string(GameTextureManager.mp1);
                }
                if (GameStringHelper.differ(phrase, (long)DuelPhase.Main2))
                {
                    gameField.animation_show_big_string(GameTextureManager.mp2);
                }
                if (GameStringHelper.differ(phrase, (long)DuelPhase.Standby))
                {
                    gameField.animation_show_big_string(GameTextureManager.sp);
                }
                gameField.realize();
                break;
            case GameMessage.Move:
                realize();
                code = r.ReadInt32();
                GPS from = r.ReadGPS();
                GPS to = r.ReadGPS();
                card = GCS_cardGet(to, false);
                if ((to.location == ((UInt32)game_location.LOCATION_OVERLAY | (UInt32)game_location.LOCATION_EXTRA)) && ((from.location & (UInt32)game_location.LOCATION_OVERLAY) == 0) && Program.I().setting.setting.Vxyz.value == true)
                {
                    Vector3 vDarkHole = Vector3.zero;
                    float real = (Program.fieldSize - 1) * 0.9f + 1f;
                    if (to.controller == 0)
                    {
                        vDarkHole = new Vector3(0, 0, -7f * real);
                    }
                    if (to.controller == 1)
                    {
                        vDarkHole = new Vector3(0, 0, 7f * real);
                    }
                    gameField.shiftBlackHole(1, vDarkHole);
                }
                else
                {
                    gameField.shiftBlackHole(-1);
                }
                if (card != null)
                {
                    if ((to.position & (int)game_position.POS_FACEDOWN) > 0)
                    {
                        if (to.location == (UInt32)game_location.LOCATION_MZONE || to.location == (UInt32)game_location.LOCATION_SZONE)
                        {
                            if (Program.I().setting.setting.Vset.value == true)
                                card.positionEffect(Program.I().mod_ocgcore_decoration_card_setted);
                            UIHelper.playSound("set", 1f);
                        }
                    }
                    if (to.location == (UInt32)game_location.LOCATION_GRAVE)
                    {
                        if ((from.location & (UInt32)game_location.LOCATION_MZONE) > 0) UIHelper.playSound("destroyed", 1f);
                        if (Program.I().setting.setting.Vmove.value == true)
                            MonoBehaviour.Destroy((GameObject)MonoBehaviour.Instantiate(Program.I().mod_ocgcore_decoration_tograve, card.gameObject.transform.position, Quaternion.identity), 5f);
                    }
                    if (to.location == (UInt32)game_location.LOCATION_REMOVED)
                    {
                        UIHelper.playSound("destroyed", 1f);
                        if (Program.I().setting.setting.Vmove.value == true)
                            card.fast_decoration(Program.I().mod_ocgcore_decoration_removed);
                    }
                }
                break;
            case GameMessage.PosChange:
                realize();
                break;
            case GameMessage.Set:
                break;
            case GameMessage.Swap:
                realize();
                break;
            case GameMessage.FieldDisabled:
                realize();
                break;
            case GameMessage.Summoning:
                code = r.ReadInt32();
                gps = r.ReadGPS();
                card = GCS_cardGet(gps, false);
                removeSelectedAnimations();
                if (card != null)
                {
                    card.set_code(code);
                    UIHelper.playSound("summon", 1f);
                    if (Program.I().setting.setting.Vsum.value == true)
                    {
                        GameObject mod = Program.I().mod_ocgcore_ss_spsummon_normal;
                        card.animationEffect(mod);
                    }
                    card.animation_show_off( true);
                }
                break;
            case GameMessage.Summoned:
                break;
            case GameMessage.SpSummoning:
                code = r.ReadInt32();
                gps = r.ReadGPS();
                removeSelectedAnimations();
                gameField.shiftBlackHole(false, get_point_worldposition(gps));
                card = GCS_cardGet(gps, false);
                if (card != null)
                {
                    card.set_code(code);
                    if (Program.I().setting.setting.Vspsum.value==true)
                    {
                        GameObject mod = Program.I().mod_ocgcore_ss_summon_light;
                        if (GameStringHelper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_EARTH))
                            mod = Program.I().mod_ocgcore_ss_summon_earth;
                        if (GameStringHelper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_DARK))
                            mod = Program.I().mod_ocgcore_ss_summon_dark;
                        if (GameStringHelper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_DEVINE))
                            mod = Program.I().mod_ocgcore_ss_summon_light;
                        if (GameStringHelper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_FIRE))
                            mod = Program.I().mod_ocgcore_ss_summon_fire;
                        if (GameStringHelper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_LIGHT))
                            mod = Program.I().mod_ocgcore_ss_summon_light;
                        if (GameStringHelper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_WATER))
                            mod = Program.I().mod_ocgcore_ss_summon_water;
                        if (GameStringHelper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_WIND))
                            mod = Program.I().mod_ocgcore_ss_summon_wind;
                        if (GameStringHelper.differ(card.get_data().Type, (long)game_type.TYPE_FUSION))
                        {
                            if (Program.I().setting.setting.Vfusion.value == true)
                            {
                                mod = Program.I().mod_ocgcore_ss_spsummon_ronghe;
                            }
                            UIHelper.playSound("specialsummon2", 1f);
                        }
                        else if (GameStringHelper.differ(card.get_data().Type, (long)game_type.TYPE_SYNCHRO))
                        {
                            if (Program.I().setting.setting.Vsync.value == true)
                            {
                                mod = Program.I().mod_ocgcore_ss_spsummon_tongtiao;
                            }
                            UIHelper.playSound("specialsummon2", 1f);

                        }
                        else if (GameStringHelper.differ(card.get_data().Type, (long)game_type.TYPE_RITUAL))
                        {
                            if (Program.I().setting.setting.Vrution.value == true)
                            {
                                mod = Program.I().mod_ocgcore_ss_spsummon_yishi;
                            }
                            UIHelper.playSound("specialsummon2", 1f);
                        }
                        else if (GameStringHelper.differ(card.get_data().Type, (long)game_type.link))
                        {
                            if (Program.I().setting.setting.Vlink.value == true)
                            {
                                float sc = Mathf.Clamp(card.get_data().Attack, 0, 3500) / 3000f;
                                Program.I().mod_ocgcore_ss_spsummon_link.GetComponent<partical_scaler>().scale = sc * 4f;
                                Program.I().mod_ocgcore_ss_spsummon_link.transform.localScale = Vector3.one * (sc * 4f);
                                card.animationEffect(Program.I().mod_ocgcore_ss_spsummon_link);
                                mod.GetComponent<partical_scaler>().scale = Mathf.Clamp(card.get_data().Attack, 0, 3500) / 3000f * 3f;
                            }
                            UIHelper.playSound("specialsummon2", 1f);
                        }
                        else
                        {
                            UIHelper.playSound("specialsummon", 1f);
                            mod.GetComponent<partical_scaler>().scale = Mathf.Clamp(card.get_data().Attack, 0, 3500) / 3000f * 3f;
                        }
                        card.animationEffect(mod);
                    }
                    else
                    {
                        if (GameStringHelper.differ(card.get_data().Type, (long)game_type.TYPE_FUSION))
                        {
                            UIHelper.playSound("specialsummon2", 1f);
                        }
                        else if (GameStringHelper.differ(card.get_data().Type, (long)game_type.TYPE_SYNCHRO))
                        {
                            UIHelper.playSound("specialsummon2", 1f);
                        }
                        else if (GameStringHelper.differ(card.get_data().Type, (long)game_type.TYPE_RITUAL))
                        {
                            UIHelper.playSound("specialsummon2", 1f);
                        }
                        else
                        {
                            UIHelper.playSound("specialsummon", 1f);
                        }
                    }
                    card.animation_show_off( true);
                }
                break;
            case GameMessage.SpSummoned:
                break;
            case GameMessage.FlipSummoning:
                realize();
                removeSelectedAnimations();
                code = r.ReadInt32();
                gps = r.ReadGPS();
                card = GCS_cardGet(gps, false);
                if (card != null)
                {
                    card.set_code(code);
                    UIHelper.playSound("summon", 1f);
                    if (Program.I().setting.setting.Vflip.value == true)
                    {
                        GameObject mod = Program.I().mod_ocgcore_ss_spsummon_normal;
                        card.animationEffect(mod);
                    }
                    card.animation_show_off( true);
                }
                break;
            case GameMessage.FlipSummoned:
                break;
            case GameMessage.Chaining:
                //removeAttackHandler();
                code = r.ReadInt32();
                gps = r.ReadGPS();
                card = GCS_cardGet(gps, false);
                if (card != null)
                {
                    card.set_code(code);
                    UIHelper.playSound("activate", 1);
                    card.animation_show_off( false);
                    if ((card.get_data().Type & (int)game_type.TYPE_MONSTER) > 0)
                    {
                        if (Program.I().setting.setting.Vactm.value == true)
                        {
                            GameObject mod = Program.I().mod_ocgcore_cs_mon_light;
                            if ((card.get_data().Attribute & (int)game_attributes.ATTRIBUTE_EARTH) > 0)
                            {
                                mod = Program.I().mod_ocgcore_cs_mon_earth;
                            }
                            if ((card.get_data().Attribute & (int)game_attributes.ATTRIBUTE_WATER) > 0)
                            {
                                mod = Program.I().mod_ocgcore_cs_mon_water;
                            }
                            if ((card.get_data().Attribute & (int)game_attributes.ATTRIBUTE_FIRE) > 0)
                            {
                                mod = Program.I().mod_ocgcore_cs_mon_fire;
                            }
                            if ((card.get_data().Attribute & (int)game_attributes.ATTRIBUTE_WIND) > 0)
                            {
                                mod = Program.I().mod_ocgcore_cs_mon_wind;
                            }
                            if ((card.get_data().Attribute & (int)game_attributes.ATTRIBUTE_LIGHT) > 0)
                            {
                                mod = Program.I().mod_ocgcore_cs_mon_light;
                            }
                            if ((card.get_data().Attribute & (int)game_attributes.ATTRIBUTE_DARK) > 0)
                            {
                                mod = Program.I().mod_ocgcore_cs_mon_dark;
                            }
                            mod.GetComponent<partical_scaler>().scale = 2f + Mathf.Clamp(card.get_data().Attack,0,3500) / 3000f * 5f;
                            card.fast_decoration(mod);
                        }
                    }
                    if ((card.get_data().Type & (int)game_type.TYPE_SPELL) > 0)
                    {
                        if (Program.I().setting.setting.Vacts.value == true)
                        {
                            card.positionEffect(Program.I().mod_ocgcore_decoration_magic_activated);
                        }
                    }
                    if ((card.get_data().Type & (int)game_type.TYPE_TRAP) > 0)
                    {
                        if (Program.I().setting.setting.Vactt.value == true)
                        {
                            card.positionShot(Program.I().mod_ocgcore_decoration_trap_activated);
                        }
                    }
                }
                realize();
                break;
            case GameMessage.Chained:
                Sleep(20);
                break;
            case GameMessage.ChainSolved:
                int id = r.ReadByte() - 1   ;
                if (id < 0)
                {
                    id = 0;
                }
                card = null;
                if (id < cardsInChain.Count)
                {
                    card = cardsInChain[id];
                    if (id >= 1)
                    {
                        if (Program.I().setting.setting.Vchain.value == true)
                        {
                            MonoBehaviour.Destroy((GameObject)MonoBehaviour.Instantiate(Program.I().mod_ocgcore_cs_bomb, card.gameObject.transform.position, Quaternion.identity), 5f);
                        }
                    }
                }
                if (card != null)
                {
                    if (card.isShowed == true)
                    {
                        card.isShowed = false;
                        realize();
                        toNearest(true);
                    }
                }
                Sleep(17);
                break;
            case GameMessage.ChainEnd:
                clearChainEnd();
                break;
            case GameMessage.ChainNegated:
            case GameMessage.ChainDisabled:
                int id_ = r.ReadByte() - 1;
                if (id_ < 0)
                {
                    id_ = 0;
                }
                card = null;
                if (id_ < cardsInChain.Count)
                {
                    card = cardsInChain[id_];
                    if (Program.I().setting.setting.Vchain.value == true)
                    {
                        card.fast_decoration(Program.I().mod_ocgcore_cs_negated);
                        Sleep(30);
                    }
                    card.animation_show_off(false, true);
                }
                if (card != null)
                {
                    if (card.isShowed == true)
                    {
                        card.isShowed = false;
                        realize();
                        toNearest(true);
                    }
                }
                break;
            case GameMessage.CardSelected:
                break;
            case GameMessage.RandomSelected:
                pIN = false;
                psum = false;
                player = localPlayer(r.ReadByte());
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    gps = r.ReadGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        if (card.p.location == (UInt32)game_location.LOCATION_SZONE)
                        {
                            if (card.p.sequence == 6 || card.p.sequence == 7)
                            {
                                pIN = true;
                            }
                        }
                        cardsInSelectAnimation.Add(card);
                        card.currentKuang = gameCard.kuangType.selected;
                        if (Program.I().setting.setting.Vchain.value == true)
                            card.add_one_decoration(Program.I().mod_ocgcore_decoration_card_selected, 3, Vector3.zero, "selected", false);
                        if (Program.I().setting.setting.Vpedium.value == true)
                        {
                            Vector3 pvector = Vector3.zero;
                            if (cardsInChain.Count == 0)
                            {
                                if (cardsInSelectAnimation.Count == 2)
                                {
                                    if (cardsInSelectAnimation[0].p.location == (UInt32)game_location.LOCATION_SZONE)
                                    {
                                        if (cardsInSelectAnimation[1].p.location == (UInt32)game_location.LOCATION_SZONE)
                                        {
                                            if (cardsInSelectAnimation[1].p.sequence == 6 || cardsInSelectAnimation[1].p.sequence == 7)
                                            {
                                                if (cardsInSelectAnimation[0].p.sequence == 6 || cardsInSelectAnimation[0].p.sequence == 7)
                                                {
                                                    if (cardsInSelectAnimation[0].p.controller == cardsInSelectAnimation[0].p.controller)
                                                    {
                                                        psum = true;
                                                        if (cardsInSelectAnimation[0].p.controller == 0)
                                                        {
                                                            pvector = new Vector3(0, 0, -9f);
                                                        }
                                                        else
                                                        {
                                                            pvector = new Vector3(0, 0, 9f);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (psum)
                            {
                                float real = (Program.fieldSize - 1) * 0.9f + 1f;
                                Program.I().mod_ocgcore_ss_p_sum_effect.transform.Find("l").localPosition = new Vector3(-15.2f * real, 0, 0);
                                Program.I().mod_ocgcore_ss_p_sum_effect.transform.Find("r").localPosition = new Vector3(14.65f * real, 0, 0);
                                MonoBehaviour.Destroy((GameObject)MonoBehaviour.Instantiate(Program.I().mod_ocgcore_ss_p_sum_effect, pvector, Quaternion.identity), 5f);
                            }
                        }
                    }
                }
                if (!pIN)  
                {
                    Sleep(30);
                }
                break;
            case GameMessage.BecomeTarget:
                int targetTime = 0;
                psum = false;
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    gps = r.ReadGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        if ((card.p.location == (UInt32)game_location.LOCATION_SZONE) && (card.p.sequence == 6 || card.p.sequence == 7))
                        {
                            targetTime += 0;
                        }
                        else if ((card.p.location & (UInt32)game_location.LOCATION_ONFIELD) > 0)
                        {
                            targetTime += 30;
                        }
                        else
                        {
                            targetTime += 50;
                        }
                        cardsInSelectAnimation.Add(card);
                        card.currentKuang = gameCard.kuangType.selected;
                        if (Program.I().setting.setting.Vchain.value == true)
                            card.add_one_decoration(Program.I().mod_ocgcore_decoration_card_selected, 3, Vector3.zero, "selected", false);
                        if (Program.I().setting.setting.Vpedium.value == true)
                        {
                            Vector3 pvector = Vector3.zero;
                            if (cardsInChain.Count == 0)
                            {
                                if (cardsInSelectAnimation.Count == 2)
                                {
                                    if (cardsInSelectAnimation[0].p.location == (UInt32)game_location.LOCATION_SZONE)
                                    {
                                        if (cardsInSelectAnimation[1].p.location == (UInt32)game_location.LOCATION_SZONE)
                                        {
                                            if (cardsInSelectAnimation[1].p.sequence == 6 || cardsInSelectAnimation[1].p.sequence == 7)
                                            {
                                                if (cardsInSelectAnimation[0].p.sequence == 6 || cardsInSelectAnimation[0].p.sequence == 7)
                                                {
                                                    if (cardsInSelectAnimation[0].p.controller == cardsInSelectAnimation[0].p.controller)
                                                    {
                                                        psum = true;
                                                        if (cardsInSelectAnimation[0].p.controller == 0)
                                                        {
                                                            pvector = new Vector3(0, 0, -9f);
                                                        }
                                                        else
                                                        {
                                                            pvector = new Vector3(0, 0, 9f);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (psum)
                            {
                                float real = (Program.fieldSize - 1) * 0.9f + 1f;
                                Program.I().mod_ocgcore_ss_p_sum_effect.transform.Find("l").localPosition = new Vector3(-15.2f * real, 0, 0);
                                Program.I().mod_ocgcore_ss_p_sum_effect.transform.Find("r").localPosition = new Vector3(14.65f * real, 0, 0);
                                MonoBehaviour.Destroy((GameObject)MonoBehaviour.Instantiate(Program.I().mod_ocgcore_ss_p_sum_effect, pvector, Quaternion.identity), 5f);
                            }

                        }
                    }
                }
                Sleep(targetTime);
                break;
            case GameMessage.Draw:
                UIHelper.playSound("draw", 1);
                realize();
                Sleep(10);
                break;
            case GameMessage.PayLpCost:
            case GameMessage.Damage:
                gameInfo.realize();
                player = localPlayer(r.ReadByte());
                val = r.ReadInt32();
                UIHelper.playSound("damage", 1f);
                gameField.animation_show_lp_num(player, false, (int)val);
                if (Program.I().setting.setting.Vdamage.value == true)
                {
                    gameField.animation_screen_blood(player, (int)val);
                }
                Sleep(60);
                break;
            case GameMessage.Recover:
                gameInfo.realize();
                player = localPlayer(r.ReadByte());
                val = r.ReadInt32();
                UIHelper.playSound("gainlp", 1f);
                gameField.animation_show_lp_num(player, true, (int)val);
                Sleep(60);
                break;
            case GameMessage.CardTarget:
            case GameMessage.Equip:
                realize();
                from = r.ReadGPS();
                to = r.ReadGPS();
                gameCard card_from = GCS_cardGet(from, false);
                gameCard card_to = GCS_cardGet(to, false);
                if (card_from != null)
                {
                    UIHelper.playSound("equip", 1f);
                    if (Program.I().setting.setting.Veqquip.value == true)
                    {
                        card_from.fast_decoration(Program.I().mod_ocgcore_decoration_magic_zhuangbei);
                    }
                }
                break;
            case GameMessage.LpUpdate:
                gameInfo.realize();
                break;
            case GameMessage.CancelTarget:
            case GameMessage.Unequip:
                realize();
                break;

            case GameMessage.AddCounter:
                cctype = r.ReadUInt16();
                gps = r.ReadShortGPS();
                card = GCS_cardGet(gps, false);
                count = r.ReadUInt16();
                string name2 = GameStringManager.get("counter", cctype);

                if (card != null)
                {
                    for (int i = 0; i < count; i++)
                    {
                        UIHelper.playSound("addcounter", 1);
                        //if (Program.YGOPro1 == false)
                        {
                            Vector3 pos = ui_helper.get_close(card.gameObject.transform.position, Program.camera_game_main, 5);
                            MonoBehaviour.Destroy((GameObject)MonoBehaviour.Instantiate(Program.I().mod_ocgcore_cs_end, pos, Quaternion.identity), 5f);
                        }
                    }
                }
                RMSshow_none(card.get_data().Name + "  " + InterString.Get("增加指示物：[?]", name2)+" *"+count.ToString());
                Sleep(10);
                break;
            case GameMessage.RemoveCounter:
                cctype = r.ReadUInt16();
                gps = r.ReadShortGPS();
                card = GCS_cardGet(gps, false);
                count = r.ReadUInt16();
                string name = GameStringManager.get("counter", cctype);
                if (card != null)
                {
                    for (int i = 0; i < count; i++)
                    {
                        UIHelper.playSound("removecounter", 1);
                        //if (Program.YGOPro1 == false)
                        {
                            Vector3 pos = ui_helper.get_close(card.gameObject.transform.position, Program.camera_game_main, 5);
                            MonoBehaviour.Destroy((GameObject)MonoBehaviour.Instantiate(Program.I().mod_ocgcore_cs_end, pos, Quaternion.identity), 5f);
                        }
                    }
                }
                RMSshow_none(card.get_data().Name + "  " + InterString.Get("减少指示物：[?]", name) + " *" + count.ToString());
                Sleep(10);
                break;
            case GameMessage.Attack:
                UIHelper.playSound("attack", 1);
                GPS p1 = r.ReadGPS();
                GPS p2 = r.ReadGPS();
                VectorAttackCard = get_point_worldposition(p1);
                VectorAttackTarget = Vector3.zero;
                if (p2.location == 0)
                {
                    bool attacker_bool_me = (p1.controller == 0);
                    if (!attacker_bool_me)
                    {
                        VectorAttackTarget = new Vector3(0, 3, -5f - 15f * Program.fieldSize);
                    }
                    else
                    {
                        if (gameField.isLong)
                        {
                            VectorAttackTarget = new Vector3(0, 3, 2f + (19f + gameField.delat) * Program.fieldSize);
                        }
                        else
                        {
                            VectorAttackTarget = new Vector3(0, 3, 2f + (19f) * Program.fieldSize);
                        }
                    }
                }
                else
                {
                    VectorAttackTarget = get_point_worldposition(p2);
                }
                Arrow.speed = 10;
                Arrow.updateSpeed();
                Sleep(40);



                //shiftArrow(VectorAttackCard, VectorAttackTarget, true, 50);
                //Program.notGo(removeAttackHandler);
                //Program.go(666, removeAttackHandler);



                if (Program.I().setting.setting.Vbattle.value == false)
                {
                    shiftArrow(VectorAttackCard, VectorAttackTarget, true, 50);
                    Program.notGo(removeAttackHandler);
                    Program.go(666, removeAttackHandler);
                }
                else
                {
                    shiftArrow(VectorAttackCard, VectorAttackTarget, true, 200);
                    Program.notGo(removeAttackHandler);
                    Program.go(800, removeAttackHandler);
                }






                //if (Program.I().setting.setting.Vbattle.value == false)
                //{
                //    Arrow.speed = 10;
                //    Arrow.updateSpeed();
                //    Sleep(40);
                //    shiftArrow(VectorAttackCard, VectorAttackTarget, true,50);
                //    Program.notGo(removeAttackHandler);
                //    Program.go(666, removeAttackHandler);
                //}
                //else
                //{
                //    Arrow.speed = 5;
                //    Arrow.updateSpeed();
                //    shiftArrow(VectorAttackCard, VectorAttackTarget, true, 200);
                //    //Program.notGo(removeAttackHandler);
                //    //Program.go(1000, removeAttackHandler);
                //}
                break;
            case GameMessage.Battle:
                if (Program.I().setting.setting.Vbattle.value == true)
                {
                    removeAttackHandler();
                    GPS gpsAttacker = r.ReadShortGPS();
                    r.ReadByte();
                    gameCard attackCard = GCS_cardGet(gpsAttacker, false);
                    if (attackCard != null)
                    {
                        YGOSharp.Card data2 = attackCard.get_data();
                        data2.Attack = r.ReadInt32();
                        data2.Defense = r.ReadInt32();
                        attackCard.set_data(data2);
                    }
                    else
                    {
                        r.ReadInt32();
                        r.ReadInt32();
                    }
                    r.ReadByte();
                    GPS gpsAttacked = r.ReadShortGPS();
                    r.ReadByte();
                    gameCard attackedCard = GCS_cardGet(gpsAttacked, false);
                    if (attackedCard != null && gpsAttacked.location != 0)
                    {
                        YGOSharp.Card data2 = attackedCard.get_data();
                        data2.Attack = r.ReadInt32();
                        data2.Defense = r.ReadInt32();
                        attackedCard.set_data(data2);
                    }
                    else
                    {
                        r.ReadInt32();
                        r.ReadInt32();
                    }
                    r.ReadByte();
                    UIHelper.playSound("explode", 0.4f);
                    int amount = (int)(Mathf.Clamp(attackCard.get_data().Attack, 0, 3500) * 0.8f);
                    iTween.ShakePosition(Program.camera_game_main.gameObject, iTween.Hash(
                                            "x", (float)amount / 1500f,
                                            "y", (float)amount / 1500f,
                                            "z", (float)amount / 1500f,
                                            "time", (float)amount / 2500f
                                            ));
                    VectorAttackCard = get_point_worldposition(gpsAttacker);
                    if (attackedCard == null || gpsAttacked.location == 0)
                    {
                        bool attacker_bool_me = gpsAttacker.controller == 0;
                        if (attacker_bool_me)
                        {
                            VectorAttackTarget = new Vector3(0, 0, 20);
                        }
                        else
                        {
                            VectorAttackTarget = new Vector3(0, 0, -20);
                        }
                    }
                    else
                    {
                        VectorAttackTarget = get_point_worldposition(gpsAttacked);
                        VectorAttackTarget += (VectorAttackTarget - VectorAttackCard) * 0.3f;
                    }
                    if ((attackedCard != null && gpsAttacked.location != 0) && (attackedCard.p.position & (UInt32)game_position.POS_FACEUP_ATTACK) > 0)
                    {
                        if (attackCard.get_data().Attack > attackedCard.get_data().Attack)
                        {
                            animation_battle(VectorAttackCard, VectorAttackTarget, attackCard);
                        }
                        else
                        {
                            animation_battle(VectorAttackTarget, VectorAttackCard, attackedCard);
                        }
                    }
                    else
                    {
                        animation_battle(VectorAttackCard, VectorAttackTarget, attackCard);
                    }
                    Sleep(40);
                }
                break;
            case GameMessage.AttackDiabled:
                //removeAttackHandler();
                break;
            case GameMessage.DamageStepStart:
                break;
            case GameMessage.DamageStepEnd:
                break;
            case GameMessage.BeChainTarget:
                break;
            case GameMessage.CreateRelation:
                break;
            case GameMessage.ReleaseRelation:
                break;
            case GameMessage.TossCoin:
                player = r.ReadByte();
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    data = r.ReadByte();
                    if (i == 0)
                    {
                        tempobj = create_s(Program.I().mod_ocgcore_coin);
                        tempobj.AddComponent<animation_screen_lock>().screen_point = new Vector3(getScreenCenter(), Screen.height / 2, 1);
                        tempobj.GetComponent<coiner>().coin_app();
                        if (data == 0)
                        {
                            tempobj.GetComponent<coiner>().tocoin(false);
                        }
                        else
                        {
                            tempobj.GetComponent<coiner>().tocoin(true);
                        }
                        destroy(tempobj, 7);
                    }
                    if (data == 0)
                    {
                        RMSshow_none(InterString.Get("硬币反面"));
                    }
                    else
                    {
                        RMSshow_none(InterString.Get("硬币正面"));
                    }
                }
                Sleep(280);
                break;
            case GameMessage.TossDice:
                player = r.ReadByte();
                count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    data = r.ReadByte();
                    if (i == 0)
                    {
                        tempobj = create_s(Program.I().mod_ocgcore_dice);
                        tempobj.AddComponent<animation_screen_lock>().screen_point = new Vector3(getScreenCenter(), Screen.height / 2, 1);
                        tempobj.GetComponent<coiner>().dice_app();
                        tempobj.GetComponent<coiner>().todice(data);
                        destroy(tempobj, 7);
                    }
                    RMSshow_none(InterString.Get("骰子结果：[?]", data.ToString()));
                }
                Sleep(280);
                break;
            case GameMessage.AnnounceRace:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                if (condition == Condition.record)
                {
                    Sleep(60);
                }
                destroy(waitObject, 0, false, true);
                player = localPlayer(r.ReadByte());
                ES_min = r.ReadByte();
                available = r.ReadUInt32();
                values = new List<messageSystemValue>();
                for (int i = 0; i < 25; i++)
                {
                    if ((available & (1 << i)) > 0)
                    {
                        values.Add(new messageSystemValue { hint = GameStringManager.get_unsafe(1020 + i), value = (1 << i).ToString() });
                    }
                }
                RMSshow_multipleChoice("returnMultiple", ES_min, values);
                break;
            case GameMessage.AnnounceAttrib:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                if (condition == Condition.record)
                {
                    Sleep(60);
                }
                destroy(waitObject, 0, false, true);
                player = localPlayer(r.ReadByte());
                ES_min = r.ReadByte();
                available = r.ReadUInt32();
                values = new List<messageSystemValue>();
                for (int i = 0; i < 7; i++)
                {
                    if ((available & (1 << i)) > 0)
                    {
                        values.Add(new messageSystemValue { hint = GameStringManager.get_unsafe(1010 + i), value = (1 << i).ToString() });
                    }
                }
                RMSshow_multipleChoice("returnMultiple", ES_min, values);
                break;
            case GameMessage.AnnounceCard:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                if (condition == Condition.record)
                {
                    Sleep(60);
                }
                destroy(waitObject, 0, false, true);
                player = localPlayer(r.ReadByte());
                ES_searchCode.Clear();
                ES_searchCode.Add(r.ReadInt32());
                ES_searchCode.Add((int)YGOSharp.OCGWrapper.Enums.searchCode.OPCODE_ISTYPE);
                RMSshow_input("AnnounceCard", InterString.Get("请输入关键字。"),"");
                break;
            case GameMessage.AnnounceCardFilter:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                if (condition == Condition.record)
                {
                    Sleep(60);
                }
                destroy(waitObject, 0, false, true);
                player = localPlayer(r.ReadByte());
                ES_searchCode.Clear();
                count = r.ReadByte();
                for (int i = 0; i < count; i++) 
                {
                    int take = r.ReadInt32();
                    ES_searchCode.Add(take);
                }
                //values = new List<messageSystemValue>();
                //values.Add(new messageSystemValue { value = "", hint = "" });
                //ES_RMS("AnnounceCard", values);
                RMSshow_input("AnnounceCard", InterString.Get("请输入关键字。"), "");
                break;
            case GameMessage.AnnounceNumber:
                if (inIgnoranceReplay() || inTheWorld())
                {
                    break;
                }
                if (condition == Condition.record)
                {
                    Sleep(60);
                }
                destroy(waitObject, 0, false, true);
                player = localPlayer(r.ReadByte());
                count = r.ReadByte();
                ES_min = 1;
                values = new List<messageSystemValue>();
                for (int i = 0; i < count; i++)
                {
                    values.Add(new messageSystemValue { hint = r.ReadUInt32().ToString(), value = i.ToString() });
                }
                RMSshow_multipleChoice("return", 1, values);
                break;
            case GameMessage.PlayerHint:
                player = localPlayer(r.ReadByte());
                int ptype = r.ReadByte();
                int pvalue = r.ReadInt32();
                string valstring = GameStringManager.get(pvalue);
                if (ptype == 6)
                {
                    if (player == 0)
                    {
                        RMSshow_none(InterString.Get("我方状态：[?]", valstring));
                    }
                    else
                    {
                        RMSshow_none(InterString.Get("对方状态：[?]", valstring));
                    }
                }
                else if (ptype == 7)
                {
                    if (player == 0)
                    {
                        RMSshow_none(InterString.Get("我方取消状态：[?]", valstring));
                    }
                    else
                    {
                        RMSshow_none(InterString.Get("对方取消状态：[?]", valstring));
                    }
                }
                break;
            case GameMessage.CardHint:
                gameCard game_card = GCS_cardGet(r.ReadGPS(), false);
                int ctype = r.ReadByte();
                int value = r.ReadInt32();
                if (game_card != null)
                {
                    if (ctype == 1)
                    {
                        animation_confirm(game_card);
                        var number = game_card.add_one_decoration(Program.I().mod_ocgcore_number, 3, new Vector3(60, 0, 0), "number", false);
                        number.game_object.GetComponent<number_loader>().set_number((int)value, 3);
                        number.scale_change_ignored = true;
                        number.game_object.transform.localScale = new Vector3(1, 1, 1);
                        number.game_object.transform.eulerAngles = new Vector3(60, 0, 0);
                        destroy(number.game_object, 2.2f);
                        Sleep(42);
                    }
                }
                break;
            case GameMessage.TagSwap:
                realize(true);
                arrangeCards();
                player = localPlayer(r.ReadByte());
                animation_suffleHand(player);
                Sleep(21);
                break;
            case GameMessage.AiName:
                break;
            case GameMessage.MatchKill:
                break;
            case GameMessage.CustomMsg:
                break;
            case GameMessage.DuelWinner:
                break;
            default:
                break;
        }
        r.BaseStream.Seek(0, 0);
    }

    private void createPlaceSelector(byte[] resp)
    {
        for (int i = 0; i < placeSelectors.Count; i++)
        {
            if (placeSelectors[i].data[0] == resp[0])
            {
                if (placeSelectors[i].data[1] == resp[1])
                {
                    if (placeSelectors[i].data[2] == resp[2])
                    {
                        return;
                    }
                }
            }
        }
        uint player_m = (uint)localPlayer(resp[0]);
        uint location = resp[1];
        uint index = resp[2];
        GPS newP = new GPS();
        newP.controller = player_m;
        newP.location = location;
        newP.sequence = index;
        newP.position = 0;
        Vector3 worldVector = get_point_worldposition(newP, null);
        var placs = create(Program.I().New_ocgcore_placeSelector, worldVector, Vector3.zero, false, null, true, Vector3.one).GetComponent<placeSelector>();
        placs.data = new byte[3];
        placs.data[0] = resp[0];
        placs.data[1] = resp[1];
        placs.data[2] = resp[2];
        placeSelectors.Add(placs);
        if (location == (uint)game_location.LOCATION_MZONE && Program.I().setting.setting.hand.value == false)
        {
            ES_placeSelected(placs);
        }
        if (location == (uint)game_location.LOCATION_SZONE && Program.I().setting.setting.handm.value == false)
        {
            ES_placeSelected(placs);
        }
    }

    private void animation_suffleHand(int player)
    {
        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
            {
                if ((cards[i].p.location & (UInt32)game_location.LOCATION_HAND) > 0)
                {
                    if (cards[i].p.controller == player)
                    {
                        Vector3 position;
                        if (cards[i].p.controller == 0)
                        {
                            position = new Vector3(0, 0, -3f - 15f * Program.fieldSize);
                        }
                        else
                        {
                            if (gameField.isLong)
                            {
                                position = new Vector3(0, 0, (19f + gameField.delat) * Program.fieldSize);
                            }
                            else
                            {
                                position = new Vector3(0, 0, (19f) * Program.fieldSize);
                            }
                        }
                        cards[i].animation_rush_to(position, new Vector3(-30, 0, 180));
                    }
                }
            }
    }

    private void clearChainEnd()
    {
        //removeAttackHandler();
        removeSelectedAnimations();
    }

    private void logicalClearChain()
    {
        for (int i = 0; i < cardsInChain.Count; i++)
        {
            cardsInChain[i].CS_clear();
        }
        cardsInChain.Clear();
    }

    private void showWait()
    {
        if (waitObject == null)
        {
            waitObject = create_s(Program.I().new_ocgcore_wait, Program.camera_main_2d.ScreenToWorldPoint(new Vector3(getScreenCenter(), Screen.height - 15f - 15f * (1.21f - Program.fieldSize) / 0.21f)), Vector3.zero, true, Program.ui_main_2d, true);
        }
    }

    void removeAttackHandler()
    {
        shiftArrow(Vector3.zero,Vector3.zero,false,50);
    }

    private void removeSelectedAnimations() 
    {
        for (int i = 0; i < cardsInSelectAnimation.Count; i++)
        {
            try
            {
                cardsInSelectAnimation[i].del_all_decoration_by_string("selected");
                cardsInSelectAnimation[i].currentKuang = gameCard.kuangType.none;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
        }
        cardsInSelectAnimation.Clear();
    }


    private void confirm(gameCard card)
    {
        Program.go(cardsForConfirm.Count * 700, confirmGPS);
        cardsForConfirm.Add(card);
    }

    private void Nconfirm()
    {
        Program.notGo(confirmGPS);
        cardsForConfirm.Clear();
    }

    List<gameCard> cardsForConfirm = new List<gameCard>();

    void confirmGPS()
    {
        if (cardsForConfirm.Count > 0)
        {
            animation_confirm(cardsForConfirm[0]);
            cardsForConfirm.RemoveAt(0);
        }
    }

    string ES_hint = "";

    string ES_selectHint = "";
    int Es_selectMSGHintType = 0;
    int Es_selectMSGHintPlayer = 0;
    int Es_selectMSGHintData = 0;

    List<gameCard> MHS_getBundle(int controller, int location)
    {
        List<gameCard> cardsInLocation = new List<gameCard>();
        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
            {
                if (cards[i].p.location == location)
                {
                    if (cards[i].p.controller == controller)
                    {
                        cardsInLocation.Add(cards[i]);
                    }
                }
            }

        return cardsInLocation;
    }

    void MHS_creatBundle(int count, int player, game_location location)
    {
        for (int i = 0; i < count; i++)
        {
            GCS_cardCreate(new GPS
            {
                controller = (UInt32)player,
                location = (UInt32)location,
                position = (int)game_position.POS_FACEDOWN_ATTACK,
                sequence = (UInt32)i,
            });
        }
    }

    List<gameCard> MHS_resizeBundle(int count, int player, game_location location)
    {
        List<gameCard> cardBow = new List<gameCard>();
        List<gameCard> waterOutOfBow = new List<gameCard>();
        for (int i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
            {
                if ((cards[i].p.location & (UInt32)location) > 0)
                {
                    if (cards[i].p.controller == player)
                    {
                        if (cardBow.Count < count)
                        {
                            cardBow.Add(cards[i]);
                        }
                        else
                        {
                            waterOutOfBow.Add(cards[i]);
                        }
                    }
                }
            }

        for (int i = 0; i < waterOutOfBow.Count; i++)
        {
            waterOutOfBow[i].hide();
        }
        while (cardBow.Count < count)
        {
            cardBow.Add(GCS_cardCreate(new GPS
            {
                controller = (UInt32)player,
                location = (UInt32)location,
                position = (int)game_position.POS_FACEDOWN_ATTACK,
                sequence = (UInt32)(cardBow.Count),
            }));
        }
        for (int i = 0; i < cardBow.Count; i++)
        {
            cardBow[i].erase_data();
            cardBow[i].p.position = (int)game_position.POS_FACEDOWN_ATTACK;
        }
        return cardBow;
    }

    void animation_battle(Vector3 VectorAttackedCard, Vector3 VectorAttackTarget, gameCard attackCard)
    {
        cookie_AttackEffect = (GameObject)MonoBehaviour.Instantiate(prewarmAttackEffect(attackCard, VectorAttackedCard, VectorAttackTarget), Vector3.zero, Quaternion.identity);
        cookie_AttackEffect.AddComponent<partical_scaler>().scale = 10f * Mathf.Clamp(attackCard.get_data().Attack, 0, 3500) / 1500f;
        MonoBehaviour.Destroy(cookie_AttackEffect, 3);
    }

    int ES_min = 0;

    int ES_max = 0;

    int ES_level = 0;

    bool ES_overFlow = false;

    int ES_sortSum = 0;

    List<int> ES_searchCode = new List<int>();


    class sortResult
    {
        public gameCard card = null;
        public int option = 0;
    }

    List<sortResult> ES_sortResult = new List<sortResult>();

    List<gameCard> cardsInChain = new List<gameCard>();

    List<gameCard> cardsInSelectAnimation = new List<gameCard>();

    List<gameCard> allCardsInSelectMessage = new List<gameCard>();

    List<gameCard> cardsSelected = new List<gameCard>();

    List<gameCard> cardsMustBeSelected = new List<gameCard>();

    List<gameCard> cardsSelectable = new List<gameCard>();

    List<gameCard> cardsInSort = new List<gameCard>();

    GameObject cookie_AttackEffect = null;

    GameObject prewarmAttackEffect(gameCard card, Vector3 from, Vector3 to)
    {
        GameObject mod = Program.I().mod_ocgcore_bs_atk_line_earth;
        if (GameStringHelper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_EARTH))
            mod = Program.I().mod_ocgcore_bs_atk_line_earth;
        if (GameStringHelper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_WATER))
            mod = Program.I().mod_ocgcore_bs_atk_line_water;
        if (GameStringHelper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_FIRE))
            mod = Program.I().mod_ocgcore_bs_atk_line_fire;
        if (GameStringHelper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_WIND))
            mod = Program.I().mod_ocgcore_bs_atk_line_wind;
        if (GameStringHelper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_DARK))
            mod = Program.I().mod_ocgcore_bs_atk_line_dark;
        if (GameStringHelper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_LIGHT))
            mod = Program.I().mod_ocgcore_bs_atk_line_light;
        if (GameStringHelper.differ(card.get_data().Attribute, (long)game_attributes.ATTRIBUTE_DEVINE))
            mod = Program.I().mod_ocgcore_bs_atk_line_light;
        mod.transform.GetChild(0).localPosition = to;
        mod.transform.GetChild(1).localPosition = from;
        return mod;
    }

    public void realizeCardsForSelect()
    {

        for (int i = 0; i < allCardsInSelectMessage.Count; i++)
        {
            allCardsInSelectMessage[i].del_all_decoration();
            allCardsInSelectMessage[i].isShowed = false;
            allCardsInSelectMessage[i].show_number(0);
            allCardsInSelectMessage[i].currentFlash = gameCard.flashType.none;
        }

        cardsSelectable.Clear();

        getSelectableCards();

        if (cardsSelected.Count == 0)
        {
            if (UIHelper.fromStringToBool(Config.Get("smartSelect_", "1")))
            {
                switch (currentMessage)
                {
                    case GameMessage.SelectTribute:
                        if (cardsSelectable.Count == 1)
                        {
                            autoSendCards();
                            return;
                        }
                        int all = 0;
                        for (int i = 0; i < cardsSelectable.Count; i++)
                        {
                            all += cardsSelectable[i].levelForSelect_1;
                        }
                        if (all == ES_min)
                        {
                            autoSendCards();
                            return;
                        }
                        break;
                    case GameMessage.SelectCard:
                    case GameMessage.SelectUnselectCard:
                        if (cardsSelectable.Count <= ES_min)
                        {
                            autoSendCards();
                            return;
                        }
                        if (ES_min == ES_max)
                        {
                            if (ifAllCardsInSameCode(cardsSelectable))
                            {
                                if (ifAllCardsInSameController(cardsSelectable))
                                {
                                    if (ifAllCardsInSameLocation(cardsSelectable))
                                    {
                                        autoSendCards();
                                        return;
                                    }
                                }
                            }
                        }
                        break;
                    case GameMessage.SelectSum:
                        if (cardsSelectable.Count <= ES_min)
                        {
                            autoSendCards();
                            return;
                        }
                        bool allSame = true;
                        int selectableLevel = 0;
                        for (int x = 0; x < cardsMustBeSelected.Count; x++)
                        {
                            selectableLevel += cardsMustBeSelected[x].levelForSelect_1;
                        }
                        for (int x = 0; x < cardsSelectable.Count; x++)
                        {
                            selectableLevel += cardsSelectable[x].levelForSelect_1;
                        }
                        if (selectableLevel != ES_level)
                        {
                            allSame = false;
                        }
                        selectableLevel = 0;
                        for (int x = 0; x < cardsMustBeSelected.Count; x++)
                        {
                            selectableLevel += cardsMustBeSelected[x].levelForSelect_2;
                        }
                        for (int x = 0; x < cardsSelectable.Count; x++)
                        {
                            selectableLevel += cardsSelectable[x].levelForSelect_2;
                        }
                        if (selectableLevel != ES_level)
                        {
                            allSame = false;
                        }
                        if (allSame)
                        {
                            autoSendCards();
                            return;
                        }
                        break;
                }
            }
        }

        for (int i = 0; i < cardsSelectable.Count; i++)
        {
            cardsSelectable[i].add_one_decoration(Program.I().mod_ocgcore_decoration_card_selecting, 2, Vector3.zero, "card_selecting");
            cardsSelectable[i].isShowed = true;
            cardsSelectable[i].currentFlash = gameCard.flashType.Select;
        }

        for (int x = 0; x < cardsMustBeSelected.Count; x++)
        {
            if (currentMessage == GameMessage.SelectSum)
            {
                cardsMustBeSelected[x].show_number((int)(cardsMustBeSelected[x].levelForSelect_2));
            }
            else
            {
                cardsMustBeSelected[x].show_number((int)(x + 1));
            }
            cardsMustBeSelected[x].isShowed = true;
        }

        for (int x = 0; x < cardsSelected.Count; x++)
        {
            if (currentMessage == GameMessage.SelectSum)
            {
                cardsSelected[x].show_number((int)(cardsSelected[x].levelForSelect_2));
            }
            else
            {
                cardsSelected[x].show_number((int)(x + 1));
            }
            cardsSelected[x].isShowed = true;
        }

        bool sendable = false;
        bool real_send = false;

        if (currentMessage == GameMessage.SelectSum)
        {
            if (cardsSelected.Count == ES_max)
            {
                sendable = true;
            }
            int selectedLevel = 0;
            for (int x = 0; x < cardsMustBeSelected.Count; x++)
            {
                selectedLevel += cardsMustBeSelected[x].levelForSelect_1;
            }
            for (int x = 0; x < cardsSelected.Count; x++)
            {
                selectedLevel += cardsSelected[x].levelForSelect_1;
            }
            if (ES_overFlow)
            {
                if (selectedLevel >= ES_level)
                {
                    sendable = true;
                    real_send = true;
                }
            }
            else
            {
                if (selectedLevel == ES_level)
                {
                    sendable = true;
                }
            }
            selectedLevel = 0;
            for (int x = 0; x < cardsMustBeSelected.Count; x++)
            {
                selectedLevel += cardsMustBeSelected[x].levelForSelect_2;
            }
            for (int x = 0; x < cardsSelected.Count; x++)
            {
                selectedLevel += cardsSelected[x].levelForSelect_2;
            }
            if (ES_overFlow)
            {
                if (selectedLevel >= ES_level)
                {
                    sendable = true;
                    real_send = true;
                }
            }
            else
            {
                if (selectedLevel == ES_level)
                {
                    sendable = true;
                }
            }
            if (cardsSelectable.Count == 0)
            {
                sendable = true;
                real_send = true;
            }
        }
        if (currentMessage == GameMessage.SelectCard)
        {
            if (cardsSelected.Count >= ES_min)
            {
                sendable = true;
            }
            if (cardsSelected.Count == ES_max || cardsSelected.Count == cardsSelectable.Count)
            {
                sendable = true;
                real_send = true;
            }
        }
        if (currentMessage == GameMessage.SelectUnselectCard)
        {
            if (cardsSelected.Count >= ES_min)
            {
                sendable = true;
            }
            if (cardsSelected.Count == ES_max || cardsSelected.Count == cardsSelectable.Count)
            {
                sendable = true;
                real_send = true;
            }
        }
        if (currentMessage == GameMessage.SelectTribute)
        {
            int all = 0;
            for (int i = 0; i < cardsSelected.Count; i++)
            {
                all += cardsSelected[i].levelForSelect_1;
            }
            if (all >= ES_min)
            {
                sendable = true;
            }
            if (all >= ES_max)
            {
                sendable = true;
                if (cardsSelectable.Count == 1)
                {
                    real_send = true;
                }
            }
            if (cardsSelected.Count == cardsSelectable.Count)
            {
                sendable = true;
                real_send = true;
            }
            if (cardsSelected.Count == ES_max)
            {
                sendable = true;
                real_send = true;
            }
        }

        if (sendable)
        {
            if (real_send)
            {
                gameInfo.removeHashedButton("sendSelected");
                sendSelectedCards();
            }
            else
            {
                if (gameInfo.queryHashedButton("sendSelected") == false)
                {
                    gameInfo.addHashedButton("sendSelected", 0, superButtonType.yes, InterString.Get("完成选择@ui"));
                }
            }
        }
        else
        {
            gameInfo.removeHashedButton("sendSelected");
        }


        realize();
        toNearest();
    }

    private void getSelectableCards()
    {
        if (currentMessage == GameMessage.SelectCard || currentMessage == GameMessage.SelectUnselectCard)
        {
            for (int i = 0; i < allCardsInSelectMessage.Count; i++)
            {
                cardsSelectable.Add(allCardsInSelectMessage[i]);
            }
        }
        if (currentMessage == GameMessage.SelectTribute)
        {
            for (int i = 0; i < allCardsInSelectMessage.Count; i++)
            {
                cardsSelectable.Add(allCardsInSelectMessage[i]);
            }
        }
        if (currentMessage == GameMessage.SelectSum)
        {
            int selectedLevel = 0;
            for (int x = 0; x < cardsMustBeSelected.Count; x++)
            {
                selectedLevel += cardsMustBeSelected[x].levelForSelect_1;
            }
            for (int x = 0; x < cardsSelected.Count; x++)
            {
                selectedLevel += cardsSelected[x].levelForSelect_1;
            }
            checkSum(selectedLevel);
            selectedLevel = 0;
            for (int x = 0; x < cardsMustBeSelected.Count; x++)
            {
                selectedLevel += cardsMustBeSelected[x].levelForSelect_2;
            }
            for (int x = 0; x < cardsSelected.Count; x++)
            {
                selectedLevel += cardsSelected[x].levelForSelect_2;
            }
            checkSum(selectedLevel);
        }
    }

    private static bool ifAllCardsInSameLocation(List<gameCard> cards)
    {
        bool re = true;
        if (cards.Count > 0)
        {
            UInt32 loc = cards[0].p.location;
            if (loc != (UInt32)game_location.LOCATION_DECK)
            {
                return false;
            }
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].p.location != loc)
                {
                    re = false;
                }
            }
        }
        return re;
    }

    private static bool ifAllCardsInSameController(List<gameCard> cards)
    {
        bool re = true;
        if (cards.Count > 0)
        {
            UInt32 con = cards[0].p.controller;
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].p.controller != con)
                {
                    re = false;
                }
            }
        }
        return re;
    }

    private static bool ifAllCardsInSameCode(List<gameCard> cards)
    {
        bool re = true;
        if (cards.Count > 0)
        {
            int code = cards[0].get_data().Id;
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].get_data().Id != code)
                {
                    re = false;
                }
                if (cards[i].get_data().Id == 0)
                {
                    re = false;
                }
            }
        }
        return re;
    }

    public static List<List<gameCard>> GetCombination(List<gameCard> t, int n)//卡片全部放到t里面，n是小于selectMax的任意整数，返回卡片张数为n的卡片全组合
    {
        if (t.Count < n)
        {
            return null;
        }
        int[] temp = new int[n];
        List<List<gameCard>> list = new List<List<gameCard>>();
        GetCombination(ref list, t, t.Count, n, temp, n);
        return list;
    }

    private static void GetCombination(ref List<List<gameCard>> list, List<gameCard> t, int n, int m, int[] b, int M)
    {
        for (int i = n; i >= m; i--)
        {
            b[m - 1] = i - 1;
            if (m > 1)
            {
                GetCombination(ref list, t, i - 1, m - 1, b, M);
            }
            else
            {
                if (list == null)
                {
                    list = new List<List<gameCard>>();
                }
                List<gameCard> temp = new List<gameCard>();
                for (int j = 0; j < b.Length; j++)
                {
                    temp.Add(t[b[j]]);
                }
                list.Add(temp);
            }
        }
    }

    private bool queryCorrectOverSumList(List<gameCard> temp,int sumlevel)  
    {
        int illusionCount = temp.Count - cardsMustBeSelected.Count;
        if (illusionCount < ES_min)
        {
            return false;
        }
        if (illusionCount > ES_max)
        {
            return false;
        }
        int okCount = 0;
        for (int i = 1; i <= temp.Count; i++)
        {
            List<List<gameCard>> totalCobination = GetCombination(temp, i);
            for (int i2 = 0; i2 < totalCobination.Count; i2++)
            {
                bool re = false;
                int sumillustration = 0;
                for (int i3 = 0; i3 < totalCobination[i2].Count; i3++)
                {
                    sumillustration += totalCobination[i2][i3].levelForSelect_1;
                }
                if (sumillustration >= sumlevel)
                {
                    re = true;
                }
                sumillustration = 0;
                for (int i3 = 0; i3 < totalCobination[i2].Count; i3++)
                {
                    sumillustration += totalCobination[i2][i3].levelForSelect_2;
                }
                if (sumillustration >= sumlevel)
                {
                    re = true;
                }
                if (re)
                {
                    okCount++;
                }
            }
        }
        return (okCount == 1);
    }

    void checkSum(int star)
    {
        List<gameCard> cards_remain_unselected = getUnselectedCards();
        if (ES_overFlow)
        {
            for (int i = 1; i <= cards_remain_unselected.Count; i++)
            {
                List<List<gameCard>> totalCobination = GetCombination(cards_remain_unselected, i);
                for (int i2 = 0; i2 < totalCobination.Count; i2++)
                {
                    List<gameCard> selectIllusion = new List<gameCard>();
                    for (int x = 0; x < totalCobination[i2].Count; x++)
                    {
                        selectIllusion.Add(totalCobination[i2][x]);
                    }
                    for (int x = 0; x < cardsSelected.Count; x++)
                    {
                        selectIllusion.Add(cardsSelected[x]);
                    }
                    for (int x = 0; x < cardsMustBeSelected.Count; x++)
                    {
                        selectIllusion.Add(cardsMustBeSelected[x]);
                    }
                    if (queryCorrectOverSumList(selectIllusion, ES_level) == true)
                    {
                        for (int i3 = 0; i3 < totalCobination[i2].Count; i3++)
                        {
                            cardsSelectable.Remove(totalCobination[i2][i3]);
                            cardsSelectable.Add(totalCobination[i2][i3]);
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < cards_remain_unselected.Count; i++)
            {
                List<gameCard> selectIllusion = new List<gameCard>();
                for (int x = 0; x < cards_remain_unselected.Count; x++)
                {
                    if (x != i)
                    {
                        selectIllusion.Add(cards_remain_unselected[x]);
                    }
                }
                bool r = checkSum_process(selectIllusion, (int)ES_level - star - cards_remain_unselected[i].levelForSelect_1, cardsSelected.Count + 1);
                if (!r && cards_remain_unselected[i].levelForSelect_1 != cards_remain_unselected[i].levelForSelect_2)
                {
                    r = checkSum_process(selectIllusion, (int)ES_level - star - cards_remain_unselected[i].levelForSelect_2,cardsSelected.Count + 1);
                }
                if (r)
                {
                    cardsSelectable.Remove(cards_remain_unselected[i]);
                    cardsSelectable.Add(cards_remain_unselected[i]);
                }
            }
        }

    }

    private List<gameCard> getUnselectedCards()
    {
        List<gameCard> cards_remain_unselected = new List<gameCard>();
        for (int x = 0; x < allCardsInSelectMessage.Count; x++)
        {
            cards_remain_unselected.Add(allCardsInSelectMessage[x]);
        }
        for (int x = 0; x < cardsSelected.Count; x++)
        {
            cards_remain_unselected.Remove(cardsSelected[x]);
        }
        for (int x = 0; x < cardsMustBeSelected.Count; x++)
        {
            cards_remain_unselected.Remove(cardsMustBeSelected[x]);
        }

        return cards_remain_unselected;
    }

    bool checkSum_process(List<gameCard> cards_temp, int sum, int selectedCount)    
    {
        if (sum == 0)
        {
            if (selectedCount < ES_min)
            {
                return false;
            }
            if (selectedCount > ES_max)
            {
                return false;
            }
            return true;
        }
        if (sum < 0)
        {
            return false;
        }

        for (int i = 0; i < cards_temp.Count; i++)
        {
            List<gameCard> new_cards = new List<gameCard>();
            for (int x = 0; x < cards_temp.Count; x++)
            {
                if (x != i)
                {
                    new_cards.Add(cards_temp[x]);
                }
            }
            bool r = checkSum_process(new_cards, sum - cards_temp[i].levelForSelect_1, selectedCount + 1);
            if (!r && cards_temp[i].levelForSelect_1 != cards_temp[i].levelForSelect_2)
            {
                r = checkSum_process(new_cards, sum - cards_temp[i].levelForSelect_2, selectedCount + 1);
            }
            if (r)
            {
                return r;
            }
        }

        return false;
    }

    void autoSendCards()
    {
        BinaryMaster m = new BinaryMaster();
        switch (currentMessage)
        {
            case GameMessage.SelectCard:
            case GameMessage.SelectUnselectCard:
            case GameMessage.SelectTribute:
                int c = ES_min;
                if (cardsSelectable.Count < c)
                {
                    c = cardsSelectable.Count;
                }
                m.writer.Write((byte)(c));
                for (int i = 0; i < c; i++)
                {
                    m.writer.Write((byte)(cardsSelectable[i].selectPtr));
                    lastExcitedController = (int)cardsSelectable[i].p.controller;
                    lastExcitedLocation = (int)cardsSelectable[i].p.location;
                }
                sendReturn(m.get());
                break;
            case GameMessage.SelectSum:
                m = new BinaryMaster();
                m.writer.Write((byte)(cardsMustBeSelected.Count + cardsSelectable.Count));
                for (int i = 0; i < cardsMustBeSelected.Count; i++)
                {
                    m.writer.Write((byte)i);
                }
                for (int i = 0; i < cardsSelectable.Count; i++)
                {
                    m.writer.Write((byte)(cardsSelectable[i].selectPtr));
                    lastExcitedController = (int)cardsSelectable[i].p.controller;
                    lastExcitedLocation = (int)cardsSelectable[i].p.location;
                }
                sendReturn(m.get());
                break;
        }
    }

    void sendSelectedCards()
    {
        BinaryMaster m;
        switch (currentMessage)
        {
            case GameMessage.SelectCard:
            case GameMessage.SelectUnselectCard:
            case GameMessage.SelectTribute:
            case GameMessage.SelectSum:
                m = new BinaryMaster();
                if (currentMessage == GameMessage.SelectUnselectCard && cardsSelected.Count == 0)
                {
                    m.writer.Write((Int32)(-1));
                    sendReturn(m.get());
                    break;
                }
                m.writer.Write((byte)(cardsMustBeSelected.Count + cardsSelected.Count));
                for (int i = 0; i < cardsMustBeSelected.Count; i++)
                {
                    m.writer.Write((byte)i);
                }
                for (int i = 0; i < cardsSelected.Count; i++)
                {
                    m.writer.Write((byte)(cardsSelected[i].selectPtr));
                    lastExcitedController = (int)cardsSelected[i].p.controller;
                    lastExcitedLocation = (int)cardsSelected[i].p.location;
                }
                sendReturn(m.get());
                break;
        }
    }

    int lastExcitedLocation = -1;
    int lastExcitedController = -1;
    bool clearAllShowedB = false;
    bool clearTimeFlag = false;

    void clearResponse()
    {

        flagForTimeConfirm = false;
        flagForCancleChain = false;
        //Package p = new Package();
        //p.Fuction = (int)YGOSharp.OCGWrapper.Enums.GameMessage.sibyl_clear;
        //TcpHelper.AddRecordLine(p);
        if (clearTimeFlag)
        {
            clearTimeFlag = false;
            MessageBeginTime = 0;
        }
        ES_selectHint = "";
        cardsInSort.Clear();
        allCardsInSelectMessage.Clear();
        cardsSelected.Clear();
        cardsMustBeSelected.Clear();
        cardsSelectable.Clear();
        ES_sortResult.Clear();
        //cardsForConfirm.Clear();
        //Program.notGo(confirmGPS);
        gameField.Phase.colliderMp2.enabled = false;
        gameField.Phase.colliderBp.enabled = false;
        gameField.Phase.colliderEp.enabled = false;

        toDefaultHint();

        clearAllSelectPlace();

        int myMaxDeck = countLocationSequence(0, game_location.LOCATION_DECK);

        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
            {
                cards[i].remove_all_cookie_button();
                cards[i].show_number(0);
                cards[i].del_all_decoration();
                cards[i].sortOptions.Clear();
                cards[i].currentFlash = gameCard.flashType.none;
                cards[i].prefered = false;
                if (cards[i].forSelect)
                {
                    cards[i].forSelect = false;
                    cards[i].isShowed = false;
                    if ((cards[i].p.location & (UInt32)game_location.LOCATION_DECK) > 0)
                    {
                        if (deckReserved == false || cards[i].p.controller != 0 || cards[i].p.sequence != myMaxDeck)
                        {
                            cards[i].erase_data();
                        }
                    }
                }
                cards[i].effects.Clear();
                if ((int)cards[i].p.location == lastExcitedLocation)
                {
                    if ((int)cards[i].p.controller == lastExcitedController)
                    {
                        cards[i].isShowed = false;
                    }
                }
                if (cards[i].p.location == (uint)game_location.LOCATION_DECK)
                {
                    cards[i].isShowed = false;
                }
                if (clearAllShowedB)
                {
                    cards[i].isShowed = false;
                }
            }
        clearAllShowedB = false;
        lastExcitedLocation = -1;
        lastExcitedController = -1;
        List<gameCard> to_clear = new List<gameCard>();
        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
            {
                if (cards[i].p.location == (uint)game_location.search)
                {
                    to_clear.Add(cards[i]);
                }
            }

        for (int i = 0; i < to_clear.Count; i++)
        {
            to_clear[i].hide();
            to_clear[i].p.location = (UInt32)game_location.LOCATION_UNKNOWN;
        }
        gameInfo.removeAll();
        RMSshow_clear();
        realize();
        toNearest();
    }

    private void clearAllSelectPlace()
    {
        for (int i = 0; i < placeSelectors.Count; i++)
        {
            if (placeSelectors[i] != null)
            {
                if (placeSelectors[i].gameObject != null)
                {
                    MonoBehaviour.DestroyImmediate(placeSelectors[i].gameObject);
                }
            }
        }
        placeSelectors.Clear();
    }

    public void Sleep(int framsIn60)    
    {
        int illustion = (int)(Program.TimePassed() + framsIn60 * 1000f / 60f);
        if (illustion > MessageBeginTime)
        {
            MessageBeginTime = illustion;
        }
    }

    public bool isFirst = false;

    public bool isObserver = false;

    public void StocMessage_TimeLimit(BinaryReader r)
    {
        int player = r.ReadByte();
        r.ReadByte();
        int time_limit = r.ReadInt16();
        TcpHelper.CtosMessage_TimeConfirm();
        gameInfo.setTime(unSwapPlayer(localPlayer(player)), time_limit);
        if (unSwapPlayer(localPlayer(player)) == 0)
        {
            destroy(waitObject, 0, false, true);
        }
    }

    public int localPlayer(int p)
    {
        if (p == 0 || p == 1)
        {
            if (isFirst)
            {
                return p;
            }
            else
            {
                return 1 - p;
            }
        }
        else
        {
            return p;
        }
    }

    bool someCardIsShowed = false;

    public void realize(bool rush = false)
    {
        someCardIsShowed = false;
        float real = (Program.fieldSize - 1) * 0.9f + 1f;
        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
            {
                cards[i].cookie_cared = false;
                cards[i].p_line_off();
                cards[i].sortButtons();
                cards[i].opMonsterWithBackGroundCard = false;
                cards[i].isMinBlockMode = false;
                cards[i].overFatherCount = 0;
            }

        List<gameCard> to_clear = new List<gameCard>();
        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
            {
                if (cards[i].p.location == (uint)game_location.LOCATION_UNKNOWN)
                {
                    to_clear.Add(cards[i]);
                }
            }

        for (int i = 0; i < to_clear.Count; i++)
        {
            to_clear[i].hide();
        }

        //for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
        //        if (cards[i].cookie_cared == false)
        //        {
        //            if (winner == 2 || (winner != -1 && cards[i].p.controller != winner))
        //            {
        //                cards[i].cookie_cared = true;
        //                cards[i].UA_give_condition(gameCardCondition.still_unclickable);
        //                if (cards[i].p.controller == 0)
        //                {
        //                    cards[i].UA_give_position(new Vector3(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, -25f)));

        //                }
        //                else
        //                {
        //                    cards[i].UA_give_position(new Vector3(UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(0f, 5f), UnityEngine.Random.Range(5f, 22f)));

        //                }
        //                cards[i].UA_give_rotation(new Vector3(UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f)));
        //                cards[i].UA_flush_all_gived_witn_lock(rush);
        //            }
        //        }

        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                if (cards[i].cookie_cared == false)
                {
                    if ((cards[i].p.location & (UInt32)game_location.LOCATION_OVERLAY) == 0)
                    {
                        if ((cards[i].p.location & (UInt32)game_location.LOCATION_SZONE) > 0)
                        {
                            cards[i].isShowed = false;
                        }
                        if ((cards[i].p.location & (UInt32)game_location.LOCATION_MZONE) > 0)
                        {
                            cards[i].isShowed = false;
                        }
                    }
                    if ((((cards[i].p.location & (UInt32)game_location.LOCATION_HAND) > 0) && (cards[i].p.controller == 0)) || ((cards[i].p.location & (UInt32)game_location.LOCATION_UNKNOWN) > 0))
                    {
                        cards[i].isShowed = true;
                    }
                    else
                    {
                        if (cards[i].isShowed && cards[i].forSelect == false)
                        {
                            someCardIsShowed = true;
                        }
                    }
                }

        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
            {
                if (cards[i].p.location == (uint)game_location.search)
                {
                    cards[i].isShowed = true;
                }
            }

        List<List<gameCard>> lines = new List<List<gameCard>>();
        UInt32 preController = 9999;
        UInt32 preLocation = 9999;
        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                if (cards[i].cookie_cared == false)
                {
                    if (cards[i].isShowed == true)
                    {
                        int lineMax = 8;
                        if (lines.Count <= 1)
                        {
                            lineMax = 6;
                        }
                        if (
                        preController != cards[i].p.controller
                        ||
                        preLocation != cards[i].p.location
                        ||
                        lines[lines.Count - 1].Count == lineMax
                            )
                        {
                            lines.Add(new List<gameCard>());
                        }
                        lines[lines.Count - 1].Add(cards[i]);
                        preController = cards[i].p.controller;
                        preLocation = cards[i].p.location;
                    }
                }

        if (lines.Count >= 2)
        {
            var lastLine = lines[lines.Count - 1];
            var preLine = lines[lines.Count - 2];
            if (lastLine.Count == 1)
            {
                if (preLine.Count > 0)   
                {
                    if (lastLine[0].p.controller == preLine[0].p.controller)
                    {
                        if (lastLine[0].p.location == preLine[0].p.location)
                        {
                            preLine.Add(lastLine[0]);
                            lines.Remove(lastLine);
                        }
                    }
                }
            }
        }

        for (int line_index = 0; line_index < lines.Count; line_index++)
        {
            for (int index = 0; index < lines[line_index].Count; index++)
            {
                Vector3 want_position = Vector3.zero;
                want_position.y = 0;
                want_position.z = -line_index * 5 - 3f - 15f * Program.fieldSize;
                if (line_index == 0)
                {
                    want_position.x = UIHelper.get_left_right_indexEnhanced(-10, 10, index, lines[line_index].Count, 5);
                }
                else
                {
                    want_position.x = UIHelper.get_left_right_indexEnhanced(-15, 15, index, lines[line_index].Count, 7);
                }
                lines[line_index][index].cookie_cared = true;
                lines[line_index][index].UA_give_condition(gameCardCondition.floating_clickable);
                lines[line_index][index].UA_give_position(want_position);
                lines[line_index][index].UA_give_rotation(new Vector3(-30, 0, 0));
                lines[line_index][index].UA_flush_all_gived_witn_lock(rush);
            }
        }

        gameField.isLong = false;

        List<gameCard> op_m = new List<gameCard>();

        List<gameCard> op_s = new List<gameCard>();

        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                {
                    if (cards[i].p.controller == 1)
                    {
                        if ((cards[i].p.location & (UInt32)game_location.LOCATION_OVERLAY) == 0)
                        {
                            if ((cards[i].p.location & (UInt32)game_location.LOCATION_MZONE) > 0)
                            {
                                op_m.Add(cards[i]);
                            }
                            if ((cards[i].p.location & (UInt32)game_location.LOCATION_SZONE) > 0)
                            {
                                op_s.Add(cards[i]);
                            }
                        }
                    }
                }
        for (int m = 0; m < op_m.Count; m++)
        {
            if ((op_m[m].p.position & (UInt32)game_position.POS_FACEUP) > 0)
            {
                for (int s = 0; s < op_s.Count; s++)
                {
                    if (op_m[m].p.sequence == op_s[s].p.sequence)
                    {
                        if (op_m[m].p.sequence < 5)
                        {
                            op_m[m].opMonsterWithBackGroundCard = true;
                            //op_m[m].isMinBlockMode = true;
                            if (Program.getVerticalTransparency() >= 0.5f)
                            {
                                gameField.isLong = true;    //这个设定暂时取消了
                            }
                        }
                    }
                }
            }
        }

        gameCard[] opM = new gameCard[7];
        gameCard[] meM = new gameCard[7];
        for (int i = 0; i < 7; i++)
        {
            opM[i] = null;
            meM[i] = null;
        }
        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
            {
                if ((cards[i].p.location & (UInt32)game_location.LOCATION_OVERLAY) == 0)
                {
                    if ((cards[i].p.location & (UInt32)game_location.LOCATION_MZONE) > 0)
                    {
                        if (cards[i].p.sequence >= 0 && cards[i].p.sequence <= 6)
                        {
                            if ((cards[i].p.position & (UInt32)game_position.POS_FACEUP) > 0)
                            {
                                if (cards[i].p.controller == 1)
                                {
                                    opM[cards[i].p.sequence] = cards[i];
                                }
                                else
                                {
                                    meM[cards[i].p.sequence] = cards[i];
                                }
                            }
                        }
                    }
                }
            }

        if (opM[1] != null)
        {
            if (opM[5]!=null)
            {
                opM[5].isMinBlockMode = true;
            }
            if (meM[6] != null)
            {
                meM[6].isMinBlockMode = true;
            }
        }

        if (opM[3] != null)
        {
            if (opM[6] != null)
            {
                opM[6].isMinBlockMode = true;
            }
            if (meM[5] != null)
            {
                meM[5].isMinBlockMode = true;
            }
        }

        if (opM[6] != null || meM[5] != null)
        {
            if (meM[1] != null)
            {
                meM[1].isMinBlockMode = true;
            }
        }

        if (opM[5] != null || meM[6] != null)
        {
            if (meM[3] != null)
            {
                meM[3].isMinBlockMode = true;
            }
        }


        gameCard[,] vvv = new gameCard[10,10];

        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
            {
                if ((cards[i].p.location & (UInt32)game_location.LOCATION_OVERLAY) == 0)
                {
                    if ((cards[i].p.location & (UInt32)game_location.LOCATION_MZONE) > 0)
                    {
                        if (cards[i].p.sequence >= 0 && cards[i].p.sequence <= 6)
                        {
                            if ((cards[i].get_data().Type & (UInt32)game_type.link) > 0)
                            {
                                if ((cards[i].p.position & (UInt32)game_position.POS_FACEUP) > 0)
                                {
                                    if (cards[i].p.controller == 1)
                                    {
                                        if (cards[i].p.sequence >= 0 && cards[i].p.sequence <= 4)
                                        {
                                            vvv[4, 4 - cards[i].p.sequence] = cards[i];
                                        }
                                        if (cards[i].p.sequence == 5)
                                        {
                                            vvv[3, 3] = cards[i];
                                        }
                                        if (cards[i].p.sequence == 6)
                                        {
                                            vvv[3, 1] = cards[i];
                                        }
                                    }
                                    else
                                    {
                                        if (cards[i].p.sequence >= 0 && cards[i].p.sequence <= 4)
                                        {
                                            vvv[2, cards[i].p.sequence] = cards[i];
                                        }
                                        if (cards[i].p.sequence == 5)
                                        {
                                            vvv[3, 1] = cards[i];
                                        }
                                        if (cards[i].p.sequence == 6)
                                        {
                                            vvv[3, 3] = cards[i];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


        List<GPS> linkPs = new List<GPS>();


        for (int curHang = 2; curHang <= 4; curHang++)
        {
            for (int curLie = 0; curLie <= 4; curLie++)
            {
                //if (vvv[curHang, curLie] != null)
                {
                    GPS currentGPS = new GPS();
                    currentGPS.location = (int)game_location.LOCATION_MZONE;
                    if (curHang == 4)
                    {
                        currentGPS.controller = 1;
                        currentGPS.sequence = (uint)(4 - curLie);
                    }
                    if (curHang == 3)
                    {
                        currentGPS.controller = 0;
                        if (currentGPS.sequence == 0)
                        {
                            continue;
                        }
                        if (currentGPS.sequence == 1)
                        {
                            currentGPS.sequence = 5;
                        }
                        if (currentGPS.sequence == 2)
                        {
                            continue;
                        }
                        if (currentGPS.sequence == 3)
                        {
                            currentGPS.sequence = 6;
                        }
                        if (currentGPS.sequence == 4)
                        {
                            continue;
                        }
                    }
                    if (curHang == 2)
                    {
                        currentGPS.controller = 0;
                        currentGPS.sequence = (uint)(curLie);
                    }

                    bool lighted = false;

                    if (curHang - 1 >= 0)
                        if (curLie - 1 >= 0)
                            if (vvv[curHang - 1, curLie - 1] != null)
                    {
                        gameCard card = vvv[curHang - 1, curLie - 1];
                        if (card.p.controller == 0)
                            if ((card.get_data().rDefense & CardFac.youshang) > 0)
                                lighted = true;
                        if (card.p.controller == 1)
                            if ((card.get_data().rDefense & CardFac.zuoxia) > 0)
                                lighted = true;
                    }

                        if (curLie - 1 >= 0)
                            if (vvv[curHang, curLie - 1] != null)
                    {
                            gameCard card = vvv[curHang, curLie - 1];
                        if (card.p.controller == 0)
                            if ((card.get_data().rDefense & CardFac.you) > 0)
                                lighted = true;
                        if (card.p.controller == 1)
                            if ((card.get_data().rDefense & CardFac.zuo) > 0)
                                lighted = true;
                    }
                        if (curLie - 1 >= 0)
                            if (vvv[curHang+1, curLie - 1] != null)
                    {
                            gameCard card = vvv[curHang + 1, curLie - 1];
                        if (card.p.controller == 0)
                            if ((card.get_data().rDefense & CardFac.youxia) > 0)
                                lighted = true;
                        if (card.p.controller == 1)
                            if ((card.get_data().rDefense & CardFac.zuoshang) > 0)
                                lighted = true;
                    }
                    if (curHang - 1 >= 0)
                            if (vvv[curHang - 1, curLie] != null)
                    {
                            gameCard card = vvv[curHang - 1, curLie];
                        if (card.p.controller == 0)
                            if ((card.get_data().rDefense & CardFac.shang) > 0)
                                lighted = true;
                        if (card.p.controller == 1)
                            if ((card.get_data().rDefense & CardFac.xia) > 0)
                                lighted = true;
                    }

                    if (vvv[curHang + 1, curLie] != null)
                    {
                        gameCard card = vvv[curHang + 1, curLie];
                        if (card.p.controller == 0)
                            if ((card.get_data().rDefense & CardFac.xia) > 0)
                                lighted = true;
                        if (card.p.controller == 1)
                            if ((card.get_data().rDefense & CardFac.shang) > 0)
                                lighted = true;
                    }
                    if (curHang - 1 >= 0)
                            if (vvv[curHang - 1, curLie + 1] != null)
                    {
                            gameCard card = vvv[curHang - 1, curLie + 1];
                        if (card.p.controller == 0)
                            if ((card.get_data().rDefense & CardFac.zuoshang) > 0)
                                lighted = true;
                        if (card.p.controller == 1)
                            if ((card.get_data().rDefense & CardFac.youxia) > 0)
                                lighted = true;
                    }

                    if (vvv[curHang, curLie + 1] != null)
                    {
                        gameCard card = vvv[curHang, curLie + 1];
                        if (card.p.controller == 0)
                            if ((card.get_data().rDefense & CardFac.zuo) > 0)
                                lighted = true;
                        if (card.p.controller == 1)
                            if ((card.get_data().rDefense & CardFac.you) > 0)
                                lighted = true;
                    }

                    if (vvv[curHang + 1, curLie + 1] != null)
                    {
                        gameCard card = vvv[curHang + 1, curLie + 1];
                        if (card.p.controller == 0)
                            if ((card.get_data().rDefense & CardFac.zuoxia) > 0)
                                lighted = true;
                        if (card.p.controller == 1)
                            if ((card.get_data().rDefense & CardFac.youshang) > 0)
                                lighted = true;
                    }

                    if (lighted)
                    {
                        linkPs.Add(currentGPS);
                    }

                }
            }
        }

        for (int i = 0; i < linkPs.Count; i++)
        {
            bool showed = false;
            for (int a = 0; a < linkMaskList.Count; a++)
            {
                if (linkMaskList[a].p.controller == linkPs[i].controller && linkMaskList[a].p.sequence == linkPs[i].sequence)
                {
                    showed = true;
                }
            }
            if (showed == false)
            {
                linkMaskList.Add(makeLinkMask(linkPs[i]));
            }
        }

        List<linkMask> removeList = new List<linkMask>();

        for (int i = 0; i < linkMaskList.Count; i++)
        {
            bool deleted = true;
            for (int a = 0; a < linkPs.Count; a++)
            {
                if (linkMaskList[i].p.controller == linkPs[a].controller && linkMaskList[i].p.sequence == linkPs[a].sequence)
                {
                    deleted = false;
                }
            }
            if (deleted == true)
            {
                removeList.Add(linkMaskList[i]);
            }
        }

        for (int i = 0; i < removeList.Count; i++)
        {
            linkMaskList.Remove(removeList[i]);
            destroy(removeList[i].gameObject);
        }

        removeList.Clear();
        removeList = null;

        for (int i = 0; i < linkMaskList.Count; i++)
        {
            shift_effect(linkMaskList[i],Program.I().setting.setting.Vlink.value);
        }

        gameField.Update();
        //op hand
        List<gameCard> line = new List<gameCard>();
        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                if (cards[i].cookie_cared == false)
                {
                    if ((cards[i].p.location & (UInt32)game_location.LOCATION_HAND) > 0 && cards[i].p.controller == 1)
                    {
                        line.Add(cards[i]);
                    }
                }
        for (int index = 0; index < line.Count; index++)
        {
            Vector3 want_position = Vector3.zero;
            want_position.y = 0;
            if (gameField.isLong)
            {
                want_position.z = (19f + gameField.delat) * Program.fieldSize + index * 0.015f;
            }
            else
            {
                want_position.z = (19f) * Program.fieldSize + index * 0.015f;
            }
            want_position.x = UIHelper.get_left_right_indexEnhanced(10, -10, index, line.Count, 5);
            line[index].cookie_cared = true;
            line[index].UA_give_position(want_position);
            if (line[index].get_data().Id > 0)
            {
                line[index].UA_give_rotation(new Vector3(-30, 0, 0));
            }
            else
            {
                line[index].UA_give_rotation(new Vector3(-30, 0, 180));
            }
            line[index].UA_give_condition(gameCardCondition.floating_clickable);
            line[index].UA_flush_all_gived_witn_lock(rush);
        }

        //effects
        for (int i = 0; i < gameField.thunders.Count; i++)
        {
            gameField.thunders[i].needDestroy = true;
        }

        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
            {
                List<gameCard> overlayed_cards = GCS_cardGetOverlayElements(cards[i]);
                int overC = 0;
                if (Program.getVerticalTransparency() > 0.5f)
                {
                    if ((cards[i].p.position & (Int32)game_position.POS_FACEUP) > 0 && (cards[i].p.location & (Int32)game_location.LOCATION_ONFIELD) > 0)
                    {
                        overC = overlayed_cards.Count;
                    }
                }
                cards[i].set_overlay_light(overC);
                cards[i].set_overlay_see_button(overlayed_cards.Count > 0);
                for (int x = 0; x < overlayed_cards.Count; x++)
                {
                    overlayed_cards[x].overFatherCount = overlayed_cards.Count;
                    if (overlayed_cards[x].isShowed)
                    {
                        animation_thunder(overlayed_cards[x].gameObject, cards[i].gameObject);
                    }
                }
                foreach (var item in cards[i].target)
                {
                    if ((item.p.location & (UInt32)game_location.LOCATION_SZONE) > 0 || (item.p.location & (UInt32)game_location.LOCATION_MZONE) > 0)
                    {
                        animation_thunder(item.gameObject, cards[i].gameObject);
                    }
                }
            }

        List<thunder_locator> needRemoveThunder = new List<thunder_locator>();
        for (int i = 0; i < gameField.thunders.Count; i++)
        {
            if (gameField.thunders[i].needDestroy == true)
            {
                needRemoveThunder.Add(gameField.thunders[i]);
            }
        }
        for (int i = 0; i < needRemoveThunder.Count; i++)
        {
            gameField.thunders.Remove(needRemoveThunder[i]);
            destroy(needRemoveThunder[i].gameObject);
        }
        needRemoveThunder.Clear();


        //p effect
        gameField.relocatePnums(Program.I().setting.setting.Vpedium.value);
        if (Program.I().setting.setting.Vpedium.value == true) 
        {
            List<gameCard> my_p_cards = new List<gameCard>();

            List<gameCard> op_p_cards = new List<gameCard>();

            for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                    if (cards[i].cookie_cared == false)
                    {
                        if ((cards[i].p.location & (UInt32)game_location.LOCATION_SZONE) > 0)
                        {
                            if (cards[i].p.sequence == 0 || cards[i].p.sequence == 4)
                            {
                                if ((cards[i].get_data().Type & (int)game_type.TYPE_PENDULUM) > 0)
                                {
                                    if (cards[i].p.controller == 0)
                                    {
                                        my_p_cards.Add(cards[i]);
                                    }
                                    else
                                    {
                                        op_p_cards.Add(cards[i]);
                                    }
                                }
                            }
                        }
                    }

            if (MasterRule >= 4)
            {
                if (my_p_cards.Count == 2)
                {
                    Debug.Log("oh");
                    gameField.me_left_p_num.GetComponent<number_loader>().set_number((int)my_p_cards[0].get_data().LScale, 3);
                    gameField.me_right_p_num.GetComponent<number_loader>().set_number((int)my_p_cards[1].get_data().LScale, 0);
                    gameField.mePHole = true;
                    my_p_cards[0].cookie_cared = true;
                    my_p_cards[0].UA_give_position(new Vector3(-10.1f * real + 1, 5, -11.5f * real - 1));
                    my_p_cards[0].UA_give_rotation(new Vector3(-60, -45, 0));
                    my_p_cards[0].UA_give_condition(gameCardCondition.floating_clickable);
                    my_p_cards[0].UA_flush_all_gived_witn_lock(rush);
                    my_p_cards[1].cookie_cared = true;
                    my_p_cards[1].UA_give_position(new Vector3(9.62f * real - 1, 5, -11.5f * real - 1));
                    my_p_cards[1].UA_give_rotation(new Vector3(-60, 45, 0));
                    my_p_cards[1].UA_give_condition(gameCardCondition.floating_clickable);
                    my_p_cards[1].UA_flush_all_gived_witn_lock(rush);
                    my_p_cards[0].p_line_on();
                    my_p_cards[1].p_line_on();
                }
                else
                {
                    gameField.me_left_p_num.GetComponent<number_loader>().set_number(-1, 3);
                    gameField.me_right_p_num.GetComponent<number_loader>().set_number(-1, 3);
                    gameField.mePHole = false;
                }
                if (op_p_cards.Count == 2)
                {
                    gameField.op_left_p_num.GetComponent<number_loader>().set_number((int)op_p_cards[1].get_data().LScale, 0);
                    gameField.op_right_p_num.GetComponent<number_loader>().set_number((int)op_p_cards[0].get_data().LScale, 3);
                    gameField.opPHole = true;
                    op_p_cards[0].cookie_cared = true;
                    op_p_cards[0].UA_give_position(new Vector3(9.62f * real - 1, 5, 11.5f * real - 1));
                    op_p_cards[0].UA_give_rotation(new Vector3(-90, 45, 0));
                    op_p_cards[0].UA_give_condition(gameCardCondition.floating_clickable);
                    op_p_cards[0].UA_flush_all_gived_witn_lock(rush);
                    op_p_cards[1].cookie_cared = true;
                    op_p_cards[1].UA_give_position(new Vector3(-10.1f * real + 1, 5, 11.5f * real - 1));
                    op_p_cards[1].UA_give_rotation(new Vector3(-90, -45, 0));
                    op_p_cards[1].UA_give_condition(gameCardCondition.floating_clickable);
                    op_p_cards[1].UA_flush_all_gived_witn_lock(rush);
                    op_p_cards[0].p_line_on();
                    op_p_cards[1].p_line_on();

                }
                else
                {
                    gameField.op_left_p_num.GetComponent<number_loader>().set_number(-1, 3);
                    gameField.op_right_p_num.GetComponent<number_loader>().set_number(-1, 3);
                    gameField.opPHole = false;
                }
            }
            else
            {
                if (my_p_cards.Count == 2)
                {
                    gameField.me_left_p_num.GetComponent<number_loader>().set_number((int)my_p_cards[0].get_data().LScale, 3);
                    gameField.me_right_p_num.GetComponent<number_loader>().set_number((int)my_p_cards[1].get_data().LScale, 3);
                    gameField.mePHole = true;
                    my_p_cards[0].cookie_cared = true;
                    my_p_cards[0].UA_give_position(new Vector3(-15.2f * real, 5, -10f));
                    my_p_cards[0].UA_give_rotation(new Vector3(-90, -45, 0));
                    my_p_cards[0].UA_give_condition(gameCardCondition.floating_clickable);
                    my_p_cards[0].UA_flush_all_gived_witn_lock(rush);
                    my_p_cards[1].cookie_cared = true;
                    my_p_cards[1].UA_give_position(new Vector3(14.65f * real, 5, -10f));
                    my_p_cards[1].UA_give_rotation(new Vector3(-90, 45, 0));
                    my_p_cards[1].UA_give_condition(gameCardCondition.floating_clickable);
                    my_p_cards[1].UA_flush_all_gived_witn_lock(rush);
                    my_p_cards[0].p_line_on();
                    my_p_cards[1].p_line_on();
                }
                else
                {
                    gameField.me_left_p_num.GetComponent<number_loader>().set_number(-1, 3);
                    gameField.me_right_p_num.GetComponent<number_loader>().set_number(-1, 3);
                    gameField.mePHole = false;
                }
                if (op_p_cards.Count == 2)
                {
                    gameField.op_left_p_num.GetComponent<number_loader>().set_number((int)op_p_cards[1].get_data().LScale, 3);
                    gameField.op_right_p_num.GetComponent<number_loader>().set_number((int)op_p_cards[0].get_data().LScale, 3);
                    gameField.opPHole = true;
                    op_p_cards[0].cookie_cared = true;
                    op_p_cards[0].UA_give_position(new Vector3(14.65f * real, 5, 8f));
                    op_p_cards[0].UA_give_rotation(new Vector3(-90, 45, 0));
                    op_p_cards[0].UA_give_condition(gameCardCondition.floating_clickable);
                    op_p_cards[0].UA_flush_all_gived_witn_lock(rush);
                    op_p_cards[1].cookie_cared = true;
                    op_p_cards[1].UA_give_position(new Vector3(-15.2f * real, 5, 8f));
                    op_p_cards[1].UA_give_rotation(new Vector3(-90, -45, 0));
                    op_p_cards[1].UA_give_condition(gameCardCondition.floating_clickable);
                    op_p_cards[1].UA_flush_all_gived_witn_lock(rush);
                    op_p_cards[0].p_line_on();
                    op_p_cards[1].p_line_on();

                }
                else
                {
                    gameField.op_left_p_num.GetComponent<number_loader>().set_number(-1, 3);
                    gameField.op_right_p_num.GetComponent<number_loader>().set_number(-1, 3);
                    gameField.opPHole = false;
                }
            }
           
        }
        else
        {
            //p effect pain

            List<gameCard> my_p_cards = new List<gameCard>();

            List<gameCard> op_p_cards = new List<gameCard>();

            for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                    if (cards[i].cookie_cared == false)
                    {
                        if ((cards[i].p.location & (UInt32)game_location.LOCATION_SZONE) > 0)
                        {
                            if (cards[i].p.sequence == 6 || cards[i].p.sequence == 7)
                            {
                                if (cards[i].p.controller == 0)
                                {
                                    my_p_cards.Add(cards[i]);
                                }
                                else
                                {
                                    op_p_cards.Add(cards[i]);
                                }
                            }
                        }
                    }

            gameField.mePHole = false;
            gameField.opPHole = false;

            if (my_p_cards.Count == 2)
            {
                gameField.me_left_p_num.GetComponent<number_loader>().set_number((int)my_p_cards[0].get_data().LScale, 3);
                gameField.me_right_p_num.GetComponent<number_loader>().set_number((int)my_p_cards[1].get_data().LScale, 0);
            }
            else
            {
                gameField.me_left_p_num.GetComponent<number_loader>().set_number(-1, 3);
                gameField.me_right_p_num.GetComponent<number_loader>().set_number(-1, 3);
            }
            if (op_p_cards.Count == 2)
            {
                gameField.op_left_p_num.GetComponent<number_loader>().set_number((int)op_p_cards[1].get_data().LScale, 0);
                gameField.op_right_p_num.GetComponent<number_loader>().set_number((int)op_p_cards[0].get_data().LScale, 3);
            }
            else
            {
                gameField.op_left_p_num.GetComponent<number_loader>().set_number(-1, 3);
                gameField.op_right_p_num.GetComponent<number_loader>().set_number(-1, 3);
            }

        }
        
        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                if (cards[i].cookie_cared == false)
                {
                    if ((cards[i].p.location & (UInt32)game_location.LOCATION_OVERLAY) > 0)
                    {
                        if ((cards[i].p.location & (UInt32)game_location.LOCATION_EXTRA) > 0)
                        {
                            cards[i].cookie_cared = true;
                            cards[i].UA_give_condition(get_point_worldcondition(cards[i].p));
                            Vector3 temp = get_point_worldposition(cards[i].p_beforeOverLayed);
                            temp.y = 0;
                            temp.y -= 2.1f + (cards[i].p.position) * 0.05f;
                            cards[i].UA_give_position(temp);
                            cards[i].UA_give_rotation(get_world_rotation(cards[i]));
                            cards[i].UA_flush_all_gived_witn_lock(rush);
                        }
                    }
                }

        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                if (cards[i].cookie_cared == false)
                {
                    cards[i].UA_give_condition(get_point_worldcondition(cards[i].p));
                    cards[i].UA_give_position(get_point_worldposition(cards[i].p, cards[i]));
                    cards[i].UA_give_rotation(get_world_rotation(cards[i]));
                    cards[i].UA_flush_all_gived_witn_lock(rush);
                }

        
        if (Program.I().setting.setting.Vfield.value)
        {
            int code = 0;

            for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                {
                    if (((cards[i].p.location & (UInt32)game_location.LOCATION_SZONE) > 0) && cards[i].p.sequence == 5)
                    {
                        if (cards[i].p.controller == 0)
                        {
                            if ((cards[i].p.position & (Int32)game_position.POS_FACEUP) > 0)
                            {
                                code = cards[i].get_data().Id;
                            }
                        }
                    }
                }

            gameField.set(0, code);

            code = 0;

            for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                {
                    if (((cards[i].p.location & (UInt32)game_location.LOCATION_SZONE) > 0) && cards[i].p.sequence == 5)
                    {
                        if (cards[i].p.controller == 1)
                        {
                            if ((cards[i].p.position & (Int32)game_position.POS_FACEUP) > 0)
                            {
                                code = cards[i].get_data().Id;
                            }
                        }
                    }
                }

            gameField.set(1, code);
        }
        else
        {
            gameField.set(0, 0);
            gameField.set(1, 0);
        }


        //camera
        float nearest_z = 0;
        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
            {
                if (nearest_z > cards[i].UA_get_accurate_position().z)
                {
                    nearest_z = cards[i].UA_get_accurate_position().z;
                }
            }
        camera_max = -3.5f - 15f * Program.fieldSize;
        camera_min = nearest_z-0.5f;
        if (camera_min > camera_max)
        {
            camera_min = camera_max;
        }

        if (InAI==false)    
        {
            if (condition != Condition.duel)
            {
                toNearest();
            }
        }

        if (someCardIsShowed)
        {
            if (gameInfo.queryHashedButton("hide_all_card") == false)
            {
                gameInfo.addHashedButton("hide_all_card", 0, superButtonType.see, InterString.Get("确认完毕@ui"));
            }
        }
        else
        {
            gameInfo.removeHashedButton("hide_all_card");
        }

        if (InAI == false && condition != Condition.duel)
        {
            if (gameInfo.queryHashedButton("swap") == false)
            {
                gameInfo.addHashedButton("swap", 0, superButtonType.change, InterString.Get("转换视角@ui"));
            }
        }
        else
        {
            gameInfo.removeHashedButton("swap");
        }


        animation_count(gameField.LOCATION_DECK_0, game_location.LOCATION_DECK, 0);
        animation_count(gameField.LOCATION_EXTRA_0, game_location.LOCATION_EXTRA, 0);
        animation_count(gameField.LOCATION_GRAVE_0, game_location.LOCATION_GRAVE, 0);
        animation_count(gameField.LOCATION_REMOVED_0, game_location.LOCATION_REMOVED, 0);
        animation_count(gameField.LOCATION_DECK_1, game_location.LOCATION_DECK, 1);
        animation_count(gameField.LOCATION_EXTRA_1, game_location.LOCATION_EXTRA, 1);
        animation_count(gameField.LOCATION_GRAVE_1, game_location.LOCATION_GRAVE, 1);
        animation_count(gameField.LOCATION_REMOVED_1, game_location.LOCATION_REMOVED, 1);
        gameField.realize();
        Program.notGo(gameInfo.realize);
        Program.go(50,gameInfo.realize);
        Program.notGo(Program.I().book.realize);
        Program.go(50, Program.I().book.realize);
        Program.I().cardDescription.realizeMonitor();
    }

    private void animation_thunder(GameObject leftGameObject, GameObject rightGameObject)
    {
        thunder_locator thunder = null;
        for (int p = 0; p < gameField.thunders.Count; p++)
        {
            if (gameField.thunders[p].leftobj == leftGameObject)
            {
                if (gameField.thunders[p].rightobj == rightGameObject)
                {
                    thunder = gameField.thunders[p];
                }
            }
        }

        if (thunder == null)
        {
            thunder = create_s(Program.I().mod_ocgcore_decoration_thunder).GetComponent<thunder_locator>();
            thunder.set_objects(leftGameObject, rightGameObject);
            gameField.thunders.Add(thunder);
        }
        thunder.needDestroy = false;
    }

    enum cardRuleComdition
    {
        meUpAtk,
        meUpDef,
        meDownAtk,
        meDownDef,
        opUpAtk,
        opUpDef,
        opDownAtk,
        opDownDef,
    }

    Vector3 get_world_rotation(gameCard card)
    {
        cardRuleComdition r = cardRuleComdition.meUpAtk;
        if ((card.p.location & (UInt32)game_location.LOCATION_DECK) > 0)
        {
            if (card.get_data().Id > 0)
            {
                r = cardRuleComdition.meUpAtk;
            }
            else
            {
                r = cardRuleComdition.meDownAtk;
            }
        }
        if ((card.p.location & (UInt32)game_location.LOCATION_GRAVE) > 0)
        {
            r = cardRuleComdition.meUpAtk;
        }
        if ((card.p.location & (UInt32)game_location.LOCATION_REMOVED) > 0)
        {
            if ((card.p.position & (UInt32)game_position.POS_FACEUP) > 0)
            {
                r = cardRuleComdition.meUpAtk;
            }
            else
            {
                r = cardRuleComdition.meDownAtk;
            }
        }
        if ((card.p.location & (UInt32)game_location.LOCATION_EXTRA) > 0)
        {
            if ((card.p.position & (UInt32)game_position.POS_FACEUP) > 0)
            {
                r = cardRuleComdition.meUpAtk;
            }
            else
            {
                r = cardRuleComdition.meDownAtk;
            }
        }
        if ((card.p.location & (UInt32)game_location.LOCATION_MZONE) > 0)
        {
            if ((card.p.position & (UInt32)game_position.POS_FACEDOWN_DEFENSE) > 0)
            {
                r = cardRuleComdition.meDownDef;
            }
            if ((card.p.position & (UInt32)game_position.POS_FACEUP_DEFENSE) > 0)
            {
                r = cardRuleComdition.meUpDef;
            }
            if ((card.p.position & (UInt32)game_position.POS_FACEDOWN_ATTACK) > 0)
            {
                r = cardRuleComdition.meDownAtk;
            }
            if ((card.p.position & (UInt32)game_position.POS_FACEUP_ATTACK) > 0)
            {
                r = cardRuleComdition.meUpAtk;
            }
        }
        if ((card.p.location & (UInt32)game_location.LOCATION_SZONE) > 0)
        {
            if ((card.p.position & (UInt32)game_position.POS_FACEUP) > 0)
            {
                r = cardRuleComdition.meUpAtk;
            }
            else
            {
                r = cardRuleComdition.meDownAtk;
            }
        }
        if ((card.p.location & (UInt32)game_location.LOCATION_OVERLAY) > 0)
        {
            r = cardRuleComdition.meUpAtk;
        }
        if (card.p.controller == 1)
        {
            switch (r)  
            {
                case cardRuleComdition.meUpAtk:
                    r = cardRuleComdition.opUpAtk;
                    break;
                case cardRuleComdition.meUpDef:
                    r = cardRuleComdition.opUpDef;
                    break;
                case cardRuleComdition.meDownAtk:
                    r = cardRuleComdition.opDownAtk;
                    break;
                case cardRuleComdition.meDownDef:
                    r = cardRuleComdition.opDownDef;
                    break;
                default:
                    break;
            }
        }
        switch (r)  
        {
            case cardRuleComdition.meUpAtk:
                return new Vector3(0, 0, 0);
            case cardRuleComdition.meUpDef:
                return new Vector3(0, -90, 0);
            case cardRuleComdition.meDownAtk:
                return new Vector3(0, 0, 180);
            case cardRuleComdition.meDownDef:
                return new Vector3(0, -90, 180);


            case cardRuleComdition.opUpAtk:
                return new Vector3(0, 180, 0);
            case cardRuleComdition.opUpDef:
                return new Vector3(0, 90, 0);
            case cardRuleComdition.opDownAtk:
                return new Vector3(0, 180, 180);
            case cardRuleComdition.opDownDef:
                return new Vector3(0, 90, 180);

            default:
                return Vector3.zero;
        }
    }

    //private Vector3 get_real_rotation(int i)
    //{
    //    Vector3 r = get_point_worldrotation(cards[i].p);
    //    if ((cards[i].p.location & (UInt32)game_location.LOCATION_DECK) > 0)
    //    {
    //        if (cards[i].get_data().Id > 0)
    //        {
    //            r = new Vector3(90, 0, 0);
    //        }
    //        else
    //        {
    //            r = new Vector3(-90, 0, 0);
    //        }
    //    }
    //    if ((cards[i].p.location & (UInt32)game_location.LOCATION_MZONE) > 0)
    //    {
    //        if ((cards[i].p.position & (UInt32)game_position.POS_FACEDOWN_DEFENSE) > 0)
    //        {
    //            r = new Vector3(-90, 0, 90);
    //        }
    //        if ((cards[i].p.position & (UInt32)game_position.POS_FACEUP_DEFENSE) > 0)
    //        {
    //            r = new Vector3(90, 0, 90);
    //        }
    //        if ((cards[i].p.position & (UInt32)game_position.POS_FACEDOWN_ATTACK) > 0)
    //        {
    //            r = new Vector3(-90, 0, 0);
    //        }
    //        if ((cards[i].p.position & (UInt32)game_position.POS_FACEUP_ATTACK) > 0)
    //        {
    //            r = new Vector3(90, 0, 0);
    //        }
    //    }
    //    if ((cards[i].p.location & (UInt32)game_location.LOCATION_SZONE) > 0)
    //    {
    //        if ((cards[i].p.position & (UInt32)game_position.POS_FACEDOWN_DEFENSE) > 0)
    //        {
    //            r = new Vector3(-90, 0, 90);
    //        }
    //        if ((cards[i].p.position & (UInt32)game_position.POS_FACEUP_DEFENSE) > 0)
    //        {
    //            r = new Vector3(90, 0, 90);
    //        }
    //        if ((cards[i].p.position & (UInt32)game_position.POS_FACEDOWN_ATTACK) > 0)
    //        {
    //            r = new Vector3(-90, 0, 0);
    //        }
    //        if ((cards[i].p.position & (UInt32)game_position.POS_FACEUP_ATTACK) > 0)
    //        {
    //            r = new Vector3(90, 0, 0);
    //        }
    //    }
    //    if ((cards[i].p.location & (UInt32)game_location.LOCATION_GRAVE) > 0)
    //    {
    //        r = new Vector3(90, 0, 0);
    //    }
    //    if ((cards[i].p.location & (UInt32)game_location.LOCATION_REMOVED) > 0)
    //    {
    //        if ((cards[i].p.position & (UInt32)game_position.POS_FACEUP) > 0)
    //        {
    //            r = new Vector3(90, 0, 0);
    //        }
    //        else
    //        {
    //            r = new Vector3(-90, 0, 0);
    //        }
    //    }
    //    if ((cards[i].p.location & (UInt32)game_location.LOCATION_EXTRA) > 0)
    //    {
    //        if ((cards[i].p.position & (UInt32)game_position.POS_FACEUP) > 0)
    //        {
    //            r = new Vector3(90, 0, 0);
    //        }
    //        else
    //        {
    //            r = new Vector3(-90, 0, 0);
    //        }
    //    }
    //    if ((cards[i].p.location & (UInt32)game_location.LOCATION_OVERLAY) > 0)
    //    {
    //        r = new Vector3(90, 0, 0);
    //    }
    //    if (cards[i].p.controller == 1)
    //    {
    //        r.z += 179f;
    //    }

    //    return r;
    //}

    private void animation_count(TMPro.TextMeshPro textmesh, game_location location, int player)
    {
        int count = 0;
        int countU = 0; 
        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
            {
                if (cards[i].p.controller == player)
                {
                    if ((cards[i].p.location & (UInt32)location) > 0)
                    {
                        count++;
                        if ((cards[i].p.position & (UInt32)game_position.POS_FACEUP) > 0)
                        {
                            countU++;
                        }
                    }
                }
            }
        if (count < 2)
        {
            textmesh.text = "";
        }
        else
        {
            if (location== game_location.LOCATION_EXTRA)    
            {
                textmesh.text = count.ToString()+"("+ countU .ToString()+ ")";
            }
            else
            {
                textmesh.text = count.ToString();
            }
        }
    }

    float camera_max = -17.5f;

    float camera_min = -17.5f;

    public void toNearest(bool fix=false)
    {
        if (fix)
        {
            if (Program.cameraPosition.z < camera_min)
            {
                Program.cameraPosition.z = camera_min;
                Program.cameraPosition.x = 0;
                Program.cameraPosition.y = 23;
            }
        }
        else
        {
            Program.cameraPosition.z = camera_min;
            Program.cameraPosition.x = 0;
            Program.cameraPosition.y = 23;
        }
        Program.cameraRotation = new Vector3(60, 0, 0);
    }

    public gameCard GCS_cardCreate(GPS p)
    {
        gameCard c = null;
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].md5 == md5Maker)
            {
                c = cards[i];
                c.p = p;
            }
        }
        if (c == null)
        {
            c = new gameCard();
            c.md5 = md5Maker;
            c.p = p;
            cards.Add(c);
        }
        c.show();
        c.p = p;
        c.controllerBased = p.controller;
        md5Maker++;
        return c;
    }

    public gameCard GCS_cardGet(GPS p, bool create)
    {
        gameCard c = null;
        if ((p.location & (UInt32)game_location.LOCATION_OVERLAY) > 0)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].p.location == p.location)
                {
                    if (cards[i].p.controller == p.controller)
                    {
                        if (cards[i].p.sequence == p.sequence)
                        {
                            if (cards[i].p.position == p.position)
                            {
                                if (cards[i].gameObject.activeInHierarchy)
                                {
                                    c = cards[i];
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].p.location == p.location)
                {
                    if (cards[i].p.controller == p.controller)
                    {
                        if (cards[i].p.sequence == p.sequence)
                        {
                            if (cards[i].gameObject.activeInHierarchy)
                            {
                                c = cards[i];
                                break;
                            }
                        }
                    }
                }
            }
        }
        if (p.location == 0)
        {
            c = null;
        }
        if (create == true)
        {
            if (c == null)
            {
                c = GCS_cardCreate(p);
            }
        }
        return c;
    }

    public List<gameCard> GCS_cardGetOverlayElements(gameCard c)
    {
        List<gameCard> cas = new List<gameCard>();
        if (c != null)
        {
            if ((c.p.location & (UInt32)game_location.LOCATION_OVERLAY) == 0)
            {
                for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                    {
                        if ((cards[i].p.location & (UInt32)game_location.LOCATION_OVERLAY) > 0)
                            if (cards[i].p.controller == c.p.controller)
                                if ((cards[i].p.location | (UInt32)game_location.LOCATION_OVERLAY) == (c.p.location | (UInt32)game_location.LOCATION_OVERLAY))
                                    if (cards[i].p.sequence == c.p.sequence)
                                        cas.Add(cards[i]);
                    }
            }
        }
        return cas;
    }

    List<int> keys = new List<int>();

    public gameCard GCS_cardMove(GPS p1, GPS p2, bool print = true, bool swap = false)
    {

        //from card
        gameCard card_from = GCS_cardGet(p1, true);

        try
        {
            if (reportShowAll)
            {
                if (print)
                {
                    if (swap)
                    {
                        //printDuelLog(UIHelper.getGPSstringLocation(p1) + InterString.Get("交换") + UIHelper.getGPSstringLocation(p2) + UIHelper.getGPSstringName(card_from));
                    }
                    else
                    {
                        //printDuelLog(UIHelper.getGPSstringLocation(p1) + InterString.Get("移到") + UIHelper.getGPSstringLocation(p2) + UIHelper.getGPSstringName(card_from));
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
        }


        //to card
        gameCard card_to = GCS_cardGet(p2, false);

        card_from.isShowed = false;
        card_from.ChainUNlock();

        if (swap == false)
        {
            if ((p1.location != p2.location) || ((p2.position & (int)game_position.POS_FACEDOWN) > 0))
            {
                card_from.target.Clear();
                for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
                    {
                        cards[i].removeTarget(card_from);
                    }
                card_from.disabled = false;
                card_from.refreshData();
            }
        }

        if ((p2.location & (UInt32)game_location.LOCATION_OVERLAY) > 0)
        {
            card_from.p_beforeOverLayed = p1;
        }


        List<gameCard> overlayed_cards_of_cardFrom = GCS_cardGetOverlayElements(card_from);
        List<gameCard> overlayed_cards_of_cardTo = GCS_cardGetOverlayElements(card_to);

        //begin analyse
        if (swap)
        {
            if (card_from != null)
                card_from.p = p2;
            if (card_to != null)
                card_to.p = p1;
        }
        else
        {
            if (card_to == null)
            {
                if (card_from != null)
                    card_from.p = p2;
            }
            else
            {
                if (card_from == card_to)
                {
                    if (card_from != null)
                        card_from.p = p2;
                }
                else
                {
                    if ((card_to.p.location & (UInt32)game_location.LOCATION_OVERLAY) == 0)
                    {
                        if (((card_to.p.location & (UInt32)game_location.LOCATION_MZONE) > 0) || ((card_to.p.location & (UInt32)game_location.LOCATION_SZONE) > 0))
                        {
                            if (card_from != null)
                                card_from.p = p2;
                            if (card_to != null)
                                card_to.p = p1;
                        }
                        else
                        {
                            if (card_from != null)
                            {
                                GCS_cardRelocate(card_from,p2);
                            }
                        }

                    }
                    else
                    {
                        if (card_from != null)
                        {
                            card_from.p = p2;
                            card_from.p.position += 500;
                        }
                    }
                }
            }
        }

        //overlay 
        if (card_from != null)
        {
            for (int i = 0; i < overlayed_cards_of_cardFrom.Count; i++)
            {
                overlayed_cards_of_cardFrom[i].p.controller = card_from.p.controller;
                overlayed_cards_of_cardFrom[i].p.location = card_from.p.location | (UInt32)game_location.LOCATION_OVERLAY;
                overlayed_cards_of_cardFrom[i].p.sequence = card_from.p.sequence;
                overlayed_cards_of_cardFrom[i].p.position += 1000;
            }
        }

        if (card_to != null)
        {
            for (int i = 0; i < overlayed_cards_of_cardTo.Count; i++)
            {
                overlayed_cards_of_cardTo[i].p.controller = card_to.p.controller;
                overlayed_cards_of_cardTo[i].p.location = card_to.p.location | (UInt32)game_location.LOCATION_OVERLAY;
                overlayed_cards_of_cardTo[i].p.sequence = card_to.p.sequence;
                overlayed_cards_of_cardTo[i].p.position += 1000;
            }
        }

        arrangeCards();
        return card_from;
    }

    void GCS_cardRelocate(gameCard card_from, GPS p2)
    {
        List<gameCard> cardsInLocation = MHS_getBundle((int)p2.controller, (int)p2.location);
        cardsInLocation.Remove(card_from);
        cardsInLocation.Sort((left, right) =>
        {
            int a = 0;
            if (left.p.sequence > right.p.sequence)
            {
                a = 1;
            }
            else if (left.p.sequence < right.p.sequence)
            {
                a = -1;
            }
            return a;
        });
        if ((int)p2.sequence < 0)
        {
            cardsInLocation.Insert(0, card_from);
        }
        else if ((int)p2.sequence > cardsInLocation.Count)
        {
            cardsInLocation.Insert(cardsInLocation.Count, card_from);
        }
        else
        {
            cardsInLocation.Insert((int)p2.sequence, card_from);
        }
        for (int i = 0; i < cardsInLocation.Count; i++) 
        {
            cardsInLocation[i].p.sequence = (uint)i;
        }
        card_from.p = p2;
    }

    private void arrangeCards()
    {
        //sort 
        cards.Sort((left, right) =>
        {
            int a = 1;
            if (left.p.controller > right.p.controller)
            {
                a = 1;
            }
            else if (left.p.controller < right.p.controller)
            {
                a = -1;
            }
            else
            {
                if (left.p.location == (UInt32)game_location.LOCATION_HAND && right.p.location != (UInt32)game_location.LOCATION_HAND)
                {
                    a = -1;
                }
                else if (left.p.location != (UInt32)game_location.LOCATION_HAND && right.p.location == (UInt32)game_location.LOCATION_HAND)
                {
                    a = 1;
                }
                else
                {
                    if ((left.p.location | (UInt32)game_location.LOCATION_OVERLAY) > (right.p.location | (UInt32)game_location.LOCATION_OVERLAY))
                    {
                        a = -1;
                    }
                    else if ((left.p.location | (UInt32)game_location.LOCATION_OVERLAY) < (right.p.location | (UInt32)game_location.LOCATION_OVERLAY))
                    {
                        a = 1;
                    }
                    else
                    {
                        if (left.p.sequence > right.p.sequence)
                        {
                            a = 1;
                        }
                        else if (left.p.sequence < right.p.sequence)
                        {
                            a = -1;
                        }
                        else
                        {
                            if ((left.p.location & (UInt32)game_location.LOCATION_OVERLAY) > (right.p.location & (UInt32)game_location.LOCATION_OVERLAY))
                            {
                                a = -1;
                            }
                            else if ((left.p.location & (UInt32)game_location.LOCATION_OVERLAY) < (right.p.location & (UInt32)game_location.LOCATION_OVERLAY))
                            {
                                a = 1;
                            }
                            else
                            {
                                if (left.p.position > right.p.position)
                                {
                                    a = 1;
                                }
                                else if (left.p.position < right.p.position)
                                {
                                    a = -1;
                                }
                            }
                        }
                    }
                }
            }
            return a;
        });

        /////rebuild
        UInt32 preController = 9999;
        UInt32 preLocation = 9999;
        UInt32 preSequence = 9999;

        UInt32 sequenceWriter = 0;
        int positionWriter = 0;

        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
            {
                if (preController != cards[i].p.controller)
                {
                    sequenceWriter = 0;
                }
                if ((preLocation | (UInt32)game_location.LOCATION_OVERLAY) != (cards[i].p.location | (UInt32)game_location.LOCATION_OVERLAY))
                {
                    sequenceWriter = 0;
                }
                if (preSequence != cards[i].p.sequence)
                {
                    positionWriter = 0;
                }

                if ((cards[i].p.location & (UInt32)game_location.LOCATION_MZONE) == 0)
                {
                    if ((cards[i].p.location & (UInt32)game_location.LOCATION_SZONE) == 0)
                    {
                        cards[i].p.sequence = sequenceWriter;
                    }
                }

                if ((cards[i].p.location & (UInt32)game_location.LOCATION_OVERLAY) > 0)
                {
                    cards[i].p.position = positionWriter;
                    positionWriter++;
                }
                else
                {
                    sequenceWriter++;
                }

                preController = cards[i].p.controller;
                preLocation = cards[i].p.location;
                preSequence = cards[i].p.sequence;
            }
    }

    int cookie_matchKill = 0;

    int md5Maker = 0;

    string ES_turnString = "";

    string ES_phaseString = "";

    void toDefaultHint()
    {
        gameField.setHint(ES_turnString + ES_phaseString);
    }

    void toDefaultHintLogical()
    {
        gameField.setHintLogical(ES_turnString + ES_phaseString);
    }

    void returnFromDeckEdit()
    {
        TcpHelper.CtosMessage_UpdateDeck(((DeckManager)Program.I().deckManager).getRealDeck());
    }

    public GameField gameField;

    enum duelResult
    {
        disLink,win,lose,draw
    }

    duelResult result = duelResult.disLink;

    public override void show()
    {
        if (isShowed == true)
        {
            Menu.deleteShell();
        }
        base.show();
        Program.I().light.transform.eulerAngles = new Vector3(50, -50, 0);
        Program.cameraPosition = new Vector3(0, 23, -18.5f - 3.2f * (Program.fieldSize - 1f) / 0.21f);
        Program.camera_game_main.transform.position = Program.cameraPosition*1.5f;
        Program.cameraRotation = new Vector3(60, 0, 0);
        Program.camera_game_main.transform.eulerAngles = Program.cameraRotation;
        Program.reMoveCam(getScreenCenter());
        gameField = new GameField();    
        if (paused)
        {
            try
            {
                EventDelegate.Execute(UIHelper.getByName<UIButton>(toolBar, "go_").onClick);
            }
            catch (Exception e) 
            {
                paused = false;
            }
        }
        deckReserved = false;
        surrended = false;
        Program.I().room.duelEnded = false;
        gameInfo.swaped = false;
        keys.Clear();
        currentMessageIndex = -1;
        result = duelResult.disLink;
        theWorldIndex = 0;
        gameInfo.setTimeStill(0);
        sideReference.Clear();
        confirmedCards.Clear();

    }

    public override void hide()
    {
        Program.I().cardDescription.shiftCardShower(true);
        InAI = false;
        MessageBeginTime = 0;
        currentMessage = GameMessage.Waiting;
        Packages_ALL.Clear();
        Packages.Clear();
        cardsForConfirm.Clear();
        logicalClearChain();
        deckReserved = false;
        if (isShowed)
        {
            clearResponse();
            Program.I().book.clear();
            Program.I().book.hide();
        }
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].hide();
        }
        paused = false;
        condition = Condition.N;
        base.hide();
    }

    public void ES_gameButtonClicked(gameButton btn)
    {
        if (btn.cookieString == "see_overlay")
        {
            if (btn.cookieCard != null)
            {
                btn.cookieCard.ES_exit_excited(true);
                List<gameCard> cas = GCS_cardGetOverlayElements(btn.cookieCard);
                for (int i = 0; i < cas.Count; i++)
                {
                    cas[i].isShowed = !cas[i].isShowed;
                    cas[i].flash_line_off();
                    //if (cas[i].isShowed)
                    //{
                    //    cas[i].set_text(GameStringHelper.diefang);
                    //}
                    //else
                    //{
                    //    cas[i].set_text("");
                    //}
                }
                realize();
                toNearest();
            }
            return;
        }
        switch (currentMessage)
        {
            case GameMessage.SelectBattleCmd:
            case GameMessage.SelectIdleCmd:
                if (btn.hint == InterString.Get("发动效果@ui"))
                {
                    if (btn.cookieCard.effects.Count > 0)
                    {
                        if (btn.cookieCard.effects.Count == 1)
                        {
                            BinaryMaster binaryMaster = new BinaryMaster();
                            binaryMaster.writer.Write(btn.cookieCard.effects[0].ptr);
                            sendReturn(binaryMaster.get());
                        }
                        else
                        {
                            List<messageSystemValue> values = new List<messageSystemValue>();
                            for (int i = 0; i < btn.cookieCard.effects.Count; i++)
                            {
                                values.Add(new messageSystemValue { hint = btn.cookieCard.effects[i].desc, value = btn.cookieCard.effects[i].ptr.ToString() });
                            }
                            values.Add(new messageSystemValue { hint = InterString.Get("取消"), value = "hide" });
                            RMSshow_singleChoice("return", values);
                        }
                    }
                    return;
                }
                lastExcitedController = (int)btn.cookieCard.p.controller;
                lastExcitedLocation = (int)btn.cookieCard.p.location;
                BinaryMaster p = new BinaryMaster();
                p.writer.Write((int)btn.response);
                sendReturn(p.get());
                break;
            case GameMessage.SelectEffectYn:
                break;
            case GameMessage.SelectYesNo:
                break;
            case GameMessage.SelectOption:
                break;
            case GameMessage.SelectCard:
                break;
            case GameMessage.SelectUnselectCard:
                break;
            case GameMessage.SelectChain:
                break;
            case GameMessage.SelectPlace:
                break;
            case GameMessage.SelectPosition:
                break;
            case GameMessage.SelectTribute:
                break;
            case GameMessage.SortChain:
                break;
            case GameMessage.SelectCounter:
                break;
            case GameMessage.SelectSum:
                break;
            case GameMessage.SelectDisfield:
                break;
            case GameMessage.AnnounceRace:
                break;
            case GameMessage.AnnounceAttrib:
                break;
            case GameMessage.AnnounceCard:
            case GameMessage.AnnounceCardFilter:
                break;
            case GameMessage.AnnounceNumber:
                break;
        }
    }

    public void ES_gameUIbuttonClicked(gameUIbutton btn)
    {
        if (btn.hashString == "clearCounter")
        {
            for (int i = 0; i < allCardsInSelectMessage.Count; i++)
            {
                allCardsInSelectMessage[i].counterSELcount = 0;
                allCardsInSelectMessage[i].show_number(allCardsInSelectMessage[i].counterSELcount);
            }
            return;
        }
        if (btn.hashString == "sendSelected")
        {
            sendSelectedCards();
            return;
        }
        if (btn.hashString == "hide_all_card")
        {
            if (flagForTimeConfirm)
            {
                flagForTimeConfirm = false;
                MessageBeginTime = Program.TimePassed();
            }
            clearAllShowed();
            return;
        }
        if (btn.hashString == "swap")
        {
            GCS_swapALL();
            return;
        }
        switch (currentMessage)
        {
            case GameMessage.SelectBattleCmd:
            case GameMessage.SelectIdleCmd:
                BinaryMaster p = new BinaryMaster();
                p.writer.Write((int)btn.response);
                sendReturn(p.get());
                break;
            case GameMessage.SelectEffectYn:
            case GameMessage.SelectYesNo:
            case GameMessage.SelectCard:
            case GameMessage.SelectUnselectCard:
            case GameMessage.SelectTribute:
            case GameMessage.SelectChain:
                clearAllShowedB = true;
                BinaryMaster binaryMaster = new BinaryMaster();
                binaryMaster.writer.Write(btn.response);
                sendReturn(binaryMaster.get());
                break;
            case GameMessage.SelectPlace:
                break;
            case GameMessage.SelectPosition:
                break;
            case GameMessage.SortChain:
                break;
            case GameMessage.SelectCounter:
                break;
            case GameMessage.SelectSum:
                break;
            case GameMessage.SelectDisfield:
                break;
            case GameMessage.AnnounceRace:
                break;
            case GameMessage.AnnounceAttrib:
                break;
            case GameMessage.AnnounceCard:
            case GameMessage.AnnounceCardFilter:
                clearResponse();
                realize();
                toNearest();
                RMSshow_input("AnnounceCard", InterString.Get("请输入关键字。"), "");
                break;
            case GameMessage.AnnounceNumber:
                break;
        }
    }

    private void GCS_swapALL(bool realized=true) 
    {
        isFirst = !isFirst;
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].p.controller = 1 - cards[i].p.controller;
            cards[i].p_beforeOverLayed.controller = 1 - cards[i].p_beforeOverLayed.controller;
            cards[i].isShowed = false;
            cards[i].controllerBased = 1 - cards[i].controllerBased;
        }
        gameInfo.swaped = !gameInfo.swaped;
        if (realized)
        {
            realize(true);
        }
    }

    private void clearAllShowed()
    {
        for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
            {
                cards[i].isShowed = false;
            }
        realize();
        toNearest();
    }

    public delegate void responseHandler(byte[] buffer);
    public responseHandler handler = null;

    int theWorldIndex = 0;

    public bool inTheWorld() 
    {
        return currentMessageIndex < theWorldIndex;
    }

    public void sendReturn(byte[] buffer)
    {
        if (paused) 
        {
            EventDelegate.Execute(UIHelper.getByName<UIButton>(toolBar, "go_").onClick);
        }
        clearResponse();
        if (handler != null)
        {
            handler(buffer);
        }
    }  

    List<sortResult> ES_sortCurrent = new List<sortResult>();

    public void ES_cardClicked(gameCard card)
    {
        if (card != null)
        {
            lastExcitedController = (int)card.p.controller;
            lastExcitedLocation = (int)card.p.location;
        }
        switch (currentMessage)
        {
            case GameMessage.SelectBattleCmd:
                break;
            case GameMessage.SelectIdleCmd:
                break;
            case GameMessage.SelectEffectYn:
                break;
            case GameMessage.SelectYesNo:
                break;
            case GameMessage.SelectOption:
                break;
            case GameMessage.SortChain:
            case GameMessage.SortCard:
                if (card.forSelect)
                {
                    for (int i = 0; i < cardsInSort.Count; i++)
                    {
                        cardsInSort[i].show_number(0);
                    }
                    List<int> avaliableSortOptions = new List<int>();
                    for (int i = 0; i < card.sortOptions.Count; i++)
                    {
                        avaliableSortOptions.Add(card.sortOptions[i]);
                    }
                    for (int i = 0; i < ES_sortResult.Count; i++)
                    {
                        avaliableSortOptions.Remove(ES_sortResult[i].option);
                    }
                    if (avaliableSortOptions.Count == 0)
                    {
                        List<sortResult> remove = new List<sortResult>();
                        for (int i = 0; i < ES_sortResult.Count; i++)
                        {
                            if (ES_sortResult[i].card == card)
                            {
                                remove.Add(ES_sortResult[i]);
                            }
                        }
                        for (int i = 0; i < remove.Count; i++)
                        {
                            ES_sortResult.Remove(remove[i]);
                        }
                        remove.Clear();
                    }
                    if (avaliableSortOptions.Count == 1)
                    {
                        ES_sortResult.Add(new sortResult
                        {
                            card = card,
                            option = avaliableSortOptions[0]
                        });
                    }
                    if (avaliableSortOptions.Count > 1)
                    {
                        ES_sortCurrent.Clear();
                        for (int i = 0; i < avaliableSortOptions.Count; i++)
                        {
                            ES_sortCurrent.Add(new sortResult
                            {
                                card = card,
                                option = avaliableSortOptions[i]
                            });
                        }
                        List<messageSystemValue> values = new List<messageSystemValue>();
                        values.Add(new messageSystemValue { hint = InterString.Get("顺发动顺序排序"), value = "shun" });
                        values.Add(new messageSystemValue { hint = InterString.Get("逆发动顺序排序"), value = "fan" });
                        values.Add(new messageSystemValue { hint = InterString.Get("确认其他场上的卡"), value = "hide" });
                        RMSshow_singleChoice("sort", values);
                    }
                    if (ES_sortResult.Count == ES_sortSum)
                    {
                        sendSorted();
                    }
                    else
                    {
                        for (int i = 0; i < ES_sortResult.Count; i++)
                        {
                            ES_sortResult[i].card.show_number(i + 1, true);
                        }
                    }
                }
                break;
            case GameMessage.SelectCard:
            case GameMessage.SelectUnselectCard:
            case GameMessage.SelectTribute:
            case GameMessage.SelectSum:
                if (card.forSelect)
                {
                    bool selectable = false;

                    for (int i = 0; i < cardsSelectable.Count; i++)
                    {
                        if (card == cardsSelectable[i])
                        {
                            selectable = true;
                        }
                    }

                    if (selectable)
                    {
                        bool selected = false;
                        for (int i = 0; i < cardsSelected.Count; i++)
                        {
                            if (card == cardsSelected[i])
                            {
                                selected = true;
                            }
                        }
                        if (selected == false)
                        {
                            cardsSelected.Add(card);
                        }
                        else
                        {
                            cardsSelected.Remove(card);
                        }
                    }
                    else
                    {
                        cardsSelected.Remove(card);
                    }
                    realizeCardsForSelect();
                }
                break;
            case GameMessage.SelectChain:
                if (card.forSelect)
                {
                    if (card.effects.Count > 0)
                    {
                        if (card.effects.Count == 1)
                        {
                            BinaryMaster binaryMaster = new BinaryMaster();
                            binaryMaster.writer.Write(card.effects[0].ptr);
                            sendReturn(binaryMaster.get());
                        }
                        else
                        {
                            List<messageSystemValue> values = new List<messageSystemValue>();
                            for (int i = 0; i < card.effects.Count; i++)
                            {
                                if (card.effects[i].flag == 0)
                                {
                                    if (card.effects[i].desc.Length > 2)
                                    {
                                        values.Add(new messageSystemValue { hint = card.effects[i].desc, value = card.effects[i].ptr.ToString() });
                                    }
                                    else
                                    {
                                        values.Add(new messageSystemValue { hint = InterString.Get("发动效果@ui"), value = card.effects[i].ptr.ToString() });
                                    }
                                }
                                if (card.effects[i].flag == 1)
                                {
                                    values.Add(new messageSystemValue { hint = InterString.Get("适用「[?]」的效果", card.get_data().Name), value = card.effects[i].ptr.ToString() });
                                }
                                if (card.effects[i].flag == 2)
                                {
                                    values.Add(new messageSystemValue { hint = InterString.Get("重置「[?]」的控制权", card.get_data().Name), value = card.effects[i].ptr.ToString() });
                                }
                            }
                            values.Add(new messageSystemValue { hint = InterString.Get("取消"), value = "hide" });
                            RMSshow_singleChoice("return", values);
                        }
                    }
                }
                break;
            case GameMessage.SelectPlace:
                break;
            case GameMessage.SelectPosition:
                break;
            case GameMessage.SelectCounter:
                if (card.forSelect)
                {
                    if (card.counterSELcount < card.counterCANcount)
                    {
                        card.counterSELcount++;
                    }
                    int sum = 0;
                    for (int i = 0; i < allCardsInSelectMessage.Count; i++)
                    {
                        sum += allCardsInSelectMessage[i].counterSELcount;
                    }
                    if (sum == ES_min)
                    {
                        BinaryMaster binaryMaster = new BinaryMaster();
                        for (int i = 0; i < allCardsInSelectMessage.Count; i++)
                        {
                            if (Config.ClientVersion>=0x133d)   
                            {
                                binaryMaster.writer.Write((short)allCardsInSelectMessage[i].counterSELcount);
                            }
                            else
                            {
                                binaryMaster.writer.Write((byte)allCardsInSelectMessage[i].counterSELcount);
                            }
                        }
                        sendReturn(binaryMaster.get());
                    }
                    else
                    {
                        for (int i = 0; i < allCardsInSelectMessage.Count; i++)
                        {
                            allCardsInSelectMessage[i].show_number(allCardsInSelectMessage[i].counterSELcount);
                        }
                    }
                }
                break;
            case GameMessage.SelectDisfield:
                break;
            case GameMessage.AnnounceRace:
                break;
            case GameMessage.AnnounceAttrib:
                break;
            case GameMessage.AnnounceCard:
            case GameMessage.AnnounceCardFilter:
                if (card.forSelect)
                {
                    BinaryMaster binaryMaster = new BinaryMaster();
                    binaryMaster.writer.Write((UInt32)card.get_data().Id);
                    sendReturn(binaryMaster.get());
                }
                break;
            case GameMessage.AnnounceNumber:
                break;
        }
    }

    List<placeSelector> placeSelectors = new List<placeSelector>();

    public void ES_placeSelected(placeSelector data)
    {
        data.selected = !data.selected;
        switch (currentMessage) 
        {
            case GameMessage.SelectPlace:
            case GameMessage.SelectDisfield:
                int all = 0;
                BinaryMaster binaryMaster = new BinaryMaster();
                for (int i = 0; i < placeSelectors.Count; i++)
                {
                    if (placeSelectors[i].selected)
                    {
                        binaryMaster.writer.Write(placeSelectors[i].data);
                        all++;
                    }
                }
                if (all == ES_min)
                {
                    ES_min = -2;
                    sendReturn(binaryMaster.get());
                }
                if (ES_min == -2)
                {
                    clearAllSelectPlace();
                }
                break;
            default:
                clearResponse();
                break;
        }
    }

    public override void ES_RMS(string hashCode, List<messageSystemValue> result)
    {
        base.ES_RMS(hashCode, result);
        BinaryMaster binaryMaster;
        switch (hashCode)
        {
            case "return":
                if (result[0].value != "hide")
                {
                    try
                    {
                        binaryMaster = new BinaryMaster();
                        binaryMaster.writer.Write(Int32.Parse(result[0].value));
                        sendReturn(binaryMaster.get());
                    }
                    catch (System.Exception e)
                    {
                        UnityEngine.Debug.Log(e);
                    }
                }
                break;
            case "autoForceChainHandler":
                if (result[0].value != "hide")
                {
                    if (result[0].value == "yes")
                    {
                        autoForceChainHandler = autoForceChainHandlerType.autoHandleAll;
                        try
                        {
                            binaryMaster = new BinaryMaster();
                            binaryMaster.writer.Write(0);
                            sendReturn(binaryMaster.get());
                        }
                        catch (System.Exception e)
                        {
                            UnityEngine.Debug.Log(e);
                        }
                    }
                    if (result[0].value == "no")
                    {
                        autoForceChainHandler = autoForceChainHandlerType.afterClickManDo;
                    }
                }
                break;
            case "returnMultiple":
                binaryMaster = new BinaryMaster();
                UInt32 res = 0;
                foreach (var item in result)
                {
                    try
                    {
                        res |= UInt32.Parse(item.value);
                    }
                    catch (System.Exception e)
                    {
                        UnityEngine.Debug.Log(e);
                    }
                }
                binaryMaster.writer.Write(res);
                sendReturn(binaryMaster.get());
                break;
            case "AnnounceCard":
                List<YGOSharp.Card> datas = YGOSharp.CardsManager.search(result[0].value, ES_searchCode);
                int max = datas.Count;
                if (max > 49)
                {
                    max = 49;
                }
                for (int i = 0; i < max; i++)
                {
                    GPS p = new GPS
                    {
                        controller = 0,
                        location = (UInt32)game_location.search,
                        sequence = (UInt32)i,
                        position = 0,
                    };
                    gameCard card = GCS_cardCreate(p);
                    card.set_data(datas[i]);
                    card.forSelect = true;
                    card.add_one_decoration(Program.I().mod_ocgcore_decoration_card_selecting, 2, Vector3.zero, "card_selecting");
                }
                realize();
                gameInfo.addHashedButton("clear", 0, superButtonType.no, InterString.Get("重新输入@ui"));
                toNearest();
                gameField.setHint(InterString.Get("请选择需要宣言的卡片。"));
                break;
            case "sort":
                if (result[0].value != "hide")
                {
                    for (int i = 0; i < cardsInSort.Count; i++)
                    {
                        cardsInSort[i].show_number(0);
                    }
                    if (result[0].value == "shun")
                    {
                        for (int i = 0; i < ES_sortCurrent.Count; i++)
                        {
                            ES_sortResult.Add(ES_sortCurrent[i]);
                        }
                    }
                    if (result[0].value == "fan")
                    {
                        for (int i = 0; i < ES_sortCurrent.Count; i++)
                        {
                            ES_sortResult.Add(ES_sortCurrent[ES_sortCurrent.Count - i - 1]);
                        }
                    }
                    if (ES_sortResult.Count == ES_sortSum)
                    {
                        sendSorted();
                    }
                    else
                    {
                        for (int i = 0; i < ES_sortResult.Count; i++)
                        {
                            ES_sortResult[i].card.show_number(i + 1, true);
                        }
                    }
                }
                break;
        }
    }

    public override void ES_RMS_ForcedYesNo(messageSystemValue result)
    {
        base.ES_RMS_ForcedYesNo(result);
        if (result.value == "yes")
        {
            surrended = true;
            if (TcpHelper.tcpClient != null && TcpHelper.tcpClient.Connected)
            {
                if (paused) 
                {
                    EventDelegate.Execute(UIHelper.getByName<UIButton>(toolBar, "go_").onClick);
                }
                TcpHelper.CtosMessage_Surrender();
            }
            else
            {
                onExit();
            }
        }
    }

    public Dictionary<int, int> sideReference = new Dictionary<int, int>();

    void onDuelResultConfirmed()
    {

        if (Program.I().room.duelEnded == true || surrended || TcpHelper.tcpClient == null || TcpHelper.tcpClient.Connected == false)
        {
            surrended = false;
            Program.I().room.duelEnded = false;
            Program.I().room.needSide = false;
            Program.I().room.sideWaitingObserver = false;
            onExit();
            return;
        }

        if (Program.I().room.needSide == true)
        {
            Program.I().room.needSide = false;
            RMSshow_none(InterString.Get("右侧为您准备了对手上一局使用的卡。"));
            ((DeckManager)Program.I().deckManager).shiftCondition(DeckManager.Condition.changeSide);
            returnTo();
            ((DeckManager)Program.I().deckManager).deck = TcpHelper.deck;
            ((DeckManager)Program.I().deckManager).FormCodedDeckToObjectDeck();
            ((CardDescription)Program.I().cardDescription).setTitle(Config.Get("deckInUse", "miaowu"));
            ((DeckManager)Program.I().deckManager).setGoodLooking(true);
            ((DeckManager)Program.I().deckManager).returnAction = returnFromDeckEdit;
            return;
        }

        if (condition != Condition.duel)
        {
            hideCaculator();
            return;
        }

        RMSshow_yesOrNoForce(InterString.Get("你确定要投降吗？"), new messageSystemValue { value = "yes", hint = "yes" }, new messageSystemValue { value = "no", hint = "no" });
    }

    private void sendSorted()
    {
        BinaryMaster m = new BinaryMaster();
        byte[] bytes = new byte[ES_sortResult.Count];
        for (int i = 0; i < ES_sortResult.Count; i++)
        {
            bytes[ES_sortResult[i].option] = (byte)i;
        }
        for (int i = 0; i < ES_sortResult.Count; i++)
        {
            m.writer.Write(bytes);
        }
        sendReturn(m.get());
    }

    bool rightExcited = false;
    public override void ES_mouseDownRight()
    {
        if (gameInfo.queryHashedButton("sendSelected") == true)
        {
            return;
        }
        if (flagForCancleChain)
        {
            return;
        }
        if (gameInfo.queryHashedButton("hide_all_card") == true)
        {
            if (flagForTimeConfirm)
            {
                return;
            }
        }
        if (gameInfo.queryHashedButton("cancleSelected") == true)
        {
            return;
        }
        rightExcited = true;
        //gameInfo.ignoreChain_set(true);
        base.ES_mouseDownRight();
    }


    bool leftExcited = false;
    public override void ES_mouseDownEmpty()    
    {
        if (Program.I().setting.setting.spyer.value == false)
            if (gameInfo.queryHashedButton("hide_all_card") == false)
            {
                //gameInfo.keepChain_set(true);
                leftExcited = true;
            }
        base.ES_mouseDownEmpty();
    }

    public override void ES_mouseUpEmpty()
    {
        if (Program.I().setting.setting.spyer.value)
        {
            Program.I().cardDescription.shiftCardShower(false);
        }
        if (gameInfo.queryHashedButton("hide_all_card") == true)
        {
            if (flagForTimeConfirm)
            {
                flagForTimeConfirm = false;
                MessageBeginTime = Program.TimePassed();
            }
            clearAllShowed();
        }
        else
        {
            if (Program.I().setting.setting.spyer.value == false)
                if (leftExcited)
                {
                    if (Input.GetKey(KeyCode.A) == false)
                    {
                        leftExcited = false;
                        //gameInfo.keepChain_set(false);
                    }

                }
        }
        base.ES_mouseUpEmpty();
    }

    public override void ES_mouseUpGameObject(GameObject gameObject)
    {
        if (gameObject==gameInfo.instance_lab.gameObject)  
        {
            ES_mouseUpEmpty();
            return;
        }
        if (leftExcited)
        {
            if (Input.GetKey(KeyCode.A) == false)
            {
                leftExcited = false;
                //gameInfo.keepChain_set(false);
            }
        }
        base.ES_mouseUpGameObject(gameObject);
    }

    public override void ES_mouseUpRight()
    {
        base.ES_mouseUpRight();
        if (rightExcited)
        {
            if (Input.GetKey(KeyCode.S) == false)
            {
                rightExcited = false;
                //gameInfo.ignoreChain_set(false);
            }
        }
        if (gameInfo.queryHashedButton("sendSelected") == true)
        {
            sendSelectedCards();
            return;
        }
        if (flagForCancleChain)
        {
            flagForCancleChain = false;
            clearAllShowedB = true;
            BinaryMaster binaryMaster = new BinaryMaster();
            binaryMaster.writer.Write((Int32)(-1));
            sendReturn(binaryMaster.get());
            return;
        }
        if (gameInfo.queryHashedButton("hide_all_card") == true)
        {
            if (flagForTimeConfirm)
            {
                flagForTimeConfirm = false;
                MessageBeginTime = Program.TimePassed();
                clearAllShowed();
                return;
            }
        }
        if (gameInfo.queryHashedButton("cancleSelected") == true)
        {
            BinaryMaster binaryMaster = new BinaryMaster();
            binaryMaster.writer.Write(-1);
            sendReturn(binaryMaster.get());
            return;
        }

    }

    void animation_confirm(gameCard target)
    {
        Program.I().cardDescription.setData(target.get_data(), target.p.controller == 0 ? GameTextureManager.myBack : GameTextureManager.opBack, target.tails.managedString);
        target.animation_confirm_screenCenter(new Vector3(-30, 0, 0), 0.2f, 0.5f);
    }

    public void animation_show_card_code(long code)
    {
        code_for_show = code;
        AddUpdateAction_s(animation_show_card_code_handler);
        Sleep(30);
    }
    long code_for_show = 0;

    public bool InAI = false;

    void animation_show_card_code_handler()
    {
        Texture2D texture = GameTextureManager.get(code_for_show, GameTextureType.card_picture);
        if (texture != null)
        {
            RemoveUpdateAction_s(this.animation_show_card_code_handler);
            //Vector3 position = Program.camera_game_main.ScreenToWorldPoint(new Vector3(getScreenCenter(), Screen.height / 2f, 10));
            //GameObject obj = create_s(Program.I().mod_simple_quad);
            //obj.AddComponent<animation_screen_lock>().screen_point = new Vector3(getScreenCenter(), Screen.height / 2f, 6);
            //obj.transform.eulerAngles = new Vector3(60, 0, 0);
            //obj.GetComponent<Renderer>().material.mainTexture = texture;
            //obj.transform.localPosition = position;
            //obj.transform.localScale = new Vector3(3.2f, 4.6f, 1f);
            //destroy(obj, 1f);
            pro1CardShower shower = create(Program.I().Pro1_CardShower, Program.I().ocgcore.centre(), Vector3.zero, false, Program.ui_main_2d, true).GetComponent<pro1CardShower>();
            shower.card.mainTexture = texture;
            shower.mask.mainTexture = GameTextureManager.Mask;
            shower.disable.mainTexture = GameTextureManager.negated;
            shower.gameObject.transform.localScale = new Vector3(Screen.height / 650f, Screen.height / 650f, Screen.height / 650f);
            destroy(shower.gameObject, 0.5f);
        }
    }
}
