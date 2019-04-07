using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YGOSharp.OCGWrapper.Enums;

public enum gameCardCondition
{
    floating_clickable = 1,
    still_unclickable = 2,
    verticle_clickable = 3,
}

public struct GPS   
{
    public UInt32 controller;   
    public UInt32 location; 
    public UInt32 sequence; 
    public int position;
}

public class Effect    
{
    public int ptr;
    public string desc;
    public int flag;
}

public class gameCard : OCGobject
{
    public GPS p;

    public uint controllerBased = 0;

    public int md5 = -233;

    public List<gameCard> target = new List<gameCard>();

    public void addTarget(gameCard card_)
    {
        bool exist = false;
        foreach (var item in target)
        {
            if (item == card_)
            {
                exist = true;
            }
        }
        if (exist == false)
        {
            target.Add(card_);
        }
    }

    public void removeTarget(gameCard card_)    
    {
        target.Remove(card_);
    }

    public bool isShowed = false;

    public bool isMinBlockMode = true;

    //public bool getIfInMinMode()
    //{
    //    return isMinBlockMode && (ES_excited_unsafe_should_not_be_changed_dont_touch_this==false);
    //}

    public bool cookie_cared = false;

    public bool forSelect = false;

    public int selectPtr = 0;

    public int levelForSelect_1 = 0;

    public int levelForSelect_2 = 0;

    public int counterCANcount = 0;

    public int counterSELcount = 0;

    public bool prefered = false;

    YGOSharp.Card data;

    GameObject gameObject_face;

    GameObject gameObject_back;

    GameObject gameObject_event_main;

    GameObject gameObject_event_card_bed;

    TMPro.TextMeshPro cardHint;

    public gameCardCondition condition = gameCardCondition.floating_clickable;

    GameObject game_object_verticle_drawing = null;

    TMPro.TextMeshPro verticle_number = null;

    GameObject game_object_verticle_Star = null;    

    GameObject game_object_monster_cloude = null;

    ParticleSystem game_object_monster_cloude_ParticleSystem = null;

    //public int ability = 2500;

    GameObject obj_number = null;

    BoxCollider VerticleCollider = null;

    int number_showing = 0;

    public List<Effect> effects = new List<Effect>();

    public List<int> sortOptions = new List<int>();

    public gameCard()
    {
        gameObject =Program.I().create(Program.I().mod_ocgcore_card);
        gameObject_face = gameObject.transform.Find("card").Find("face").gameObject;
        gameObject_back = gameObject.transform.Find("card").Find("back").gameObject;
        gameObject_event_main = gameObject.transform.Find("card").Find("event").gameObject;
        cardHint = gameObject.transform.Find("text").GetComponent<TMPro.TextMeshPro>();
        SpSummonFlash = insFlash("0099ff");
        ActiveFlash = insFlash("00ff66");
        SelectFlash = insFlash("ff8000");
        for (int i = 0; i < 2; i++)
        {
            SpSummonFlash[i].gameObject.SetActive(false);
            ActiveFlash[i].gameObject.SetActive(false);
            SelectFlash[i].gameObject.SetActive(false);
        }
        selectKuang = insKuang(Program.I().New_selectKuang);
        chainKuang = insKuang(Program.I().New_chainKuang);
        selectKuang.SetActive(false);
        chainKuang.SetActive(false);
        gameObject.SetActive(false);

    }

    public bool forceRefreshCondition = false;

    public void show()
    {
        clearCookie();
        gameObject.SetActive(true);
        Program.I().ocgcore.AddUpdateAction_s(Update);
        refreshFunctions.Clear();
        refreshFunctions.Add(RefreshFunction_ES);
        refreshFunctions.Add(RefreshFunction_decoration);
        refreshFunctions.Add(card_picture_handler);
        forceRefreshCondition = true;
        gameObject.transform.position = accurate_position;
        gameObject.transform.eulerAngles = accurate_rotation;
    }

    public void hide()
    {
        try
        {
            set_overlay_light(0);
            clearCookie();
            UIHelper.clearITWeen(gameObject);
            del_all_decoration();
            for (int i = 0; i < allObjects.Count; i++)
            {
                MonoBehaviour.Destroy(allObjects[i]);
            }
            allObjects.Clear();
            set_text("");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
        gameObject.SetActive(false);
    }

    void clearCookie()
    {
        Program.I().ocgcore.RemoveUpdateAction_s(Update);
        isShowed = false;
        prefered = false;
        erase_data();
        target.Clear();
        loaded_cardPictureCode = -1;
        loaded_cardCode = -1;
        loaded_back = -1;
        loaded_specialHint = -1;
        loaded_verticalDrawingCode = -1;
        loaded_verticalDrawingReal = Program.getVerticalTransparency() > 0.5f;
        loaded_verticalDrawingNumber = -1;
        loaded_verticalOverAttribute = -1;
        loaded_verticalatk = -1;
        loaded_verticaldef = -1;
        loaded_verticalpos = -1;
        loaded_verticalcon = -1;
        loaded_controller = -1;
        loaded_location = -1;
        p = new GPS
        {
            controller = 0,
            location = 0,
            position = 0,
            sequence = 0
        };
        CS_clear();
    }

    List<Action> refreshFunctions = new List<Action>();

    public void Update()
    {
        for (int i = 0; i < refreshFunctions.Count; i++)
        {
            try
            {
                refreshFunctions[i]();
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
        }
    }

    GameObject nagaSign = null; 

    public bool disabled = false;

    public enum flashType
    {
        SpSummon,Active, Select, none
    }

    public enum kuangType
    {
        selected,chaining, none
    }

    public flashType currentFlash = flashType.none;

    public kuangType currentKuang = kuangType.none;

    flashType currentFlashPre = flashType.none;

    kuangType currentKuangPre = kuangType.none; 

    FlashingController[] SpSummonFlash, ActiveFlash, SelectFlash;

    GameObject selectKuang, chainKuang;

    FlashingController MouseFlash;

    void RefreshFunction_decoration()
    {
        for (int i = 0; i < cardDecorations.Count; i++)
        {
            if (cardDecorations[i].game_object != null)
            {
                Vector3 screenposition = Vector3.zero;
                if (cardDecorations[i].up_of_card)
                {
                    screenposition = Program.camera_game_main.WorldToScreenPoint(gameObject_face.transform.position + new Vector3(0, 1.2f, 1.2f * 1.732f));
                }
                else
                {
                    screenposition = Program.camera_game_main.WorldToScreenPoint(gameObject_face.transform.position);
                }
                Vector3 worldposition = Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z - cardDecorations[i].relative_position));
                cardDecorations[i].game_object.transform.eulerAngles = cardDecorations[i].rotation;
                cardDecorations[i].game_object.transform.position = worldposition;
                if (cardDecorations[i].scale_change_ignored == false)
                    cardDecorations[i].game_object.transform.localScale += (new Vector3(1, 1, 1) - cardDecorations[i].game_object.transform.localScale) * 0.3f;
            }
        }
        for (int i = 0; i < overlay_lights.Count; i++)
        {
            overlay_lights[i].transform.position = gameObject_face.transform.position + new Vector3(0, 1.8f, 0);
        }
        if (obj_number != null)
        {
            Vector3 screenposition = Program.camera_game_main.WorldToScreenPoint(gameObject_face.transform.position + new Vector3(0, 1f * 2.4f, 1.732f * 2.4f));
            Vector3 worldposition = Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z - 5));
            obj_number.transform.position = worldposition;
        }
        if (disabled == true && (((p.location & (UInt32)CardLocation.MonsterZone) > 0) || ((p.location & (UInt32)CardLocation.SpellZone) > 0)))
        {
            if (nagaSign == null)
            {
                nagaSign = create(Program.I().mod_simple_quad);
                nagaSign.transform.localScale = Vector3.zero;
                nagaSign.GetComponent<Renderer>().material.mainTexture = GameTextureManager.negated;
            }
            if (game_object_verticle_drawing != null && Program.getVerticalTransparency() > 0.5f)
            {
                if (nagaSign.transform.parent!= game_object_verticle_drawing.transform)
                {
                    nagaSign.transform.SetParent(game_object_verticle_drawing.transform);
                    nagaSign.transform.localRotation = Quaternion.identity;
                    nagaSign.transform.localScale = Vector3.zero;
                    nagaSign.transform.localPosition = new Vector3(0,0,-0.25f);
                }
                try
                {
                    Vector3 devide = game_object_verticle_drawing.transform.localScale;
                    if (Vector3.Distance(Vector3.zero, devide) > 0.01f)
                        nagaSign.transform.localScale = (new Vector3(2.4f / devide.x, 2.4f / devide.y, 2.4f / devide.z));
                }
                catch (Exception)
                {
                }
            }
            else
            {
                if (nagaSign.transform.parent != gameObject_face.transform)
                {
                    nagaSign.transform.SetParent(gameObject_face.transform);
                    nagaSign.transform.localRotation = Quaternion.identity;
                    nagaSign.transform.localScale = Vector3.zero;
                    nagaSign.transform.localPosition = new Vector3(0, 0, -0.25f);
                }
                try
                {
                    Vector3 devide = gameObject_face.transform.localScale;
                    if (Vector3.Distance(Vector3.zero, devide) > 0.01f)
                        nagaSign.transform.localScale = (new Vector3(2.4f / devide.x, 2.4f / devide.y, 2.4f / devide.z));
                }
                catch (Exception)
                {
                }
            }
        }
        else
        {
            if (nagaSign != null)
            {
                destroy(nagaSign,0.6f,true,true);
            }
        }
        if (currentKuangPre!=currentKuang)  
        {
            currentKuangPre = currentKuang;
            switch (currentKuang)
            {
                case kuangType.selected:
                    selectKuang.SetActive(true);
                    chainKuang.SetActive(false);
                    break;
                case kuangType.chaining:
                    selectKuang.SetActive(false);
                    chainKuang.SetActive(true);
                    break;
                case kuangType.none:
                    selectKuang.SetActive(false);
                    chainKuang.SetActive(false);
                    break;
            }

        }
        if (currentFlashPre != currentFlash)
        {
            currentFlashPre = currentFlash;
            switch (currentFlash)
            {
                case flashType.SpSummon:
                    for (int i = 0; i < 2; i++)
                    {
                        ActiveFlash[i].gameObject.SetActive(false);
                        SelectFlash[i].gameObject.SetActive(false);
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        SpSummonFlash[i].gameObject.SetActive(true);
                    }
                    break;
                case flashType.Active:
                    for (int i = 0; i < 2; i++)
                    {
                        SpSummonFlash[i].gameObject.SetActive(false);
                        SelectFlash[i].gameObject.SetActive(false);
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        ActiveFlash[i].gameObject.SetActive(true);
                    }
                    break;
                case flashType.Select:
                    for (int i = 0; i < 2; i++)
                    {
                        SpSummonFlash[i].gameObject.SetActive(false);
                        ActiveFlash[i].gameObject.SetActive(false);
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        SelectFlash[i].gameObject.SetActive(true);
                    }
                    break;
                case flashType.none:
                    for (int i = 0; i < 2; i++)
                    {
                        SpSummonFlash[i].gameObject.SetActive(false);
                        ActiveFlash[i].gameObject.SetActive(false);
                        SelectFlash[i].gameObject.SetActive(false);
                    }
                    break;
            }
        }
        handlerChain();
    }

    #region ES_system

    private bool ES_mouse_check()
    {
        bool re = false;
        if (gameObject_event_main != null)
        {
            if (Program.pointedGameObject == gameObject_event_main)
            {
                re = true;
            }
        }
        if (gameObject_event_card_bed != null)
        {
            if (Program.pointedGameObject == gameObject_event_card_bed)
            {
                re = true;
            }
        }
        if (game_object_verticle_drawing != null)
        {
            if (Program.pointedGameObject == game_object_verticle_drawing)
            {
                re = true;
            }
        }
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].gameObjectEvent != null)
            {
                if (Program.pointedGameObject == buttons[i].gameObjectEvent)
                {
                    re = true;
                }
            }
        }
        if (condition == gameCardCondition.still_unclickable)
        {
            re = false;
        }
        return re;
    }

    public void ES_lock(float time)
    {
        ES_exit_excited(false);
        MonoBehaviour.Destroy(gameObject.AddComponent<card_locker>(), time);
    }

    private bool ES_check_locked()
    {
        bool return_value = false;
        if (gameObject.transform.GetComponent<card_locker>() != null)
        {
            return_value = true;
        }
        return return_value;
    }

    private bool ES_excited_unsafe_should_not_be_changed_dont_touch_this = false;

    private void RefreshFunction_ES()
    {
        if (Program.InputGetMouseButtonUp_0 && ES_mouse_check())
        {
            Program.I().ocgcore.ES_cardClicked(this);
        }

        if (ES_excited_unsafe_should_not_be_changed_dont_touch_this)
        {
            //当前在excited态
            if (ES_mouse_check())
            {
                //刷新excited的数据
                ES_excited_handler();
            }
            else
            {
                //退出excited态
                ES_exit_excited(true);
            }
        }
        else
        {
            //当前不在excited态
            if (ES_mouse_check())
            {
                if (ES_check_locked() == false)
                {
                    //进入excited态
                    ES_enter_excited();
                }
                else
                {
                    //无作为
                }
            }
            else
            {
                //无作为
            }
        }
    }

    private void ES_excited_handler()
    {
        if (ES_excited_unsafe_should_not_be_changed_dont_touch_this)
        {
            ES_excited_handler_close_up_handler();
            ES_excited_handler_button_shower();
            ES_excited_handler_event_cookie_card_bed();
        }
    }

    //float deltaTimeCloseUp=0;
    //private void ES_excited_handler_close_up_handler()
    //{
    //    float faT = 0.25f;
    //    deltaTimeCloseUp += Time.deltaTime;
    //    if (deltaTimeCloseUp > faT)
    //    {
    //        deltaTimeCloseUp = faT;
    //    }
    //    Vector3 screenposition = Program.camera_game_main.WorldToScreenPoint(accurate_position);
    //    Vector3 worldposition = Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z - 10));
    //    gameObject.transform.position = new Vector3
    //        (
    //        iTween.easeOutQuad(accurate_position.x, worldposition.x, deltaTimeCloseUp / faT),
    //        iTween.easeOutQuad(accurate_position.y, worldposition.y, deltaTimeCloseUp / faT),
    //        iTween.easeOutQuad(accurate_position.z, worldposition.z, deltaTimeCloseUp / faT)
    //        );
    //    if (game_object_verticle_drawing != null)
    //    {
    //        card_verticle_drawing_handler();
    //    }
    //}

    private void ES_excited_handler_close_up_handler()
    {
        Vector3 screenposition = Program.camera_game_main.WorldToScreenPoint(accurate_position);
        Vector3 worldposition = Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z - 10));
        gameObject.transform.position += (worldposition - gameObject.transform.position) * 35f * Program.deltaTime;
        if (game_object_verticle_drawing != null)
        {
            card_verticle_drawing_handler();
        }
    }

    private void ES_excited_handler_button_shower()
    {
        if (opMonsterWithBackGroundCard)   
        {
            Vector3 vector_of_begin = Vector3.zero;
            if ((p.position & (UInt32)CardPosition.Attack) > 0)
            {
                vector_of_begin = gameObject_face.transform.position + new Vector3(0, 0, -2f);
            }
            else
            {
                vector_of_begin = gameObject_face.transform.position + new Vector3(0, 0, -1.5f);
            }
            vector_of_begin = Program.camera_game_main.WorldToScreenPoint(vector_of_begin);
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].show(vector_of_begin - i * (new Vector3(0, 65f * 0.7f * (float)Screen.height / 700f)) - (new Vector3(0, 20f * 0.7f * (float)Screen.height / 700f)));
            }
            return;
        }

        if (condition == gameCardCondition.floating_clickable)
        {
            Vector3 vector_of_begin = gameObject_face.transform.position + new Vector3(0, 1, 1.732f);
            vector_of_begin = Program.camera_game_main.WorldToScreenPoint(vector_of_begin);
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].show(vector_of_begin + i * (new Vector3(0, 65f * 0.7f * (float)Screen.height / 700f)) + (new Vector3(0, 35f * 0.7f * (float)Screen.height / 700f)));
            }
            return;
        }

        if (condition== gameCardCondition.verticle_clickable)   
        {
            if (VerticleCollider == null)
            {
                Vector3 vector_of_begin;
                if ((p.position & (UInt32)CardPosition.Attack) > 0)
                {
                    vector_of_begin = gameObject_face.transform.position + new Vector3(0, 0, 2);
                }
                else
                {
                    vector_of_begin = gameObject_face.transform.position + new Vector3(0, 0, 1.5f);
                }
                vector_of_begin = Program.camera_game_main.WorldToScreenPoint(vector_of_begin);
                for (int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].show(vector_of_begin + i * (new Vector3(0, 65f * 0.7f * (float)Screen.height / 700f)) + (new Vector3(0, 35f * 0.7f * (float)Screen.height / 700f)));
                }
            }
            else
            {
                float h = loaded_verticalDrawingK * 0.618f;
                Vector3 vector_of_begin = Vector3.zero;
                float l = (0.5f * game_object_verticle_drawing.transform.localScale.y * (h - 0.5f));
                vector_of_begin = game_object_verticle_drawing.transform.position + new Vector3(0, l, l * 1.732f);
                vector_of_begin = Program.camera_game_main.WorldToScreenPoint(vector_of_begin);
                for (int i = 0; i < buttons.Count; i++)
                {
                    buttons[i].show(vector_of_begin + i * (new Vector3(0, 65f * 0.7f * (float)Screen.height / 700f)) + (new Vector3(0, 35f * 0.7f * (float)Screen.height / 700f)));
                }
            }
            return;
        }
    }

    private void ES_excited_handler_event_cookie_card_bed()
    {
        if (condition != gameCardCondition.verticle_clickable)
        {
            if (gameObject_event_card_bed == null)
            {
                gameObject_event_card_bed
                    = create(Program.I().mod_ocgcore_hidden_button, gameObject.transform.position); 
            }
        }
        else
        {
            if (gameObject_event_card_bed != null)
            {
                destroy(gameObject_event_card_bed);
            }
        }
    }

    private void ES_enter_excited()
    {
        //Program.I().audio.clip = Program.I().dididi;
        //Program.I().audio.Play();
        //deltaTimeCloseUp = 0;
        iTween[] iTweens = gameObject.GetComponents<iTween>();
        for (int i = 0; i < iTweens.Length; i++) MonoBehaviour.DestroyImmediate(iTweens[i]);
        if (condition == gameCardCondition.floating_clickable)
        {
            flash_line_on();
            iTween.RotateTo(gameObject, new Vector3(-30, 0, 0), 0.3f);
        }
        ES_excited_unsafe_should_not_be_changed_dont_touch_this = true;
        showMeLeft(true);
        List<gameCard> overlayed_cards = Program.I().ocgcore.GCS_cardGetOverlayElements(this);
        Vector3 screen = Program.camera_game_main.WorldToScreenPoint(gameObject.transform.position);
        screen.z = 0;
        float k = ((float)Screen.height) / 700f;
        for (int x = 0; x < overlayed_cards.Count; x++)
        {
            if (overlayed_cards[x].isShowed == false)
            {
                float pianyi = 130f;
                if (Program.getVerticalTransparency() < 0.5f)
                {
                    pianyi =90f;
                }
                Vector3 screen_vector_to_move = screen + new Vector3(pianyi * k + 60f * k * (overlayed_cards.Count - overlayed_cards[x].p.position - 1), 0, 12f + 2f * (overlayed_cards.Count - overlayed_cards[x].p.position - 1));
                overlayed_cards[x].flash_line_on();
                overlayed_cards[x].TweenTo(Camera.main.ScreenToWorldPoint(screen_vector_to_move), new Vector3(-30, 0, 0),true);
            }
        }
    }

    void showMeLeft(bool force=false)
    {
        Program.I().cardDescription.setData(data, p.controller == 0 ? GameTextureManager.myBack : GameTextureManager.opBack, tails.managedString, force);
    }

    public void ES_exit_excited(bool move_to_original_place)
    {
        iTween[] iTweens = gameObject.GetComponents<iTween>();
        for (int i = 0; i < iTweens.Length; i++) MonoBehaviour.DestroyImmediate(iTweens[i]);
        flash_line_off();
        ES_excited_unsafe_should_not_be_changed_dont_touch_this = false;
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].hide();
        }
        destroy(gameObject_event_card_bed);
        if (move_to_original_place)
        {
            ES_safe_card_move_to_original_place();
        }
        List<gameCard> overlayed_cards = Program.I().ocgcore.GCS_cardGetOverlayElements(this);
        for (int x = 0; x < overlayed_cards.Count; x++)
        {
            overlayed_cards[x].ES_safe_card_move_to_original_place();
            overlayed_cards[x].flash_line_off();
        }
        MonoBehaviour.Destroy(gameObject.AddComponent<card_locker>(), 0.3f);
    }

    public void ES_safe_card_move_to_original_place()
    {
        TweenTo(accurate_position,accurate_rotation);
    }

    private void ES_safe_card_move(Hashtable move_hash, Hashtable rotate_hash)
    {
        UIHelper.clearITWeen(gameObject);
        MonoBehaviour.DestroyImmediate(gameObject.GetComponent<screenFader>());
        if (Math.Abs((int)(gameObject.transform.eulerAngles.z)) == 180)
        {
            Vector3 p = gameObject.transform.eulerAngles;
            p.z = 179f;
            gameObject.transform.eulerAngles = p;
        }
        iTween.MoveTo(gameObject, move_hash);
        iTween.RotateTo(gameObject, rotate_hash);
    }

    #endregion

    #region UA_system

    //UA_system
    Vector3 gived_position = Vector3.zero;
    Vector3 gived_rotation = Vector3.zero;
    Vector3 accurate_position = Vector3.zero;
    Vector3 accurate_rotation = Vector3.zero;

    public void UA_give_position(Vector3 p)
    {
        gived_position = p;
    }

    public Vector3 UA_get_accurate_position()
    {
        return accurate_position;
    }

    public void UA_give_rotation(Vector3 r)
    {
        gived_rotation = r;
    }

    public void UA_flush_all_gived_witn_lock(bool rush)
    {
        if (Vector3.Distance(gived_position, accurate_position) > 0.001f || Vector3.Distance(gived_rotation, accurate_rotation) > 0.001f)
        {
            float time = 0.25f;
            time += Vector3.Distance(gived_position, gameObject.transform.position) * 0.05f / 20f;
            ES_lock(time+0.1f);
            UA_reloadCardHintPosition();
            if (rush)
            {
                UIHelper.clearITWeen(gameObject);
                gameObject.transform.position = gived_position;
                gameObject.transform.eulerAngles = gived_rotation;
            }
            else
            {
                TweenTo(gived_position, gived_rotation);
                if (
                    Program.I().ocgcore.currentMessage == GameMessage.Move
                     ||
                    Program.I().ocgcore.currentMessage == GameMessage.Swap
                     ||
                    Program.I().ocgcore.currentMessage == GameMessage.PosChange
                     ||
                    Program.I().ocgcore.currentMessage == GameMessage.FlipSummoning
                    )
                {
                    Program.I().ocgcore.Sleep((int)(30f * time));
                }
            }
            accurate_position = gived_position;
            accurate_rotation = gived_rotation;
        }
    }

    public void TweenTo(Vector3 pos, Vector3 rot, bool exciting = false)
    {
        float time = 0.1f;
        time += Vector3.Distance(pos, gameObject.transform.position) * 0.2f / 30f;
        if (time < 0.1f)
        {
            time = 0.1f;
        }
        if (time > 0.3f)
        {
            time = 0.3f;
        }
        //time *= 20;
        iTween.EaseType e = iTween.EaseType.easeOutQuad;

        if (Vector3.Distance(Vector3.zero, pos) < Vector3.Distance(Vector3.zero, gameObject.transform.position))
        {
            e = iTween.EaseType.easeInQuad;
        }

        if (
            ((Math.Abs(gived_rotation.x) < 10 && Vector3.Distance(pos, gameObject.transform.position) > 1f))
            ||
            (accurate_position.x == pos.x && accurate_position.y < pos.y && accurate_position.z == pos.z)
        )
        {
            Vector3 from = gameObject.transform.position;
            Vector3 to = pos;
            Vector3[] path = new Vector3[30];
            for (int i = 0; i < 30; i++)
            {
                path[i] = from + (to - from) * (float)i / 29f + (new Vector3(0, 1.5f, 0)) * (float)Math.Sin(3.1415926 * (double)i / 29d);
            }
            if (exciting)   
            {
                ES_safe_card_move(
                       iTween.Hash(
                       "x", pos.x,
                       "y", pos.y,
                       "z", pos.z,
                       "path", path,
                       "time", time
                       ),
                       iTween.Hash
                       (
                       "x", rot.x,
                       "y", rot.y,
                       "z", rot.z,
                       "time", time
                       )
                       );
            }
            else
            {
                ES_safe_card_move(
                       iTween.Hash(
                       "x", pos.x,
                       "y", pos.y,
                       "z", pos.z,
                       "path", path,
                       "time", time,
                       "easetype", e
                       ),
                       iTween.Hash
                       (
                       "x", rot.x,
                       "y", rot.y,
                       "z", rot.z,
                       "time", time,
                       "easetype", e
                       )
                       );
            }

        }
        else
        {
            if (exciting)   
            {
                ES_safe_card_move(
                          iTween.Hash(
                          "x", pos.x,
                          "y", pos.y,
                          "z", pos.z,
                          "time", time
                          ),
                          iTween.Hash
                          (
                          "x", rot.x,
                          "y", rot.y,
                          "z", rot.z,
                           "time", time
                          )
                         );
            }
            else
            {
                ES_safe_card_move(
                          iTween.Hash(
                          "x", pos.x,
                          "y", pos.y,
                          "z", pos.z,
                          "time", time,
                          "easetype", e
                          ),
                          iTween.Hash
                          (
                          "x", rot.x,
                          "y", rot.y,
                          "z", rot.z,
                           "time", time,
                           "easetype", e
                          )
                         );
            }

        }
    }

    private void UA_reloadCardHintPosition()
    {
        if ((p.location & (UInt32)CardLocation.MonsterZone) > 0 && (p.location & (UInt32)CardLocation.Overlay) == 0)
        {
            if (p.controller == 0)
            {
                if ((p.position & (UInt32)CardPosition.Attack) > 0)
                {
                    cardHint.gameObject.transform.localPosition = new Vector3(0, 0, -2.5f);
                    cardHint.gameObject.transform.localEulerAngles = new Vector3(60, 0, 0);
                }
                else
                {
                    cardHint.gameObject.transform.localPosition = new Vector3(-2.5f, 0, 0);
                    cardHint.gameObject.transform.localEulerAngles = new Vector3(60, 90, 0);
                }
            }
            else
            {
                if ((p.position & (UInt32)CardPosition.Attack) > 0)
                {
                    cardHint.gameObject.transform.localPosition = new Vector3(0, 0, 2.5f);
                    cardHint.gameObject.transform.localEulerAngles = new Vector3(40, 180, 0);
                }
                else
                {
                    cardHint.gameObject.transform.localPosition = new Vector3(2.5f, 0, 0);
                    cardHint.gameObject.transform.localEulerAngles = new Vector3(40, -90, 0);
                }
            }
        }
        else
        {
            cardHint.gameObject.transform.localPosition = new Vector3(0, 0, -2.5f);
            cardHint.gameObject.transform.localEulerAngles = new Vector3(90, 0, 0);
        }
    }

    //private void bugOfUnity()
    //{
    //    this.gameObject.transform.eulerAngles = this.accurate_rotation;
    //}

    public void UA_give_condition(gameCardCondition c)
    {
        if (condition != c || forceRefreshCondition)
        {
            condition = c;
            forceRefreshCondition = false;
            if (condition == gameCardCondition.floating_clickable)
            {
                try
                {
                    gameObject_event_main.GetComponent<MeshCollider>().enabled = true;
                    gameObject.transform.Find("card").GetComponent<animation_floating_slow>().enabled = true;
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e);
                }
                destroy(game_object_monster_cloude);
                destroy(game_object_verticle_drawing);
                if (verticle_number != null) destroy(verticle_number.gameObject);
                destroy(game_object_verticle_Star);
                refreshFunctions.Remove(this.card_verticle_drawing_handler);
                refreshFunctions.Remove(this.monster_cloude_handler);
                loaded_controller = -1;
                loaded_location = -1;
                refreshFunctions.Add(this.card_floating_text_handler);
                //caculateAbility();
            }
            if (condition == gameCardCondition.still_unclickable)
            {
                try
                {
                    gameObject_event_main.GetComponent<MeshCollider>().enabled = false;
                    gameObject.transform.Find("card").GetComponent<animation_floating_slow>().enabled = false;
                    destroy(gameObject_event_card_bed);
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e);
                }
                destroy(game_object_monster_cloude);
                destroy(game_object_verticle_drawing);
                if (verticle_number!=null) destroy(verticle_number.gameObject); 
                destroy(game_object_verticle_Star);
                refreshFunctions.Remove(this.card_verticle_drawing_handler);
                refreshFunctions.Remove(this.monster_cloude_handler);
                refreshFunctions.Remove(this.card_floating_text_handler);
                gameObject.transform.Find("card").transform.localPosition = Vector3.zero;
                set_text("");
                //caculateAbility();
            }
            if (condition == gameCardCondition.verticle_clickable)
            {
                try
                {
                    gameObject_event_main.GetComponent<MeshCollider>().enabled = true;
                    gameObject.transform.Find("card").GetComponent<animation_floating_slow>().enabled = true;
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e);
                }
                loaded_verticalDrawingCode = 0;
                loaded_verticalDrawingNumber = -1;
                loaded_verticalOverAttribute = -1;
                loaded_verticalatk = -1;
                loaded_verticaldef = -1;
                loaded_verticalpos = -1;
                loaded_verticalcon = -1;
                refreshFunctions.Add(this.card_verticle_drawing_handler);
                refreshFunctions.Add(this.monster_cloude_handler);
                refreshFunctions.Remove(this.card_floating_text_handler);
                //caculateAbility();
            }
        }
    }
    int loaded_controller = -1;
    int loaded_location = -1;
    private void card_floating_text_handler()
    {
        if (loaded_controller!= p.controller|| loaded_location!= p.location)    
        {
            loaded_controller = (int)p.controller;
            loaded_location = (int)p.location;
            set_text("");
            if (p.controller == 0 && (p.location & (UInt32)CardLocation.Deck) > 0)
            {
                set_text(GameStringHelper.kazu);
            }
            if (p.controller == 0 && (p.location & (UInt32)CardLocation.Extra) > 0)
            {
                set_text(GameStringHelper.ewai);
            }
            if (p.controller == 0 && (p.location & (UInt32)CardLocation.Grave) > 0)
            {
                set_text(GameStringHelper.mudi);
            }
            if (p.controller == 0 && (p.location & (UInt32)CardLocation.Removed) > 0)
            {
                set_text(GameStringHelper.chuwai);
            }
            if (p.controller == 1 && (p.location & (UInt32)CardLocation.Deck) > 0)
            {
                set_text("<#ff8888>" + GameStringHelper.kazu + "</color>");
            }
            if (p.controller == 1 && (p.location & (UInt32)CardLocation.Extra) > 0)
            {
                set_text("<#ff8888>" + GameStringHelper.ewai + "</color>");
            }
            if (p.controller == 1 && (p.location & (UInt32)CardLocation.Grave) > 0)
            {
                set_text("<#ff8888>" + GameStringHelper.mudi + "</color>");
            }
            if (p.controller == 1 && (p.location & (UInt32)CardLocation.Removed) > 0)
            {
                set_text("<#ff8888>" + GameStringHelper.chuwai + "</color>");
            }
        }
    }

    void monster_cloude_handler()
    {
        if (Program.MonsterCloud)
        {
            if (game_object_monster_cloude == null)
            {
                game_object_monster_cloude = create(Program.I().mod_ocgcore_card_cloude, gameObject.transform.position);
                game_object_monster_cloude_ParticleSystem = game_object_monster_cloude.GetComponent<ParticleSystem>();
            }
        }
        else
        {
            if (game_object_monster_cloude != null)
            {
                destroy(game_object_monster_cloude);
                game_object_monster_cloude = null;
                game_object_monster_cloude_ParticleSystem = null;
            }
        }
        if (game_object_monster_cloude != null)
        {
            if (game_object_monster_cloude_ParticleSystem != null)
            {
                Vector3 screenposition = Program.camera_game_main.WorldToScreenPoint(gameObject.transform.position);
                game_object_monster_cloude.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z + 3));
                game_object_monster_cloude_ParticleSystem.startSize = UnityEngine.Random.Range(3f, 3f + (20f - 3f) * (float)(Mathf.Clamp(data.Attack,0,3000)) / 3000f);
                if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Earth))
                {
                    game_object_monster_cloude_ParticleSystem.startColor =
                        new Color(
                            200f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                            80f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                            0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f));
                }
                if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Water))
                {
                    game_object_monster_cloude_ParticleSystem.startColor =
                       new Color(
                           0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                           0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                           255f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f));
                }
                if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Fire))
                {
                    game_object_monster_cloude_ParticleSystem.startColor =
                      new Color(
                          255f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                          0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                          0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f));
                }
                if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Wind))
                {
                    game_object_monster_cloude_ParticleSystem.startColor =
                      new Color(
                          0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                          140f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                          0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f));
                }
                if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Dark))
                {
                    game_object_monster_cloude_ParticleSystem.startColor =
                       new Color(
                           158f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                           0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                           158f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f));
                }
                if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Light))
                {
                    game_object_monster_cloude_ParticleSystem.startColor =
                        new Color(
                            255f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                            140f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                            0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f));
                }
                if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Divine))
                {
                    game_object_monster_cloude_ParticleSystem.startColor =
                        new Color(
                            255f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                            140f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f),
                            0f / 255f + UnityEngine.Random.Range(-0.2f, 0.2f));
                }
            }
        }
    }
    int loaded_verticalDrawingCode = -1;
    bool loaded_verticalDrawingReal = false;
    float loaded_verticalDrawingK = 1;
   // bool picLikeASquare = false;
    int loaded_verticalDrawingNumber = -1;
    int loaded_verticalatk = -1;
    int loaded_verticaldef = -1;
    int loaded_verticalpos = -1;
    int loaded_verticalcon = -1;
    int loaded_verticalColor = -1;
    int loaded_verticalOverAttribute = -1;
    float k_verticle = 1;
    float VerticleTransparency = 1f;
    public bool opMonsterWithBackGroundCard=false;    
    void card_verticle_drawing_handler()
    {
        if (game_object_verticle_drawing == null || loaded_verticalDrawingCode != data.Id || loaded_verticalDrawingReal != Program.getVerticalTransparency() > 0.5f)
        {
            if (Program.getVerticalTransparency() > 0.5f)
            {
                Texture2D texture = GameTextureManager.get(data.Id, GameTextureType.card_verticle_drawing);
                if (texture != null)
                {
                    loaded_verticalDrawingCode = data.Id;
                    loaded_verticalDrawingK = GameTextureManager.getK(data.Id, GameTextureType.card_verticle_drawing);
                   // picLikeASquare = GameTextureManager.getB(data.Id, GameTextureType.card_verticle_drawing);
                    if (game_object_verticle_drawing == null)
                    {
                        game_object_verticle_drawing = create(Program.I().mod_simple_quad, gameObject.transform.position, new Vector3(60, 0, 0));
                        VerticleTransparency = 1f;
                    }
                    if (loaded_verticalDrawingReal != Program.getVerticalTransparency() > 0.5f)
                    {
                        loaded_verticalDrawingReal = Program.getVerticalTransparency() > 0.5f;
                        game_object_verticle_drawing.transform.localScale = Vector3.zero;
                    }
                    game_object_verticle_drawing.GetComponent<Renderer>().material.mainTexture = texture;
                    k_verticle = (float)texture.width / (float)texture.height;
                }
            }
            else
            {
                Texture2D texture = GameTextureManager.N;
                loaded_verticalDrawingCode = data.Id;
                loaded_verticalDrawingK = 1;
               // picLikeASquare = true;
                if (game_object_verticle_drawing == null)
                {
                    game_object_verticle_drawing = create(Program.I().mod_simple_quad, gameObject.transform.position, new Vector3(60, 0, 0));
                    VerticleTransparency = 1f;
                }
                if (loaded_verticalDrawingReal != Program.getVerticalTransparency() > 0.5f)
                {
                    loaded_verticalDrawingReal = Program.getVerticalTransparency() > 0.5f;
                    game_object_verticle_drawing.transform.localScale = Vector3.zero;
                }
                game_object_verticle_drawing.GetComponent<Renderer>().material.mainTexture = texture;
                k_verticle = (float)texture.width / (float)texture.height;
            }
        }
        else
        {
            float trans = 1f;
            //if (opMonsterWithBackGroundCard && loaded_verticalDrawingK < 0.9f && ability / loaded_verticalDrawingK > 2600f / 0.9f)  
            //{
            //    trans = 0.5f;
            //}
            //else
            //{
            //    trans = 1f;
            //}
            trans *= Program.getVerticalTransparency();
            if (trans < 0)
            {
                trans = 0;
            }
            if (trans > 1)
            {
                trans = 1;
            }
            if (trans!= VerticleTransparency)       
            {
                VerticleTransparency = trans;
                game_object_verticle_drawing.GetComponent<Renderer>().material.color = new Color(1, 1, 1, trans);
            }
            if (Program.getVerticalTransparency() <= 0.5f||opMonsterWithBackGroundCard) 
            {
                if (VerticleCollider != null)
                {
                    MonoBehaviour.DestroyImmediate(VerticleCollider);
                    VerticleCollider = null;
                }
            }
            else
            {
                if (VerticleCollider == null)
                {
                    VerticleCollider = game_object_verticle_drawing.AddComponent<BoxCollider>();
                }
            }

            Vector3 want_scale = Vector3.zero;
            float showscale = (isMinBlockMode ? 4.2f : Program.verticleScale) / loaded_verticalDrawingK;
            want_scale = new Vector3(showscale * k_verticle, showscale, 1);

            game_object_verticle_drawing.transform.position = get_verticle_drawing_vector(gameObject_face.transform.position);
            game_object_verticle_drawing.transform.localScale += (want_scale - game_object_verticle_drawing.transform.localScale) * Program.deltaTime * 10f;

            if (VerticleCollider != null)
            {
                float h = loaded_verticalDrawingK * 0.618f;
                VerticleCollider.size = new Vector3(4.3f / want_scale.x, h, 0.5f);
                VerticleCollider.center = new Vector3(0, -0.5f + 0.5f * h, 0);
            }


            int color = 0;

            if ((data.Type & (int)CardType.Tuner) > 0)
            {
                color = 1;
            }

            if ((data.Type & (int)CardType.Xyz) > 0)
            {
                color = 2;
            }
            if ((data.Type & (int)CardType.Link) > 0)
            {
                color = 3;
                data.Level = 0;
                for (int i = 0; i < 32; i++)
                {
                    if ((data.LinkMarker & 1 << i) > 0)
                    {
                        data.Level++;
                    }
                }
            }

            if (verticle_number == null || loaded_verticalDrawingNumber != (int)data.Level || loaded_verticalColor != color)
            {
                loaded_verticalDrawingNumber = (int)data.Level;
                loaded_verticalColor = color;
                if (verticle_number == null)
                {
                    verticle_number = create(Program.I().new_ui_textMesh, Vector3.zero, new Vector3(60, 0, 0), true, null, true, new Vector3(3 * 1.8f * 0.04f, 3 * 1.8f * 0.04f, 3 * 1.8f * 0.04f)).GetComponent<TMPro.TextMeshPro>();
                }
                if (game_object_verticle_Star == null)
                {
                    game_object_verticle_Star = create(Program.I().mod_simple_quad, Vector3.zero, new Vector3(60, 0, 0), true, null, true, new Vector3(3 * 1.8f * 0.17f, 3 * 1.8f * 0.17f, 3 * 1.8f * 0.17f));
                }
                if (color == 0)
                {
                    verticle_number.text = data.Level.ToString();
                    game_object_verticle_Star.GetComponent<Renderer>().material.mainTexture = GameTextureManager.L;
                }
                if (color == 1)
                {
                    verticle_number.text = "<#FFFF00>" + data.Level.ToString() + "</color>";
                    game_object_verticle_Star.GetComponent<Renderer>().material.mainTexture = GameTextureManager.L;
                }
                if (color == 2)
                {
                    verticle_number.text = "<#999999>" + data.Level.ToString() + "</color>";
                    game_object_verticle_Star.GetComponent<Renderer>().material.mainTexture = GameTextureManager.R;
                }
                if (color == 3)
                {
                    verticle_number.text = "<#E1FFFF>" + data.Level.ToString() + "</color>";
                    game_object_verticle_Star.GetComponent<Renderer>().material.mainTexture = GameTextureManager.LINK;
                }
            }

            if (Program.getVerticalTransparency() < 0.5f)
            {
                Vector3 screen_number_pos;
                screen_number_pos = 2 * gameObject_face.transform.position - cardHint.gameObject.transform.position;
                screen_number_pos = Program.camera_game_main.WorldToScreenPoint(screen_number_pos + new Vector3(-0.25f, 0, -0.7f));
                screen_number_pos.z -= 2f;
                verticle_number.transform.position = Program.camera_game_main.ScreenToWorldPoint(screen_number_pos);
                if (game_object_verticle_Star != null)
                {
                    screen_number_pos = 2 * gameObject_face.transform.position - cardHint.gameObject.transform.position;
                    screen_number_pos = Program.camera_game_main.WorldToScreenPoint(screen_number_pos + new Vector3(-1.5f, 0, -0.7f));
                    screen_number_pos.z -= 2f;
                    game_object_verticle_Star.transform.position = Program.camera_game_main.ScreenToWorldPoint(screen_number_pos);
                }
            }
            else
            {
                Vector3 screen_number_pos;
                screen_number_pos = Program.camera_game_main.WorldToScreenPoint(cardHint.gameObject.transform.position + new Vector3(-0.61f, 0.65f, 0.65f * 1.732f));
                screen_number_pos.z -= 2f;
                verticle_number.transform.position = Program.camera_game_main.ScreenToWorldPoint(screen_number_pos);
                if (game_object_verticle_Star != null)
                {
                    screen_number_pos = Program.camera_game_main.WorldToScreenPoint(cardHint.gameObject.transform.position + new Vector3(-1.86f, 0.65f, 0.65f * 1.732f));
                    screen_number_pos.z -= 2f;
                    game_object_verticle_Star.transform.position = Program.camera_game_main.ScreenToWorldPoint(screen_number_pos);
                }
            }

            if (loaded_verticalatk != data.Attack || loaded_verticaldef != data.Defense  || loaded_verticalpos!=p.position|| loaded_verticalcon!=p.controller)
            {
                loaded_verticalatk = data.Attack;
                loaded_verticaldef = data.Defense;
                loaded_verticalpos = p.position;
                loaded_verticalcon = (int)p.controller;
                if ((data.Type&(uint)CardType.Link)>0)   
                {
                    string raw = "";
                    YGOSharp.Card data_raw = YGOSharp.CardsManager.Get(data.Id);
                    if (data.Attack > data_raw.Attack)
                    {
                        raw += "<#7fff00>" + data.Attack.ToString() + "</color>";
                    }
                    if (data.Attack < data_raw.Attack)
                    {
                        raw += "<#dda0dd>" + data.Attack.ToString() + "</color>";
                    }
                    if (data.Attack == data_raw.Attack)
                    {
                        raw += data.Attack.ToString();
                    }
                    if (p.sequence==5||p.sequence==6)
                    {
                        raw += "(" + (p.controller == 0 ? GameStringHelper._wofang : GameStringHelper._duifang) + ")";
                    }
                    set_text(raw.Replace("-2", "?"));
                }
                else
                {
                    string raw = "";
                    YGOSharp.Card data_raw = YGOSharp.CardsManager.Get(data.Id);
                    if ((loaded_verticalpos & (int)CardPosition.Attack) > 0)
                    {
                        if (data.Attack > data_raw.Attack)
                        {
                            raw += "<#7fff00>" + data.Attack.ToString() + "</color>";
                        }
                        if (data.Attack < data_raw.Attack)
                        {
                            raw += "<#dda0dd>" + data.Attack.ToString() + "</color>";
                        }
                        if (data.Attack == data_raw.Attack)
                        {
                            raw += data.Attack.ToString();
                        }
                        raw += "/";
                        raw += "<#888888>" + data.Defense.ToString() + "</color>";
                        if (p.sequence == 5 || p.sequence == 6)
                        {
                            raw += "(" + (p.controller == 0 ? GameStringHelper._wofang : GameStringHelper._duifang) + ")";
                        }
                        set_text(raw.Replace("-2", "?"));
                    }
                    else
                    {
                        raw += "<#888888>" + data.Attack.ToString() + "</color>";
                        raw += "/";
                        if (data.Defense > data_raw.Defense)
                        {
                            raw += "<#7fff00>" + data.Defense.ToString() + "</color>";
                        }
                        if (data.Defense < data_raw.Defense)
                        {
                            raw += "<#dda0dd>" + data.Defense.ToString() + "</color>";
                        }
                        if (data.Defense == data_raw.Defense)
                        {
                            raw += data.Defense.ToString();
                        }
                        if (p.sequence == 5 || p.sequence == 6)
                        {
                            raw += "(" + (p.controller == 0 ? GameStringHelper._wofang : GameStringHelper._duifang) + ")";
                        }
                        set_text(raw.Replace("-2", "?"));
                    }
                }
               

            }
        }
    }

    //private float caculateBoxWidth()
    //{
    //    float colliderWidth = 1f;
    //    float showscale = 2f + (float)(ability - 1000) / 1000f;
    //    if (showscale > 4) showscale = 4;
    //    if (showscale < 2) showscale = 2;
    //    showscale *= 1.8f / loaded_verticalDrawingK;
    //    showscale *= k_verticle;
    //    colliderWidth = 4.3f / showscale;
    //    return colliderWidth;
    //}

    //public void caculateAbility()
    //{
    //    if (condition== gameCardCondition.verticle_clickable)
    //    {
    //        if ((p.position & (UInt32)CardPosition.Attack) > 0)
    //        {
    //            ability = data.Attack;
    //        }
    //        else
    //        {
    //            ability = data.Defense;
    //        }
    //    }
    //    else
    //    {
    //        ability = data.Attack;
    //    }
    //    if (ability > 3000)
    //    {
    //        ability = 3000;
    //    }
    //    if (ability < 0)
    //    {
    //        ability = 0;
    //    }
    //}

    #endregion

    #region data

    public void set_data(YGOSharp.Card d)
    {
        data = d;
        //caculateAbility();
        if (Program.I().cardDescription.ifShowingThisCard(data))
        {
            showMeLeft();
        }
    }

    public void set_code(int code)
    {
        if (code>0)
        {
            if (data.Id != code)
            {
                set_data(YGOSharp.CardsManager.Get(code));
                data.Id = code;
                if (p.controller == 1)
                {
                    if (Program.I().ocgcore.condition== Ocgcore.Condition.duel) 
                    {
                        if (!Program.I().ocgcore.sideReference.ContainsKey(code))   
                        {
                            Program.I().ocgcore.sideReference.Add(code, code);
                        }
                    }
                }
            }
        }
    }

    public void refreshData()
    {
        YGOSharp.CardsManager.Get(data.Id).cloneTo(data);
        set_data(data);
        clear_all_tail();
    }

    public void erase_data()    
    {
        set_data(YGOSharp.CardsManager.Get(0));
        disabled = false;
        clear_all_tail();
    }

    public YGOSharp.Card get_data()
    {
        return data;
    }

    int loaded_cardPictureCode = -1;
    int loaded_cardCode = -1;   
    int loaded_back = -1;
    int loaded_specialHint = -1;
    bool cardCodeChangedButNowLoadedPic = false;

    void card_picture_handler()
    {
        if (loaded_cardCode != data.Id)
        {
            loaded_cardCode = data.Id;
            cardCodeChangedButNowLoadedPic = true;
        }
        if (loaded_cardPictureCode != data.Id)
        {
            Texture2D texture = GameTextureManager.get(data.Id, GameTextureType.card_picture, p.controller == 0 ? GameTextureManager.myBack : GameTextureManager.opBack);
            if (texture != null)
            {
                loaded_cardPictureCode = data.Id;
                gameObject_face.GetComponent<Renderer>().material.mainTexture = texture;
            }
            else
            {
                if (cardCodeChangedButNowLoadedPic) 
                {
                    gameObject_face.GetComponent<Renderer>().material.mainTexture = GameTextureManager.unknown;
                    cardCodeChangedButNowLoadedPic = false;
                }
            }
        }
        if (p.controller != loaded_back)
        {
            try
            {
                loaded_back = (int)p.controller;
                UIHelper.getByName(gameObject, "back").GetComponent<Renderer>().material.mainTexture = loaded_back == 0 ? GameTextureManager.myBack : GameTextureManager.opBack;
                if (data.Id == 0)
                {
                    UIHelper.getByName(gameObject, "face").GetComponent<Renderer>().material.mainTexture = loaded_back == 0 ? GameTextureManager.myBack : GameTextureManager.opBack;
                }
                del_one_tail(GameStringHelper.opHint);
                if (loaded_back != controllerBased)
                {
                    add_string_tail(GameStringHelper.opHint);
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
        }
        int special_hint = 0;
        if ((p.position & (int)CardPosition.FaceDown) > 0)
        {
            if ((p.location & (int)CardLocation.Removed) > 0)
            {
                special_hint = 1;
            }
        }
        if ((p.position & (int)CardPosition.FaceUp) > 0)
        {
            if ((p.location & (int)CardLocation.Extra) > 0)
            {
                special_hint = 2;
            }
        }
        if (loaded_specialHint!= special_hint)
        {
            loaded_specialHint = special_hint;
            if (loaded_specialHint==0)  
            {
                del_one_tail(GameStringHelper.licechuwai);
                del_one_tail(GameStringHelper.biaoceewai);
            }
            if (loaded_specialHint == 1)
            {
                add_string_tail(GameStringHelper.licechuwai);
            }
            if (loaded_specialHint == 2)
            {
                add_string_tail(GameStringHelper.biaoceewai);
            }
        }
    }

    public MultiStringMaster tails = new MultiStringMaster();

    public void add_string_tail(string str)
    {
        tails.Add(str);
        if (Program.I().cardDescription.ifShowingThisCard(data))    
        {
            showMeLeft();
        }
    }

    public void clear_all_tail()
    {
        tails.clear();
        if (Program.I().cardDescription.ifShowingThisCard(data))
        {
            showMeLeft();
        }
    }

    public void del_one_tail(string str)
    {
        tails.remove(str);
        if (Program.I().cardDescription.ifShowingThisCard(data))
        {
            showMeLeft();
        }
    }

    #endregion

    #region tools


    public bool isHided()
    {
        if ((p.location & (int)CardLocation.Deck) > 0)
        {
            return true;
        }
        if ((p.location & (int)CardLocation.Extra) > 0)
        {
            return true;
        }
        if ((p.location & (int)CardLocation.Removed) > 0)
        {
            return true;
        }
        if ((p.location & (int)CardLocation.Grave) > 0)
        {
            return true;
        }
        return false;
    }

    public void set_text(string s)
    {
        cardHint.gameObject.SetActive(s != "");
        cardHint.text = s;
    }

    private int get_color_num_int()
    {
        int re = 0;
        //
        if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Earth))
        {
            re = 0;
        }
        if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Water))
        {
            re = 3;
        }
        if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Fire))
        {
            re = 5;
        }
        if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Wind))
        {
            re = 2;
        }
        if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Dark))
        {
            re = 4;
        }
        if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Light))
        {
            re = 1;
        }
        if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Divine))
        {
            re = 1;
        }
        //
        return re;
    }

    Vector3 get_verticle_drawing_vector(Vector3 facevector)
    {
        Vector3 want_position = Vector3.zero;
        if (isMinBlockMode) 
        {
            want_position = facevector;
            want_position.y += (4.2f / loaded_verticalDrawingK) / 2f * 0.5f;
            want_position.z += ((4.2f / loaded_verticalDrawingK) / 2f * 1.732f * 0.5f)-1.85f;
        }
        else
        {
            float showscale = Program.verticleScale;
            want_position = facevector;
            want_position.y += (showscale / loaded_verticalDrawingK) / 2f * 0.5f;
            want_position.z += ((showscale / loaded_verticalDrawingK) / 2f * 1.732f * 0.5f) - (showscale * 1.3f / 3.6f - 0.8f);
        }
        return want_position;
    }


    #endregion

    #region publicTools

    public void show_number(int number,bool add=false)  
    {
        if (add)
        {
            show_number(number_showing * 10 + number);
            return;
        }
        if (number == 0)
        {
            if (obj_number != null)
            {
                iTween.ScaleTo(obj_number, Vector3.zero, 0.3f);
                destroy(obj_number, 0.6f);
            }
        }
        else
        {
            if (obj_number == null)
            {
                obj_number = create(Program.I().mod_ocgcore_card_number_shower);
                obj_number.transform.GetComponent<TMPro.TextMeshPro>().text = number.ToString();
                obj_number.transform.localScale = Vector3.zero;
                iTween.ScaleTo(obj_number, new Vector3(1, 1, 1), 0.3f);
                iTween.RotateTo(obj_number, new Vector3(60, 0, 0), 0.3f);
            }
            else if (number_showing != number)
            {
                iTween.ScaleTo(obj_number, Vector3.zero, 0.6f);
                destroy(obj_number, 0.6f);
                obj_number = create(Program.I().mod_ocgcore_card_number_shower);
                obj_number.transform.GetComponent<TMPro.TextMeshPro>().text = number.ToString();
                obj_number.transform.localScale = Vector3.zero;
                iTween.ScaleTo(obj_number, new Vector3(1, 1, 1), 0.3f);
                iTween.RotateTo(obj_number, new Vector3(60, 0, 0), 0.3f);
            }
        }
        number_showing = number;
    }

    #endregion

    #region button

    List<gameButton> buttons = new List<gameButton>();

    public void add_one_button(gameButton b)    
    {
        b.cookieCard = this;
        buttons.Add(b);
    }

    public bool query_hint_button(string hint)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].hint == hint)
            {
                return true;
            }
        }
        return false;
    }

    public void remove_all_cookie_button()
    {   
        List<gameButton> buttons_to_remove = new List<gameButton>();
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].notCookie == false)
            {
                buttons[i].hide();
                buttons_to_remove.Add(buttons[i]);
            }
        }
        for (int i = 0; i < buttons_to_remove.Count; i++)
        {
            buttons.Remove(buttons_to_remove[i]);
        }
        buttons_to_remove.Clear();
    }

    public void remove_all_unCookie_button()    
    {
        List<gameButton> buttons_to_remove = new List<gameButton>();
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].notCookie == true)
            {
                buttons[i].hide();
                buttons_to_remove.Add(buttons[i]);
            }
        }
        for (int i = 0; i < buttons_to_remove.Count; i++)
        {
            buttons.Remove(buttons_to_remove[i]);
        }
        buttons_to_remove.Clear();
    }

    #endregion

    #region decoration

    public class cardDecoration
    {
        public GameObject game_object;
        public float relative_position;
        public Vector3 rotation;
        public string desctiption;
        public bool scale_change_ignored = false;
        public bool cookie = true;
        public bool up_of_card = false;
    }

    List<cardDecoration> cardDecorations = new List<cardDecoration>();

    public cardDecoration add_one_decoration(GameObject mod, float relative_position, Vector3 rotation, string desctiption, bool cookie = true, bool up = false)
    {
        cardDecoration c = new cardDecoration();
        c.desctiption = desctiption;
        c.up_of_card = up;
        c.cookie = cookie;
        c.relative_position = relative_position;
        c.rotation = rotation;
        c.game_object = create(mod, gameObject_face.transform.position);
        c.game_object.transform.eulerAngles = rotation;
        c.game_object.transform.localScale = Vector3.zero;
        cardDecorations.Add(c);
        return c;
    }

    public void fast_decoration(GameObject mod)
    {
        destroy(add_one_decoration(mod, -0.5f, Vector3.zero, "",false).game_object, 5);
    }

    public void animationEffect(GameObject mod)
    {
        MonoBehaviour.Destroy((GameObject)MonoBehaviour.Instantiate(mod, UA_get_accurate_position(), Quaternion.identity), 5f);
    }

    public void positionEffect(GameObject mod)
    {
        MonoBehaviour.Destroy((GameObject)MonoBehaviour.Instantiate(mod, Program.I().ocgcore.get_point_worldposition(p), Quaternion.identity), 5f);
    }

    public void positionShot(GameObject mod)
    {
        MonoBehaviour.Destroy((GameObject)MonoBehaviour.Instantiate(mod, Program.I().ocgcore.get_point_worldposition(p), Quaternion.identity), 1f);
    }

    public void del_all_decoration_by_string(string desctiption)
    {
        List<cardDecoration> to_remove = new List<cardDecoration>();
        for (int i = 0; i < cardDecorations.Count; i++)
        {
            if (cardDecorations[i].desctiption == desctiption)
            {
                to_remove.Add(cardDecorations[i]);
                destroy(cardDecorations[i].game_object);
            }
        }
        for (int i = 0; i < to_remove.Count; i++)
        {
            cardDecorations.Remove(to_remove[i]);
        }
    }

    public void del_all_decoration()
    {
        List<cardDecoration> to_remove = new List<cardDecoration>();
        for (int i = 0; i < cardDecorations.Count; i++)
        {
            if (cardDecorations[i].game_object != null && cardDecorations[i].cookie)
            {
                to_remove.Add(cardDecorations[i]);
                destroy(cardDecorations[i].game_object);
            }
        }
        for (int i = 0; i < to_remove.Count; i++)
        {
            cardDecorations.Remove(to_remove[i]);
        }
    }

    #endregion

    #region overlay

    List<GameObject> overlay_lights = new List<GameObject>();

    public void add_one_overlay_light()
    {
        GameObject mod = Program.I().mod_ocgcore_ol_light;
        if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Earth))
        {
            mod = Program.I().mod_ocgcore_ol_earth;
        }
        if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Water))
        {
            mod = Program.I().mod_ocgcore_ol_water;
        }
        if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Fire))
        {
            mod = Program.I().mod_ocgcore_ol_fire;
        }
        if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Wind))
        {
            mod = Program.I().mod_ocgcore_ol_wind;
        }
        if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Dark))
        {
            mod = Program.I().mod_ocgcore_ol_dark;
        }
        if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Light))
        {
            mod = Program.I().mod_ocgcore_ol_light;
        }
        GameObject obj = create(mod, gameObject_face.transform.position);
        overlay_lights.Add(obj);
    }

    public void del_one_overlay_light()
    {
        if (overlay_lights.Count > 0)
        {
            destroy(overlay_lights[0]);
            overlay_lights.RemoveAt(0);
        }
    }


 
    public void set_overlay_light(int number)
    {
        if (number != 0)
        {
            if (loaded_verticalOverAttribute != data.Attribute)
            {
                loaded_verticalOverAttribute = data.Attribute;
                while (overlay_lights.Count > 0)
                {
                    del_one_overlay_light();
                }
            }
        }
        while (overlay_lights.Count != number)
        {
            if (number > overlay_lights.Count)
            {
                add_one_overlay_light();
            }
            if (number < overlay_lights.Count)
            {
                del_one_overlay_light();
            }
        }
    }

    public void set_overlay_see_button(bool on)
    {
        gameButton re = null;
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].cookieString == "see_overlay")
            {
                re = buttons[i];
            }
        }
        if (on)
        {
            if (re == null)
            {
                gameButton button = new gameButton(0, InterString.Get("查看素材"), superButtonType.see);
                button.cookieString = "see_overlay";
                button.notCookie = true;
                button.cookieCard = this;
                add_one_button(button);
            }
        }
        else
        {
            if (re != null)
            {
                remove_all_unCookie_button();
            }
        }
    }

    #endregion

    #region lines

    FlashingController[] insFlash(string color)
    {
        FlashingController[] ret = new FlashingController[2];
        ret[0] = insFlashONE(color);
        ret[1] = insFlashONE(color);
        ret[1].transform.localEulerAngles = new Vector3(180, 0, 0);
        return ret;
    }

    GameObject insKuang(GameObject mod)
    {
        GameObject ret = null;
        ret = Program.I().create(mod);
        ret.transform.SetParent(gameObject_face.transform, false);
        ret.transform.localScale = new Vector3(0.1f/3f,0.1f, 0.1f / 4f);
        ret.transform.localEulerAngles = new Vector3(90,0,0);
        return ret;
    }

    FlashingController insFlashONE(string color)
    {
        FlashingController flash = null;
        Program.camera_game_main.GetComponent<HighlightingEffect>().enabled = true;
        flash = Program.I().create(Program.I().mod_ocgcore_card_figure_line).GetComponent<FlashingController>();
        flash.transform.SetParent(gameObject_face.transform, false);
        flash.transform.localPosition = Vector3.zero;
        Color tcl = Color.yellow;
        ColorUtility.TryParseHtmlString(color, out tcl);
        flash.flashingStartColor = tcl;
        ColorUtility.TryParseHtmlString("000000", out tcl);
        flash.flashingEndColor = tcl;
        return flash;
    }

    public void flash_line_on()
    {
        Program.camera_game_main.GetComponent<HighlightingEffect>().enabled = true;
        if (MouseFlash==null)   
        {
            MouseFlash = create(Program.I().mod_ocgcore_card_figure_line).GetComponent<FlashingController>();
            MouseFlash.transform.SetParent(gameObject_face.transform, false);
            MouseFlash.transform.localPosition = Vector3.zero;
            Color tcl = Color.yellow;
            ColorUtility.TryParseHtmlString("ff8000", out tcl);
            MouseFlash.flashingStartColor = tcl;
            ColorUtility.TryParseHtmlString("ffffff", out tcl);
            MouseFlash.flashingEndColor = tcl;
        }
        MouseFlash.gameObject.SetActive(true);
    }

    public void flash_line_off()
    {
        if (MouseFlash != null)
        {
            MouseFlash.gameObject.SetActive(false);
        }
    }

    GameObject p_line = null;

    public void p_line_on()
    {
        Program.camera_game_main.GetComponent<HighlightingEffect>().enabled = true;
        if (p_line != null) destroy(p_line);
        p_line = create(Program.I().mod_ocgcore_card_figure_line);
        p_line.transform.SetParent(gameObject_face.transform, false);
        p_line.transform.localPosition = Vector3.zero;
        p_line.GetComponent<FlashingController>().flashingStartColor = Color.blue;
        p_line.GetComponent<FlashingController>().flashingEndColor = Color.gray;
        p_line.GetComponent<FlashingController>().flashingFrequency = 0.5f;
    }

    public void p_line_off()
    {
        if (p_line != null)
        {
            destroy(p_line);
            p_line = null;
        }
    }

    #endregion

    #region animation

    public void animation_confirm(Vector3 position, Vector3 rotation, float time_move, float time_still)
    {
        ES_lock(time_move + time_move + time_still);
        confirm_step_time_still = time_still;
        confirm_step_time_move = time_move;
        confirm_step_r = rotation;
        iTween[] iTweens = gameObject.GetComponents<iTween>();
        for (int i = 0; i < iTweens.Length; i++) MonoBehaviour.Destroy(iTweens[i]);
        iTween.MoveTo(gameObject, iTween.Hash(
                            "x", position.x,
                            "y", position.y,
                            "z", position.z,
                            "onupdate", (Action)RefreshFunction_decoration,
                            "oncomplete", (Action)confirm_step_2,
                            "time", confirm_step_time_move
                            ));
        iTween.RotateTo(gameObject, iTween.Hash(
                            "x", confirm_step_r.x,
                            "y", confirm_step_r.y,
                            "z", confirm_step_r.z,
                            "time", confirm_step_time_move
                            ));
    }

    public void animation_confirm_screenCenter(Vector3 rotation, float time_move, float time_still)   
    {
        ES_lock(time_move + time_move + time_still);
        confirm_step_time_still = time_still;
        confirm_step_time_move = time_move;
        confirm_step_r = rotation;
        iTween[] iTweens = gameObject.GetComponents<iTween>();
        for (int i = 0; i < iTweens.Length; i++) MonoBehaviour.DestroyImmediate(iTweens[i]);
        iTween.RotateTo(gameObject, iTween.Hash(
                            "x", confirm_step_r.x,
                            "y", confirm_step_r.y,
                            "z", confirm_step_r.z,
                             "easetype", iTween.EaseType.spring,
                            "onupdate", (Action)RefreshFunction_decoration, 
                            "oncomplete", (Action)confirm_step_2,
                            "time", confirm_step_time_move
                            ));
        var ttt = gameObject.AddComponent<screenFader>();
        ttt.from = gameObject.transform.position;
        ttt.time = time_move;
        ttt.deltaTimeCloseUp = 0;
        MonoBehaviour.Destroy(ttt, time_move + time_still);
    }

    Vector3 confirm_step_r = Vector3.zero;

    float confirm_step_time_still = 0;

    float confirm_step_time_move = 0;

    void confirm_step_2()
    {
        iTween.RotateTo(gameObject, iTween.Hash(
                            "x", confirm_step_r.x,
                            "y", confirm_step_r.y,
                            "z", confirm_step_r.z,
                            "onupdate", (Action)RefreshFunction_decoration,
                            "oncomplete", (Action)confirm_step_3,
                            "time", confirm_step_time_still
                            ));
    }

    void confirm_step_3()
    {
        iTween.RotateTo(gameObject, iTween.Hash(
                            "x", accurate_rotation.x,
                            "y", accurate_rotation.y,
                            "z", accurate_rotation.z,
                            "time", confirm_step_time_move
                            ));
        iTween.MoveTo(gameObject, iTween.Hash(
                            "x", accurate_position.x,
                            "y", accurate_position.y,
                            "z", accurate_position.z,
                            "onupdate", (Action)RefreshFunction_decoration,
                            "time", confirm_step_time_move,
                             "easetype", iTween.EaseType.easeInQuad
                            ));
    }

    public void animation_shake_to(float time)
    {
        ES_lock(time);
        gameObject.transform.position = accurate_position;
        gameObject.transform.eulerAngles = accurate_rotation;
        iTween[] iTweens = gameObject.GetComponents<iTween>();
        for (int i = 0; i < iTweens.Length; i++) MonoBehaviour.Destroy(iTweens[i]);
        iTween.ShakePosition(gameObject, iTween.Hash(
                            "x", 1,
                            "y", 1,
                            "z", 1,
                            "time", time,
                            "oncomplete", (Action)ES_safe_card_move_to_original_place
                            ));
    }


    public void animation_rush_to(Vector3 position, Vector3 rotation)
    {
        ES_lock(0.4f);
        iTween[] iTweens = gameObject.GetComponents<iTween>();
        for (int i = 0; i < iTweens.Length; i++)
            MonoBehaviour.Destroy(iTweens[i]);
        MonoBehaviour.DestroyImmediate(gameObject.GetComponent<screenFader>());
        iTween.MoveTo(gameObject, iTween.Hash(
                            "x", position.x,
                            "y", position.y,
                            "z", position.z,
                            "time", 0.2f
                            ));
        iTween.RotateTo(gameObject, iTween.Hash(
                            "x", rotation.x,
                            "y", rotation.y,
                            "z", rotation.z,
                             "onupdate", (Action)RefreshFunction_decoration,
                            "oncomplete", (Action)ES_safe_card_move_to_original_place,
                            "time", 0.21f
                            ));
    }

    public void animation_show_off(bool summon, bool disabled = false)   
    {
        if (Ocgcore.inSkiping) 
        {
            return;
        }

        show_off_disabled = disabled;
        show_off_begin_time = Program.TimePassed();
        show_off_shokewave = summon;

        if (show_off_disabled)
        {
            refreshFunctions.Add(SOH_dis);
            Program.I().ocgcore.Sleep(42);
        }
        else if (show_off_shokewave)
        {
            if (Program.I().setting.setting.showoff.value == false || File.Exists("picture/closeup/" + data.Id.ToString() + ".png") == false || (data.Attack < Program.I().setting.atk && data.Level < Program.I().setting.star))
            {
                refreshFunctions.Add(SOH_nSum);
                Program.I().ocgcore.Sleep(30);
            }
            else
            {
                refreshFunctions.Add(SOH_sum);
                Program.I().ocgcore.Sleep(72);
            }
        }
        else
        {
            if (Program.I().setting.setting.showoffWhenActived.value == false || File.Exists("picture/closeup/" + data.Id.ToString() + ".png") == false)
            {
                refreshFunctions.Add(SOH_nAct);
                Program.I().ocgcore.Sleep(42);
            }
            else
            {
                refreshFunctions.Add(SOH_act);
                Program.I().ocgcore.Sleep(42);
            }
        }
    }

    bool show_off_shokewave = false;

    bool show_off_disabled = false;

    int show_off_begin_time = 0;

    private void SOH_act()
    {
        Texture2D tex = GameTextureManager.get(data.Id, GameTextureType.card_picture);
        Texture2D texc = GameTextureManager.get(data.Id, GameTextureType.card_feature);
        if (tex != null)
        {
            if (texc != null)
            {
                float k = GameTextureManager.getK(data.Id, GameTextureType.card_feature);
                refreshFunctions.Remove(SOH_act);
                YGO1superShower shower = create(Program.I().Pro1_superCardShowerA, Program.I().ocgcore.centre(true), Vector3.zero, false, Program.ui_main_2d, true).GetComponent<YGO1superShower>();
                shower.card.mainTexture = tex;
                shower.closeup.mainTexture = texc;
                shower.closeup.height = (int)(500f / k);
                shower.closeup.width = (int)((500f / k) * ((float)texc.width) / ((float)texc.height));
                Ocgcore.LRCgo = shower.gameObject;
                destroy(shower.gameObject, 0.7f, false, true);
            }
        }
    }

    private void SOH_nAct()
    {
        Texture2D tex = GameTextureManager.get(data.Id, GameTextureType.card_picture);
        if (tex != null)
        {
            refreshFunctions.Remove(SOH_nAct);
            pro1CardShower shower = create(Program.I().Pro1_CardShower, Program.I().ocgcore.centre(), Vector3.zero, false, Program.ui_main_2d, true).GetComponent<pro1CardShower>();
            shower.card.mainTexture = tex;
            shower.mask.mainTexture = GameTextureManager.Mask;
            shower.disable.mainTexture = GameTextureManager.negated;
            shower.transform.localScale = Vector3.zero;
            shower.gameObject.transform.localScale = new Vector3(Screen.height / 650f, Screen.height / 650f, Screen.height / 650f);
            shower.run();
            Ocgcore.LRCgo = shower.gameObject;
            destroy(shower.gameObject, 0.7f, false, true);
        }
    }

    private void SOH_sum()
    {
        Texture2D tex = GameTextureManager.get(data.Id, GameTextureType.card_picture);
        Texture2D texc = GameTextureManager.get(data.Id, GameTextureType.card_feature);
        if (tex != null)
        {
            if (texc != null)
            {
                float k = GameTextureManager.getK(data.Id, GameTextureType.card_feature);
                refreshFunctions.Remove(SOH_sum);
                YGO1superShower shower = create(Program.I().Pro1_superCardShower, Program.I().ocgcore.centre(true), Vector3.zero, false, Program.ui_main_2d, true).GetComponent<YGO1superShower>();
                shower.card.mainTexture = tex;
                shower.closeup.mainTexture = texc;
                shower.closeup.height = (int)(500f / k);
                shower.closeup.width = (int)((500f / k) * ((float)texc.width) / ((float)texc.height));
                Ocgcore.LRCgo = shower.gameObject;
                destroy(shower.gameObject, 2f, false, true);
            }
        }
    }

    private void SOH_nSum()
    {
        Texture2D tex = GameTextureManager.get(data.Id, GameTextureType.card_picture);
        if (tex != null)
        {
            refreshFunctions.Remove(SOH_nSum);
            pro1CardShower shower = create(Program.I().Pro1_CardShower, Program.I().ocgcore.centre(), Vector3.zero, false, Program.ui_main_2d, true).GetComponent<pro1CardShower>();
            shower.card.mainTexture = tex;
            shower.mask.mainTexture = GameTextureManager.Mask;
            shower.disable.mainTexture = GameTextureManager.negated;
            shower.transform.localScale = Vector3.zero;
            iTween.ScaleTo(shower.gameObject, iTween.Hash(
               "scale",
               new Vector3(Screen.height / 650f, Screen.height / 650f, Screen.height / 650f),
               "time",
               0.5f
               ));
            Ocgcore.LRCgo = shower.gameObject;
            destroy(shower.gameObject, 0.5f, false, true);
        }
    }

    private void SOH_dis()  
    {
        Texture2D tex = GameTextureManager.get(data.Id, GameTextureType.card_picture);
        if (tex != null)
        {
            refreshFunctions.Remove(SOH_dis);
            pro1CardShower shower = create(Program.I().Pro1_CardShower, Program.I().ocgcore.centre(), Vector3.zero, false, Program.ui_main_2d, true).GetComponent<pro1CardShower>();
            shower.card.mainTexture = tex;
            shower.mask.mainTexture = GameTextureManager.Mask;
            shower.disable.mainTexture = GameTextureManager.negated;
            shower.transform.localScale = Vector3.zero;
            shower.gameObject.transform.localScale = new Vector3(Screen.height / 650f, Screen.height / 650f, Screen.height / 650f);
            shower.Dis();
            Ocgcore.LRCgo = shower.gameObject;
            destroy(shower.gameObject, 0.7f, false, true);
        }
    }

    public void sortButtons()
    {
        buttons.Sort((left, right) =>
        {
            return getButtonGravity(right) - getButtonGravity(left);
        });
    }

    int getButtonGravity(gameButton left)
    {
        gameButton button = left;
        int gravity = 0;
        switch (button.type)
        {
            case superButtonType.act:
                gravity = 1;
                break;
            case superButtonType.attack:
                gravity = 7;
                break;
            case superButtonType.change:
                gravity = 6;
                break;
            case superButtonType.see:
                gravity = 5;
                break;
            case superButtonType.set:
                gravity = 4;
                break;
            case superButtonType.spsummon:
                gravity = 2;
                break;
            case superButtonType.summon:
                gravity = 3;
                break;
        }

        return gravity;
    }

    #endregion

    #region cs

    public void ChainUNlock()
    {
        for (int i = 0; i < chains.Count; i++)  
        {
            if (chains[i].G != null)
            {
                Program.I().ocgcore.allChainPanelFixedContainer.Remove(chains[i].G.gameObject);
                chains[i].G.transform.SetParent(Program.I().transform,true);
            }
        }
    }

    void handlerChain()
    {
        for (int i = 0; i < chains.Count; i++)  
        {
            if (chains[i].G == null)
            {
                chains[i].G = create(Program.I().new_ocgcore_chainCircle).GetComponent<chainMono>();
                Program.I().ocgcore.allChainPanelFixedContainer.Add(chains[i].G.gameObject);
                chains[i].G.text.text = chains[i].i.ToString();
                chains[i].G.text.color = GameTextureManager.chainColor;
                chains[i].G.text.enableVertexGradient = false;
                chains[i].G.circle.material.mainTexture = GameTextureManager.Chain;
                chains[i].G.gameObject.transform.localScale = Vector3.zero;
                chains[i].G.flashing = false;
            }
            chainMono decorationChain = chains[i].G;
            if (game_object_verticle_drawing != null && Program.getVerticalTransparency() > 0.5f)
            {
                if (decorationChain.transform.parent != Program.I().transform)
                {
                    if (decorationChain.transform.parent != game_object_verticle_drawing.transform)
                    {
                        decorationChain.transform.SetParent(game_object_verticle_drawing.transform);
                        decorationChain.transform.localRotation = Quaternion.identity;
                        decorationChain.transform.localScale = Vector3.zero;
                        decorationChain.transform.localPosition = Vector3.zero;
                    }
                    try
                    {
                        Vector3 devide = game_object_verticle_drawing.transform.localScale;
                        if (Vector3.Distance(Vector3.zero, devide) > 0.01f)
                        {
                            decorationChain.transform.localScale = (new Vector3(5f / devide.x, 5f / devide.y, 5f / devide.z));
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    decorationChain.transform.localScale = (new Vector3(5, 5, 5));
                }
            }
            else
            {
                if (decorationChain.transform.parent != Program.I().transform)
                {
                    if (decorationChain.transform.parent != gameObject_face.transform)
                    {
                        decorationChain.transform.SetParent(gameObject_face.transform);
                        decorationChain.transform.localRotation = Quaternion.identity;
                        decorationChain.transform.localScale = Vector3.zero;
                        decorationChain.transform.localPosition = Vector3.zero;
                    }
                    try
                    {
                        Vector3 devide = gameObject_face.transform.localScale;
                        if (Vector3.Distance(Vector3.zero, devide) > 0.01f)
                        {
                            decorationChain.transform.localScale = (new Vector3(5f / devide.x, 5f / devide.y, 5f / devide.z));
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    decorationChain.transform.localScale = (new Vector3(5, 5, 5));
                }
            }
        }
        if (CS_ballIsShowed)    
        {
            if (Program.I().setting.setting.Vchain.value == true)
            {
                if (ballChain == null)
                {
                    ballChain = add_one_decoration(Program.I().mod_ocgcore_cs_chaining, 3, Vector3.zero, "chaining", false).game_object;
                    ballChain.GetComponent<slowFade>().yse = (condition != gameCardCondition.verticle_clickable || Program.getVerticalTransparency() < 0.5f);
                }
            }
        }
        else
        {
            if (ballChain!=null)    
            {
                del_all_decoration_by_string("chaining");
                Vector3 pos = UIHelper.get_close(gameObject.transform.position, Program.camera_game_main, 5);
                if (Program.I().setting.setting.Vchain.value == true)
                {
                    MonoBehaviour.Destroy((GameObject)MonoBehaviour.Instantiate(Program.I().mod_ocgcore_cs_end, pos, Quaternion.identity), 5f);
                }
                if (ballChain != null)  
                {
                    destroy(ballChain);
                }
                ballChain = null;
            }
        }
    }

    GameObject ballChain;

    public bool CS_ballIsShowed = false;

    public void CS_showBall()
    {
        CS_ballIsShowed = true;
        currentKuang = kuangType.chaining;
    }

    public void CS_hideBall()   
    {
        CS_ballIsShowed = false;
    }

    public void CS_ballToNumber()   
    {
        if (CS_ballIsShowed)
        {
            currentKuang = kuangType.chaining;
            CS_hideBall();
            CS_addChainNumber(1);
        }
    }

    List<chainMonoW> chains = new List<chainMonoW>();

    class chainMonoW
    {
        public chainMono G;
        public int i;
        //public Vector3 bornPosition = default(Vector3);
        //public Vector3 bornAngle = default(Vector3);
    }

    public void CS_addChainNumber(int i)
    {
        currentKuang = kuangType.chaining;
        chainMonoW w = new chainMonoW();
        w.i = i;
        w.G = null;
        chains.Add(w);
    }

    public void CS_removeOneChainNumber()   
    {
        if (chains.Count > 0)
        {
            chainMono decorationChain = chains[chains.Count - 1].G;
            if (decorationChain!=null)  
            {
                if (Program.I().ocgcore.inTheWorld())
                {
                    destroy(decorationChain.gameObject);
                }
                else
                {
                    decorationChain.flashing = true;
                    destroy(decorationChain.gameObject, 0.7f);
                }
            }
            chains.RemoveAt(chains.Count - 1);
            currentKuang = kuangType.none;
        }
    }

    public void CS_removeAllChainNumber()   
    {
        while (chains.Count>0)  
        {
            CS_removeOneChainNumber();
        }
    }

    public void CS_clear()
    {
        CS_hideBall();
        CS_removeAllChainNumber();
        currentKuang = kuangType.none;
    }

    #endregion

    public GPS p_beforeOverLayed;
    public int overFatherCount;
}
